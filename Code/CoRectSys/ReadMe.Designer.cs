﻿namespace CREC
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
            this.ReadMeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReadMeTextBox.BackColor = System.Drawing.Color.White;
            this.ReadMeTextBox.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReadMeTextBox.HideSelection = false;
            this.ReadMeTextBox.Location = new System.Drawing.Point(16, 15);
            this.ReadMeTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ReadMeTextBox.Multiline = true;
            this.ReadMeTextBox.Name = "ReadMeTextBox";
            this.ReadMeTextBox.ReadOnly = true;
            this.ReadMeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReadMeTextBox.Size = new System.Drawing.Size(1004, 524);
            this.ReadMeTextBox.TabIndex = 1;
            this.ReadMeTextBox.Text = resources.GetString("ReadMeTextBox.Text");
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(416, 376);
            this.DummyTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(132, 22);
            this.DummyTextBox.TabIndex = 0;
            // 
            // ReadMe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 555);
            this.Controls.Add(this.ReadMeTextBox);
            this.Controls.Add(this.DummyTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReadMe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ReadMe";
            this.Shown += new System.EventHandler(this.ReadMe_Shown);
            this.DpiChanged += new System.Windows.Forms.DpiChangedEventHandler(this.ReadMe_DpiChanged);
            this.SizeChanged += new System.EventHandler(this.ReadMe_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReadMeTextBox;
        private System.Windows.Forms.TextBox DummyTextBox;
    }
}