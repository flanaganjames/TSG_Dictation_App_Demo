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
                var sql = @"
                    SELECT * FROM dialog d WHERE d.name = @Dialog;
                    SELECT * FROM 'group';
                    SELECT * FROM subgroup;
                    SELECT * FROM element;
                    SELECT * FROM group_complaints;
                    SELECT * FROM group_complaint_groups;
                    SELECT * FROM element_complaints;
                    SELECT * FROM element_complaint_groups;
                    SELECT * FROM complaintGroup;
                    SELECT * FROM complaint c WHERE c.name = @Complaint";
                using (var multi = conn.QueryMultiple(sql,  new { Dialog = dialog_name, Complaint = complaint_name }))
                {
                    var data = new Collection();
                    try
                    {
                        data = new Collection
                        {
                            dialog = multi.Read<Dialog>().Single(),
                            groups = multi.Read<Group>().ToList(),
                            subgroups = multi.Read<Subgroup>().ToList(),
                            elements = multi.Read<Element>().ToList(),
                            group_complaints = multi.Read<Group_Complaints>().ToList(),
                            group_complaint_groups = multi.Read<Group_Complaint_Groups>().ToList(),
                            element_complaints = multi.Read<Element_Complaints>().ToList(),
                            element_complaint_groups = multi.Read<Element_Complaint_Groups>().ToList(),
                            complaintgroups = multi.Read<ComplaintGroup>().ToList(),
                            complaint = multi.Read<Complaint>().Single()
                        };
                    }
                    catch
                    {
                        MessageBox.Show("Not a valid dialog name or complaint");
                        this.Close();
                        return;
                    }


                    this.Text = data.dialog.Name;

                    // print names
                    richTextBox1.Text += data.complaint.Name + "\n";

                    foreach (Group group in data.dialog.GroupsForComplaint(data))
                    {
                        richTextBox1.Text += "\n" + group.Name + "\n";
                        richTextBox1.Text += "  -" + String.Join(" \n  -", group.ElementsForComplaint(data).Select(x => x.Name).ToList());
                        foreach (Subgroup subgroup in group.Subgroups(data))
                        {
                            richTextBox1.Text += "\n  =" + subgroup.Name + "\n";
                            richTextBox1.Text += "    -" + String.Join(" \n    -", subgroup.Elements(data).Select(x => x.Name).ToList());
                        }
                        richTextBox1.Text += "\n  =More:\n";
                        richTextBox1.Text += "    -" + String.Join(" \n  -", group.Elements(data).Except( group.ElementsForComplaint(data)).Select(x => x.Name).ToList());
                    }
                    richTextBox1.Text += "\n\nHidden Groups:";
                    foreach (Group group in data.dialog.Groups(data).Except( data.dialog.GroupsForComplaint(data)))
                    {
                        richTextBox1.Text += "\n" + group.Name + "\n";
                        richTextBox1.Text += "  -" + String.Join(" \n  -", group.ElementsForComplaint(data).Select(x => x.Name).ToList());
                        foreach (Subgroup subgroup in group.Subgroups(data))
                        {
                            richTextBox1.Text += "\n  =" + subgroup.Name + "\n";
                            richTextBox1.Text += "    -" + String.Join(" \n    -", subgroup.Elements(data).Select(x => x.Name).ToList());
                        }
                    }
                }
            }
        }
    }

    public class Collection
    {
        public Dialog dialog { get; set; }
        public IEnumerable<Group> groups { get; set; }
        public IEnumerable<Subgroup> subgroups { get; set; }
        public IEnumerable<Element> elements { get; set; }
        public IEnumerable<ComplaintGroup> complaintgroups { get; set; }
        public Complaint complaint { get; set; }
        public IEnumerable<Group_Complaints> group_complaints { get; set; }
        public IEnumerable<Group_Complaint_Groups> group_complaint_groups { get; set; }
        public IEnumerable<Element_Complaints> element_complaints { get; set; }
        public IEnumerable<Element_Complaint_Groups> element_complaint_groups { get; set; }
    }

    public class Dialog {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Group> Groups (Collection data) {
            return data.groups.Where(x => x.Dialog_id == this.Id);
        }
        public IEnumerable<Group> GroupsForComplaint(Collection data)
        {
            return this.Groups(data).Where(
                       x => x.All_complaints
                    || x.Complaints(data).Contains(data.complaint.Id) 
                    || x.ComplaintGroups(data).Contains(data.complaint.Complaintgroup_id));
        }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Dialog_id { get; set; }

        public List<int> Complaints(Collection data)
        {
            return data.group_complaints.Where(x => x.Group_id == this.Id).Select(x => x.Complaint_id).ToList();
        }
        public List<int> ComplaintGroups(Collection data)
        {
            return data.group_complaint_groups.Where(x => x.Group_id == this.Id).Select(x => x.Complaint_group_id).ToList();
        }

        public IEnumerable<Subgroup> Subgroups(Collection data)
        {
            return data.subgroups.Where(x => x.Group_id == this.Id);
        }
        public IEnumerable<Element> Elements(Collection data)
        {
            return data.elements.Where(x => x.Group_id == this.Id);
        }

        public bool All_complaints { get; set; }

        public IEnumerable<Element> ElementsForComplaint(Collection data)
        {
            return this.Elements(data).Where(
                       x => x.All_complaints 
                    || x.Complaints(data).Contains(data.complaint.Id) 
                    || x.ComplaintGroups(data).Contains(data.complaint.Complaintgroup_id));
        }
    }
    public class Subgroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Group_id { get; set; }

        public IEnumerable<Element> Elements(Collection data)
        {
            return data.elements.Where(x => x.Subgroup_id == this.Id);
        }

    }
    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Group_id { get; set; }
        public int Subgroup_id { get; set; }
        public List<int> Complaints(Collection data)
        {
            return data.element_complaints.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_id).ToList();
        }
        public List<int> ComplaintGroups(Collection data)
        {
            return data.element_complaint_groups.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_group_id).ToList();
        }

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
        public int Id { get; set; }
        public string Name { get; set; }
        public int Complaintgroup_id { get; set; }
    }

    public class Group_Complaints
    {
        public int Group_id { get; set; }
        public int Complaint_id { get; set; }
    }
    public class Group_Complaint_Groups
    {
        public int Group_id { get; set; }
        public int Complaint_group_id { get; set; }
    }
    public class Element_Complaints
    {
        public int Element_id { get; set; }
        public int Complaint_id { get; set; }
    }
    public class Element_Complaint_Groups
    {
        public int Element_id { get; set; }
        public int Complaint_group_id { get; set; }
    }

}
