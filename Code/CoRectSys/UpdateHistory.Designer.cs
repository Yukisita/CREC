﻿namespace CREC
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
            this.UpdateHistoryTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UpdateHistoryTextBox.BackColor = System.Drawing.Color.White;
            this.UpdateHistoryTextBox.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.UpdateHistoryTextBox.HideSelection = false;
            this.UpdateHistoryTextBox.Location = new System.Drawing.Point(12, 12);
            this.UpdateHistoryTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UpdateHistoryTextBox.Multiline = true;
            this.UpdateHistoryTextBox.Name = "UpdateHistoryTextBox";
            this.UpdateHistoryTextBox.ReadOnly = true;
            this.UpdateHistoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.UpdateHistoryTextBox.Size = new System.Drawing.Size(754, 420);
            this.UpdateHistoryTextBox.TabIndex = 1;
            this.UpdateHistoryTextBox.Text = resources.GetString("UpdateHistoryTextBox.Text");
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(312, 301);
            this.DummyTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(100, 19);
            this.DummyTextBox.TabIndex = 0;
            // 
            // UpdateHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 444);
            this.Controls.Add(this.UpdateHistoryTextBox);
            this.Controls.Add(this.DummyTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UpdateHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdateHistory";
            this.Shown += new System.EventHandler(this.UpdateHistory_Shown);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.UpdateHistory_DpiChanged);
            this.SizeChanged += new System.EventHandler(this.UpdateHistory_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UpdateHistoryTextBox;
        private System.Windows.Forms.TextBox DummyTextBox;
    }
}