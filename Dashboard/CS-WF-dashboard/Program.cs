using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Documents;
using System.Windows.Forms;
using System.Runtime.InteropServices;


/*
 * There are some failures of encapsulation here.
 * For example, we're adding the link click handler in the main program,
 * even though the dashboard is defined in a separate class.
 */

namespace Dashboard
{
        // this is an ugly way to pass data blobs around
        // but it's required so that we're thread-insulated
    public class Dashboard
    {
            // these next two lines are for the attempt to bring 
            // the dashboard to the foreground
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int dashht = 550;
        private const int warnht = 250;
        private const int dashwid = 255;
        private System.Drawing.Size dsize = new System.Drawing.Size(dashwid, dashht);
        private System.Drawing.Size wsize = new System.Drawing.Size(dashwid, warnht);
        private System.Drawing.Size tsize = new System.Drawing.Size(dashwid, dashht+warnht);
        private System.Drawing.Point wpos = new System.Drawing.Point(0, dashht);
        private System.Drawing.Point dpos = new System.Drawing.Point(0, 0);
        public String dashpath = @"C:/TEMP/Sullivan/dashboard.rtf";
        public String warnpath = @"C:/TEMP/Sullivan/dashwarn.rtf";
        public String rtfmissing = "{\\rtf1\\ansi\\pard No dashboard available!\\par}";
        public WindowsFormsApplication1.Form1 dash;
        public RichTextBox dashrtf, warnrtf;
        public String rtfcontents;
        private bool DASHfail = false;
        private int dashboards_running = 0;
        private int updates = 0;
        private bool showing_warning = false;

        // Damn .Net:  there are two distinct RichTextBox classes,
        // with subtly different properties and methods.
        //  We're using System.Windows.Forms.RichTextBox
 
        public Dashboard()
        {
                // the overall dashboard window 
            dash = new WindowsFormsApplication1.Form1(); // dash = new Form();
            dash.Size = dsize;
            // dash.Visible = true; //  this appears to steal focus
            dash.Text = "The Sullivan Group Dashboard";
                // the dashboard RTF control
            dashrtf = new RichTextBox();
            dashrtf.Size = dsize;
            dashrtf.Location = dpos;
            dashrtf.ReadOnly = true;
            dashrtf.DetectUrls = true;
            dashrtf.BackColor = Color.White;
            dash.Controls.Add(dashrtf);
                // basic parameters for the warning box RTF control
            warnrtf = new RichTextBox();
            warnrtf.Size = wsize;
            warnrtf.Location = wpos;
            warnrtf.ReadOnly = true;
            warnrtf.BackColor = Color.Yellow;
        }

            // we can invoke failnote if we're already running,
            // though normally we'd just exit
        public void failnote()
        {
            DASHfail = true;
        }
            // and another debugging method, which adds a number
            // to window title bar, so we can note how many
            // instances of the program are running, or how
            // many times we've updated
        public void running(int i)
        {
            dashboards_running = i;
            dash.Text = string.Format("Sullivan Group {0}", i);
        }

        public void RefreshDash()
        {
            ////  .... for test purposes ....
            //// flash the background so we know the update loop is running
            //this.dash.Refresh();
            //this.dashrtf.BackColor = Color.LightGray;
            //this.dashrtf.Clear(); this.dashrtf.Refresh(); this.dash.Refresh();
            //Thread.Sleep(100);

                // update with the real contents of the main dashboard
            this.dashrtf.Rtf = RTFcontents();
            this.dashrtf.Refresh();
                // are we showing the warning box, but shouldn't?
            if (showing_warning && !File.Exists(warnpath))
            {
                    // remove warning box and resize the dashboard
                this.dash.Size = dsize;
                this.dash.Controls.Remove(warnrtf);
                showing_warning = false;
            }
                // are we not showing the warning box, but should?
            if (!showing_warning && File.Exists(warnpath))
            {
                    // expand the dashboard and add the warning box
                this.dash.Size = tsize;
                this.dash.Controls.Add(warnrtf);
                showing_warning = true;
            }
                // update the warning box contents if necessary
            if (File.Exists(warnpath))
            {
                this.warnrtf.Rtf = File.ReadAllText(this.warnpath);
                this.warnrtf.Refresh();
            }
                // now do the refresh of the whole window
            this.dash.Refresh();
     
            updates++;  // number of times we've updated
            //    // this following stuff is to ensure focus & foregrounding
            //    //  .... now handled by the MU pulling the window into the foreground
            //if (!this.dash.CanFocus) this.dashrtf.BackColor = Color.Red;
            //this.dash.Visible = true; this.dash.Enabled = true; this.dash.TopMost = true;
            //// this.dash.Focus();  
            //this.dash.BringToFront();
            //this.dashrtf.Visible = true; this.dashrtf.Enabled = true; 
            //// this.dashrtf.Focus(); 
            //this.dashrtf.BringToFront();
            //this.dash.Show();
            // IntPtr wHnd = this.dash.Handle;
            // SetForegroundWindow(wHnd);
        }

