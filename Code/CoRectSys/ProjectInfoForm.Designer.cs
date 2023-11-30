namespace ColRECt
{
    partial class ProjectInfoForm
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
            this.ProjcetNameLabel = new System.Windows.Forms.Label();
            this.ProjcetCreatedDateLabel = new System.Windows.Forms.Label();
            this.ProjcetModifiedDateLabel = new System.Windows.Forms.Label();
            this.ProjcetAccessedDateLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProjcetNameLabel
            // 
            this.ProjcetNameLabel.AutoSize = true;
            this.ProjcetNameLabel.Font = new System.Drawing.Font("Meiryo UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetNameLabel.Location = new System.Drawing.Point(12, 9);
            this.ProjcetNameLabel.Name = "ProjcetNameLabel";
            this.ProjcetNameLabel.Size = new System.Drawing.Size(83, 30);
            this.ProjcetNameLabel.TabIndex = 0;
            this.ProjcetNameLabel.Text = "label1";
            // 
            // ProjcetCreatedDateLabel
            // 
            this.ProjcetCreatedDateLabel.AutoSize = true;
            this.ProjcetCreatedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetCreatedDateLabel.Location = new System.Drawing.Point(12, 57);
            this.ProjcetCreatedDateLabel.Name = "ProjcetCreatedDateLabel";
            this.ProjcetCreatedDateLabel.Size = new System.Drawing.Size(55, 20);
            this.ProjcetCreatedDateLabel.TabIndex = 0;
            this.ProjcetCreatedDateLabel.Text = "label1";
            this.ProjcetCreatedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjcetModifiedDateLabel
            // 
            this.ProjcetModifiedDateLabel.AutoSize = true;
            this.ProjcetModifiedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetModifiedDateLabel.Location = new System.Drawing.Point(12, 88);
            this.ProjcetModifiedDateLabel.Name = "ProjcetModifiedDateLabel";
            this.ProjcetModifiedDateLabel.Size = new System.Drawing.Size(55, 20);
            this.ProjcetModifiedDateLabel.TabIndex = 0;
            this.ProjcetModifiedDateLabel.Text = "label1";
            this.ProjcetModifiedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjcetAccessedDateLabel
            // 
            this.ProjcetAccessedDateLabel.AutoSize = true;
            this.ProjcetAccessedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetAccessedDateLabel.Location = new System.Drawing.Point(12, 119);
            this.ProjcetAccessedDateLabel.Name = "ProjcetAccessedDateLabel";
            this.ProjcetAccessedDateLabel.Size = new System.Drawing.Size(55, 20);
            this.ProjcetAccessedDateLabel.TabIndex = 0;
            this.ProjcetAccessedDateLabel.Text = "label1";
            this.ProjcetAccessedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjectInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(440, 148);
            this.Controls.Add(this.ProjcetAccessedDateLabel);
            this.Controls.Add(this.ProjcetModifiedDateLabel);
            this.Controls.Add(this.ProjcetCreatedDateLabel);
            this.Controls.Add(this.ProjcetNameLabel);
            this.Name = "ProjectInfoForm";
            this.Text = "プロジェクト情報";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProjcetNameLabel;
        private System.Windows.Forms.Label ProjcetCreatedDateLabel;
        private System.Windows.Forms.Label ProjcetModifiedDateLabel;
        private System.Windows.Forms.Label ProjcetAccessedDateLabel;
    }
}