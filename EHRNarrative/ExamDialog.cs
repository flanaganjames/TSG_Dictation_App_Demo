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
        public Collection data;
        public EHRNarrative narrative_window;
        private List<AccordianRow> rows;

        public List<AccordianRow> Rows
        {
            get { return this.rows; }
            set { }
        }

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

            RenderDialog(data);
        }
        new public int Show() {
            if (data == null)
            {
                this.Dispose();
                return 0;
            }
            else
            {
                base.Show();
                this.BringToFront();
                return (int)this.Handle;
            }
        }

        private Collection LoadContent(string dialog_name, string complaint_name)
        {
            var data = new Collection();
            using (var conn = new SQLiteConnection("Data Source=dialog_content.sqlite3;Version=3;"))
            {
                conn.Open();
                var sql = @"
                    SELECT * FROM dialog d WHERE d.name = @Dialog;
                    SELECT * FROM dialog;
                    SELECT * FROM 'group';
                    SELECT * FROM subgroup;
                    SELECT * FROM element;
                    SELECT * FROM dialogelement;
                    SELECT * FROM textelement;
                    SELECT * FROM group_complaints;
                    SELECT * FROM group_complaint_groups;
                    SELECT * FROM element_complaints;
                    SELECT * FROM element_complaint_groups;
                    SELECT * FROM dialogelement_complaints;
                    SELECT * FROM dialogelement_complaint_groups;
                    SELECT * FROM complaintGroup;
                    SELECT * FROM complaint c WHERE c.name = @Complaint";
                using (var multi = conn.QueryMultiple(sql, new { Dialog = dialog_name, Complaint = complaint_name }))
                {
                    try
                    {
                        data = new Collection
                        {
                            dialog = multi.Read<Dialog>().Single(),
                            dialogs = multi.Read<Dialog>().ToList(),
                            groups = multi.Read<Group>().ToList(),
                            subgroups = multi.Read<Subgroup>().ToList(),
                            elements = multi.Read<Element>().ToList(),
                            dialoglinkelements = multi.Read<DialogLinkElement>().ToList(),
                            textelements = multi.Read<TextElement>().ToList(),
                            group_complaints = multi.Read<Group_Complaints>().ToList(),
                            group_complaint_groups = multi.Read<Group_Complaint_Groups>().ToList(),
                            element_complaints = multi.Read<Element_Complaints>().ToList(),
                            element_complaint_groups = multi.Read<Element_Complaint_Groups>().ToList(),
                            dialogelement_complaints = multi.Read<DialogElement_Complaints>().ToList(),
                            dialogelement_complaint_groups = multi.Read<DialogElement_Complaint_Groups>().ToList(),
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

            foreach (int group_id in data.elements.Where(x => x.Recommended).Select(x => x.Group_id).Distinct())
            {
                data.groups.Where(x => x.Id == group_id).First().Recommended = true;
            }
            foreach (int subgroup_id in data.elements.Where(x => x.Recommended && x.Subgroup_id > 0).Select(x => x.Subgroup_id).Distinct())
            {
                data.subgroups.Where(x => x.Id == subgroup_id).First().Recommended = true;
            }
            foreach (Element element in data.dialog.AllElements(data))
            {
                element.Dialog = data.dialog;
            }

            return data;
        }

        private void RenderDialog(Collection data) {
            int columnWidth = 200;
            int columnGutter = 20;
            int columns = data.dialog.Is_text_dialog ? data.dialog.Groups(data).Count() : data.dialog.GroupsForComplaint(data).Count();
            int columnsPerRow = (System.Windows.Forms.Screen.GetWorkingArea(this).Width - columnGutter-40) / (columnWidth + columnGutter);

            this.Width = (Math.Min(columns, columnsPerRow)) * (columnWidth + columnGutter) + columnGutter*2;

            rows = new List<AccordianRow>();

            if (data.dialog.Is_text_dialog)
            {
                foreach (var item in data.dialog.Groups(data).Select((group, i) => new { i, group }))
                {
                    RenderTextGroupListbox(rows, item.group, item.i, columnsPerRow, columnGutter, columnWidth);
                }
            }
            else
            {
                foreach (var item in data.dialog.GroupsForComplaint(data).Select((group, i) => new { i, group }))
                {
                    RenderGroupListbox(rows, item.group, item.i, columnsPerRow, columnGutter, columnWidth);
                }
            }

            rows[0].Height = rows[0].MaxHeight;

            int lastBottom = 0;
            foreach (AccordianRow row in rows)
            {
                row.Width = columnsPerRow * (columnWidth + columnGutter) + columnGutter;
                row.Dock = DockStyle.Top;
                lastBottom += row.Height;

                this.Controls.Add(row);
                this.Controls.SetChildIndex(row, 0);
            }


            if (!data.dialog.Is_text_dialog && data.dialog.GroupsAdditional(data).Count() > 0)
            {
                Label addnlGroupLabel = RenderAddnlLabel(columnGutter);
                this.Controls.Add(addnlGroupLabel);
                this.Controls.SetChildIndex(addnlGroupLabel, 0);

                lastBottom += addnlGroupLabel.Height;

                List<AccordianRow> addnlRows = new List<AccordianRow>();

                foreach (var item in data.dialog.GroupsAdditional(data).Select((group, i) => new { i, group }))
                {
                    RenderGroupListbox(addnlRows, item.group, item.i, columnsPerRow, columnGutter, columnWidth);
                }

                foreach (AccordianRow row in addnlRows)
                {
                    row.Width = columnsPerRow * (columnWidth + columnGutter) + columnGutter;
                    row.Dock = DockStyle.Top;
                    lastBottom += row.Height;

                    this.Controls.Add(row);
                    this.Controls.SetChildIndex(row, 0);
                }
                rows.AddRange(addnlRows);
            }

            RenderButtonBar();
                
            this.Height = lastBottom + (rows.Max(x => x.MaxHeight) - rows[0].Height) + 150;
            this.CenterToScreen();
        }

        private void RenderGroupListbox(List<AccordianRow> rows, Group group, int i, int columnsPerRow, int columnGutter, int columnWidth) {
            int row = i / columnsPerRow;
            AccordianRow currentPanel;

            try
            {
                currentPanel = rows[row];
            }
            catch
            {
                rows.Add(new AccordianRow());
                currentPanel = rows[row];
            }

            //create listbox for this group
            var listbox = new EHRListBox();
            group.SetAllDefaults(data);
            listbox.AddElements(group.ElementsForComplaint(data));
            listbox.AddElements(group.DialogLinkElementsForComplaint(data));
            listbox.AddGroups(group.Subgroups(data), data);
            if (group.ElementsAdditional(data).Count() + group.DialogLinkElementsAdditional(data).Count() > 0)
                listbox.Items.Add(new EHRListBoxGroup(group.ElementsAdditional(data), listbox));

            //draw headings
            var heading = new GroupLabel(listbox, group);
            heading.Heading = group.Name;
            heading.Top = 15;
            heading.Left = columnGutter + (i % columnsPerRow) * (columnWidth + columnGutter);
            heading.Height = listbox.Height + listbox.Top + 15;
            currentPanel.MaxHeight = Math.Max(currentPanel.MaxHeight, heading.Height);
            currentPanel.Controls.Add(heading);
        }

        private void RenderTextGroupListbox(List<AccordianRow> rows, Group group, int i, int columnsPerRow, int columnGutter, int columnWidth)
        {
            int row = i / columnsPerRow;
            AccordianRow currentPanel;

            try
            {
                currentPanel = rows[row];
            }
            catch
            {
                rows.Add(new AccordianRow());
                currentPanel = rows[row];
            }

            //create listbox for this group
            var listbox = new EHRListBox();
            listbox.AddElements(group.TextElements(data));

            //draw headings
            var heading = new GroupLabel(listbox, group);
            heading.HideButtons();
            heading.Heading = group.Name;
            heading.Top = 15;
            heading.Left = columnGutter + (i % columnsPerRow) * (columnWidth + columnGutter);
            heading.Height = listbox.Height + listbox.Top + 15;
            currentPanel.MaxHeight = Math.Max(currentPanel.MaxHeight, heading.Height);
            currentPanel.Controls.Add(heading);
        }

        private Label RenderAddnlLabel(int columnGutter)
        {
            var addnlGroupLabel = new Label();
            addnlGroupLabel.Text = "    Additional Groups:";
            addnlGroupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            addnlGroupLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            addnlGroupLabel.Height = 50;
            addnlGroupLabel.TextAlign = ContentAlignment.BottomLeft;
            addnlGroupLabel.Dock = DockStyle.Top;
            return addnlGroupLabel;
        }

        private void RenderButtonBar()
        {
            Panel buttonBar = new Panel();
            buttonBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonBar.Top = this.ClientSize.Height - 50;
            buttonBar.Left = 0;
            buttonBar.Height = 43;
            buttonBar.Width = this.ClientSize.Width;

            Button DoneButton = new Button();
            DoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            DoneButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DoneButton.Name = "DoneButton";
            DoneButton.Size = new System.Drawing.Size(181, 23);
            DoneButton.Location = new System.Drawing.Point((buttonBar.Width - DoneButton.Width - 10), 10);
            DoneButton.TabIndex = 2;
            DoneButton.Text = "Save and close";
            DoneButton.UseVisualStyleBackColor = true;
            DoneButton.Click += new System.EventHandler(this.DoneButton_Click);

            if (data.dialog.NextDialog(data) != null)
            {
                Button NextButton = new Button();
                NextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                NextButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                NextButton.Name = "NextButton";
                NextButton.Text = "Save and continue to " + data.dialog.NextDialog(data).Name;
                Size textSize = TextRenderer.MeasureText(NextButton.Text, NextButton.Font);
                NextButton.Size = new System.Drawing.Size(textSize.Width + 30, 23);
                NextButton.Location = new System.Drawing.Point((buttonBar.Width - NextButton.Width - 10), 10);
                NextButton.TabIndex = 3;
                NextButton.UseVisualStyleBackColor = true;
                NextButton.Tag = data.dialog.NextDialog(data).Name; // TODO: actually subclass the button to have a proper property
                NextButton.Click += new System.EventHandler(this.NextButton_Click);

                DoneButton.Location = new System.Drawing.Point((buttonBar.Width - DoneButton.Width - NextButton.Width - 20), 10);
                buttonBar.Controls.Add(NextButton);
            }

            buttonBar.Controls.Add(DoneButton);

            if (data.dialog.GroupsForComplaint(data).Count() > 2)
            {
                Button AllNormalButton = new Button();
                AllNormalButton.Text = "All Normal";
                AllNormalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                AllNormalButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                AllNormalButton.Size = new System.Drawing.Size(130, 23);
                AllNormalButton.Location = new System.Drawing.Point(10, 10);
                AllNormalButton.Click += new System.EventHandler(this.AllNormalButton_Click);
                buttonBar.Controls.Add(AllNormalButton);

                Button ClearAllButton = new Button();
                ClearAllButton.Text = "Clear All";
                ClearAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                ClearAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ClearAllButton.Size = new System.Drawing.Size(130, 23);
                ClearAllButton.Location = new System.Drawing.Point(10 + AllNormalButton.Width + 10, 10);
                ClearAllButton.Click += new System.EventHandler(this.ClearAllButton_Click);
                buttonBar.Controls.Add(ClearAllButton);
            }

            this.Controls.Add(buttonBar);
        }

        private void InsertEHRText()
        {
            bool textInserted = false;

            foreach (IEnumerable<Element> keywordGroup in data.elements
                .Where(x => x.selected != null)
                .OrderBy(x => x.normal)
                .GroupBy(x => x.EHR_keyword)
                )
            {
                var keyword = keywordGroup.First().EHR_keyword;
                var EHRString = String.Join("; ", keywordGroup.Select(x => x.EHRString).ToList());
                if (narrative_window.ReplaceKeyword("[" + keyword + "]/" + EHRString) || narrative_window.ReplaceKeyword("[\\cf2 " + keyword + "\\cf1 ]/" + EHRString))
                    textInserted = true;
            }

            foreach (String EHR_text in data.elements
                .Where(x => x.selected != null && x.EHR_replace != null && x.EHR_replace != "")
                .Select(x => x.EHR_replace).Distinct()
                )
            {
                try
                {
                    if (narrative_window.ReplaceKeyword(EHR_text))
                        textInserted = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error occured while trying to replace text using this command: \"" + EHR_text + "\"\nError Message:\n" + e.Message);
                }
            }

            foreach (IEnumerable<TextElement> keywordGroup in data.textelements
                .Where(x => x.selected)
                .OrderBy(x => x.Order)
                .GroupBy(x => x.EHR_keyword)
                )
            {
                var keyword = keywordGroup.First().EHR_keyword;
                var EHRString = String.Join("  \\n", keywordGroup.Select(x => x.Content).ToList());
                if (narrative_window.ReplaceKeyword("[" + keyword + "]/" + EHRString) || narrative_window.ReplaceKeyword("[\\cf2 " + keyword + "\\cf1 ]/" + EHRString))
                    textInserted = true;
            }

            if (textInserted)
                narrative_window.NextField();
        }

        private void UpdateSLC()
        {
            List<String> groupData = new List<String>();
            foreach (Group group in data.groups.Where(x => x.SelectedItemCount(data) > 0))
            {
                groupData.Add("dataqual " + data.dialog.Name + " " + group.Name + " " + group.SelectedItemCount(data).ToString());
            }

            foreach (String SLC_command in data.elements
                .Where(x => x.selected != null && x.SLC_command != null && x.SLC_command != "")
                .Select(x => x.SLC_command).Distinct()
                )
            {
                groupData.Add(SLC_command);
            }

            narrative_window.NotifySLC(String.Join(" ! ", groupData));

        }

        private void AllNormalButton_Click(object sender, EventArgs e)
        {
            foreach (Group group in data.dialog.GroupsForComplaint(data))
            {
                foreach (Element element in group.ElementsForComplaint(data))
                {
                    if (element.Is_present_normal)
                    {
                        element.selected = "present";

                        if (element.Recommended && element.selected != null)
                            element.RecommendedActive = false;
                    }
                }
            }
            CheckAllRecommended();
            this.Refresh();
        }
        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            foreach (Group group in data.dialog.GroupsForComplaint(data))
            {
                foreach (Element element in group.ElementsForComplaint(data))
                {
                    element.selected = null;

                    if (element.Recommended)
                        element.RecommendedActive = true;
                }
            }
            CheckAllRecommended();
            this.Refresh();
        }
        private void CheckAllRecommended()
        {
            foreach (var control in this.Controls)
            {
                if (control is AccordianRow)
                {
                    AccordianRow row = (AccordianRow)control;
                    foreach (GroupLabel group in row.Controls)
                    {
                        group.CheckRecommended();
                    }
                }
            }
        }
        private void DoneButton_Click(object sender, EventArgs e)
        {
            InsertEHRText();
            UpdateSLC();
            this.Close();
            this.Dispose();
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            DoneButton_Click(sender, e);
            new ExamDialog(this.narrative_window, (string)button.Tag, data.complaint.Name).Show();
        }
    }

    public partial class AccordianRow : Panel
    {
        public int MaxHeight { get; set; }
        public int MinHeight = 35;
        public bool Opened { get; set; }

        public AccordianRow()
        {
            this.Opened = false;
            this.Height = this.MinHeight;
        }

        public void Open()
        {
            ExamDialog d = (ExamDialog)this.Parent;

            foreach (AccordianRow row in d.Rows)
            {
                row.Close();
            }

            this.Height = this.MaxHeight;
            this.Opened = true;
        }

        public void Close()
        {
            this.Height = MinHeight;
            this.Opened = false;
        }
    }

    public partial class GroupLabel : Panel {
        private EHRListBox _listBox;
        private Group _group;

        private Label heading;
        private Label icon;

        public string Heading
        {
            get { return this.heading.Text; }
            set { this.heading.Text = value; }
        }

        public EHRListBox ListBox
        {
            get { return this._listBox; }
            set { }
        }

        public GroupLabel(EHRListBox ListBox, Group group)
        {
            int maxListBoxHeight = System.Windows.Forms.Screen.GetWorkingArea(this).Height - 280; // minus window chrome, headings, button bar, allowance for addnl rows

            this.Width = 200;
            this.BorderStyle = BorderStyle.None;

            icon = new Label();
            icon.Width = 16;
            icon.Height = 16;
            icon.Top = 1;
            icon.Visible = false;
            this.Controls.Add(icon);

            heading = new Label();
            heading.Width = this.Width;
            heading.Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold, GraphicsUnit.Point);
            heading.ForeColor = Color.DarkSlateGray;
            heading.AutoEllipsis = true;
            this.Controls.Add(heading);

            this._listBox = ListBox;
            this._group = group;

            this._listBox.Width = this.Width;
            this._listBox.Height = Math.Min(this._listBox.Items.Count * this._listBox.ItemHeight, maxListBoxHeight);
            this._listBox.Top = 45;

            //draw select alls
            var selectAllButton = new SelectAllButton(this._listBox);
            selectAllButton.Top = 18;
            selectAllButton.Left = 0;
            this.Controls.Add(selectAllButton);

            var clearbutton = new ClearAllButton(this._listBox);
            clearbutton.Top = 18;
            clearbutton.Left = 100;
            this.Controls.Add(clearbutton);

            this.heading.Click += new System.EventHandler(this.ClickHeader);
            this._listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkRecommended);
            selectAllButton.Click += new System.EventHandler(this.checkRecommended);
            clearbutton.Click += new System.EventHandler(this.checkRecommended);

            this.Controls.Add(this._listBox);

            CheckRecommended();
        }
        public void HideButtons()
        {
            this.Controls.Remove(this.Controls.Find("SelectAll", false).First());
            this.Controls.Remove(this.Controls.Find("ClearAll", false).First());
        }

        private void ClickHeader(object Sender, EventArgs e)
        {
            AccordianRow row = (AccordianRow)this.Parent;

            row.Open();
        }

        private void checkRecommended(object Sender, MouseEventArgs e) { CheckRecommended(); }
        private void checkRecommended(object Sender, EventArgs e) { CheckRecommended(); }

        public void CheckRecommended()
        {
            if (!this._group.Recommended) return;
            

            heading.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            icon.Visible = true;
            
            this._group.RecommendedActive = false;

            List<Element> elements = new List<Element>();
            List<EHRListBoxGroup> subgroups = new List<EHRListBoxGroup>();
            foreach (var item in this._listBox.Items)
            {
                if (item is EHRListBoxItem)
                {
                    EHRListBoxItem itemItem = (EHRListBoxItem)item;
                    elements.Add(itemItem.Element);
                }
                else if (item is EHRListBoxGroup)
                {
                    EHRListBoxGroup itemGroup = (EHRListBoxGroup)item;
                    subgroups.Add(itemGroup);
                    try
                    {
                        itemGroup.Subgroup.RecommendedActive = false;
                    }
                    catch { }
                    foreach (EHRListBoxItem itemItem in itemGroup.Popover.Listbox.Items)
                    {
                        elements.Add(itemItem.Element);
                    }
                }
            }
            if (elements.Where(x => x.Recommended).Any())
            {
                if (elements.Where(x => x.Recommended && x.selected == null).Any())
                {
                    IEnumerable<Element> recommendedElements = elements.Where(x => x.Recommended);
                    IEnumerable<String> keywords = recommendedElements.Select(x => x.EHR_keyword).Distinct();
                    foreach (string keyword in keywords)
                    {
                        IEnumerable<Element> elementsForKeyword = recommendedElements.Where(x => x.EHR_keyword == keyword);
                        if (elementsForKeyword.Where(x => x.selected != null).Any())
                        {
                            foreach (Element element in elementsForKeyword)
                            {
                                element.RecommendedActive = false;
                            }
                        }
                        else
                        {
                            this._group.RecommendedActive = true;

                            foreach (Element element in elementsForKeyword)
                            {
                                element.RecommendedActive = true;
                                try
                                {
                                    subgroups.Where(x => x.Subgroup.Id == element.Subgroup_id).FirstOrDefault().Subgroup.RecommendedActive = true;
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            else
            {
                IEnumerable<String> keywords = elements.Select(x => x.EHR_keyword).Distinct();
                foreach (string keyword in keywords)
                {
                    bool thereAreNoSelectedElementsForThisKeyword = !elements.Where(x => x.EHR_keyword == keyword && x.selected != null).Any();
                    if (thereAreNoSelectedElementsForThisKeyword)
                        this._group.RecommendedActive = true;
                }
            }

            //set group coloring if either the group is recommended or any Elements are actively recommended
            if (this._group.Recommended && this._group.RecommendedActive)
            {
                heading.ForeColor = Color.FromName("DarkRed");
                icon.Image = Image.FromFile("Assets/exclamation.png");
            }
            else if (this._group.Recommended && !this._group.RecommendedActive)
            {
                heading.ForeColor = Color.FromName("DarkSlateGray");
                icon.Image = Image.FromFile("Assets/checkmark.png");
            }

            this.Refresh();
        }
    }

    public partial class SelectAllButton : Button
    {
        private EHRListBox _group;
        public SelectAllButton(EHRListBox group)
        {
            _group = group;
            Text = "Normal";
            Name = "SelectAll";
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.CornflowerBlue;
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
            Name = "ClearAll";
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.CornflowerBlue;
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
