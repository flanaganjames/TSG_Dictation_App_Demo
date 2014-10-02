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
        private EHRListBoxGroup _parentListItem;
        public SubmenuPopover(EHRListBoxGroup parent, Subgroup subgroup, Collection data)
        {
            InitializeComponent();
            this._parentListItem = parent;
            this._listBox = new EHRListBox();
            this._listBox.AddElements(subgroup.Elements(data));
            this.Controls.Add(this._listBox);

            this.Width = 220;
            this.Height = 400;
            this.Visible = false;
        }
        public SubmenuPopover(EHRListBoxGroup parent, IEnumerable<Element> elements)
        {
            InitializeComponent();
            this._parentListItem = parent;
            this._listBox = new EHRListBox();
            this._listBox.AddElements(elements);
            this.Controls.Add(this._listBox);

            this.Width = 220;
            this.Height = 400;
            this.Visible = false;
        }
    }
}
