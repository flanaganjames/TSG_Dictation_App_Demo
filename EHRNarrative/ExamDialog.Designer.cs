namespace EHRNarrative
{
    partial class ExamDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ehrListBox1 = new EHRListBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(260, 237);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // ehrListBox1
            // 
            this.ehrListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ehrListBox1.FormattingEnabled = true;
            this.ehrListBox1.ItemHeight = 66;
            this.ehrListBox1.Location = new System.Drawing.Point(12, 91);
            this.ehrListBox1.Name = "ehrListBox1";
            this.ehrListBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ehrListBox1.Size = new System.Drawing.Size(260, 95);
            this.ehrListBox1.TabIndex = 1;
            // 
            // ExamDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.ehrListBox1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "ExamDialog";
            this.Text = "ExamDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private EHRListBox ehrListBox1;


    }
}