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
            this.commandInput = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.dialogInput = new System.Windows.Forms.TextBox();
            this.cleanTemplateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chestPainButton
            // 
            this.chestPainButton.Location = new System.Drawing.Point(13, 13);
            this.chestPainButton.Name = "chestPainButton";
            this.chestPainButton.Size = new System.Drawing.Size(75, 23);
            this.chestPainButton.TabIndex = 0;
            this.chestPainButton.Text = "Chest Pain";
            this.chestPainButton.UseVisualStyleBackColor = true;
            this.chestPainButton.Click += new System.EventHandler(this.chestPainButton_Click);
            // 
            // nextFieldButton
            // 
            this.nextFieldButton.Location = new System.Drawing.Point(95, 13);
            this.nextFieldButton.Name = "nextFieldButton";
            this.nextFieldButton.Size = new System.Drawing.Size(75, 23);
            this.nextFieldButton.TabIndex = 1;
            this.nextFieldButton.Text = "Next Field";
            this.nextFieldButton.UseVisualStyleBackColor = true;
            this.nextFieldButton.Click += new System.EventHandler(this.nextFieldButton_Click);
            // 
            // showDialogButton
            // 
            this.showDialogButton.Location = new System.Drawing.Point(13, 42);
            this.showDialogButton.Name = "showDialogButton";
            this.showDialogButton.Size = new System.Drawing.Size(85, 23);
            this.showDialogButton.TabIndex = 2;
            this.showDialogButton.Text = "Show Dialog:";
            this.showDialogButton.UseVisualStyleBackColor = true;
            this.showDialogButton.Click += new System.EventHandler(this.showDialogButton_Click);
            // 
            // commandInput
            // 
            this.commandInput.Location = new System.Drawing.Point(12, 71);
            this.commandInput.Name = "commandInput";
            this.commandInput.Size = new System.Drawing.Size(240, 20);
            this.commandInput.TabIndex = 3;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(61, 97);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(140, 23);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Send Custom Command";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // dialogInput
            // 
            this.dialogInput.Location = new System.Drawing.Point(105, 42);
            this.dialogInput.Name = "dialogInput";
            this.dialogInput.Size = new System.Drawing.Size(147, 20);
            this.dialogInput.TabIndex = 5;
            this.dialogInput.Text = "Review of Systems";
            // 
            // cleanTemplateButton
            // 
            this.cleanTemplateButton.Location = new System.Drawing.Point(177, 13);
            this.cleanTemplateButton.Name = "cleanTemplateButton";
            this.cleanTemplateButton.Size = new System.Drawing.Size(75, 23);
            this.cleanTemplateButton.TabIndex = 6;
            this.cleanTemplateButton.Text = "Clean";
            this.cleanTemplateButton.UseVisualStyleBackColor = true;
            this.cleanTemplateButton.Click += new System.EventHandler(this.cleanTemplateButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 129);
            this.Controls.Add(this.cleanTemplateButton);
            this.Controls.Add(this.dialogInput);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.commandInput);
            this.Controls.Add(this.showDialogButton);
            this.Controls.Add(this.nextFieldButton);
            this.Controls.Add(this.chestPainButton);
            this.Name = "Form1";
            this.Text = "Dragon Commands";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button chestPainButton;
        private System.Windows.Forms.Button nextFieldButton;
        private System.Windows.Forms.Button showDialogButton;
        private System.Windows.Forms.TextBox commandInput;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox dialogInput;
        private System.Windows.Forms.Button cleanTemplateButton;

    }
}

