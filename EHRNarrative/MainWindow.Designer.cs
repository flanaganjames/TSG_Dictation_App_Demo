namespace EHRNarrative
{
    partial class EHRNarrative
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EHRNarrative));
            this.HealthRecordText = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.current_label = new System.Windows.Forms.Label();
            this.last_action_label = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // HealthRecordText
            // 
            this.HealthRecordText.AcceptsTab = true;
            this.HealthRecordText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HealthRecordText.Location = new System.Drawing.Point(12, 12);
            this.HealthRecordText.Name = "HealthRecordText";
            this.HealthRecordText.Size = new System.Drawing.Size(760, 538);
            this.HealthRecordText.TabIndex = 0;
            this.HealthRecordText.Text = "";
            this.HealthRecordText.TextChanged += new System.EventHandler(this.HealthRecordText_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.last_action_label);
            this.panel1.Controls.Add(this.current_label);
            this.panel1.Location = new System.Drawing.Point(547, 390);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(225, 160);
            this.panel1.TabIndex = 1;
            // 
            // current_label
            // 
            this.current_label.AutoSize = true;
            this.current_label.Location = new System.Drawing.Point(3, 12);
            this.current_label.Name = "current_label";
            this.current_label.Size = new System.Drawing.Size(93, 13);
            this.current_label.TabIndex = 0;
            this.current_label.Text = "Current Keywords:";
            // 
            // last_action_label
            // 
            this.last_action_label.AutoSize = true;
            this.last_action_label.Location = new System.Drawing.Point(102, 12);
            this.last_action_label.Name = "last_action_label";
            this.last_action_label.Size = new System.Drawing.Size(63, 13);
            this.last_action_label.TabIndex = 1;
            this.last_action_label.Text = "Last Action:";
            // 
            // EHRNarrative
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.HealthRecordText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EHRNarrative";
            this.Text = "Electronic Health Record Narrative";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HealthRecordText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label last_action_label;
        private System.Windows.Forms.Label current_label;
    }
}

