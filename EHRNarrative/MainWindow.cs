﻿using System;
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

        private ArrayList keyword_list = null;

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

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_USER:
                    MessageBox.Show("Message Received: " + msg.WParam + " - " + msg.LParam);
                    break;
                case WM_COPYDATA:
                    COPYDATASTRUCT msgStr = new COPYDATASTRUCT();
                    Type type = msgStr.GetType();
                    msgStr = (COPYDATASTRUCT)msg.GetLParam(type);
                    MessageBox.Show("String Message Received: " + msgStr.lpData + ", " + msgStr.dwData + ", " + msgStr.cbData);
                    break;
            }
            base.WndProc(ref msg);
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

            string command_string = "";
            foreach (string keyword in added_keywords)
            {
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "add " + keyword);
                //command_string += "add " + keyword;
            }
            foreach (string keyword in removed_keywords)
            {
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
        }
    }
}
