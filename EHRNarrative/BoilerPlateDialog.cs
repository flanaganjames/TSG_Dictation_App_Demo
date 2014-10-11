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
    public partial class BoilerPlateDialog : Form
    {
        TextDialog data;
        EHRNarrative narrative_window;

        public BoilerPlateDialog(EHRNarrative parent, string ehr_keyword)
        {
            InitializeComponent();
            narrative_window = parent;

            try
            {
                data = LoadContent(ehr_keyword);
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

            this.Text = data.Name;

            Render(data.TextElements);
        }

        new public int Show()
        {
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

        private void Render(IList<TextElement> elements)
        {
            int gutterWidth = 20;
            int maxWidth = 500;
            int minWidth = 200;
            int maxHeight = 600;

            int columns = elements.Count();
            int columnWidth = (System.Windows.Forms.Screen.GetWorkingArea(this).Width - gutterWidth - 16) / columns - gutterWidth;
            columnWidth = Math.Max(Math.Min(columnWidth, maxWidth), minWidth);

            this.Width = gutterWidth + columns * (columnWidth + gutterWidth) + 16;

            int biggestHeight = 0;
            foreach (var e in elements.Select((element, i) => new { i, element }))
            {
                TextBox text = new TextBox();
                text.Top = 8;
                text.Left = gutterWidth + e.i * (columnWidth + gutterWidth);
                text.Width = columnWidth;
                text.Multiline = true;
                text.ReadOnly = true;
                text.BorderStyle = BorderStyle.Fixed3D;
                text.Cursor = System.Windows.Forms.Cursors.Hand;
                text.Text = e.element.boiler_plate;
                text.Select(0, 0);
                text.Height = TextRenderer.MeasureText(text.Text, text.Font, new Size(text.Width, maxHeight), TextFormatFlags.WordBreak).Height;

                text.Click += new EventHandler(text_Click);
                text.MouseEnter += new EventHandler(text_MouseEnter);
                text.MouseLeave += new EventHandler(text_MouseLeave);
                
                this.Controls.Add(text);

                biggestHeight = Math.Max(biggestHeight, text.Height);
            }

            this.Height = biggestHeight + 56;
            this.CenterToScreen();
        }

        void text_MouseLeave(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.BackColor = SystemColors.Control;
        }

        void text_MouseEnter(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.BackColor = SystemColors.ControlLight;
        }

        void text_Click(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            narrative_window.ReplaceKeyword(this.data.EHR_keyword + "/" + t.Text);
            this.Close();
            this.Dispose();
        }

        private TextDialog LoadContent(string ehr_keyword)
        {
            using (var conn = new SQLiteConnection("Data Source=dialog_content.sqlite3;Version=3;"))
            {
                conn.Open();
                var sql = @"SELECT * FROM 'textdialog' d INNER JOIN 'textelement' e ON e.TextDialog_id = d.Id WHERE d.EHR_keyword = @keyword";

                var lookup = new Dictionary<int, TextDialog>();
                conn.Query<TextDialog, TextElement, TextDialog>(sql, (textdialog, textelement) =>
                {
                    TextDialog dialog;
                    if (!lookup.TryGetValue(textdialog.Id, out dialog))
                    {
                        lookup.Add(textdialog.Id, dialog = textdialog);
                    }
                    if (dialog.TextElements == null)
                    {
                        dialog.TextElements = new List<TextElement>();
                    }
                    dialog.TextElements.Add(textelement);
                    return dialog;
                }, new { keyword = ehr_keyword });

                return (TextDialog)lookup.First().Value;
            }
        }
    }
}
