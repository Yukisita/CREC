namespace CoRectSys
{
    partial class ConfidentialData
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
            this.DummyTextBox = new System.Windows.Forms.TextBox();
            this.ConfidentialDataTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(330, 253);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(100, 19);
            this.DummyTextBox.TabIndex = 0;
            // 
            // ConfidentialDataTextBox
            // 
            this.ConfidentialDataTextBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ConfidentialDataTextBox.Location = new System.Drawing.Point(12, 12);
            this.ConfidentialDataTextBox.Multiline = true;
            this.ConfidentialDataTextBox.Name = "ConfidentialDataTextBox";
            this.ConfidentialDataTextBox.Size = new System.Drawing.Size(776, 426);
            this.ConfidentialDataTextBox.TabIndex = 1;
            // 
            // ConfidentialData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ConfidentialDataTextBox);
            this.Controls.Add(this.DummyTextBox);
            this.Name = "ConfidentialData";
            this.Text = "ConfidentialData";
            this.Load += new System.EventHandler(this.ConfidentialData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DummyTextBox;
        private System.Windows.Forms.TextBox ConfidentialDataTextBox;
    }
}