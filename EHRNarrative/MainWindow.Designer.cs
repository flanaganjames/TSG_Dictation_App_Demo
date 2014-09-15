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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EHRNarrative));
            this.HealthRecordText = new System.Windows.Forms.RichTextBox();
            this.dashboardTimer = new System.Windows.Forms.Timer(this.components);
            this.check_button = new System.Windows.Forms.Button();
            this.ignore_warnings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HealthRecordText
            // 
            this.HealthRecordText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.HealthRecordText.Location = new System.Drawing.Point(12, 12);
            this.HealthRecordText.Name = "HealthRecordText";
            this.HealthRecordText.Size = new System.Drawing.Size(760, 505);
            this.HealthRecordText.TabIndex = 0;
            this.HealthRecordText.Text = "";
            this.HealthRecordText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HealthRecordText_KeyUp);
            // 
            // dashboardTimer
            // 
            this.dashboardTimer.Enabled = true;
            this.dashboardTimer.Interval = 2000;
            this.dashboardTimer.Tick += new System.EventHandler(this.dashboardTimer_Tick);
            // 
            // check_button
            // 
            this.check_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.check_button.Location = new System.Drawing.Point(653, 523);
            this.check_button.Name = "check_button";
            this.check_button.Size = new System.Drawing.Size(120, 30);
            this.check_button.TabIndex = 1;
            this.check_button.TabStop = false;
            this.check_button.Text = "Check for warnings";
            this.check_button.UseVisualStyleBackColor = true;
            this.check_button.Click += new System.EventHandler(this.check_button_Click);
            // 
            // ignore_warnings
            // 
            this.ignore_warnings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ignore_warnings.Location = new System.Drawing.Point(653, 523);
            this.ignore_warnings.Name = "ignore_warnings";
            this.ignore_warnings.Size = new System.Drawing.Size(120, 30);
            this.ignore_warnings.TabIndex = 2;
            this.ignore_warnings.TabStop = false;
            this.ignore_warnings.Text = "Ignore warnings";
            this.ignore_warnings.UseVisualStyleBackColor = true;
            this.ignore_warnings.Visible = false;
            this.ignore_warnings.Click += new System.EventHandler(this.ignore_warnings_Click);
            // 
            // EHRNarrative
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ignore_warnings);
            this.Controls.Add(this.check_button);
            this.Controls.Add(this.HealthRecordText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EHRNarrative";
            this.Text = "Electronic Health Record Narrative";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HealthRecordText;
        private System.Windows.Forms.Timer dashboardTimer;
        private System.Windows.Forms.Button check_button;
        private System.Windows.Forms.Button ignore_warnings;
    }
}

