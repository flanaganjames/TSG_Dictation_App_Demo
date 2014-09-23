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
            set { this._element = value; }
        }

        public void drawItem(DrawItemEventArgs e, Padding margin, Font font, StringFormat aligment)
        {

            // if selected, mark the background differently
            if (this.Element.selected == "present")
            {
                e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);
            }
            else if (this.Element.selected == "not present")
            {
                e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            // draw some item separator
            e.Graphics.DrawLine(Pens.DarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

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
            this._fmt.LineAlignment = StringAlignment.Near;
            this._font = new Font(this.Font, FontStyle.Bold);
            SetOptions();
        }

        private void SetOptions()
        {
            this.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ItemHeight = (int)this._font.GetHeight() + this.Margin.Vertical;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(MouseSelectItem);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // prevent from error Visual Designer
            if (this.Items.Count > 0)
            {
                EHRListBoxItem item = (EHRListBoxItem)this.Items[e.Index];
                item.drawItem(e, this.Margin, this._font, this._fmt);
            }
        }

        // TODO: Mouse event
        private void MouseSelectItem(object sender, MouseEventArgs e)
        {
            EHRListBoxItem item = (EHRListBoxItem)this.Items[IndexFromPoint(e.X, e.Y)];

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                item.Element.selected = "present";
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                item.Element.selected = "not present";
            }

            this.Refresh();
        }
    }
}