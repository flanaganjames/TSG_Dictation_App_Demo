using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EHRNarrative
{
    public class EHRListBoxItem 
    {
        private Element _element;

        public EHRListBoxItem(Element element)
        {
            this._element = element;
        }

        public Element Element
        {
            get { return this._element; }
            set { }
        }

        public void drawItem(DrawItemEventArgs e, Padding margin, Font font, StringFormat aligment)
        {

            // if selected, mark the background differently
            if (this.Element.selected == "present")
            {
                e.Graphics.FillRectangle(Brushes.LightGoldenrodYellow, e.Bounds);
            }
            else if (this.Element.selected == "not present")
            {
                //e.Graphics.FillRectangle(Brushes.LightSalmon, e.Bounds);
                e.Graphics.DrawLine(Pens.Black, e.Bounds.X, e.Bounds.Y + e.Bounds.Height, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            }

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal,
                                                 (int)font.GetHeight() + 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this.Element.Name, font, Brushes.Black, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }

    }
    public class EHRListBoxGroup
    {
        private Subgroup _group;

        public EHRListBoxGroup(Subgroup group)
        {
            this._group = group;
        }

        public EHRListBoxGroup()
        {
            
        }
        public String Name
        {
            get
            {
                if (this._group == null)
                    return "More...";
                else
                    return _group.Name;
            }
        }

        public void drawItem(DrawItemEventArgs e, Padding margin, Font font, StringFormat aligment)
        {

            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal,
                                                 (int)font.GetHeight() + 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this.Name, font, Brushes.Black, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }
    }

    public partial class EHRListBox : ListBox
    {
        private StringFormat _fmt;
        private Font _font;

        public EHRListBox(Font font, StringAlignment aligment, StringAlignment lineAligment)
        {
            this._font = font;
            this._fmt = new StringFormat();
            this._fmt.Alignment = aligment;
            this._fmt.LineAlignment = lineAligment;
            SetOptions();
        }

        public EHRListBox()
        {
            InitializeComponent();
            this._fmt = new StringFormat();
            this._fmt.Alignment = StringAlignment.Near;
            this._fmt.LineAlignment = StringAlignment.Center;
            this._font = new Font(this.Font, FontStyle.Bold);
            this.Cursor = Cursors.Hand;
            SetOptions();
        }

        public void AddElements(IEnumerable<Element> elements)
        {
            foreach (Element element in elements)
            {
                this.Items.Add(new EHRListBoxItem(element));
            }
        }
        public void AddGroups(IEnumerable<Subgroup> groups)
        {
            foreach (Subgroup group in groups)
            {
                this.Items.Add(new EHRListBoxGroup(group));
            }
        }
        public void SelectAllNL()
        {
            foreach (var item in this.Items)
            {
                if (item is EHRListBoxItem)
                {
                    EHRListBoxItem listItem = (EHRListBoxItem)item;
                    if (listItem.Element.Is_present_normal)
                        listItem.Element.selected = "present";
                    else
                        listItem.Element.selected = "not present";
                }
            }
            this.Refresh();
        }
        public void ClearAll()
        {
            foreach (var item in this.Items)
            {
                if (item is EHRListBoxItem)
                {
                    EHRListBoxItem listItem = (EHRListBoxItem)item;
                    listItem.Element.selected = null;
                }
            }
            this.Refresh();
        }

        private void SetOptions()
        {
            this.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ItemHeight = Math.Max(30, (int)this._font.GetHeight() + this.Margin.Vertical);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(MouseSelectItem);

            this.BackColor = SystemColors.Control;
            this.BorderStyle = BorderStyle.None;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // prevent from error Visual Designer
            if (this.Items.Count > 0)
            {
                if (this.Items[e.Index] is EHRListBoxItem)
                {
                    EHRListBoxItem item = (EHRListBoxItem)this.Items[e.Index];
                    item.drawItem(e, this.Margin, this._font, this._fmt);
                }
                else if (this.Items[e.Index] is EHRListBoxGroup)
                {
                    EHRListBoxGroup item = (EHRListBoxGroup)this.Items[e.Index];
                    item.drawItem(e, this.Margin, this._font, this._fmt);
                }
            }
        }

        private void MouseSelectItem(object sender, MouseEventArgs e)
        {
            EHRListBoxItem item;
            try
            {
                item = (EHRListBoxItem)this.Items[IndexFromPoint(e.X, e.Y)];
            }
            catch
            {
                return;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (item.Element.selected == null)
                    item.Element.selected = "present";
                else if (item.Element.selected == "present")
                    item.Element.selected = "not present";
                else if (item.Element.selected == "not present")
                    item.Element.selected = null;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (item.Element.selected == null)
                    item.Element.selected = "not present";
                else if (item.Element.selected == "not present")
                    item.Element.selected = "present";
                else if (item.Element.selected == "present")
                    item.Element.selected = null;
            }

            this.Refresh();
        }
    }
}