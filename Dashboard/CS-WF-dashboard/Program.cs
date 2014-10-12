using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            /*
             * We have three stacked panels: status, E/M, warning.
             * The RTF boxes for them are named dashST, dashEM, dashW,
             * and their heights, positions & sizes are XXXht, XXXpos, XXXsz
             * (where we need explicit variables for them)
             * Overall dash variables for the dash form are just dashXX,
             * dashOXX for the overall size (including non-client elements),
             * and dashTXX for the overall size with warning panel
             */
        public WindowsFormsApplication1.Form1 dash; 
        public RichTextBox dashST, dashEM, dashW;
        public ArrayList dashLinks = new ArrayList();
        private System.Drawing.Size dashOsz, dashTsz, dashWsz;
        private System.Drawing.Point dashWpos;
        private int dashOht, dashOwid, dashht, dashwid;
        private int dashSTht, dashEMht, dashWht;
        private float dashBht;    // dash button height & y-location
                // dashBht is a float, since it's converted to pixels from points
        private int dashBwid;   // dash button width within the status panel
        private float dashResX, dashResY;  // dashboard resolution

        public String dashSTpath = "dashboard.rtf";
        public String dashEMpath = "dashem.rtf";
        public String dashWpath = "dashwarn.rtf";
        public String STmissing = @"{\rtf1\ansi\pard No dashboard available!\par}";
        public String EMmissing = @"{\rtf1\ansi\pard E/M advice unavailable!\par}";
        public String AlreadyRunning = @"{\rtf1\ansi\pard ALREADY RUNNING!!!\par}";

        public String dashSTrtf, dashEMrtf;
        private bool DASHfail = false;
        private int dashboards_running = 0;
        private int updates = 0;
        private bool showing_warning = false;

        // Damn .Net:  there are two distinct RichTextBox classes,
        // with subtly different properties and methods.
        //  We're using System.Windows.Forms.RichTextBox
 
        public Dashboard()
        {
                // create the overall dashboard window 
            dash = new WindowsFormsApplication1.Form1(); // dash = new Form();
            System.Drawing.Graphics g = dash.CreateGraphics();
            dashResX = g.DpiX; dashResY = g.DpiY;

                // these are the fixed dimensions -- others are calculated from these
            dashOht = 565;
            dashOwid = 255;
            dashWht = 200;
            dashEMht = 140;
                //   but first, we need to normalize these to 96 dpi
            dashOht = (int) Math.Ceiling((float) dashOht * dashResY / 96f);
            dashOwid = (int) Math.Ceiling((float) dashOwid * dashResX / 96f);
            dashWht = (int) Math.Ceiling((float) dashWht * dashResY / 96f);
            dashEMht = (int) Math.Ceiling((float) dashEMht * dashResY / 96f);

                // now set up the overall dashboard
            dash.Size = dashOsz = new System.Drawing.Size(dashOwid, dashOht);
            // dash.Visible = true; //  this appears to steal focus
            dash.Text = "The Sullivan Group dashboard";
                    // what's the usable size, minus window decoration? 
            dashht = dash.ClientSize.Height;
            dashwid = dash.ClientSize.Width;
                    // now calculate the RTF panel sizes & positions
            dashSTht = dashht - dashEMht;
                    // need to declare warning panel size & position objects
                    // now, since they're used later, external to the constructor
            dashWpos = new System.Drawing.Point(0, dashht);
            dashWsz = new System.Drawing.Size(dashwid, dashWht);
            dashTsz = new System.Drawing.Size(dashOwid, dashOht + dashWht);

                    // the dashboard status panel
            dashST = new RichTextBox();
            dashST.Location = new System.Drawing.Point(0,0);
            dashST.Size = new System.Drawing.Size(dashwid, dashSTht);
            dashST.ReadOnly = true;
            dashST.DetectUrls = false;
            dashST.BackColor = Color.White;
            dash.Controls.Add(dashST);
            dashBwid = dashST.ClientSize.Width;
            /*
             * For future consideration, we could make the status panel dashSTht+dashEMht
             * high, but not change the position of the E/M panel.  Then we could allow
             * double-clicking on the E/M panel to hide it in case the status panel had
             * more information than we could see at a glance.  This would be the 
             * alternative to scroll bars on the status panel.
             */

                // the E/M panel
            dashEM = new RichTextBox();
            dashEM.Size = new System.Drawing.Size(dashwid, dashEMht);
            dashEM.Location = new System.Drawing.Point(0, dashSTht);
            dashEM.ReadOnly = true;
            dashEM.BackColor = Color.White;
            dash.Controls.Add(dashEM);

                // the warning box -- defined but not instantiated yet
            dashW = new RichTextBox();
            dashW.Size = dashWsz;
            dashW.Location = dashWpos;
            dashW.ReadOnly = true;
            dashW.BackColor = Color.Yellow;

                // tooltip for the E/M panel
            String tooltiptext;
            tooltiptext = "Actual E/M code should be assigned by\n";
            tooltiptext += "billing professionals and based on medical\n";
            tooltiptext += "necessity and is not necessarily based on\n";
            tooltiptext += "levels supported by documentation.";
            ToolTip dashtip = new ToolTip();
            dashtip.SetToolTip(dashEM, tooltiptext);
        }

        public void button_MouseClick(object sender, MouseEventArgs e, String l)
        {
            string contents = dashSTcontents();
            doLink(l);
        }

            // we can invoke failnote if we're already running, for
            // debugging purposes, though normally we'd just exit
        [Conditional("DEBUG")]
        public void failnote()
        {
            DASHfail = true;
        }

            // and another debugging method, which adds a number
            // to window title bar, so we can note how many
            // instances of the program are running, or how
            // many times we've updated
        [Conditional("DEBUG")]
        public void running(int i)
        {
            dashboards_running = i;
            dash.Text = string.Format("Sullivan Group {0}", i);
        }

        [Conditional("DEBUG")]
        public void flash()
        {
            //  .... for test purposes ....
            // flash the background so we know the update loop is running
            this.dash.Refresh();
            this.dashST.BackColor = Color.LightGray;
            this.dashST.Clear(); this.dashST.Refresh(); this.dash.Refresh();
            Thread.Sleep(100);
            this.dashST.BackColor = Color.White;
        }

        public void refreshDash()
        {
            flash();
                // update with the real contents of the main dashboard
            this.dashST.Rtf = dashSTcontents();
            this.dashST.Refresh();
            this.dashEM.Rtf = dashEMcontents();
            this.dashEM.Refresh();
                // update buttons for the links
            refreshDashButtons();
                // handle the warning panel
            refreshDashWarn();
                // now refresh of the whole window
            this.dash.Refresh();

            updates++;  // number of times we've updated
        }

        public void refreshDashButtons()
        {
                // basic parameters
                    // height of the link button
            dashBht = 24f; // 24 half-points is a constant from SLC

                // clear out the old buttons
            foreach (RichTextBox button in dashLinks)
            {
                this.dash.Controls.Remove(button);
                    // notice that we're not deleting the event handler from
                    // the button, just removing the button from the window
            }
            dashLinks.Clear();

                // find links in the dashboard and add buttons for them
            bool have_heights = true;
            Match m = Regex.Match(dashSTrtf, @"HYPERLINK \d+ \}\}\{\\fldrslt\{.+?\}");
            if (!m.Success)
            {
                    // this is bullet-proofing in case we lose the heights of
                    // the links from the dashboard -- we can recalculate them
                m = Regex.Match(dashSTrtf, @"{\\fldrslt\{.+?\}");
                have_heights = false;
            }
            int n = 0;
            while (m.Success)
            {
                    // isolate the link value -- not very bullet proof
                Char [] sep = { ' ', '}' };
                String [] v = m.Value.Split(sep);
                    // 0: "HYPERLINK", 1: height, 2,3,4: markup, 5: link name
                    // or if missing heights: 0: markup, 1: link name, 2: markup
                int height = have_heights
                    ? Int32.Parse(v[1]) : height = 85 + n * (int)dashBht;
                String link = have_heights ? v[5] : v[1];
                String t = @"{\rtf1\ansi"
                    + @"{\fonttbl{\f0\fswiss Verdana;}{\f1\froman Times New Roman;}}"
                    + @"{\colortbl;\red0\green0\blue238;}"
                    + @"\ul\f0\cf1\fs20\li100 " + link + @"\par}";
                        // using the HTML5 recommended color for unvisited links;
                        // wikipedia uses a slightly different one: @"\red6\green69\blue173;"
                    // make a button out of it
                RichTextBox button = new RichTextBox();
                dashLinks.Add(button);
                SizeF ss = new SizeF(dashBwid, dashBht / 144 * dashResY);
                PointF pp = new PointF(0f, (float)height / 144 * dashResY); 
                button.Size = System.Drawing.Size.Round(ss);
                button.Location = System.Drawing.Point.Round(pp);
                button.BackColor = Color.White;
                    //!!! ForeColor doesn't count: need to select color in the RTF, so we need a colormap
                button.ForeColor = Color.Blue;
                button.Cursor = Cursors.Hand;
                button.BorderStyle = BorderStyle.None;
                button.ReadOnly = true;
                button.Rtf = t;
                button.Refresh();
                dashST.Controls.Add(button);
                button.MouseClick += new MouseEventHandler((sender, e) => button_MouseClick(sender, e, link));
                n++;
                m = m.NextMatch();
            }
        }

        public void refreshDashWarn()
        {
            // are we showing the warning box, but shouldn't?
            if (showing_warning && !File.Exists(dashWpath))
            {
                // remove warning box and resize the dashboard
                this.dash.Size = dashOsz;
                this.dash.Controls.Remove(dashW);
                showing_warning = false;
            }
            // are we not showing the warning box, but should?
            if (!showing_warning && File.Exists(dashWpath))
            {
                // expand the dashboard and add the warning box
                this.dash.Size = dashTsz;
                this.dash.Controls.Add(dashW);
                showing_warning = true;
            }
            // update the warning box contents if necessary
            if (File.Exists(dashWpath))
            {
                this.dashW.Rtf = File.ReadAllText(this.dashWpath);
                this.dashW.Refresh();
            }
        }

        public string dashSTcontents()
        {
            if (File.Exists(this.dashSTpath))  // we should only be reading the file if it's been updated
            {
                dashSTrtf = File.ReadAllText(this.dashSTpath);
            }
            else
            {
                dashSTrtf = this.STmissing;
            }
            if (DASHfail) dashSTrtf = this.AlreadyRunning;
            return dashSTrtf;
        }

        public string dashEMcontents()
        {
            if (File.Exists(this.dashEMpath))  // we should only be reading the file if it's been updated
            {
                dashEMrtf = File.ReadAllText(this.dashEMpath);
            }
            else
            {
                dashEMrtf = this.EMmissing;
            }
            if (DASHfail) dashEMrtf = this.AlreadyRunning;
            return dashEMrtf;
        }


        void doLink(string link)
        {
            // this table really needs to be external to the program
            String[] links = {
            "TSG chest pain: www.thesullivangroup.com/rsqassist/contents/102_dictation/102_11_chest_pain_male_40_and_over_adult.html",
            "Chest Pain Evaluation: www.thesullivangroup.com/rsqassist/contents/102_dictation/102_11_chest_pain_male_40_and_over_adult.html",
            "TSG sore throat: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            "Sore Throat Adult: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            "Sore Throat Evaluation: www.thesullivangroup.com/rsqassist/contents/024_sore_throat_and_toothache/024_009_sore_throat_toothache_adult_resources.html",
            "Chest Pain Resources: file:001_chest_pain_myocardial_infarction_and_thrombolysis/001_007_chest_pain_resources.html",
            "Differential Diagnosis Tool: file:001_chest_pain_myocardial_infarction_and_thrombolysis/001_006_chest_pain_interactive_differential_diagnosis.html",
            "RSQ Assist: http://tsg-demo/rsqassist/",
            "TAD risk: file:054_bp_ebm/054_002_thoracic_aortic_dissection.html",
            };
            String badLink = "file:oops.html";
            String localFiles = "file:///C:/TEMP/Sullivan/RSQ_Files_05.06.2014/";
            String resolvedLink = "";

                // strip off the required syntactic sugar
            String[] sugar = { "http://", "http:", "www.", "www" };
            foreach (string ss in sugar)
            {
                if (link.StartsWith(ss)) link = link.Replace(ss, "");
            }
                // strip out underscores and dashes
            link = link.Replace('-', ' ');
            link = link.Replace('_', ' ');
            link += ":";  // ensure delimiter for comparision

            // find the link in the links list
            foreach (string ss in links)
            {
                if (ss.StartsWith(link, StringComparison.CurrentCultureIgnoreCase))
                {
                    int i = ss.IndexOf(':');
                    resolvedLink = ss.Substring(i + 1).Trim();
                    break;
                }
            }

            if (resolvedLink.Length == 0)
                resolvedLink = badLink;

            if (resolvedLink.StartsWith("file:"))
            {
                int i = resolvedLink.IndexOf(':');
                resolvedLink = resolvedLink.Substring(i + 1);
                resolvedLink = localFiles + resolvedLink;
            }

            if (!(resolvedLink.StartsWith("http://")
                || resolvedLink.StartsWith("https://")
                || resolvedLink.StartsWith("file://")))
            {
                resolvedLink = "http://" + resolvedLink;
            }

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", resolvedLink);
            }
            catch
            {
                throw new NotImplementedException();
            }
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

                    // update timer event
                    // (like with the RichTextBox class, annoyingly there are
                    //  three distinct Timer classes, so we fully qualify it)
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Enabled = true;
                timer.Interval = 1000;
                timer.Tick += new EventHandler((sender, e) => timer_Tick(sender, e, D));

                    // normal run
                D.refreshDash();
                System.Windows.Forms.Application.Run(D.dash);
                // System.Windows.Forms.Application.Exit();
            }
        }

        static void timer_Tick(object sender, EventArgs e, Dashboard D)
        {
            D.refreshDash();
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

    } // end program

    

    
}
