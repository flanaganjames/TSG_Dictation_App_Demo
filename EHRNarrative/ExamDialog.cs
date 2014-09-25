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
        Collection data;
        EHRNarrative narrative_window;

        public ExamDialog(EHRNarrative parent, string dialog_name, string complaint_name)
        {
            InitializeComponent();
            narrative_window = parent;
            try
            {
                data = LoadContent(dialog_name, complaint_name);
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("Not a valid dialog name or complaint");
                data = null;
                return;
            }

            this.Text = data.dialog.Name;

            RenderElements(data);
        }
        new public void Show() {
            if (data == null)
                return;
            else
                base.Show();
        }

        private Collection LoadContent(string dialog_name, string complaint_name)
        {
            var data = new Collection();
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
                using (var multi = conn.QueryMultiple(sql, new { Dialog = dialog_name, Complaint = complaint_name }))
                {
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
                        throw new NotImplementedException("Not a valid dialog name or complaint");
                    }
                }
                conn.Close();
            }
            return data;
        }

        private void RenderElements(Collection data) {
            int columnWidth = 200;
            int columnGutter = 20;
            int itemHeight = 35;
            int columns = data.dialog.GroupsForComplaint(data).Count();

            int rows;
            if ((columns + 1) * (columnWidth + columnGutter) < System.Windows.Forms.Screen.GetWorkingArea(this).Width)
                rows = 1;
            else
                rows = 2;

            int columnsPerRow = (int)Math.Ceiling((double)columns / (double)rows);

            this.Width = (columnsPerRow + 1) * (columnWidth + columnGutter);
            this.Height = 100 + data.dialog.GroupsForComplaint(data).Select(x => x.ItemCount(data)).Max() * itemHeight * rows;
            this.CenterToScreen();

            foreach (var item in data.dialog.GroupsForComplaint(data).Select((group, i) => new { i, group }))
            {
                //draw headings
                var heading = new GroupLabel();
                heading.Text = item.group.Name;
                heading.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                heading.Top = 4 + this.Height / rows * (int)(item.i / columnsPerRow);
                this.Controls.Add(heading);

                //draw multiselects
                var listbox = new EHRListBox();
                listbox.AddElements(item.group.ElementsForComplaint(data));
                listbox.AddGroups(item.group.Subgroups(data));
                if (item.group.ElementsAdditional(data).Count() > 0)
                    listbox.Items.Add(new EHRListBoxGroup());
                listbox.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                listbox.Top = 25 + heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                listbox.Width = columnWidth;
                listbox.Height = listbox.Items.Count * listbox.ItemHeight;
                this.Controls.Add(listbox);

                //draw select alls
                var button = new SelectAllButton(listbox);
                button.Top = heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                button.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                this.Controls.Add(button);
                var clearbutton = new ClearAllButton(listbox);
                clearbutton.Top = heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                clearbutton.Left = 100 + columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                this.Controls.Add(clearbutton);

            }
            //draw extra group column:
            foreach (var item in data.dialog.GroupsAdditional(data).Select((group, i) => new { i, group }))
            {
                AdditionalGroupsList.Items.Add(item.group.Name);
            }
                
        }

        private void InsertEHRText()
        {
            foreach (IEnumerable<Element> keywordGroup in data.elements
                .Where(x => x.selected != null)
                .OrderBy(x => x.Is_present_normal)
                .GroupBy(x => x.EHR_keyword)
                )
            {
                var keyword = keywordGroup.First().EHR_keyword;
                var EHRString = String.Join("; ", keywordGroup.Select(x => x.EHRString).ToList());
                narrative_window.ReplaceKeyword("[" + keyword + "]/" + EHRString);
            }
        }
        private void UpdateSLC()
        {
            List<String> groupData = new List<String>();
            foreach (Group group in data.groups.Where(x => x.SelectedItemCount(data) > 0))
            {
                groupData.Add("dataqual " + data.dialog.Name + " " + group.Name + " " + group.SelectedItemCount(data).ToString());
            }
            narrative_window.NotifySLC(String.Join(" ! ", groupData));
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            InsertEHRText();
            UpdateSLC();
            this.Close();
        }


    }

    public partial class GroupLabel : Label {
        public GroupLabel()
        {
            Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold, GraphicsUnit.Point);
            ForeColor = SystemColors.WindowFrame;
            Width = 200;
        }
    }
    public partial class SelectAllButton : Button
    {
        private EHRListBox _group;
        public SelectAllButton(EHRListBox group)
        {
            _group = group;
            Text = "All Normal";
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = SystemColors.Highlight;
            TextAlign = ContentAlignment.BottomLeft;
            Width = 100;
            Click += new System.EventHandler(this.SelectAll);
        }
        public void SelectAll(object sender, EventArgs e)
        {
            this._group.SelectAllNL();
        }
    }
    public partial class ClearAllButton : Button
    {
        private EHRListBox _group;
        public ClearAllButton(EHRListBox group)
        {
            _group = group;
            Text = "Clear All";
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = SystemColors.Highlight;
            TextAlign = ContentAlignment.BottomLeft;
            Width = 100;
            Click += new System.EventHandler(this.ClearAll);
        }
        public void ClearAll(object sender, EventArgs e)
        {
            this._group.ClearAll();
        }
    }
}
