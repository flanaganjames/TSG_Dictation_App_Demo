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
            this.AdditionalGroups = new System.Windows.Forms.Panel();
            this.DoneButton = new System.Windows.Forms.Button();
            this.AdditionalGroupsList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AdditionalGroups.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(259, 236);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // AdditionalGroups
            // 
            this.AdditionalGroups.BackColor = System.Drawing.SystemColors.Control;
            this.AdditionalGroups.Controls.Add(this.DoneButton);
            this.AdditionalGroups.Controls.Add(this.AdditionalGroupsList);
            this.AdditionalGroups.Controls.Add(this.label1);
            this.AdditionalGroups.Dock = System.Windows.Forms.DockStyle.Right;
            this.AdditionalGroups.Location = new System.Drawing.Point(140, 0);
            this.AdditionalGroups.Name = "AdditionalGroups";
            this.AdditionalGroups.Size = new System.Drawing.Size(200, 337);
            this.AdditionalGroups.TabIndex = 1;
            // 
            // DoneButton
            // 
            this.DoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DoneButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoneButton.Location = new System.Drawing.Point(7, 311);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(181, 23);
            this.DoneButton.TabIndex = 2;
            this.DoneButton.Text = "Done";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // AdditionalGroupsList
            // 
            this.AdditionalGroupsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AdditionalGroupsList.BackColor = System.Drawing.SystemColors.Control;
            this.AdditionalGroupsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AdditionalGroupsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdditionalGroupsList.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.AdditionalGroupsList.FormattingEnabled = true;
            this.AdditionalGroupsList.ItemHeight = 18;
            this.AdditionalGroupsList.Location = new System.Drawing.Point(7, 26);
            this.AdditionalGroupsList.Name = "AdditionalGroupsList";
            this.AdditionalGroupsList.Size = new System.Drawing.Size(181, 270);
            this.AdditionalGroupsList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Additional Groups:";
            // 
            // ExamDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 337);
            this.Controls.Add(this.AdditionalGroups);
            this.Controls.Add(this.richTextBox1);
            this.Name = "ExamDialog";
            this.Text = "ExamDialog";
            this.AdditionalGroups.ResumeLayout(false);
            this.AdditionalGroups.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel AdditionalGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox AdditionalGroupsList;
        private System.Windows.Forms.Button DoneButton;

    }
}