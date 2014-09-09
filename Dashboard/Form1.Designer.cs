namespace Dashboard
{
    partial class Dashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            this.ComplaintLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IncompleteListBox = new System.Windows.Forms.ListBox();
            this.CompletedListBox = new System.Windows.Forms.ListBox();
            this.requiredProgressBar = new System.Windows.Forms.ProgressBar();
            this.RecommendedProgressBar = new System.Windows.Forms.ProgressBar();
            this.ResourceLinks = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // ComplaintLabel
            // 
            this.ComplaintLabel.AutoSize = true;
            this.ComplaintLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComplaintLabel.Location = new System.Drawing.Point(12, 9);
            this.ComplaintLabel.Name = "ComplaintLabel";
            this.ComplaintLabel.Size = new System.Drawing.Size(166, 18);
            this.ComplaintLabel.TabIndex = 0;
            this.ComplaintLabel.Text = "No current complaint";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Required";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(12, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Incomplete";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(12, 283);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Completed";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(12, 363);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Recommended";
            // 
            // IncompleteListBox
            // 
            this.IncompleteListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.IncompleteListBox.FormattingEnabled = true;
            this.IncompleteListBox.Location = new System.Drawing.Point(15, 208);
            this.IncompleteListBox.Name = "IncompleteListBox";
            this.IncompleteListBox.Size = new System.Drawing.Size(212, 65);
            this.IncompleteListBox.TabIndex = 7;
            // 
            // CompletedListBox
            // 
            this.CompletedListBox.BackColor = System.Drawing.SystemColors.Menu;
            this.CompletedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CompletedListBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CompletedListBox.FormattingEnabled = true;
            this.CompletedListBox.Location = new System.Drawing.Point(15, 305);
            this.CompletedListBox.Name = "CompletedListBox";
            this.CompletedListBox.Size = new System.Drawing.Size(212, 52);
            this.CompletedListBox.TabIndex = 8;
            // 
            // requiredProgressBar
            // 
            this.requiredProgressBar.Location = new System.Drawing.Point(15, 138);
            this.requiredProgressBar.Name = "requiredProgressBar";
            this.requiredProgressBar.Size = new System.Drawing.Size(212, 23);
            this.requiredProgressBar.TabIndex = 9;
            // 
            // RecommendedProgressBar
            // 
            this.RecommendedProgressBar.Location = new System.Drawing.Point(15, 384);
            this.RecommendedProgressBar.Name = "RecommendedProgressBar";
            this.RecommendedProgressBar.Size = new System.Drawing.Size(212, 23);
            this.RecommendedProgressBar.TabIndex = 10;
            // 
            // ResourceLinks
            // 
            this.ResourceLinks.AutoSize = true;
            this.ResourceLinks.Location = new System.Drawing.Point(13, 31);
            this.ResourceLinks.Name = "ResourceLinks";
            this.ResourceLinks.Size = new System.Drawing.Size(55, 13);
            this.ResourceLinks.TabIndex = 11;
            this.ResourceLinks.TabStop = true;
            this.ResourceLinks.Text = "linkLabel1";
            this.ResourceLinks.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ResourceLinks_LinkClicked);
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 511);
            this.Controls.Add(this.ResourceLinks);
            this.Controls.Add(this.RecommendedProgressBar);
            this.Controls.Add(this.requiredProgressBar);
            this.Controls.Add(this.CompletedListBox);
            this.Controls.Add(this.IncompleteListBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComplaintLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Dashboard";
            this.Text = "RSQ© Guidance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ComplaintLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox IncompleteListBox;
        private System.Windows.Forms.ListBox CompletedListBox;
        private System.Windows.Forms.ProgressBar requiredProgressBar;
        private System.Windows.Forms.ProgressBar RecommendedProgressBar;
        private System.Windows.Forms.LinkLabel ResourceLinks;
    }
}

