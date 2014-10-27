using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EHRNarrative
{
    public partial class EHRNarrative : Form
    {
        #region Windows Framework Imports
        //Used with WM_COPYDATA for string messages
        public struct COPYDATASTRUCT
        {
            //ULONG_PTR The data to be passed to the receiving application
            public ulong dwData;
            //DWORD The size, in bytes, of the data pointed to by the lpData member
            public ulong cbData;
            //PVOID (void pointer) The data to be passed to the receiving application. This member can be NULL
            //However, we are using it differently to just store a string.
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        //For use with Window Messages
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(int hWnd, int command);

        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x4A;
        #endregion

        private List<EHRLine> topLevelLines = null;
        private List<String> considerLines = null;
        private List<String> insertedConsiderables = null;
        private System.EventHandler hrTextChanged = null;
        private bool dashboard_launched = false;
        private string complaint;

        #region Window Messaging Helpers
        public void bringAppToFront(int hWnd)
        {
            if (!dashboard_launched)
            {
                ShowWindow(hWnd, 9);  // 9 is SW_RESTORE
                System.Threading.Thread.Sleep(1000);
                this.Activate();
                dashboard_launched = true;
            }
        }

        public int sendCustomMessage(int hWnd, int wParam, string msg)
        {
            int result = 0;

            if (hWnd > 0)
            {
                byte[] msgArray = System.Text.Encoding.Default.GetBytes(msg);
                int len = msgArray.Length;
                COPYDATASTRUCT cds;
                cds.dwData = 0;
                cds.cbData = (ulong)len + 1;
                cds.lpData = msg;
                result = SendMessage(hWnd, WM_COPYDATA, wParam, ref cds);
            }

            return result;
        }

        public int sendWindowsMessage(int hWnd, int Msg, int wParam, int lParam)
        {
            int result = 0;

            if (hWnd > 0)
            {
                result = SendMessage(hWnd, Msg, wParam, lParam);
            }

            return result;
        }

        public int getWindowId(string className, string windowName)
        {
            return FindWindow(className, windowName);
        }
        #endregion

        /// <summary>
        /// Parse incoming messages to this Window
        /// </summary>
        /// <param name="msg">Reference to a Windows Message object</param>
        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_USER:
                    break;
                case (WM_USER + 0x1):
                    break;
                case WM_COPYDATA:
                    COPYDATASTRUCT msgCarrier = new COPYDATASTRUCT();
                    Type type = msgCarrier.GetType();
                    msgCarrier = (COPYDATASTRUCT)msg.GetLParam(type);
                    String msgString = msgCarrier.lpData;

                    ParseVBACommand(msgString);

                    break;
            }
            base.WndProc(ref msg);
        }

        [Conditional("DEBUG")]
        private void EnableTestingButtons()
        {
            mockDragonButton.Visible = true;
            mockDragonButton.Enabled = true;
        }

        public EHRNarrative()
        {
            InitializeComponent();

            EnableTestingButtons();

            dashboardTimer.Stop();

            hrTextChanged = new System.EventHandler(this.HealthRecordText_TextChanged);
            topLevelLines = new List<EHRLine>();
            considerLines = new List<string>();
            insertedConsiderables = new List<string>();
            dashboard_launched = false;

            ParseLabels();
            ParseConsiderables();

            HealthRecordText.TextChanged += hrTextChanged;
        }

        public bool ReplaceKeyword(String commandStr)
        {
            this.HealthRecordText.TextChanged -= hrTextChanged;

            string command_str = "";
            command_str = ParseReplaceCommand(commandStr.Trim());

            NotifySLC(command_str);

            this.HealthRecordText.TextChanged += hrTextChanged;

            if (string.IsNullOrEmpty(command_str))
                return false;
            return true;
        }

        private void ParseVBACommand(String commandStr)
        {
            this.HealthRecordText.TextChanged -= hrTextChanged;

            String[] initializer = new String[] { ":%c" };

            String[] commands = commandStr.Split(initializer, StringSplitOptions.RemoveEmptyEntries);

            string command_str = "";
            foreach (String command in commands)
            {
                switch (command.Trim().Split(' ')[0])
                {
                    case "START":
                        ParseLabels();
                        break;
                    case "NEXT_FIELD":
                        NextField();
                        break;
                    case "LOAD_TEMPLATE":
                        System.Diagnostics.Process.Start("Dashboard.exe");
                        char[] separator = new char[] { ' ' };
                        this.complaint = command.Trim().Split(separator, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        LoadTemplate(complaint);
                        break;
                    case "DIALOG":
                        var dialogName = command.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        ExamDialog ed = new ExamDialog(this, dialogName, this.complaint);
                        int edhWnd = ed.Show();
                        SetForegroundWindow(edhWnd);
                        break;
                    case "TEXT_DIALOG":
                        var ehr_keyword = this.HealthRecordText.SelectedText;
                        BoilerPlateDialog bpd = new BoilerPlateDialog(this, ehr_keyword);
                        int bpdhWnd = bpd.Show();
                        SetForegroundWindow(bpdhWnd);
                        break;
                    case "CLEAN":
                        CleanCurrentTemplate();
                        break;
                    case "SEND_TO_SLC":
                        string slc_command = command.Trim().Split(new char[] { ' ' }, 2)[1].Trim();
                        NotifySLC(slc_command);
                        break;
                    default:
                        command_str = ParseReplaceCommand(command);
                        break;
                }
            }

            NotifySLC(command_str);

            this.HealthRecordText.TextChanged += hrTextChanged;
        }

        private void LoadTemplate(string template)
        {
            string home = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string templates = System.IO.Path.Combine(home, "templates");
            template = template.Trim() + ".rtf";
            template = System.IO.Path.Combine(templates, template);

            HealthRecordText.Clear();
            HealthRecordText.LoadFile(template);

            ParseLabels();
            ParseConsiderables();

            dashboardTimer.Start();
        }

        private string ParseReplaceCommand(String command)
        {
            if (String.IsNullOrEmpty(command))
            {
                return "";
            }

            List<String> command_strings = new List<String>();

            char[] separator = new char[] { '/' };
            String[] parts = command.Replace("\\\\n", "\\par").Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string lookup = parts[0];
            string newText = parts[1];
            string insert = "over";

            //Check if the text to be inserted is a considerable line
            string considerTest = newText.Replace("\\par", "\n").Trim();
            if (considerTest.StartsWith("[") && considerTest.EndsWith("]"))
            {
                if (insertedConsiderables.Contains(considerTest))
                {
                    return "";
                }
            }

            if (lookup.Contains("%SELECTED"))
            {
                lookup = HealthRecordText.SelectedText;
                insert = "cursor";
            }
            if (parts.Length == 3)
            {
                if (parts[2].ToLower().Contains("before"))
                {
                    insert = "before";
                }
                else if (parts[2].ToLower().Contains("after"))
                {
                    insert = "after";
                }
                else
                {
                    //Do Error!
                }
            }

            //Find where our lookup text is
            int lookupPosition = HealthRecordText.Rtf.IndexOf(lookup);
            if (lookupPosition == -1)
            {
                if (lookup.Contains("[") && lookup.Contains("]"))
                {
                    Match match = new Regex(@"\[(\\cf\d+ )?" + lookup.Replace("[", "").Replace("]", "") + @"(\\cf\d+ )?\]")
                                            .Match(HealthRecordText.Rtf);
                    if (match.Success)
                    {
                        lookup = match.Value;
                        lookupPosition = match.Index;
                    }
                }
            }
            int lookupLength = lookup.Length;

            //There is nothing to do since our lookup doesn't exist, so abort.
            if (lookupPosition == -1 && !insert.Contains("cursor"))
            {
                return "";
            }

            //Actually insert the new text in the proper location in the RTF
            if (insert.Contains("over"))
            {
                HealthRecordText.Rtf = HealthRecordText.Rtf.Replace(lookup, newText);
            }
            else if (insert.Contains("before"))
            {
                HealthRecordText.Rtf = HealthRecordText.Rtf.Insert(lookupPosition, newText);
            }
            else if (insert.Contains("after"))
            {
                HealthRecordText.Rtf = HealthRecordText.Rtf.Insert(lookupPosition + lookupLength, " " + newText);
            }
            else if (insert.Contains("cursor"))
            {
                HealthRecordText.SelectedText = newText.Replace("\\par", "\n");
            }
            else
            {
                //Do Error!
            }

            //Set cursor position to the end of the inserted text
            if (!insert.Contains("cursor"))
            {
                lookup = parts[0].Replace("\\par", "\n").Replace("\\cf2 ", "").Replace("\\cf1 ", "");
                newText = newText.Replace("\\par", "\n");
                int selectOffset = 0;
                if (newText.EndsWith("\n"))
                {
                    selectOffset = 1;
                }
                HealthRecordText.Select(HealthRecordText.Text.IndexOf(newText) + newText.Length - selectOffset, 0);
            }
            //Notify the SLC of any relevant changes
            if (IsAnEHRLine(lookup))
            {
                if (newText.Trim() == "" || lookup.Trim().Split(':')[0] != newText.Trim().Split(':')[0])
                {
                    //Either we deleted the line entirely or completel replaced it
                    command_strings.Add("delete " + lookup.Substring(lookup.IndexOf('['), lookup.IndexOf(']') - lookup.IndexOf('[') + 1));
                }
            }

            if (IsAConsiderable(lookup))
            {
                //We can't track the difference between deleting the considerable and inputting text. So they will always send data.
                command_strings.Add("data " + lookup.Trim());
            }

            if (IsAnEHRLine(newText))
            {
                command_strings.Add("add " + newText.Substring(newText.IndexOf('['), newText.IndexOf(']') - newText.IndexOf('[') + 1));
            }

            if (GetLabelFromKeyword(lookup.Trim()) != null && !(newText.Trim().StartsWith("[") && newText.Trim().EndsWith("]")))
            {
                if (newText.Trim() != "")
                {
                    command_strings.Add("data " + lookup.Trim());
                }
                else
                {
                    command_strings.Add("del " + lookup.Trim());
                }
            }
            else if (newText.Trim().StartsWith("[") && newText.Trim().EndsWith("]"))
            {
                considerLines.Add(newText.Trim());
                insertedConsiderables.Add(newText.Trim());

                command_strings.Add("add " + newText.Trim());
            }

            command_strings.AddRange(CheckForVitals());

            string command_str = String.Join(" ! ", command_strings);
            return command_str;
        }

        private void CleanCurrentTemplate()
        {
            Regex rgx1 = new Regex(@"(?<=(\\[\w\d\\]+ )).+(?=(\\[\w\d\\]+ ))(?=.*\[.*\])");
            Regex rgx = new Regex(@"((?<!(\\[\w\d\\]+)).)*\[.*\](\\par|(?!(\\[\w\d\\]+)).)*");
            HealthRecordText.Rtf = rgx1.Replace(HealthRecordText.Rtf, "");
            HealthRecordText.Rtf = rgx.Replace(HealthRecordText.Rtf, "");
        }

        public void NotifySLC(string command_str)
        {
            command_str = command_str.Trim();
            if (command_str.EndsWith("!"))
                command_str = command_str.Substring(0, command_str.LastIndexOf("!") - 1);

            if (command_str != "")
            {
                try
                {
                    //System.Diagnostics.Process.Start("SLC.MOCK.exe", command_str);
                    System.Diagnostics.Process.Start("SLC.exe", command_str);
                }
                catch
                {
                    MessageBox.Show("There was an error when trying to call the SLC!");
                }

                dashboardTimer.Start();
            }
        }

        private void NotifySLC(List<String> command_list)
        {
            if (command_list.Any())
            {
                string command_string = String.Join(" ! ", command_list);
                NotifySLC(command_string);
            }
        }

        private void ParseConsiderables()
        {
            considerLines.Clear();
            insertedConsiderables.Clear();

            considerLines = FindConsiderables();
            insertedConsiderables = considerLines.ToList();
        }

        private void ParseLabels()
        {
            topLevelLines.Clear();

            topLevelLines = FindEHRLines();
        }

        private bool IsAConsiderable(string line)
        {
            if (considerLines.Contains(line.Trim()))
                return true;
            return false;
        }

        private bool IsAnEHRLine(string line)
        {
            if (line.Contains(':'))
            {
                string[] sides = line.Split(':');
                string pattern = @"(\[[^]]*\])";
                Regex rgx = new Regex(pattern);
                Match m = rgx.Match(sides[1]);
                if (m.Success)
                {
                    return true;
                }
            }
            return false;
        }

        private String GetLabelFromKeyword(string keyword)
        {
            foreach (EHRLine line in topLevelLines)
            {
                if (line.keyword.Equals(keyword.Trim()))
                {
                    return line.label;
                }
            }
            return null;
        }

        private String GetKeywordFromLabel(string label)
        {
            foreach (EHRLine line in topLevelLines)
            {
                if (line.label.Equals(label))
                {
                    return line.keyword;
                }
            }
            return null;
        }

        private String GetTextFromLabel(string label)
        {
            foreach (EHRLine line in topLevelLines)
            {
                if (line.label.Equals(label))
                {
                    return line.text;
                }
            }
            return null;
        }

        private List<EHRLine> FindEHRLines()
        {
            List<EHRLine> list = new List<EHRLine>();
            foreach (String line in new LineReader(() => new StringReader(HealthRecordText.Text)))
            {
                if (line.Contains(':'))
                {
                    string[] sides = line.Split(':');
                    string pattern = @"(\[[^]]*\])";
                    Regex rgx = new Regex(pattern);
                    Match m = rgx.Match(sides[1]);
                    if (m.Success)
                    {
                        list.Add(new EHRLine(sides[0].Trim(), m.Groups[1].Value, m.Groups[2].Value));
                    }
                    else if (GetKeywordFromLabel(sides[0].Trim()) != null)
                    {
                        list.Add(new EHRLine(sides[0].Trim(), GetKeywordFromLabel(sides[0].Trim()), sides[1].Trim()));
                    }
                }
            }
            return list;
        }

        private List<string> FindConsiderables()
        {
            List<string> list = new List<string>();
            foreach (String line in new LineReader(() => new StringReader(HealthRecordText.Text)))
            {
                if (line.Trim().StartsWith("[") && line.TrimEnd().EndsWith("]"))
                {
                    list.Add(line.Trim());
                }
            }
            return list;
        }

        private List<String> CheckForVitals()
        {
            List<String> commands = new List<String>();
            foreach (String line in new LineReader(() => new StringReader(HealthRecordText.Text)))
            {
                Regex pulse = new Regex(@"\b(Pulse|P|Heart Rate|HR)\b ?(\w+ )?(?<value>\d{2,3})\b( bpm\b| per minute\b| beats per minute\b)?", RegexOptions.IgnoreCase);
                Match pulse_match = pulse.Match(line);
                while (pulse_match.Success)
                {
                    if (Convert.ToInt32(pulse_match.Groups["value"].Value) >= 30)
                    {
                        commands.Add("VS p " + pulse_match.Groups["value"]);
                    }
                    pulse_match = pulse_match.NextMatch();
                }
            }
            return commands;
        }

        private void HealthRecordText_TextChanged(object sender, EventArgs e)
        {
            if (HealthRecordText.Text.Trim() == "")
            {
                NotifySLC("reset");
                topLevelLines.Clear();
                return;
            }

            CheckEHRLineStatus();
            CheckConsiderableLineStatus();
        }

        private void CheckConsiderableLineStatus()
        {
            List<string> command_strings = new List<string>();

            List<string> possibleConsiderables = FindConsiderables();

            foreach (string considerable in considerLines.ToList())
            {
                if (!possibleConsiderables.Contains(considerable))
                {
                    command_strings.Add("data " + considerable);
                    considerLines.Remove(considerable);
                }
            }

            NotifySLC(command_strings);
        }

        private void CheckEHRLineStatus()
        {
            List<EHRLine> lines = FindEHRLines();

            IEnumerable<string> keyword_list = topLevelLines.Select(x => x.keyword);
            IEnumerable<string> current_keywords = lines.Select(x => x.keyword);
            current_keywords = current_keywords.Where(x => x.Any()).ToList();
            IEnumerable<string> added_keywords = current_keywords.ToArray().Except(keyword_list.ToArray());

            List<String> command_strings = new List<String>();
           
            if (added_keywords.Any())
            {
                command_strings.Add("add " + String.Join(" ! add ", added_keywords));
            }

            //TODO: skipping if adding keyword and value at same time
            IEnumerable<string> blank_labels = topLevelLines.Where(x => x.text == "").Select(x => x.label);
            foreach (EHRLine line in lines)
            {
                if (blank_labels.Contains(line.label) && line.text != "")
                {
                    command_strings.Add("data " + line.keyword);
                }
            }

            //Detects places where a field text has been deleted
            IEnumerable<string> new_blanks = lines.Where(x => x.text == "").Select(x => x.label);
            foreach (EHRLine old_line in topLevelLines)
            {
                if (new_blanks.Contains(old_line.label) && old_line.text != "")
                {
                    command_strings.Add("delete " + old_line.keyword);
                }
            }


            //TODO: Check for removed labels (del)
            //Removing a label removes the requirement in the SLC
            IEnumerable<string> label_list = topLevelLines.Select(x => x.label);
            IEnumerable<string> current_labels = lines.Select(x => x.label);
            IEnumerable<string> removed_labels = label_list.ToArray().Except(current_labels.ToArray());
            foreach (string label in removed_labels)
            {
                string keyword = GetKeywordFromLabel(label);

                if (keyword != null && keyword != "")
                {
                    command_strings.Add("del " + keyword);
                }
            }

            //check for vitals
            command_strings.AddRange(CheckForVitals());

            NotifySLC(command_strings);

            topLevelLines = lines;
        }

        private void HealthRecordText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                NextField();
            }
        }

        public void NextField()
        {
            if (HealthRecordText.Text.Contains('[') && HealthRecordText.Text.Contains(']'))
            {
                int current = HealthRecordText.SelectionStart + HealthRecordText.SelectionLength;

                int next = HealthRecordText.Text.IndexOf('[', current);
                if (next == -1)
                {
                    HealthRecordText.Select(0, 0);
                    NextField();
                    return;
                }

                int close = HealthRecordText.Text.IndexOf(']', next);
                if (close == -1)
                {
                    HealthRecordText.Select(0, 0);
                    NextField();
                    return;
                }

                HealthRecordText.Select(next, close - next + 1);
            }
            HealthRecordText.Focus();
        }

        private void dashboardTimer_Tick(object sender, EventArgs e)
        {
            int dashboardHWnd = 0;
            int tries = 0;
            while (dashboardHWnd == 0 && tries < 5)
            {
                dashboardHWnd = getWindowId(null, "The Sullivan Group dashboard");
                tries += 1;
            }

            if (dashboardHWnd != 0)
            {
                dashboardTimer.Stop();
                bringAppToFront(dashboardHWnd);
            }
        }

        private void mockDragonButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("MockDragon.exe");
        }
    }
}
