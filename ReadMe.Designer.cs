namespace CoRectSys
{
    partial class ReadMe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReadMe));
            this.ReadMeTextBox = new System.Windows.Forms.TextBox();
            this.DummyTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ReadMeTextBox
            // 
            this.ReadMeTextBox.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReadMeTextBox.HideSelection = false;
            this.ReadMeTextBox.Location = new System.Drawing.Point(12, 12);
            this.ReadMeTextBox.Multiline = true;
            this.ReadMeTextBox.Name = "ReadMeTextBox";
            this.ReadMeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReadMeTextBox.Size = new System.Drawing.Size(776, 426);
            this.ReadMeTextBox.TabIndex = 1;
            this.ReadMeTextBox.Text = resources.GetString("ReadMeTextBox.Text");
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(312, 301);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(100, 19);
            this.DummyTextBox.TabIndex = 0;
            // 
            // ReadMe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ReadMeTextBox);
            this.Controls.Add(this.DummyTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReadMe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ReadMe";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReadMeTextBox;
        private System.Windows.Forms.TextBox DummyTextBox;
    }
}