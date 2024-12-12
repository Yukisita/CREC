namespace CREC
{
    partial class CloseBackUpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseBackUpForm));
            this.CloseBackUpProgressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CloseBackUpProgressBar
            // 
            this.CloseBackUpProgressBar.Location = new System.Drawing.Point(24, 112);
            this.CloseBackUpProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CloseBackUpProgressBar.MarqueeAnimationSpeed = 50;
            this.CloseBackUpProgressBar.Maximum = 500;
            this.CloseBackUpProgressBar.Name = "CloseBackUpProgressBar";
            this.CloseBackUpProgressBar.Size = new System.Drawing.Size(459, 29);
            this.CloseBackUpProgressBar.Step = 1;
            this.CloseBackUpProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.CloseBackUpProgressBar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(100, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 45);
            this.label1.TabIndex = 1;
            this.label1.Text = "バックアップ作成中";
            // 
            // CloseBackUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 181);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CloseBackUpProgressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CloseBackUpForm";
            this.Text = "CREC";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar CloseBackUpProgressBar;
        private System.Windows.Forms.Label label1;
    }
}