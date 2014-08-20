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

namespace EHRNarrative
{
    public partial class EHRNarrative : Form
    {
        private ArrayList keyword_list = null;

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
                string match_str = match.Value.Replace("[", "").Replace("]", "");

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
                command_string += "add " + keyword;
            }
            foreach (string keyword in removed_keywords)
            {
                last_action_label.Text += "Removed " + keyword + "\n";
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "del " + keyword);
                command_string += "del " + keyword;
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
