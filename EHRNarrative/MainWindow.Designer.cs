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
            this.SuspendLayout();
            // 
            // HealthRecordText
            // 
            this.HealthRecordText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.HealthRecordText.Location = new System.Drawing.Point(12, 12);
            this.HealthRecordText.Name = "HealthRecordText";
            this.HealthRecordText.Size = new System.Drawing.Size(760, 538);
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
            // EHRNarrative
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.HealthRecordText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EHRNarrative";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electronic Health Record Narrative";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox HealthRecordText;
        private System.Windows.Forms.Timer dashboardTimer;
    }
}

