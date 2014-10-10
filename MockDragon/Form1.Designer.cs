namespace TestWindowsMessage
{
    partial class Form1
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
            this.chestPainButton = new System.Windows.Forms.Button();
            this.nextFieldButton = new System.Windows.Forms.Button();
            this.showDialogButton = new System.Windows.Forms.Button();
            this.ehrCommandInput = new System.Windows.Forms.TextBox();
            this.sendToEHRButton = new System.Windows.Forms.Button();
            this.dialogInput = new System.Windows.Forms.TextBox();
            this.cleanTemplateButton = new System.Windows.Forms.Button();
            this.slcCommandInput = new System.Windows.Forms.TextBox();
            this.sendToSLCButton = new System.Windows.Forms.Button();
            this.customCommandsGroup = new System.Windows.Forms.GroupBox();
            this.showTextDialogButton = new System.Windows.Forms.Button();
            this.customCommandsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // chestPainButton
            // 
            this.chestPainButton.Location = new System.Drawing.Point(12, 12);
            this.chestPainButton.Name = "chestPainButton";
            this.chestPainButton.Size = new System.Drawing.Size(75, 23);
            this.chestPainButton.TabIndex = 0;
            this.chestPainButton.Text = "Chest Pain";
            this.chestPainButton.UseVisualStyleBackColor = true;
            this.chestPainButton.Click += new System.EventHandler(this.chestPainButton_Click);
            // 
            // nextFieldButton
            // 
            this.nextFieldButton.Location = new System.Drawing.Point(93, 12);
            this.nextFieldButton.Name = "nextFieldButton";
            this.nextFieldButton.Size = new System.Drawing.Size(75, 23);
            this.nextFieldButton.TabIndex = 1;
            this.nextFieldButton.Text = "Next Field";
            this.nextFieldButton.UseVisualStyleBackColor = true;
            this.nextFieldButton.Click += new System.EventHandler(this.nextFieldButton_Click);
            // 
            // showDialogButton
            // 
            this.showDialogButton.Location = new System.Drawing.Point(12, 39);
            this.showDialogButton.Name = "showDialogButton";
            this.showDialogButton.Size = new System.Drawing.Size(85, 23);
            this.showDialogButton.TabIndex = 2;
            this.showDialogButton.Text = "Show Dialog:";
            this.showDialogButton.UseVisualStyleBackColor = true;
            this.showDialogButton.Click += new System.EventHandler(this.showDialogButton_Click);
            // 
            // ehrCommandInput
            // 
            this.ehrCommandInput.Location = new System.Drawing.Point(6, 37);
            this.ehrCommandInput.Name = "ehrCommandInput";
            this.ehrCommandInput.Size = new System.Drawing.Size(222, 20);
            this.ehrCommandInput.TabIndex = 3;
            // 
            // sendToEHRButton
            // 
            this.sendToEHRButton.Location = new System.Drawing.Point(27, 63);
            this.sendToEHRButton.Name = "sendToEHRButton";
            this.sendToEHRButton.Size = new System.Drawing.Size(180, 23);
            this.sendToEHRButton.TabIndex = 4;
            this.sendToEHRButton.Text = "Send Command to EHRNarrative";
            this.sendToEHRButton.UseVisualStyleBackColor = true;
            this.sendToEHRButton.Click += new System.EventHandler(this.sendToEHRButton_Click);
            // 
            // dialogInput
            // 
            this.dialogInput.Location = new System.Drawing.Point(100, 41);
            this.dialogInput.Name = "dialogInput";
            this.dialogInput.Size = new System.Drawing.Size(147, 20);
            this.dialogInput.TabIndex = 5;
            this.dialogInput.Text = "Review of Systems";
            // 
            // cleanTemplateButton
            // 
            this.cleanTemplateButton.Location = new System.Drawing.Point(172, 12);
            this.cleanTemplateButton.Name = "cleanTemplateButton";
            this.cleanTemplateButton.Size = new System.Drawing.Size(75, 23);
            this.cleanTemplateButton.TabIndex = 6;
            this.cleanTemplateButton.Text = "Clean";
            this.cleanTemplateButton.UseVisualStyleBackColor = true;
            this.cleanTemplateButton.Click += new System.EventHandler(this.cleanTemplateButton_Click);
            // 
            // slcCommandInput
            // 
            this.slcCommandInput.Location = new System.Drawing.Point(6, 99);
            this.slcCommandInput.Name = "slcCommandInput";
            this.slcCommandInput.Size = new System.Drawing.Size(222, 20);
            this.slcCommandInput.TabIndex = 7;
            // 
            // sendToSLCButton
            // 
            this.sendToSLCButton.Location = new System.Drawing.Point(48, 125);
            this.sendToSLCButton.Name = "sendToSLCButton";
            this.sendToSLCButton.Size = new System.Drawing.Size(139, 23);
            this.sendToSLCButton.TabIndex = 8;
            this.sendToSLCButton.Text = "Send Command to SLC";
            this.sendToSLCButton.UseVisualStyleBackColor = true;
            this.sendToSLCButton.Click += new System.EventHandler(this.sendToSLCButton_Click);
            // 
            // customCommandsGroup
            // 
            this.customCommandsGroup.Controls.Add(this.ehrCommandInput);
            this.customCommandsGroup.Controls.Add(this.sendToSLCButton);
            this.customCommandsGroup.Controls.Add(this.sendToEHRButton);
            this.customCommandsGroup.Controls.Add(this.slcCommandInput);
            this.customCommandsGroup.Location = new System.Drawing.Point(12, 98);
            this.customCommandsGroup.Name = "customCommandsGroup";
            this.customCommandsGroup.Size = new System.Drawing.Size(234, 160);
            this.customCommandsGroup.TabIndex = 9;
            this.customCommandsGroup.TabStop = false;
            this.customCommandsGroup.Text = "Custom Commands";
            // 
            // showTextDialogButton
            // 
            this.showTextDialogButton.Location = new System.Drawing.Point(12, 69);
            this.showTextDialogButton.Name = "showTextDialogButton";
            this.showTextDialogButton.Size = new System.Drawing.Size(235, 23);
            this.showTextDialogButton.TabIndex = 10;
            this.showTextDialogButton.Text = "Show Text Dialog (for highlighted text)";
            this.showTextDialogButton.UseVisualStyleBackColor = true;
            this.showTextDialogButton.Click += new System.EventHandler(this.showTextDialogButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 269);
            this.Controls.Add(this.showTextDialogButton);
            this.Controls.Add(this.customCommandsGroup);
            this.Controls.Add(this.cleanTemplateButton);
            this.Controls.Add(this.dialogInput);
            this.Controls.Add(this.showDialogButton);
            this.Controls.Add(this.nextFieldButton);
            this.Controls.Add(this.chestPainButton);
            this.Name = "Form1";
            this.Text = "Dragon Commands";
            this.customCommandsGroup.ResumeLayout(false);
            this.customCommandsGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button chestPainButton;
        private System.Windows.Forms.Button nextFieldButton;
        private System.Windows.Forms.Button showDialogButton;
        private System.Windows.Forms.TextBox ehrCommandInput;
        private System.Windows.Forms.Button sendToEHRButton;
        private System.Windows.Forms.TextBox dialogInput;
        private System.Windows.Forms.Button cleanTemplateButton;
        private System.Windows.Forms.TextBox slcCommandInput;
        private System.Windows.Forms.Button sendToSLCButton;
        private System.Windows.Forms.GroupBox customCommandsGroup;
        private System.Windows.Forms.Button showTextDialogButton;

    }
}

