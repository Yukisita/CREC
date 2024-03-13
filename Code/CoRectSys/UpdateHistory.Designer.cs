namespace CoRectSys
{
    partial class UpdateHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateHistory));
            this.UpdateHistoryTextBox = new System.Windows.Forms.TextBox();
            this.DummyTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // UpdateHistoryTextBox
            // 
            this.UpdateHistoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateHistoryTextBox.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.UpdateHistoryTextBox.HideSelection = false;
            this.UpdateHistoryTextBox.Location = new System.Drawing.Point(20, 18);
            this.UpdateHistoryTextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.UpdateHistoryTextBox.Multiline = true;
            this.UpdateHistoryTextBox.Name = "UpdateHistoryTextBox";
            this.UpdateHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.UpdateHistoryTextBox.Size = new System.Drawing.Size(1291, 637);
            this.UpdateHistoryTextBox.TabIndex = 1;
            this.UpdateHistoryTextBox.Text = resources.GetString("UpdateHistoryTextBox.Text");
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(520, 452);
            this.DummyTextBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(164, 25);
            this.DummyTextBox.TabIndex = 0;
            // 
            // UpdateHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 675);
            this.Controls.Add(this.UpdateHistoryTextBox);
            this.Controls.Add(this.DummyTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "UpdateHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdateHistory";
            this.Shown += new System.EventHandler(this.UpdateHistory_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UpdateHistoryTextBox;
        private System.Windows.Forms.TextBox DummyTextBox;
    }
}