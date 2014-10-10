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
    public partial class TextDialog : Form
    {
        TextDialog data;
        EHRNarrative narrative_window;

        public TextDialog(EHRNarrative parent, string ehr_keyword)
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

            RenderElements(data);
        }

        private void RenderElements(TextDialog data)
        {
            throw new NotImplementedException();
        }

        private TextDialog LoadContent(string ehr_keyword)
        {
            using (var conn = new SQLiteConnection("Data Source=dialog_content.sqlite3;Version=3;"))
            {
                conn.Open();
                var sql = @"SELECT * FROM #textdialog d INNER JOIN #textelement e ON e.TextDialog_id = d.Id WHERE d.EHR_keyword = " + ehr_keyword;

                var lookup = new Dictionary<int, TextDialog>();
                conn.Query<TextDialog, TextElement, TextDialog>(sql, (textdialog, textelement) => {
                    TextDialog dialog;
                    if (!lookup.TryGetValue(textdialog.Id, out dialog)){
                        lookup.Add(textdialog.Id, dialog = textdialog);
                    }
                    if (dialog.TextElements == null){
                        dialog.TextElements = new List<TextElement>();
                    }
                    dialog.TextElements.Add(textelement);
                    return dialog;
                });

                return (TextDialog)lookup.First().Value;
            }
        }
    }
}
