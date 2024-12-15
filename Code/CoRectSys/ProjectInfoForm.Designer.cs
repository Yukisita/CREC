namespace CREC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectInfoForm));
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
            this.ProjcetNameLabel.Location = new System.Drawing.Point(16, 11);
            this.ProjcetNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjcetNameLabel.Name = "ProjcetNameLabel";
            this.ProjcetNameLabel.Size = new System.Drawing.Size(104, 38);
            this.ProjcetNameLabel.TabIndex = 0;
            this.ProjcetNameLabel.Text = "label1";
            // 
            // ProjcetCreatedDateLabel
            // 
            this.ProjcetCreatedDateLabel.AutoSize = true;
            this.ProjcetCreatedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetCreatedDateLabel.Location = new System.Drawing.Point(16, 71);
            this.ProjcetCreatedDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjcetCreatedDateLabel.Name = "ProjcetCreatedDateLabel";
            this.ProjcetCreatedDateLabel.Size = new System.Drawing.Size(70, 25);
            this.ProjcetCreatedDateLabel.TabIndex = 0;
            this.ProjcetCreatedDateLabel.Text = "label1";
            this.ProjcetCreatedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjcetModifiedDateLabel
            // 
            this.ProjcetModifiedDateLabel.AutoSize = true;
            this.ProjcetModifiedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetModifiedDateLabel.Location = new System.Drawing.Point(16, 110);
            this.ProjcetModifiedDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjcetModifiedDateLabel.Name = "ProjcetModifiedDateLabel";
            this.ProjcetModifiedDateLabel.Size = new System.Drawing.Size(70, 25);
            this.ProjcetModifiedDateLabel.TabIndex = 0;
            this.ProjcetModifiedDateLabel.Text = "label1";
            this.ProjcetModifiedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjcetAccessedDateLabel
            // 
            this.ProjcetAccessedDateLabel.AutoSize = true;
            this.ProjcetAccessedDateLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProjcetAccessedDateLabel.Location = new System.Drawing.Point(16, 149);
            this.ProjcetAccessedDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjcetAccessedDateLabel.Name = "ProjcetAccessedDateLabel";
            this.ProjcetAccessedDateLabel.Size = new System.Drawing.Size(70, 25);
            this.ProjcetAccessedDateLabel.TabIndex = 0;
            this.ProjcetAccessedDateLabel.Text = "label1";
            this.ProjcetAccessedDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ProjectInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(587, 185);
            this.Controls.Add(this.ProjcetAccessedDateLabel);
            this.Controls.Add(this.ProjcetModifiedDateLabel);
            this.Controls.Add(this.ProjcetCreatedDateLabel);
            this.Controls.Add(this.ProjcetNameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProjectInfoForm";
            this.Text = "プロジェクト情報";
            this.Load += new System.EventHandler(this.ProjectInfoForm_Load);
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