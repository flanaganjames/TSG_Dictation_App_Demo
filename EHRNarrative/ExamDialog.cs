using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Simple.Data;

namespace EHRNarrative
{
    public partial class ExamDialog : Form
    {
        public ExamDialog(string dialog_name, string complaint_name)
        {
            InitializeComponent();
            var db = Database.OpenConnection("Data Source=dialog_content.sqlite3;Version=3;");

            IEnumerable<Dialog> dialogs = db.Dialogs.FindAllByName(dialog_name).WithMany(db.Dialogs.Group).WithMany(db.Dialogs.Group.Element);
            if (dialogs.Count() == 0)
            {
                MessageBox.Show("Not a valid dialog name");
                this.Close();
                return;
            }

            Dialog dialog = dialogs.First();
            foreach (Group group in dialog.Group)
            {
                group.Element = db.Elements.FindAllByGroup_id(group.Group_id).Where(db.Elements.Subgroup_id == null);
                group.Subgroups = db.Subgroups.FindAllByGroup_id(group.Group_id).WithMany(db.Subgroups.Elements);
            }

            Complaint complaint = db.Complaints.Find(db.Complaints.Name == complaint_name);


            this.Text = dialog.Name;

            // print names
            richTextBox1.Text += complaint.Name + "\n";

            foreach (Group group in dialog.Group)
            {
                richTextBox1.Text += "\n" + group.Name + "\n";
                richTextBox1.Text += "  -" + String.Join(" \n  -", group.Element.Where(x => x.All_complaints).Select(x => x.Name).ToList());
                foreach (Subgroup subgroup in group.Subgroups)
                {
                        richTextBox1.Text += "\n  =" + subgroup.Name + "\n";
                        richTextBox1.Text += "    -" + String.Join(" \n    -", subgroup.Elements.Select(x => x.Name).ToList());
                }
            }
        }
    }

    public class Dialog {
        public int Dialog_id { get; set; }
        public string Name { get; set; }
        public IList<Group> Group { get; set; }
    }

    public class Group
    {
        public int Group_id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Element> Element { get; set; }
        public IEnumerable<Subgroup> Subgroups { get; set; }

        public bool All_complaints { get; set; }

    }
    public class Subgroup
    {
        public int Subgroup_id { get; set; }
        public string Name { get; set; }
        public int Group_id { get; set; }
        public IList<Element> Elements { get; set; }

    }
    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Group_id { get; set; }
        public int Subgroup_id { get; set; }
        public IList<Complaint> Complaints { get; set; }

        public bool All_complaints { get; set; }
        public string EHR_keyword { get; set; }
        public bool Is_present_normal { get; set; }
        public bool Default_present { get; set; }
        public string Present_text { get; set; }
        public string Not_present_text { get; set; }
    }

    public class ComplaintGroup 
    {
        public int Complaintgroup_id { get; set; }
        public string Name { get; set; }
        public IList<Complaint> Complaint { get; set; }
    }
    public class Complaint
    {
        public int Complaint_id { get; set; }
        public string Name { get; set; }
        public int Complaintgroup_id { get; set; }
    }
}
