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
            catch (NotImplementedException e)
            {
                MessageBox.Show(e.Message);
                data = null;
                return;
            } 
            catch (BadImageFormatException) 
            {
                MessageBox.Show("Missing file: dialog_content.sqlite3");
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
                        if (conn.Query(@"SELECT * FROM dialog d WHERE d.name = @Dialog", new {Dialog=dialog_name}).Count() != 1)
                            throw new NotImplementedException("\"" + dialog_name + "\" is not a valid dialog name");
                        else if (conn.Query(@"SELECT * FROM complaint c WHERE c.name = @Complaint", new { Complaint = complaint_name }).Count() != 1)
                            throw new NotImplementedException("\"" + complaint_name + "\" is not a valid complaint");
                        throw;
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
            if ((columns) * (columnWidth + columnGutter) < System.Windows.Forms.Screen.GetWorkingArea(this).Width)
                rows = 1;
            else
                rows = 2;

            
            int maxListBoxHeight = System.Windows.Forms.Screen.GetWorkingArea(this).Height/rows - 100;
            int maxListBoxItems = data.dialog.GroupsForComplaint(data).Select(x => x.ItemCount(data)).Max();

            int columnsPerRow = (int)Math.Ceiling((double)columns / (double)rows);

            this.Width = (columnsPerRow) * (columnWidth + columnGutter) + columnGutter*2;
            this.Height = (85 + Math.Min(maxListBoxItems * itemHeight, maxListBoxHeight)) * rows;
            this.CenterToScreen();

            foreach (var item in data.dialog.GroupsForComplaint(data).Select((group, i) => new { i, group }))
            {
                //create listbox for this group
                var listbox = new EHRListBox();
                item.group.SetAllDefaults(data);
                listbox.AddElements(item.group.ElementsForComplaint(data));
                listbox.AddGroups(item.group.Subgroups(data), data);
                if (item.group.ElementsAdditional(data).Count() > 0)
                    listbox.Items.Add(new EHRListBoxGroup(item.group.ElementsAdditional(data)));
                
                //draw headings
                var heading = new GroupLabel(listbox, item.group);
                heading.Text = item.group.Name;
                heading.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                heading.Top = 4 + this.Height / rows * (int)(item.i / columnsPerRow);
                this.Controls.Add(heading);

                //position listbox under heading
                //listbox.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                //listbox.Top = 25 + heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                //listbox.Width = columnWidth;
                //listbox.Height = Math.Min(listbox.Items.Count * listbox.ItemHeight, maxListBoxHeight);

                //draw select alls
                var button = new SelectAllButton(listbox);
                //button.Top = heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                //button.Left = columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                heading.Controls.Add(button);

                var clearbutton = new ClearAllButton(listbox);
                //clearbutton.Top = heading.Height + this.Height / rows * (int)(item.i / columnsPerRow);
                //clearbutton.Left = 100 + columnGutter + (item.i % columnsPerRow) * (columnWidth + columnGutter);
                heading.Controls.Add(clearbutton);
            }

            //draw extra group column:
            //foreach (var item in data.dialog.GroupsAdditional(data).Select((group, i) => new { i, group }))
            //{
            //    AdditionalGroupsList.Items.Add(item.group.Name);
            //}
                
        }

        private void InsertEHRText()
        {
            foreach (IEnumerable<Element> keywordGroup in data.elements
                .Where(x => x.selected != null)
                .OrderBy(x => x.normal)
                .GroupBy(x => x.EHR_keyword)
                )
            {
                var keyword = keywordGroup.First().EHR_keyword;
                var EHRString = String.Join("; ", keywordGroup.Select(x => x.EHRString).ToList());
                narrative_window.ReplaceKeyword("[" + keyword + "]/" + EHRString);
                narrative_window.ReplaceKeyword("[\\cf2 " + keyword + "\\cf1 ]/" + EHRString);
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

    public partial class GroupLabel : GroupBox {
        private EHRListBox _listBox;
        private Group _group;

        public EHRListBox ListBox
        {
            get { return this._listBox; }
            set { }
        }

        public GroupLabel(EHRListBox ListBox, Group group)
        {
            Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold, GraphicsUnit.Point);
            ForeColor = SystemColors.WindowFrame;
            Width = 200;

            this._listBox = ListBox;
            this._group = group;
            this.Controls.Add(this._listBox);
        }

        public void CheckRecommended()
        {
            this._group.RecommendedActive = false;

            foreach (var item in this._listBox.Items)
            {
                if (item is EHRListBoxItem)
                {
                    EHRListBoxItem itemItem = (EHRListBoxItem)item;

                    if (itemItem.Element.Recommended && itemItem.Element.selected == null)
                    {
                        this._group.RecommendedActive = true;
                    }
                }
                else if (item is EHRListBoxGroup)
                {
                    EHRListBoxGroup groupItem = (EHRListBoxGroup)item;

                    //TODO: Search elements in the subgroup and see if we need to mark this group as recommended?
                }
            }

            //set group coloring if either the group is recommended or any Elements are actively recommended
            if (this._group.Recommended && this._group.RecommendedActive)
                this.ForeColor = Color.FromName("DarkRed");
            else if (this._group.Recommended && !this._group.RecommendedActive)
                this.ForeColor = Color.FromName("DarkSlateBlue");

            this.Refresh();
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
