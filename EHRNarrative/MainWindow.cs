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

        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x4A;
        #endregion

        private List<EHRLine> topLevelLines = null;
        private System.EventHandler hrTextChanged = null;

        #region Window Messaging Helpers
        public bool bringAppToFront(int hWnd)
        {
            return SetForegroundWindow(hWnd);
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
                    MessageBox.Show("Message received from external program: " + msg.WParam + " - " + msg.LParam);
                    break;
                case WM_COPYDATA:
                    COPYDATASTRUCT msgCarrier = new COPYDATASTRUCT();
                    Type type = msgCarrier.GetType();
                    msgCarrier = (COPYDATASTRUCT)msg.GetLParam(type);
                    //MessageBox.Show("String Message Received: " + msgCarrier.lpData + ", " + msgCarrier.dwData + ", " + msgCarrier.cbData);
                    String msgString = msgCarrier.lpData;

                    ParseVBACommand(msgString);

                    break;
            }
            base.WndProc(ref msg);
        }

        public EHRNarrative()
        {
            InitializeComponent();

            hrTextChanged = new System.EventHandler(this.HealthRecordText_TextChanged);

            topLevelLines = new List<EHRLine>();

            ParseLabels();
            HealthRecordText.TextChanged += hrTextChanged;
        }

        private void ParseVBACommand(String commandStr)
        {
            if (commandStr.Contains("%START"))
            {
                ParseLabels();
            }
            else if (commandStr.Contains("%NEXT_FIELD"))
            {

            }
            else
            {
                this.HealthRecordText.TextChanged -= hrTextChanged;

                String[] initializer = new String[] { ":%s" };
                String[] separator = new String[] { "/" };

                String[] commands = commandStr.Split(initializer, StringSplitOptions.RemoveEmptyEntries);

                string command_str = "";
                foreach (String command in commands)
                {
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
                }

                if (command_str != "")
                {
                    System.Diagnostics.Process.Start("SLC.exe", command_str);
                }

                this.HealthRecordText.TextChanged += hrTextChanged;
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
                    string pattern = @"(\[[^]]*\])(.*)";
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

        private void HealthRecordText_TextChanged(object sender, EventArgs e)
        {
            if (HealthRecordText.Text.Trim() == "")
            {
                System.Diagnostics.Process.Start("SLC.exe", "reset");
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
            
            if (command_strings.Any())
            {
                string command_string = String.Join(" ! ", command_strings);
                System.Diagnostics.Process.Start("SLC.exe", command_string);
            }

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
            int current = HealthRecordText.SelectionStart + HealthRecordText.SelectionLength;

            int next = HealthRecordText.Text.IndexOf('[', current);
            if (next == -1)
            {
                HealthRecordText.Select(0, 0);
                return;
            }

            int close = HealthRecordText.Text.IndexOf(']', next);
            if (close == -1)
            {
                HealthRecordText.Select(0, 0);
                return;
            }

            HealthRecordText.Select(next, close - next + 1);
        }
    }
}
