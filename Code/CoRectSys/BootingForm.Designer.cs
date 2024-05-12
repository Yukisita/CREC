namespace ColRECt
{
    partial class BootingForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CRECVersionLabel = new System.Windows.Forms.Label();
            this.BootingProgressLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ColRECt.Properties.Resources.CREC256_icon;
            this.pictureBox1.Location = new System.Drawing.Point(7, 7);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(140, 146);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // CRECVersionLabel
            // 
            this.CRECVersionLabel.AutoSize = true;
            this.CRECVersionLabel.Font = new System.Drawing.Font("メイリオ", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CRECVersionLabel.Location = new System.Drawing.Point(149, 7);
            this.CRECVersionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CRECVersionLabel.Name = "CRECVersionLabel";
            this.CRECVersionLabel.Size = new System.Drawing.Size(154, 28);
            this.CRECVersionLabel.TabIndex = 2;
            this.CRECVersionLabel.Text = "CREC v7.08.01";
            // 
            // BootingProgressLabel
            // 
            this.BootingProgressLabel.AutoSize = true;
            this.BootingProgressLabel.Font = new System.Drawing.Font("メイリオ", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BootingProgressLabel.Location = new System.Drawing.Point(15, 166);
            this.BootingProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BootingProgressLabel.Name = "BootingProgressLabel";
            this.BootingProgressLabel.Size = new System.Drawing.Size(291, 21);
            this.BootingProgressLabel.TabIndex = 3;
            this.BootingProgressLabel.Text = "アプリケーション起動中...おまちください。";
            this.BootingProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(224, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "S.Yukisita";
            // 
            // BootingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(303, 192);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BootingProgressLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.CRECVersionLabel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "BootingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label CRECVersionLabel;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label BootingProgressLabel;
    }
}