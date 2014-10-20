using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EHRNarrative
{
    public partial class SubmenuPopover : UserControl
    {
        private EHRListBox _listBox;
        public EHRListBox Listbox { get { return this._listBox; } }
        private EHRListBoxGroup _parentListItem;
        public SubmenuPopover(EHRListBoxGroup parent, Subgroup subgroup, Collection data)
        {
            InitializeComponent();
            this._parentListItem = parent;
            this._listBox = new EHRListBox();
            this._listBox.AddElements(subgroup.Elements(data));
            this._listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkSelection);

            initialize();
        }
        public SubmenuPopover(EHRListBoxGroup parent, IEnumerable<Element> elements)
        {
            InitializeComponent();
            this._parentListItem = parent;
            this._listBox = new EHRListBox();
            this._listBox.AddElements(elements);
            this._listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkSelection);

            initialize();
        }
        private void initialize()
        {
            this.timer1.Stop();

            this._listBox.Height = Math.Min(600, this._listBox.Items.Count * this._listBox.ItemHeight);
            this._listBox.Width = 190;
            this._listBox.Top = 5;
            this._listBox.Left = 5;

            this.Controls.Add(this._listBox);

            this.Width = 200;
            this.Height = this._listBox.Height + 10;
            this.Visible = false;
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.LightGray;

            this.Enabled = true;
        }

        private void checkSelection(object Sender, MouseEventArgs e) { checkSelection(); }
        private void checkSelection() { 
            bool hasSelectedItems = false;
            foreach (var listitem in this._listBox.Items)
            {
                try
                {
                    EHRListBoxItem item = (EHRListBoxItem)listitem;
                    if (item.Element.selected != null)
                        hasSelectedItems = true;
                }
                catch { }
            }
            if (hasSelectedItems)
                this._parentListItem.ChildSelected = true;
            else
                this._parentListItem.ChildSelected = false;

            GroupLabel grouplabel = (GroupLabel)this._parentListItem.Parent.Parent;
            grouplabel.CheckRecommended();
            this.Refresh();

        }

        new public void Hide()
        {
            this.timer1.Start();
        }

        public void HideNow()
        {
            base.Hide();
            this._parentListItem.Parent.SubmenuClosed(this._parentListItem);
        }

        private void HideIfShould()
        {
            if (!ClientRectangle.Contains(PointToClient(Control.MousePosition)) && !this._parentListItem.HasMouse)
            {
                base.Hide();

                this._parentListItem.Parent.SubmenuClosed(this._parentListItem);
            }
        }

        private void SubmenuPopover_MouseLeave(object sender, EventArgs e)
        {
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.HideIfShould();
        }
    }
}