        public string RTFcontents()
        {
            if (File.Exists(this.dashpath))  // we should only be reading the file if it's been updated
            {
                rtfcontents = File.ReadAllText(this.dashpath);
            }
            else
            {
                rtfcontents = this.rtfmissing;
            }
            if (DASHfail) rtfcontents = "{\\rtf1\\ansi ALREADY RUNNING!!!\\par}";
            return rtfcontents;
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            int xx = ProgramRunning(System.AppDomain.CurrentDomain.FriendlyName);
            if (xx == 1)
            {
                /*
                 * we really need a way to actually do a mid-stream exit here;
                 * is that even possible in C#?
                 * the commented-out code immediately below is what we really
                 * want to run here
                 */
                //if (ProgramRunning(System.AppDomain.CurrentDomain.FriendlyName) > 1)
                //{
                //    System.Windows.Forms.Application.Exit();
                //}

                Dashboard D = new Dashboard();

                // dashrtf = new RichTextBox(); // RichTextBox dashrtf = new RichTextBox();
                // D.dashrtf.BackColor = Color.White; // Color.LightBlue;

                    // event handlers for mouse clicks
                D.dashrtf.LinkClicked += new LinkClickedEventHandler(dashrtf_LinkClicked);
                D.dashrtf.MouseClick += new MouseEventHandler((sender,e) 
                    => dashrtf_MouseClick(sender,e,D));

                    // update timer event
                    // (like with the RichTextBox class, annoyingly there are
                    //  three distinct Timer classes, so we fully qualify it)
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Enabled = true;
                timer.Interval = 1000;
                timer.Tick += new EventHandler((sender, e) => timer_Tick(sender, e, D));

                    // normal run
                D.RefreshDash();
                System.Windows.Forms.Application.Run(D.dash);
                // System.Windows.Forms.Application.Exit();
            }
        }
        
        static void timer_Tick(object sender, EventArgs e, Dashboard D)
        {
            D.RefreshDash();
        }

            // a simple utility routine
            // (silly that this isn't a standard method on String)
        static string truncate(string s, int n)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= n ? s : s.Substring(0, n);
        }

            // given a program name, we want to check it in the process
            // table both as "foo" and "foo.exe"; this routine returns
            // the other one -- "foo.exe" if called with "foo" and
            // vice versa
        static string exe_other(string s)
        {
            int n = s.IndexOf(".exe", 0, StringComparison.CurrentCultureIgnoreCase);
            if (n >= 0)
            {
                // note that we're assuming we *end* with .exe, but
                // cut off at the .exe even if it's in the middle
                return truncate(s, n);
            }
            else
            {
                return s + ".exe";
            }
        }

            // given a program name, find out if there are other instances
            // of it running on the system right now
        static int ProgramRunning(string pgm)
        {
            int howmany = System.Diagnostics.Process.GetProcessesByName(pgm).Length;
            string op = exe_other(pgm);
            howmany += System.Diagnostics.Process.GetProcessesByName(op).Length;
            return (howmany);
        }

        static void dashrtf_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string link = e.LinkText;

            doLink(link);
        }

            /*
             * this is here as a placeholder:  we may need to recognize arbitrary
             * mouse clicks to activate links, in which case we will come here.
             * This will require some additional information in the dashboard RTF
             * to tell us the Y-axis of each link.  We'll then have to parse the 
             * RTF to extract that list, which will resolve to the link label,
             * which is the argument to doLink().
             */      
        static void dashrtf_MouseClick(object sender, MouseEventArgs e, Dashboard D)
        {
            string contents = D.RTFcontents();

        }

        static void doLink(string link)
        {
                // this table really needs to be external
            String[] links = {
            "TSG-chest-pain: www.thesullivangroup.com/rsqassist/contents/102_dictation/102_11_chest_pain_male_40_and_over_adult.html",
            "Chest_Pain_Evaluation: www.thesullivangroup.com/rsqassist/contents/102_dictation/102_11_chest_pain_male_40_and_over_adult.html",
            "Chest Pain Evaluation: www.thesullivangroup.com/rsqassist/contents/102_dictation/102_11_chest_pain_male_40_and_over_adult.html",
            "TSG-sore-throat: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            "Sore_Throat_Adult: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            "Sore Throat Adult: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            };

                // strip off the required syntactic sugar
            String[] sugar = { "http://", "http:", "www.", "www" };
            foreach (string ss in sugar)
            {
                if (link.StartsWith(ss)) link = link.Replace(ss, "");
            }
            
                // find the link in the links list
            foreach (string ss in links)
            {
                if (ss.StartsWith(link, StringComparison.CurrentCultureIgnoreCase))
                {
                    Char[] sep = {' ', ':'};
                    String[] elements = ss.Split(sep);
                    int n = elements.Length;
                    link = elements[n-1];
                }
            }

            if (!(link.StartsWith("http://") || link.StartsWith("https://")))
            {
                link = "http://" + link;
            }

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", link);
            }
            catch
            {
                try { 
                    System.Diagnostics.Process.Start("explorer.exe", 
                        "http://alumnus.caltech.edu/~copeland/oops.html" ); 
                }
                catch { throw new NotImplementedException(); }
            }
        }


    } // end program

    

    
}
