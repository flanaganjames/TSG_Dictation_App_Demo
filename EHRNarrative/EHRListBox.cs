using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            Brush textColor = Brushes.Black;
            // if selected, mark the background differently
            if (this.Element.selected == "present")
            {
                e.Graphics.FillRectangle(Brushes.LightSkyBlue, e.Bounds);
            }
            else if (this.Element.selected == "not present")
            {
                //e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                e.Graphics.DrawLine(Pens.DarkSlateGray, e.Bounds.X, e.Bounds.Y + e.Bounds.Height, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
                textColor = Brushes.DarkSlateGray;
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            }

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);


            if (this.Element.Recommended && this.Element.RecommendedActive == true)
            {
                e.Graphics.DrawImage(Image.FromFile("Assets/exclamation.png"), new Rectangle(e.Bounds.X + 2, e.Bounds.Y + e.Bounds.Height / 2 - 7, 14, 14));
                textColor = Brushes.DarkRed;
            }
            else if (this.Element.Recommended)
            {
                e.Graphics.DrawImage(Image.FromFile("Assets/checkmark.png"), new Rectangle(e.Bounds.X + 2, e.Bounds.Y + e.Bounds.Height / 2 - 7, 14, 14));

                textColor = Brushes.DarkSlateBlue;
            }


            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal + 10,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal - 10,
                                                 (int)font.GetHeight() * 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this.Element.Name, font, textColor, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }

    }

    public class EHRListBoxTextItem
    {
        private TextElement _element;
        public TextElement Element { get { return this._element; } }

        public EHRListBoxTextItem(TextElement element)
        {
            this._element = element;
        }

        public void drawItem(DrawItemEventArgs e, Padding margin, Font font, StringFormat aligment)
        {
            Brush textColor = Brushes.Black;
            if (this.Element.selected)
            {
                e.Graphics.FillRectangle(Brushes.LightSkyBlue, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            }

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal + 10,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal - 10,
                                                 (int)font.GetHeight() * 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this.Element.Title, font, textColor, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }
    }

    public class EHRListBoxGroup
    {
        public bool HasMouse { get; set; }
        public bool ChildSelected { get; set; }

        private EHRListBox _parent;
        public EHRListBox Parent
        {
            get { return this._parent; }
            set { }
        }

        private Subgroup _group;

        private SubmenuPopover _popover;
        public SubmenuPopover Popover
        {
            get { return this._popover; }
            set { }
        }

        private Rectangle _bounds;
        public Rectangle Bounds
        {
            get { return this._bounds; }
            set { }
        }

        public EHRListBoxGroup(Subgroup group, Collection data, EHRListBox parent)
        {
            this._group = group;
            this._popover = new SubmenuPopover(this, this._group, data);

            this._parent = parent;
        }
        public EHRListBoxGroup(IEnumerable<Element> elements, EHRListBox parent)
        {
            this._popover = new SubmenuPopover(this, elements);

            this._parent = parent;
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
            this._bounds = e.Bounds;
            var textcolor = Brushes.DimGray;
            Brush backcolor = SystemBrushes.Control;
            if (this.Popover.Visible)
            {
                backcolor = Brushes.LightGray;
            }
            e.Graphics.FillRectangle(backcolor, e.Bounds);

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            if (this.ChildSelected)
            {
                textcolor = Brushes.CornflowerBlue;
                e.Graphics.DrawImage(Image.FromFile("Assets/right-chevron-blue.png"), new Rectangle(e.Bounds.Right - 16, e.Bounds.Y + e.Bounds.Height / 2 - 7, 16, 16));
            }
            else
            {
                e.Graphics.DrawImage(Image.FromFile("Assets/right-chevron.png"), new Rectangle(e.Bounds.Right - 16, e.Bounds.Y + e.Bounds.Height / 2 - 7, 16, 16));
            }

            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal + 10,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal - 10,
                                                 (int)font.GetHeight() * 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this.Name, font, textcolor, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }
    }
        
    public class EHRListBoxDialogLink
    {
        
        private DialogLinkElement _element;
        public int DialogId { get { return this._element.Linked_dialog_id; } set { } }

        private Rectangle _bounds;
        public Rectangle Bounds
        {
            get { return this._bounds; }
            set { }
        }

        public EHRListBoxDialogLink(DialogLinkElement element)
        {
            this._element = element;
        }
        public void drawItem(DrawItemEventArgs e, Padding margin, Font font, StringFormat aligment)
        {
            this._bounds = e.Bounds;
            var backcolor = SystemBrushes.Control;
            e.Graphics.FillRectangle(backcolor, e.Bounds);

            // draw some item separator
            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            // calculate bounds for title text drawing
            Rectangle textBounds = new Rectangle(e.Bounds.X + margin.Horizontal + 10,
                                                 e.Bounds.Y + margin.Top,
                                                 e.Bounds.Width - margin.Right - margin.Horizontal - 10,
                                                 (int)font.GetHeight() * 2);

            // draw the text within the bounds
            e.Graphics.DrawString(this._element.Name, font, Brushes.DimGray, textBounds, aligment);

            // put some focus rectangle
            e.DrawFocusRectangle();
        }
    }

    public partial class EHRListBox : ListBox
    {
        private StringFormat _fmt;
        private Font _font;
        private EHRListBoxGroup displayedGroup;
        private EHRListBoxTextItem hoveredTextItem;
        private ToolTip toolTip;

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
        public void AddElements(IEnumerable<DialogLinkElement> elements)
        {
            foreach (DialogLinkElement element in elements)
            {
                this.Items.Add(new EHRListBoxDialogLink(element));
            }
        }
        public void AddElements(IEnumerable<TextElement> elements)
        {
            foreach (TextElement element in elements)
            {
                this.Items.Add(new EHRListBoxTextItem(element));
            }
        }
        public void AddGroups(IEnumerable<Subgroup> groups, Collection data)
        {
            foreach (Subgroup group in groups)
            {
                this.Items.Add(new EHRListBoxGroup(group, data, this));
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
                        listItem.Element.selected = null;

                    if (listItem.Element.Recommended && listItem.Element.selected != null)
                        listItem.Element.RecommendedActive = false;
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
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(MouseHoverItem);
            this.MouseLeave += new System.EventHandler(LeaveMenu);

            this.BackColor = SystemColors.Control;
            this.BorderStyle = BorderStyle.None;

            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 25000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;
            toolTip.IsBalloon = true;

            toolTip.Active = false;

            this.displayedGroup = null;
            this.hoveredTextItem = null;
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
                else if (this.Items[e.Index] is EHRListBoxDialogLink)
                {
                    EHRListBoxDialogLink item = (EHRListBoxDialogLink)this.Items[e.Index];
                    item.drawItem(e, this.Margin, this._font, this._fmt);
                }
                else if (this.Items[e.Index] is EHRListBoxTextItem)
                {
                    EHRListBoxTextItem item = (EHRListBoxTextItem)this.Items[e.Index];
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
                EHRListBoxDialogLink linkItem;
                try
                {
                    linkItem = (EHRListBoxDialogLink)this.Items[IndexFromPoint(e.X, e.Y)];
                    ExamDialog form = (ExamDialog)this.FindForm();
                    new ExamDialog(form.narrative_window, form.data.dialogs.Where(x => x.Id == linkItem.DialogId).First().Name, form.data.complaint.Name).Show();
                    return;
                }
                catch
                {
                    EHRListBoxTextItem textItem;
                    try
                    {
                        textItem = (EHRListBoxTextItem)this.Items[IndexFromPoint(e.X, e.Y)];
                        textItem.Element.selected = !textItem.Element.selected;
                        this.Refresh();
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
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

        private void MouseHoverItem(object sender, MouseEventArgs e)
        {

            EHRListBoxGroup group;
            try
            {
                group = (EHRListBoxGroup)this.Items[IndexFromPoint(e.X, e.Y)];
                group.HasMouse = true;
                this.toolTip.Active = false;
                this.hoveredTextItem = null;

                if (this.displayedGroup != group && this.displayedGroup != null)
                {
                    this.displayedGroup.HasMouse = false;
                    this.displayedGroup.Popover.HideNow();
                }

                if (this.displayedGroup == null || this.displayedGroup != group)
                {
                    this.displayedGroup = group;

                    Point screenCoords = this.PointToScreen(Point.Empty);
                    Point absoluteCoords = new Point(screenCoords.X - this.FindForm().Location.X-8, screenCoords.Y - this.FindForm().Location.Y-32);

                    int rightEdge = absoluteCoords.X + this.Width;
                    int topEdge = absoluteCoords.Y + group.Bounds.Top;

                    int left = (rightEdge + group.Popover.Width) < this.FindForm().Width ? rightEdge : absoluteCoords.X - group.Popover.Width;
                    int top = Math.Max(10, topEdge + group.Bounds.Height / 2 - group.Popover.Height / 2);
                    if (top + group.Popover.Height > this.FindForm().Height - 10)
                        top = this.FindForm().Height - group.Popover.Height - 10;

                    group.Popover.Location = new System.Drawing.Point(left, top);
                    this.FindForm().Controls.Add(group.Popover);
                    group.Popover.BringToFront();
                    group.Popover.Show();

                    this.Refresh();
                }
            }
            catch
            {
                if (this.displayedGroup != null)
                {
                    this.displayedGroup.HasMouse = false;
                    this.displayedGroup.Popover.Hide();
                }

                try
                {
                    EHRListBoxTextItem textItem = (EHRListBoxTextItem)this.Items[IndexFromPoint(e.X, e.Y)];
                    if (textItem != this.hoveredTextItem)
                    {
                        this.hoveredTextItem = textItem;
                        this.toolTip.SetToolTip(this, Utils.FormatStringToWidth(textItem.Element.Content, 100));
                        this.toolTip.Active = true;
                    }
                }
                catch
                {
                    this.toolTip.Active = false;
                    this.hoveredTextItem = null;
                }
            }
        }

        public void SubmenuClosed(EHRListBoxGroup group)
        {
            if (this.displayedGroup == group)
            {
                this.displayedGroup = null;
                this.Refresh();
            }
        }

        private void LeaveMenu(object sender, EventArgs e)
        {
            if (this.displayedGroup != null)
            {
                this.displayedGroup.HasMouse = false;
                this.displayedGroup.Popover.Hide();
            }
        }
    }
}