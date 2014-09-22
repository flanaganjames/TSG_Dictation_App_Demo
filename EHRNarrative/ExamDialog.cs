using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.Common;

using Dapper;

namespace EHRNarrative
{
    public partial class ExamDialog : Form
    {
        public ExamDialog(string dialog_name, string complaint_name)
        {
            InitializeComponent();
            using (var conn = new SQLiteConnection("Data Source=dialog_content.sqlite3;Version=3;"))
            {
                conn.Open();
                var lookup = new Dictionary<int, Dialog>();
                conn.Query<Dialog, Group, Dialog>(@"
                    SELECT * FROM dialog d
                    INNER JOIN 'group' g ON d.id = g.dialog_id
                    WHERE d.name = @Name",
                    (d, g) => {
                        Dialog dl;
                        if (!lookup.TryGetValue(d.Id, out dl)){
                            lookup.Add(d.Id, dl = d);
                        }
                        if (dl.Group == null)
                            dl.Group = new List<Group>();
                        dl.Group.Add(g);
                        return dl;
                    },
                    new { Name = dialog_name });//.FirstOrDefault();

                Dialog dialog = lookup.First().Value;

                if (lookup.Values.Count() < 1)
                {
                    MessageBox.Show("Not a valid dialog name");
                    this.Close();
                    return;
                }

                Complaint complaint = conn.Query<Complaint>(@"
                    SELECT * FROM complaint c WHERE c.name = @Name",
                    new { Name = complaint_name }).FirstOrDefault();

                foreach (Group group in dialog.Group)
                {
                    var subgroup_lookup = new Dictionary<int, Subgroup>();
                    conn.Query<Subgroup, Element, Subgroup>(@"
                        SELECT * FROM subgroup s
                        INNER JOIN element e ON s.id + e.subgroup_id
                        WHERE s.group_id = @Group",
                        (s, e) => {
                            Subgroup sg;
                            if (!subgroup_lookup.TryGetValue(s.Id, out sg)){
                                subgroup_lookup.Add(s.Id, sg = s);
                            }
                            if (sg.Elements == null)
                                sg.Elements = new List<Element>();
                            sg.Elements.Add(e);
                            return sg;
                        },
                    new { Group = group.Id });
                    group.Subgroups = subgroup_lookup.Values.ToList();
                    group.Element = conn.Query<Element>(@"
                        SELECT * from element e 
                        WHERE e.group_id = @Group",
                        new { Group = group.Id });
                }


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
    }

    public class Dialog {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Group> Group { get; set; }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Element> Element { get; set; }
        public IList<Subgroup> Subgroups { get; set; }

        public bool All_complaints { get; set; }

    }
    public class Subgroup
    {
        public int Id { get; set; }
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
