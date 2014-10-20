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

namespace TestWindowsMessage
{
    public partial class Form1 : Form
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

        #region Window Messaging Helpers
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

        public Form1()
        {
            InitializeComponent();
        }

        private int getEHRWindow()
        {
            return getWindowId(null, "Electronic Health Record Narrative");
        }

        public void NotifySLC(string command_str)
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
            }
        }

        private void sendToEHRButton_Click(object sender, EventArgs e)
        {
            sendCustomMessage(getEHRWindow(), 0, ehrCommandInput.Text);
            SetForegroundWindow(getEHRWindow());
        }

        private void sendToSLCButton_Click(object sender, EventArgs e)
        {
            NotifySLC(slcCommandInput.Text);
            SetForegroundWindow(getEHRWindow());
        }

        private void chestPainButton_Click(object sender, EventArgs e)
        {
            NotifySLC("state Chest pain over 40 ! req HPI [Onset], [Movement], [TAD Risk Factors], [CAD Risk Factors], [***PE Risk Factors***] ! req Exam [Constitutional Exam], [Cardiovascular Exam], [Cardiovascular Upper Extremity Exam], [Chest Exam], [Calf Exam] ! link Chest Pain Resources ! link Differential Diagnosis Tool ! link RSQ Assist");
            System.Threading.Thread.Sleep(250);

            sendCustomMessage(getEHRWindow(), 0, ":%cLOAD_TEMPLATE Chest pain over 40:%cSTART");
        }

        private void nextFieldButton_Click(object sender, EventArgs e)
        {
            sendCustomMessage(getEHRWindow(), 0, ":%cNEXT_FIELD");
            SetForegroundWindow(getEHRWindow());
        }

        private void showDialogButton_Click(object sender, EventArgs e)
        {
            sendCustomMessage(getEHRWindow(), 0, ":%cDIALOG " + dialogInput.Text);
        }

        private void cleanTemplateButton_Click(object sender, EventArgs e)
        {
            sendCustomMessage(getEHRWindow(), 0, ":%cCLEAN");
            SetForegroundWindow(getEHRWindow());
        }

        private void showTextDialogButton_Click(object sender, EventArgs e)
        {
            sendCustomMessage(getEHRWindow(), 0, ":%cTEXT_DIALOG");
            SetForegroundWindow(getEHRWindow());
        }
    }
}
