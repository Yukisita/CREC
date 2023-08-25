namespace CoRectSys
{
    partial class AddForm
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
            this.NameBox = new System.Windows.Forms.TextBox();
            this.IDBox = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.IDLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.RegistrationDateLabel = new System.Windows.Forms.Label();
            this.RegistrationDateBox = new System.Windows.Forms.TextBox();
            this.CategoryBox = new System.Windows.Forms.TextBox();
            this.CategoryLabel = new System.Windows.Forms.Label();
            this.TagLabel = new System.Windows.Forms.Label();
            this.TagBox1 = new System.Windows.Forms.TextBox();
            this.TagBox2 = new System.Windows.Forms.TextBox();
            this.TagBox3 = new System.Windows.Forms.TextBox();
            this.RealLocationLabel = new System.Windows.Forms.Label();
            this.RealLocationBox = new System.Windows.Forms.TextBox();
            this.detailsLabel = new System.Windows.Forms.Label();
            this.DetailsBox = new System.Windows.Forms.TextBox();
            this.AddDataButton = new System.Windows.Forms.Button();
            this.AddPictureButton = new System.Windows.Forms.Button();
            this.AllowChangeIDButton = new System.Windows.Forms.Button();
            this.ConfidentialDataLabel = new System.Windows.Forms.Label();
            this.EditConfidentialDataButton = new System.Windows.Forms.Button();
            this.ConfidentialDataTextBox = new System.Windows.Forms.TextBox();
            this.ECDModeLabel = new System.Windows.Forms.Label();
            this.CloseECDModeButoon = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NameBox
            // 
            this.NameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.NameBox.Location = new System.Drawing.Point(198, 12);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(433, 43);
            this.NameBox.TabIndex = 0;
            // 
            // IDBox
            // 
            this.IDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IDBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.IDBox.Location = new System.Drawing.Point(198, 61);
            this.IDBox.Name = "IDBox";
            this.IDBox.ReadOnly = true;
            this.IDBox.Size = new System.Drawing.Size(433, 43);
            this.IDBox.TabIndex = 1;
            this.IDBox.TextChanged += new System.EventHandler(this.IDNumberBox_TextChanged);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.NameLabel.Location = new System.Drawing.Point(129, 15);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(63, 36);
            this.NameLabel.TabIndex = 2;
            this.NameLabel.Text = "名称";
            // 
            // IDLabel
            // 
            this.IDLabel.AutoSize = true;
            this.IDLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.IDLabel.Location = new System.Drawing.Point(149, 64);
            this.IDLabel.Name = "IDLabel";
            this.IDLabel.Size = new System.Drawing.Size(43, 36);
            this.IDLabel.TabIndex = 3;
            this.IDLabel.Text = "ID";
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveButton.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SaveButton.Location = new System.Drawing.Point(770, 549);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(90, 40);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // RegistrationDateLabel
            // 
            this.RegistrationDateLabel.AutoSize = true;
            this.RegistrationDateLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RegistrationDateLabel.Location = new System.Drawing.Point(105, 113);
            this.RegistrationDateLabel.Name = "RegistrationDateLabel";
            this.RegistrationDateLabel.Size = new System.Drawing.Size(87, 36);
            this.RegistrationDateLabel.TabIndex = 5;
            this.RegistrationDateLabel.Text = "登録日";
            // 
            // RegistrationDateBox
            // 
            this.RegistrationDateBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RegistrationDateBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RegistrationDateBox.Location = new System.Drawing.Point(198, 110);
            this.RegistrationDateBox.Name = "RegistrationDateBox";
            this.RegistrationDateBox.Size = new System.Drawing.Size(433, 43);
            this.RegistrationDateBox.TabIndex = 2;
            // 
            // CategoryBox
            // 
            this.CategoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CategoryBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CategoryBox.Location = new System.Drawing.Point(198, 159);
            this.CategoryBox.Name = "CategoryBox";
            this.CategoryBox.Size = new System.Drawing.Size(433, 43);
            this.CategoryBox.TabIndex = 3;
            // 
            // CategoryLabel
            // 
            this.CategoryLabel.AutoSize = true;
            this.CategoryLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CategoryLabel.Location = new System.Drawing.Point(57, 162);
            this.CategoryLabel.Name = "CategoryLabel";
            this.CategoryLabel.Size = new System.Drawing.Size(135, 36);
            this.CategoryLabel.TabIndex = 8;
            this.CategoryLabel.Text = "カテゴリー";
            // 
            // TagLabel
            // 
            this.TagLabel.AutoSize = true;
            this.TagLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TagLabel.Location = new System.Drawing.Point(129, 211);
            this.TagLabel.Name = "TagLabel";
            this.TagLabel.Size = new System.Drawing.Size(63, 36);
            this.TagLabel.TabIndex = 9;
            this.TagLabel.Text = "タグ";
            // 
            // TagBox1
            // 
            this.TagBox1.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TagBox1.Location = new System.Drawing.Point(198, 208);
            this.TagBox1.Name = "TagBox1";
            this.TagBox1.Size = new System.Drawing.Size(204, 43);
            this.TagBox1.TabIndex = 4;
            // 
            // TagBox2
            // 
            this.TagBox2.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TagBox2.Location = new System.Drawing.Point(427, 208);
            this.TagBox2.Name = "TagBox2";
            this.TagBox2.Size = new System.Drawing.Size(204, 43);
            this.TagBox2.TabIndex = 5;
            // 
            // TagBox3
            // 
            this.TagBox3.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TagBox3.Location = new System.Drawing.Point(656, 208);
            this.TagBox3.Name = "TagBox3";
            this.TagBox3.Size = new System.Drawing.Size(204, 43);
            this.TagBox3.TabIndex = 6;
            // 
            // RealLocationLabel
            // 
            this.RealLocationLabel.AutoSize = true;
            this.RealLocationLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RealLocationLabel.Location = new System.Drawing.Point(33, 260);
            this.RealLocationLabel.Name = "RealLocationLabel";
            this.RealLocationLabel.Size = new System.Drawing.Size(159, 36);
            this.RealLocationLabel.TabIndex = 11;
            this.RealLocationLabel.Text = "現物保管場所";
            // 
            // RealLocationBox
            // 
            this.RealLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RealLocationBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RealLocationBox.Location = new System.Drawing.Point(198, 257);
            this.RealLocationBox.Name = "RealLocationBox";
            this.RealLocationBox.Size = new System.Drawing.Size(433, 43);
            this.RealLocationBox.TabIndex = 7;
            // 
            // detailsLabel
            // 
            this.detailsLabel.AutoSize = true;
            this.detailsLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.detailsLabel.Location = new System.Drawing.Point(81, 358);
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(111, 36);
            this.detailsLabel.TabIndex = 11;
            this.detailsLabel.Text = "詳細情報";
            // 
            // DetailsBox
            // 
            this.DetailsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DetailsBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DetailsBox.Location = new System.Drawing.Point(198, 358);
            this.DetailsBox.Multiline = true;
            this.DetailsBox.Name = "DetailsBox";
            this.DetailsBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DetailsBox.Size = new System.Drawing.Size(662, 185);
            this.DetailsBox.TabIndex = 8;
            // 
            // AddDataButton
            // 
            this.AddDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddDataButton.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddDataButton.Location = new System.Drawing.Point(214, 549);
            this.AddDataButton.Name = "AddDataButton";
            this.AddDataButton.Size = new System.Drawing.Size(204, 40);
            this.AddDataButton.TabIndex = 9;
            this.AddDataButton.Text = "データ追加";
            this.AddDataButton.UseVisualStyleBackColor = true;
            this.AddDataButton.Click += new System.EventHandler(this.AddDataButton_Click);
            // 
            // AddPictureButton
            // 
            this.AddPictureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddPictureButton.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AddPictureButton.Location = new System.Drawing.Point(501, 549);
            this.AddPictureButton.Name = "AddPictureButton";
            this.AddPictureButton.Size = new System.Drawing.Size(204, 40);
            this.AddPictureButton.TabIndex = 10;
            this.AddPictureButton.Text = "写真追加";
            this.AddPictureButton.UseVisualStyleBackColor = true;
            this.AddPictureButton.Click += new System.EventHandler(this.AddPictureButton_Click);
            // 
            // AllowChangeIDButton
            // 
            this.AllowChangeIDButton.Font = new System.Drawing.Font("メイリオ", 18F);
            this.AllowChangeIDButton.Location = new System.Drawing.Point(656, 64);
            this.AllowChangeIDButton.Name = "AllowChangeIDButton";
            this.AllowChangeIDButton.Size = new System.Drawing.Size(139, 40);
            this.AllowChangeIDButton.TabIndex = 12;
            this.AllowChangeIDButton.Text = "編集不可";
            this.AllowChangeIDButton.UseVisualStyleBackColor = true;
            this.AllowChangeIDButton.Click += new System.EventHandler(this.AllowChangeIDButton_Click);
            // 
            // ConfidentialDataLabel
            // 
            this.ConfidentialDataLabel.AutoSize = true;
            this.ConfidentialDataLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ConfidentialDataLabel.Location = new System.Drawing.Point(81, 309);
            this.ConfidentialDataLabel.Name = "ConfidentialDataLabel";
            this.ConfidentialDataLabel.Size = new System.Drawing.Size(111, 36);
            this.ConfidentialDataLabel.TabIndex = 13;
            this.ConfidentialDataLabel.Text = "機密情報";
            // 
            // EditConfidentialDataButton
            // 
            this.EditConfidentialDataButton.Font = new System.Drawing.Font("メイリオ", 18F);
            this.EditConfidentialDataButton.Location = new System.Drawing.Point(198, 306);
            this.EditConfidentialDataButton.Name = "EditConfidentialDataButton";
            this.EditConfidentialDataButton.Size = new System.Drawing.Size(139, 40);
            this.EditConfidentialDataButton.TabIndex = 15;
            this.EditConfidentialDataButton.Text = "表示/編集";
            this.EditConfidentialDataButton.UseVisualStyleBackColor = true;
            this.EditConfidentialDataButton.Click += new System.EventHandler(this.EditConfidentialDataButton_Click);
            // 
            // ConfidentialDataTextBox
            // 
            this.ConfidentialDataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfidentialDataTextBox.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ConfidentialDataTextBox.Location = new System.Drawing.Point(12, 54);
            this.ConfidentialDataTextBox.Multiline = true;
            this.ConfidentialDataTextBox.Name = "ConfidentialDataTextBox";
            this.ConfidentialDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ConfidentialDataTextBox.Size = new System.Drawing.Size(848, 489);
            this.ConfidentialDataTextBox.TabIndex = 16;
            this.ConfidentialDataTextBox.Visible = false;
            // 
            // ECDModeLabel
            // 
            this.ECDModeLabel.AutoSize = true;
            this.ECDModeLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ECDModeLabel.Location = new System.Drawing.Point(12, 15);
            this.ECDModeLabel.Name = "ECDModeLabel";
            this.ECDModeLabel.Size = new System.Drawing.Size(111, 36);
            this.ECDModeLabel.TabIndex = 17;
            this.ECDModeLabel.Text = "機密情報";
            this.ECDModeLabel.Visible = false;
            // 
            // CloseECDModeButoon
            // 
            this.CloseECDModeButoon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseECDModeButoon.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CloseECDModeButoon.Location = new System.Drawing.Point(724, 549);
            this.CloseECDModeButoon.Name = "CloseECDModeButoon";
            this.CloseECDModeButoon.Size = new System.Drawing.Size(90, 40);
            this.CloseECDModeButoon.TabIndex = 18;
            this.CloseECDModeButoon.Text = "戻る";
            this.CloseECDModeButoon.UseVisualStyleBackColor = true;
            this.CloseECDModeButoon.Visible = false;
            this.CloseECDModeButoon.Click += new System.EventHandler(this.CloseECDModeButoon_Click);
            // 
            // AddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(868, 601);
            this.Controls.Add(this.CloseECDModeButoon);
            this.Controls.Add(this.ECDModeLabel);
            this.Controls.Add(this.EditConfidentialDataButton);
            this.Controls.Add(this.ConfidentialDataLabel);
            this.Controls.Add(this.AllowChangeIDButton);
            this.Controls.Add(this.AddPictureButton);
            this.Controls.Add(this.AddDataButton);
            this.Controls.Add(this.DetailsBox);
            this.Controls.Add(this.RealLocationBox);
            this.Controls.Add(this.detailsLabel);
            this.Controls.Add(this.RealLocationLabel);
            this.Controls.Add(this.TagBox3);
            this.Controls.Add(this.TagBox2);
            this.Controls.Add(this.TagBox1);
            this.Controls.Add(this.TagLabel);
            this.Controls.Add(this.CategoryLabel);
            this.Controls.Add(this.CategoryBox);
            this.Controls.Add(this.RegistrationDateBox);
            this.Controls.Add(this.RegistrationDateLabel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.IDLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.IDBox);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.ConfidentialDataTextBox);
            this.MinimumSize = new System.Drawing.Size(16, 640);
            this.Name = "AddForm";
            this.Text = "AddForm";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.AddForm_Closing);
            this.Load += new System.EventHandler(this.AddForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.TextBox IDBox;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label IDLabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label RegistrationDateLabel;
        private System.Windows.Forms.TextBox RegistrationDateBox;
        private System.Windows.Forms.TextBox CategoryBox;
        private System.Windows.Forms.Label CategoryLabel;
        private System.Windows.Forms.Label TagLabel;
        private System.Windows.Forms.TextBox TagBox1;
        private System.Windows.Forms.TextBox TagBox2;
        private System.Windows.Forms.TextBox TagBox3;
        private System.Windows.Forms.Label RealLocationLabel;
        private System.Windows.Forms.TextBox RealLocationBox;
        private System.Windows.Forms.Label detailsLabel;
        private System.Windows.Forms.TextBox DetailsBox;
        private System.Windows.Forms.Button AddDataButton;
        private System.Windows.Forms.Button AddPictureButton;
        private System.Windows.Forms.Button AllowChangeIDButton;
        private System.Windows.Forms.Label ConfidentialDataLabel;
        private System.Windows.Forms.Button EditConfidentialDataButton;
        private System.Windows.Forms.TextBox ConfidentialDataTextBox;
        private System.Windows.Forms.Label ECDModeLabel;
        private System.Windows.Forms.Button CloseECDModeButoon;
    }
}