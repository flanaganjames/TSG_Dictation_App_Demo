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
            this.button3 = new System.Windows.Forms.Button();
            this.commandInput = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
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
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(177, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // commandInput
            // 
            this.commandInput.Location = new System.Drawing.Point(13, 201);
            this.commandInput.Name = "commandInput";
            this.commandInput.Size = new System.Drawing.Size(239, 20);
            this.commandInput.TabIndex = 3;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(62, 227);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(140, 23);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Send Custom Command";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 262);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.commandInput);
            this.Controls.Add(this.button3);
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox commandInput;
        private System.Windows.Forms.Button sendButton;

    }
}

