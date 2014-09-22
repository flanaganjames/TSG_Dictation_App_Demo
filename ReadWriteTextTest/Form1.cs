using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadWriteTextTest
{
    public partial class Form1 : Form
    {
        private int ticks = 5;

        public Form1()
        {
            InitializeComponent();

            timer1.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                countdown.Text = "Reading Text In: " + ticks.ToString() + " seconds...";
                checkBox1.Enabled = false;
                timer1.Start();
            }
            else
            {
                countdown.Text = "Writing Text In: " + ticks.ToString() + " seconds...";
                checkBox1.Enabled = false;
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ticks <= 0)
            {
                ticks = 5;
                countdown.Text = "";
                timer1.Stop();
                checkBox1.Enabled = true;

                if (!checkBox1.Checked)
                {
                    //var allText = GetWindowText.GetAllTextFromWindowByTitle("Document - WordPad");
                    var allText = WindowText.GetTextFromActiveWindowElement();
                    textBox1.Text = allText;
                }
                else
                {
                    WindowText.SetTextOfActiveWindowElement(textBox1.Text);
                }
            }
            else
            {
                ticks--;
                if (!checkBox1.Checked)
                {
                    countdown.Text = "Reading Text In: " + ticks.ToString() + " seconds...";
                }
                else
                {
                    countdown.Text = "Writing Text In: " + ticks.ToString() + " seconds...";
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Text = "Write Text";
                textBox1.ReadOnly = false;
            }
            else
            {
                button1.Text = "Read UI Elements";
                textBox1.ReadOnly = true;
                textBox1.Text = "";
            }
        }
    }

    public class WindowText
    {
        [DllImport("user32")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("user32")]
        private static extern bool GetGUIThreadInfo(IntPtr hThreadID, ref GUITHREADINFO lpgui);

        // Delegate we use to call methods when enumerating child windows.
        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, [Out] string lParam);

        private struct RECT
        {
            public int iLeft;
            public int iTop;
            public int iRight;
            public int iBottom;
        }

        private struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rectCaret;
        }

        // Callback method used to collect a list of child windows we need to capture text from.
        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            // Creates a managed GCHandle object from the pointer representing a handle to the list created in GetChildWindows.
            var gcHandle = GCHandle.FromIntPtr(pointer);

            // Casts the handle back back to a List<IntPtr>
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }

            // Adds the handle to the list.
            list.Add(handle);

            return true;
        }

        // Returns an IEnumerable<IntPtr> containing the handles of all child windows of the parent window.
        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            // Create list to store child window handles.
            var result = new List<IntPtr>();

            // Allocate list handle to pass to EnumChildWindows.
            var listHandle = GCHandle.Alloc(result);

            try
            {
                // Enumerates though all the child windows of the parent represented by IntPtr parent, executing EnumChildWindowsCallback for each. 
                EnumChildWindows(parent, EnumChildWindowsCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                // Free the list handle.
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            // Return the list of child window handles.
            return result;
        }

        private static void SetText(IntPtr handle, string text)
        {
            const uint WM_SETTEXT = 0x000C;

            SendMessage(handle, WM_SETTEXT, 0, text);
        }

        // Gets text text from a control by it's handle.
        private static string GetText(IntPtr handle)
        {
            const uint WM_GETTEXTLENGTH = 0x000E;
            const uint WM_GETTEXT = 0x000D;

            // Gets the text length.
            var length = (int)SendMessage(handle, WM_GETTEXTLENGTH, IntPtr.Zero, null);

            // Init the string builder to hold the text.
            var sb = new StringBuilder(length + 1);

            // Writes the text from the handle into the StringBuilder
            SendMessage(handle, WM_GETTEXT, (IntPtr)sb.Capacity, sb);

            // Return the text as a string.
            return sb.ToString();
        }

        // Wraps everything together. Will accept a window title and return all text in the window that matches that window title.
        public static string GetAllTextFromWindowByTitle(string windowTitle)
        {
            try
            {
                // Find the main window's handle by the title.
                var windowHWnd = FindWindowByCaption(IntPtr.Zero, windowTitle);

                return GetAllTextFromWindow(windowHWnd);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            return string.Empty;
        }

        public static string GetAllTextFromActiveWindow()
        {
            try
            {
                IntPtr windowHWnd = GetForegroundWindow();

                return GetAllTextFromWindow(windowHWnd);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            return string.Empty;
        }

        public static string GetTextFromActiveWindowElement()
        {
            try
            {
                IntPtr windowHWnd = GetForegroundWindow();
                IntPtr lpdwProcessId;
                IntPtr threadId = GetWindowThreadProcessId(windowHWnd, out lpdwProcessId);

                GUITHREADINFO lpgui = new GUITHREADINFO();
                lpgui.cbSize = Marshal.SizeOf(lpgui);

                GetGUIThreadInfo(threadId, ref lpgui);

                return GetText(lpgui.hwndFocus);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            return string.Empty;
        }

        public static void SetTextOfActiveWindowElement(string text)
        {
            try
            {
                IntPtr windowHWnd = GetForegroundWindow();
                IntPtr lpdwProcessId;
                IntPtr threadId = GetWindowThreadProcessId(windowHWnd, out lpdwProcessId);

                GUITHREADINFO lpgui = new GUITHREADINFO();
                lpgui.cbSize = Marshal.SizeOf(lpgui);

                GetGUIThreadInfo(threadId, ref lpgui);

                SetText(lpgui.hwndFocus, text);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private static string GetAllTextFromWindow(IntPtr windowHWnd)
        {
            var sb = new StringBuilder();

            try
            {
                // Loop though the child windows, and execute the EnumChildWindowsCallback method
                var childWindows = GetChildWindows(windowHWnd);

                // For each child handle, run GetText
                //foreach (var childWindowText in childWindows.Select(GetText))
                //{
                //    // Append the text to the string builder.
                //    sb.Append(childWindowText);
                //}

                foreach (var childWindowText in childWindows)
                {
                    StringBuilder lpClassName = new StringBuilder(256);
                    GetClassName(childWindowText, lpClassName, lpClassName.Capacity);
                    sb.Append("UIElement: " + lpClassName + " = " + GetText(childWindowText) + System.Environment.NewLine);
                }

                // Return the windows full text.
                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            return string.Empty;
        }
    }
}