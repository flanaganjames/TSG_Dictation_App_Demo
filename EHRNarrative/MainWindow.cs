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

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EHRNarrative
{
    public partial class EHRNarrative : Form
    {
        //Used for WM_COPYDATA for string messages
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("User32.dll")]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern Int32 FindWindow(String lpClassNAme, String lpWindowName);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(int hWnd);

        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x4A;

        private ArrayList keyword_list = null;

        public bool bringAppToFront(int hWnd)
        {
            return SetForegroundWindow(hWnd);
        }

        public int sendWindowsStringMessage(int hWnd, int wParam, string msg)
        {
            int result = 0;

            if (hWnd > 0)
            {
                byte[] sarr = System.Text.Encoding.Default.GetBytes(msg);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msg;
                cds.cbData = len + 1;
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

        public EHRNarrative()
        {
            InitializeComponent();
            keyword_list = new ArrayList();

            HealthRecordText.SelectAll();
        }

        private void HealthRecordText_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"\[([^]]*)\]";
            Regex rgx = new Regex(pattern);

            ArrayList new_keyword_list = new ArrayList();
            foreach (Match match in rgx.Matches(HealthRecordText.Text))
            {
                string match_str = match.Value;

                new_keyword_list.Add(match_str);
            }

            IEnumerable removed_keywords = keyword_list.ToArray().Except(new_keyword_list.ToArray());
            IEnumerable added_keywords = new_keyword_list.ToArray().Except(keyword_list.ToArray());

            last_action_label.Text = "Last Action:\n";
            string command_string = "";
            foreach (string keyword in added_keywords)
            {
                last_action_label.Text += "Added " + keyword + "\n";
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "add " + keyword);
                //command_string += "add " + keyword;
            }
            foreach (string keyword in removed_keywords)
            {
                last_action_label.Text += "Removed " + keyword + "\n";
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "del " + keyword);
                command_string += "data " + keyword;
            }
            if (command_string != "")
            {
                System.Diagnostics.Process.Start("SLC.exe", command_string);
            }

            keyword_list = new_keyword_list;
            current_label.Text = "Current Keywords:\n";
            foreach (string keyword in keyword_list)
            {
                current_label.Text += keyword + "\n";
            }
        }
    }
}
