﻿namespace CREC
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
            this.backUpStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CloseBackUpProgressBar
            // 
            this.CloseBackUpProgressBar.Location = new System.Drawing.Point(24, 112);
            this.CloseBackUpProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.CloseBackUpProgressBar.MarqueeAnimationSpeed = 50;
            this.CloseBackUpProgressBar.Maximum = 500;
            this.CloseBackUpProgressBar.Name = "CloseBackUpProgressBar";
            this.CloseBackUpProgressBar.Size = new System.Drawing.Size(459, 29);
            this.CloseBackUpProgressBar.Step = 1;
            this.CloseBackUpProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.CloseBackUpProgressBar.TabIndex = 0;
            // 
            // backUpStatusLabel
            // 
            this.backUpStatusLabel.AutoSize = true;
            this.backUpStatusLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.backUpStatusLabel.Location = new System.Drawing.Point(16, 33);
            this.backUpStatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backUpStatusLabel.Name = "backUpStatusLabel";
            this.backUpStatusLabel.Size = new System.Drawing.Size(320, 45);
            this.backUpStatusLabel.TabIndex = 1;
            this.backUpStatusLabel.Text = "バックアップ作成中…";
            this.backUpStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CloseBackUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 181);
            this.Controls.Add(this.backUpStatusLabel);
            this.Controls.Add(this.CloseBackUpProgressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CloseBackUpForm";
            this.Text = "CREC";
            this.Shown += new System.EventHandler(this.CloseBackUpForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label backUpStatusLabel;
        internal System.Windows.Forms.ProgressBar CloseBackUpProgressBar;
    }
}