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
            public int dwData;
            //DWORD The size, in bytes, of the data pointed to by the lpData member
            public int cbData;
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
        private System.EventHandler hrTextChanged = null;
        private bool dashboard_launched = false;
        private bool listen_for_warning_msg = false;

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
            else
            {
                SetForegroundWindow(hWnd);
                this.Activate();
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
                cds.cbData = len + 1;
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
                    if (listen_for_warning_msg)
                    {
                        enableIgnoreWarnings();
                        listen_for_warning_msg = false;
                    }
                    break;
                case (WM_USER + 0x1):
                    if (listen_for_warning_msg)
                    {
                        HealthRecordText.Clear();
                        listen_for_warning_msg = false;
                    }
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

        public EHRNarrative()
        {
            InitializeComponent();

            dashboardTimer.Stop();

            hrTextChanged = new System.EventHandler(this.HealthRecordText_TextChanged);
            topLevelLines = new List<EHRLine>();
            dashboard_launched = false;

            ParseLabels();

            HealthRecordText.TextChanged += hrTextChanged;
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
                        char[] separator = new char[] { ' ' };
                        LoadTemplate(command.Trim().Split(separator, 2, StringSplitOptions.RemoveEmptyEntries)[1]);
                        break;
                    default:
                        ParseReplaceCommand(ref command_str, command);
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

            dashboardTimer.Start();
        }

        private string ParseReplaceCommand(ref string command_str, String command)
        {
            char[] separator = new char[] { '/' };
            String[] parts = command.Replace("\\n", "" + System.Environment.NewLine).Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string lookup = parts[0].Replace("[***", "[***\\cf2 ").Replace("***]", "\\cf1 ***]");

            if (parts.Length == 3)
            {
                //Do Insert
                int position = 0;
                int len = 0;

                if (parts[0].Contains("%SELECTED"))
                {
                    position = HealthRecordText.Rtf.IndexOf(HealthRecordText.SelectedText);
                    len = HealthRecordText.SelectedText.Length;
                }
                else
                {
                    position = HealthRecordText.Rtf.IndexOf(lookup);
                    len = lookup.Length;
                }

                if (parts[2].ToLower().Contains("before"))
                {
                    HealthRecordText.Rtf = HealthRecordText.Rtf.Insert(position, parts[1] + " ");
                }
                else if (parts[2].ToLower().Contains("after"))
                {
                    HealthRecordText.Rtf = HealthRecordText.Rtf.Insert(position + len, " " + parts[1]);
                }
                else
                {
                    //Do Error!
                }
            }
            else if (parts.Length == 2)
            {
                //Do Replace
                if (parts[0].Contains("%SELECTED"))
                {
                    if (HealthRecordText.SelectedText.Trim().StartsWith("[") && HealthRecordText.SelectedText.Trim().EndsWith("]"))
                    {
                        if (command_str != "")
                        {
                            command_str += " ! ";
                        }

                        if (parts[1].Trim() != "")
                        {
                            //Send data command
                            command_str += "data " + HealthRecordText.SelectedText.Trim();
                        }
                        else
                        {
                            command_str += "del " + HealthRecordText.SelectedText.Trim();
                        }
                    }

                    HealthRecordText.SelectedText = parts[1];
                }
                else
                {
                    if (lookup.Trim().StartsWith("[") && lookup.Trim().EndsWith("]") && HealthRecordText.Rtf.Contains(lookup[0]))
                    {
                        if (command_str != "")
                        {
                            command_str += " ! ";
                        }

                        if (parts[1].Trim() != "")
                        {
                            //Send data command
                            command_str += "data " + parts[0].Trim();
                        }
                        else
                        {
                            command_str += "del " + parts[0].Trim();
                        }
                    }

                    HealthRecordText.Rtf = HealthRecordText.Rtf.Replace(lookup, parts[1]);
                }
            }
            else
            {
                //Do Error!!!
            }
            return command_str;
        }

        private void NotifySLC(string command_str)
        {
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

        private void ParseLabels()
        {
            topLevelLines.Clear();

            topLevelLines = FindEHRLines();
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
            disableIgnoreWarnings();

            if (HealthRecordText.Text.Trim() == "")
            {
                NotifySLC("reset");
                topLevelLines.Clear();
                return;
            }

            CheckEHRLineStatus();
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
            //command_strings.AddRange(CheckForVitals());

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

        private void NextField()
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
        }

        private void enableIgnoreWarnings()
        {
            ignore_warnings.Visible = true;
        }

        private void disableIgnoreWarnings()
        {
            if (ignore_warnings.Visible)
            {
                NotifySLC("ignore");
                ignore_warnings.Visible = false;
            }
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

        private void check_button_Click(object sender, EventArgs e)
        {
            listen_for_warning_msg = true;
            List<String> commands = CheckForVitals();
            commands.Add("validate");
            NotifySLC(commands);
            //NotifySLC("validate");
        }

        private void ignore_warnings_Click(object sender, EventArgs e)
        {
            HealthRecordText.Clear();
        }
    }
}
