namespace CREC
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.AllowEditLabel = new System.Windows.Forms.Label();
            this.AllowConfidentialDataLabel = new System.Windows.Forms.Label();
            this.AllowEditRadioButton = new System.Windows.Forms.RadioButton();
            this.DenyEditRadioButton = new System.Windows.Forms.RadioButton();
            this.AllowEditPanel = new System.Windows.Forms.Panel();
            this.AllowConfidentialDataRadioButton = new System.Windows.Forms.RadioButton();
            this.DenyConfidentialDataRadioButton = new System.Windows.Forms.RadioButton();
            this.AllowConfidentialDataPanel = new System.Windows.Forms.Panel();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ShowUserAssistPanel = new System.Windows.Forms.Panel();
            this.HideUserAssistRadioButton = new System.Windows.Forms.RadioButton();
            this.ShowUserAssistRadioButton = new System.Windows.Forms.RadioButton();
            this.ShowUserAssistLabel = new System.Windows.Forms.Label();
            this.AutoLoadProjectLabel = new System.Windows.Forms.Label();
            this.SetAutoLoadProjectTextBox = new System.Windows.Forms.TextBox();
            this.OpenLastTimeProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoSearchPanel = new System.Windows.Forms.Panel();
            this.DenyAutoSearchRadioButton = new System.Windows.Forms.RadioButton();
            this.AllowAutoSearchRadioButton = new System.Windows.Forms.RadioButton();
            this.AutoSearchLabel = new System.Windows.Forms.Label();
            this.RecentShownContentsLabel = new System.Windows.Forms.Label();
            this.RecentShownContentsPanel = new System.Windows.Forms.Panel();
            this.DiscardRecentShownContentsRadioButton = new System.Windows.Forms.RadioButton();
            this.SaveRecentShownContentsRadioButton = new System.Windows.Forms.RadioButton();
            this.BootUpdateCheckLabel = new System.Windows.Forms.Label();
            this.BootUpdateCheckPanel = new System.Windows.Forms.Panel();
            this.DenyBootUpdateCheckRadioButton = new System.Windows.Forms.RadioButton();
            this.AllowBootUpdateCheckRadioButton = new System.Windows.Forms.RadioButton();
            this.AllowEditPanel.SuspendLayout();
            this.AllowConfidentialDataPanel.SuspendLayout();
            this.ShowUserAssistPanel.SuspendLayout();
            this.AutoSearchPanel.SuspendLayout();
            this.RecentShownContentsPanel.SuspendLayout();
            this.BootUpdateCheckPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AllowEditLabel
            // 
            this.AllowEditLabel.AutoSize = true;
            this.AllowEditLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowEditLabel.Location = new System.Drawing.Point(20, 25);
            this.AllowEditLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AllowEditLabel.Name = "AllowEditLabel";
            this.AllowEditLabel.Size = new System.Drawing.Size(135, 36);
            this.AllowEditLabel.TabIndex = 0;
            this.AllowEditLabel.Text = "データ編集";
            // 
            // AllowConfidentialDataLabel
            // 
            this.AllowConfidentialDataLabel.AutoSize = true;
            this.AllowConfidentialDataLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowConfidentialDataLabel.Location = new System.Drawing.Point(20, 100);
            this.AllowConfidentialDataLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AllowConfidentialDataLabel.Name = "AllowConfidentialDataLabel";
            this.AllowConfidentialDataLabel.Size = new System.Drawing.Size(183, 36);
            this.AllowConfidentialDataLabel.TabIndex = 4;
            this.AllowConfidentialDataLabel.Text = "機密データ表示";
            // 
            // AllowEditRadioButton
            // 
            this.AllowEditRadioButton.AutoSize = true;
            this.AllowEditRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowEditRadioButton.Location = new System.Drawing.Point(2, 9);
            this.AllowEditRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowEditRadioButton.Name = "AllowEditRadioButton";
            this.AllowEditRadioButton.Size = new System.Drawing.Size(84, 40);
            this.AllowEditRadioButton.TabIndex = 2;
            this.AllowEditRadioButton.TabStop = true;
            this.AllowEditRadioButton.Text = "許可";
            this.AllowEditRadioButton.UseVisualStyleBackColor = true;
            // 
            // DenyEditRadioButton
            // 
            this.DenyEditRadioButton.AutoSize = true;
            this.DenyEditRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyEditRadioButton.Location = new System.Drawing.Point(124, 9);
            this.DenyEditRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.DenyEditRadioButton.Name = "DenyEditRadioButton";
            this.DenyEditRadioButton.Size = new System.Drawing.Size(84, 40);
            this.DenyEditRadioButton.TabIndex = 3;
            this.DenyEditRadioButton.TabStop = true;
            this.DenyEditRadioButton.Text = "拒否";
            this.DenyEditRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowEditPanel
            // 
            this.AllowEditPanel.Controls.Add(this.DenyEditRadioButton);
            this.AllowEditPanel.Controls.Add(this.AllowEditRadioButton);
            this.AllowEditPanel.Location = new System.Drawing.Point(253, 11);
            this.AllowEditPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowEditPanel.Name = "AllowEditPanel";
            this.AllowEditPanel.Size = new System.Drawing.Size(240, 54);
            this.AllowEditPanel.TabIndex = 1;
            // 
            // AllowConfidentialDataRadioButton
            // 
            this.AllowConfidentialDataRadioButton.AutoSize = true;
            this.AllowConfidentialDataRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowConfidentialDataRadioButton.Location = new System.Drawing.Point(2, 9);
            this.AllowConfidentialDataRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowConfidentialDataRadioButton.Name = "AllowConfidentialDataRadioButton";
            this.AllowConfidentialDataRadioButton.Size = new System.Drawing.Size(84, 40);
            this.AllowConfidentialDataRadioButton.TabIndex = 6;
            this.AllowConfidentialDataRadioButton.TabStop = true;
            this.AllowConfidentialDataRadioButton.Text = "許可";
            this.AllowConfidentialDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // DenyConfidentialDataRadioButton
            // 
            this.DenyConfidentialDataRadioButton.AutoSize = true;
            this.DenyConfidentialDataRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyConfidentialDataRadioButton.Location = new System.Drawing.Point(124, 9);
            this.DenyConfidentialDataRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.DenyConfidentialDataRadioButton.Name = "DenyConfidentialDataRadioButton";
            this.DenyConfidentialDataRadioButton.Size = new System.Drawing.Size(84, 40);
            this.DenyConfidentialDataRadioButton.TabIndex = 7;
            this.DenyConfidentialDataRadioButton.TabStop = true;
            this.DenyConfidentialDataRadioButton.Text = "拒否";
            this.DenyConfidentialDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowConfidentialDataPanel
            // 
            this.AllowConfidentialDataPanel.Controls.Add(this.DenyConfidentialDataRadioButton);
            this.AllowConfidentialDataPanel.Controls.Add(this.AllowConfidentialDataRadioButton);
            this.AllowConfidentialDataPanel.Location = new System.Drawing.Point(253, 89);
            this.AllowConfidentialDataPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowConfidentialDataPanel.Name = "AllowConfidentialDataPanel";
            this.AllowConfidentialDataPanel.Size = new System.Drawing.Size(240, 54);
            this.AllowConfidentialDataPanel.TabIndex = 5;
            // 
            // SaveButton
            // 
            this.SaveButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SaveButton.Location = new System.Drawing.Point(896, 315);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(93, 46);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ShowUserAssistPanel
            // 
            this.ShowUserAssistPanel.Controls.Add(this.HideUserAssistRadioButton);
            this.ShowUserAssistPanel.Controls.Add(this.ShowUserAssistRadioButton);
            this.ShowUserAssistPanel.Location = new System.Drawing.Point(253, 159);
            this.ShowUserAssistPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.ShowUserAssistPanel.Name = "ShowUserAssistPanel";
            this.ShowUserAssistPanel.Size = new System.Drawing.Size(240, 54);
            this.ShowUserAssistPanel.TabIndex = 8;
            // 
            // HideUserAssistRadioButton
            // 
            this.HideUserAssistRadioButton.AutoSize = true;
            this.HideUserAssistRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.HideUserAssistRadioButton.Location = new System.Drawing.Point(124, 9);
            this.HideUserAssistRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.HideUserAssistRadioButton.Name = "HideUserAssistRadioButton";
            this.HideUserAssistRadioButton.Size = new System.Drawing.Size(84, 40);
            this.HideUserAssistRadioButton.TabIndex = 10;
            this.HideUserAssistRadioButton.TabStop = true;
            this.HideUserAssistRadioButton.Text = "無効";
            this.HideUserAssistRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShowUserAssistRadioButton
            // 
            this.ShowUserAssistRadioButton.AutoSize = true;
            this.ShowUserAssistRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ShowUserAssistRadioButton.Location = new System.Drawing.Point(2, 9);
            this.ShowUserAssistRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.ShowUserAssistRadioButton.Name = "ShowUserAssistRadioButton";
            this.ShowUserAssistRadioButton.Size = new System.Drawing.Size(84, 40);
            this.ShowUserAssistRadioButton.TabIndex = 9;
            this.ShowUserAssistRadioButton.TabStop = true;
            this.ShowUserAssistRadioButton.Text = "有効";
            this.ShowUserAssistRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShowUserAssistLabel
            // 
            this.ShowUserAssistLabel.AutoSize = true;
            this.ShowUserAssistLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ShowUserAssistLabel.Location = new System.Drawing.Point(20, 170);
            this.ShowUserAssistLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.ShowUserAssistLabel.Name = "ShowUserAssistLabel";
            this.ShowUserAssistLabel.Size = new System.Drawing.Size(183, 36);
            this.ShowUserAssistLabel.TabIndex = 9;
            this.ShowUserAssistLabel.Text = "ユーザ補助表示";
            // 
            // AutoLoadProjectLabel
            // 
            this.AutoLoadProjectLabel.AutoSize = true;
            this.AutoLoadProjectLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AutoLoadProjectLabel.Location = new System.Drawing.Point(20, 239);
            this.AutoLoadProjectLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AutoLoadProjectLabel.Name = "AutoLoadProjectLabel";
            this.AutoLoadProjectLabel.Size = new System.Drawing.Size(303, 36);
            this.AutoLoadProjectLabel.TabIndex = 13;
            this.AutoLoadProjectLabel.Text = "自動プロジェクト読み込み";
            // 
            // SetAutoLoadProjectTextBox
            // 
            this.SetAutoLoadProjectTextBox.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SetAutoLoadProjectTextBox.Location = new System.Drawing.Point(26, 277);
            this.SetAutoLoadProjectTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.SetAutoLoadProjectTextBox.Name = "SetAutoLoadProjectTextBox";
            this.SetAutoLoadProjectTextBox.Size = new System.Drawing.Size(436, 32);
            this.SetAutoLoadProjectTextBox.TabIndex = 14;
            // 
            // OpenLastTimeProjectCheckBox
            // 
            this.OpenLastTimeProjectCheckBox.AutoSize = true;
            this.OpenLastTimeProjectCheckBox.Location = new System.Drawing.Point(26, 319);
            this.OpenLastTimeProjectCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.OpenLastTimeProjectCheckBox.Name = "OpenLastTimeProjectCheckBox";
            this.OpenLastTimeProjectCheckBox.Size = new System.Drawing.Size(325, 40);
            this.OpenLastTimeProjectCheckBox.TabIndex = 15;
            this.OpenLastTimeProjectCheckBox.Text = "前回のプロジェクトを開く";
            this.OpenLastTimeProjectCheckBox.UseVisualStyleBackColor = true;
            this.OpenLastTimeProjectCheckBox.CheckedChanged += new System.EventHandler(this.OpenLastTimeProjectCheckBox_CheckedChanged);
            // 
            // AutoSearchPanel
            // 
            this.AutoSearchPanel.Controls.Add(this.DenyAutoSearchRadioButton);
            this.AutoSearchPanel.Controls.Add(this.AllowAutoSearchRadioButton);
            this.AutoSearchPanel.Location = new System.Drawing.Point(743, 11);
            this.AutoSearchPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AutoSearchPanel.Name = "AutoSearchPanel";
            this.AutoSearchPanel.Size = new System.Drawing.Size(240, 54);
            this.AutoSearchPanel.TabIndex = 17;
            // 
            // DenyAutoSearchRadioButton
            // 
            this.DenyAutoSearchRadioButton.AutoSize = true;
            this.DenyAutoSearchRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyAutoSearchRadioButton.Location = new System.Drawing.Point(124, 9);
            this.DenyAutoSearchRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.DenyAutoSearchRadioButton.Name = "DenyAutoSearchRadioButton";
            this.DenyAutoSearchRadioButton.Size = new System.Drawing.Size(84, 40);
            this.DenyAutoSearchRadioButton.TabIndex = 3;
            this.DenyAutoSearchRadioButton.TabStop = true;
            this.DenyAutoSearchRadioButton.Text = "無効";
            this.DenyAutoSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowAutoSearchRadioButton
            // 
            this.AllowAutoSearchRadioButton.AutoSize = true;
            this.AllowAutoSearchRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowAutoSearchRadioButton.Location = new System.Drawing.Point(3, 9);
            this.AllowAutoSearchRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowAutoSearchRadioButton.Name = "AllowAutoSearchRadioButton";
            this.AllowAutoSearchRadioButton.Size = new System.Drawing.Size(84, 40);
            this.AllowAutoSearchRadioButton.TabIndex = 2;
            this.AllowAutoSearchRadioButton.TabStop = true;
            this.AllowAutoSearchRadioButton.Text = "有効";
            this.AllowAutoSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // AutoSearchLabel
            // 
            this.AutoSearchLabel.AutoSize = true;
            this.AutoSearchLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AutoSearchLabel.Location = new System.Drawing.Point(559, 25);
            this.AutoSearchLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.AutoSearchLabel.Name = "AutoSearchLabel";
            this.AutoSearchLabel.Size = new System.Drawing.Size(111, 36);
            this.AutoSearchLabel.TabIndex = 16;
            this.AutoSearchLabel.Text = "自動検索";
            // 
            // RecentShownContentsLabel
            // 
            this.RecentShownContentsLabel.AutoSize = true;
            this.RecentShownContentsLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RecentShownContentsLabel.Location = new System.Drawing.Point(559, 100);
            this.RecentShownContentsLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.RecentShownContentsLabel.Name = "RecentShownContentsLabel";
            this.RecentShownContentsLabel.Size = new System.Drawing.Size(159, 36);
            this.RecentShownContentsLabel.TabIndex = 18;
            this.RecentShownContentsLabel.Text = "表示履歴保存";
            // 
            // RecentShownContentsPanel
            // 
            this.RecentShownContentsPanel.Controls.Add(this.DiscardRecentShownContentsRadioButton);
            this.RecentShownContentsPanel.Controls.Add(this.SaveRecentShownContentsRadioButton);
            this.RecentShownContentsPanel.Location = new System.Drawing.Point(743, 89);
            this.RecentShownContentsPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.RecentShownContentsPanel.Name = "RecentShownContentsPanel";
            this.RecentShownContentsPanel.Size = new System.Drawing.Size(240, 54);
            this.RecentShownContentsPanel.TabIndex = 19;
            // 
            // DiscardRecentShownContentsRadioButton
            // 
            this.DiscardRecentShownContentsRadioButton.AutoSize = true;
            this.DiscardRecentShownContentsRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DiscardRecentShownContentsRadioButton.Location = new System.Drawing.Point(124, 9);
            this.DiscardRecentShownContentsRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.DiscardRecentShownContentsRadioButton.Name = "DiscardRecentShownContentsRadioButton";
            this.DiscardRecentShownContentsRadioButton.Size = new System.Drawing.Size(84, 40);
            this.DiscardRecentShownContentsRadioButton.TabIndex = 3;
            this.DiscardRecentShownContentsRadioButton.TabStop = true;
            this.DiscardRecentShownContentsRadioButton.Text = "無効";
            this.DiscardRecentShownContentsRadioButton.UseVisualStyleBackColor = true;
            // 
            // SaveRecentShownContentsRadioButton
            // 
            this.SaveRecentShownContentsRadioButton.AutoSize = true;
            this.SaveRecentShownContentsRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SaveRecentShownContentsRadioButton.Location = new System.Drawing.Point(3, 9);
            this.SaveRecentShownContentsRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.SaveRecentShownContentsRadioButton.Name = "SaveRecentShownContentsRadioButton";
            this.SaveRecentShownContentsRadioButton.Size = new System.Drawing.Size(84, 40);
            this.SaveRecentShownContentsRadioButton.TabIndex = 2;
            this.SaveRecentShownContentsRadioButton.TabStop = true;
            this.SaveRecentShownContentsRadioButton.Text = "有効";
            this.SaveRecentShownContentsRadioButton.UseVisualStyleBackColor = true;
            // 
            // BootUpdateCheckLabel
            // 
            this.BootUpdateCheckLabel.AutoSize = true;
            this.BootUpdateCheckLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BootUpdateCheckLabel.Location = new System.Drawing.Point(559, 170);
            this.BootUpdateCheckLabel.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.BootUpdateCheckLabel.Name = "BootUpdateCheckLabel";
            this.BootUpdateCheckLabel.Size = new System.Drawing.Size(183, 36);
            this.BootUpdateCheckLabel.TabIndex = 20;
            this.BootUpdateCheckLabel.Text = "起動時更新確認";
            // 
            // BootUpdateCheckPanel
            // 
            this.BootUpdateCheckPanel.Controls.Add(this.DenyBootUpdateCheckRadioButton);
            this.BootUpdateCheckPanel.Controls.Add(this.AllowBootUpdateCheckRadioButton);
            this.BootUpdateCheckPanel.Location = new System.Drawing.Point(743, 159);
            this.BootUpdateCheckPanel.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.BootUpdateCheckPanel.Name = "BootUpdateCheckPanel";
            this.BootUpdateCheckPanel.Size = new System.Drawing.Size(240, 54);
            this.BootUpdateCheckPanel.TabIndex = 21;
            // 
            // DenyBootUpdateCheckRadioButton
            // 
            this.DenyBootUpdateCheckRadioButton.AutoSize = true;
            this.DenyBootUpdateCheckRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyBootUpdateCheckRadioButton.Location = new System.Drawing.Point(124, 9);
            this.DenyBootUpdateCheckRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.DenyBootUpdateCheckRadioButton.Name = "DenyBootUpdateCheckRadioButton";
            this.DenyBootUpdateCheckRadioButton.Size = new System.Drawing.Size(84, 40);
            this.DenyBootUpdateCheckRadioButton.TabIndex = 3;
            this.DenyBootUpdateCheckRadioButton.TabStop = true;
            this.DenyBootUpdateCheckRadioButton.Text = "無効";
            this.DenyBootUpdateCheckRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowBootUpdateCheckRadioButton
            // 
            this.AllowBootUpdateCheckRadioButton.AutoSize = true;
            this.AllowBootUpdateCheckRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowBootUpdateCheckRadioButton.Location = new System.Drawing.Point(3, 9);
            this.AllowBootUpdateCheckRadioButton.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.AllowBootUpdateCheckRadioButton.Name = "AllowBootUpdateCheckRadioButton";
            this.AllowBootUpdateCheckRadioButton.Size = new System.Drawing.Size(84, 40);
            this.AllowBootUpdateCheckRadioButton.TabIndex = 2;
            this.AllowBootUpdateCheckRadioButton.TabStop = true;
            this.AllowBootUpdateCheckRadioButton.Text = "有効";
            this.AllowBootUpdateCheckRadioButton.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1007, 387);
            this.Controls.Add(this.BootUpdateCheckPanel);
            this.Controls.Add(this.BootUpdateCheckLabel);
            this.Controls.Add(this.RecentShownContentsPanel);
            this.Controls.Add(this.RecentShownContentsLabel);
            this.Controls.Add(this.AutoSearchPanel);
            this.Controls.Add(this.AutoSearchLabel);
            this.Controls.Add(this.SetAutoLoadProjectTextBox);
            this.Controls.Add(this.AutoLoadProjectLabel);
            this.Controls.Add(this.ShowUserAssistPanel);
            this.Controls.Add(this.ShowUserAssistLabel);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AllowConfidentialDataPanel);
            this.Controls.Add(this.AllowEditPanel);
            this.Controls.Add(this.AllowConfidentialDataLabel);
            this.Controls.Add(this.AllowEditLabel);
            this.Controls.Add(this.OpenLastTimeProjectCheckBox);
            this.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "設定";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.AllowEditPanel.ResumeLayout(false);
            this.AllowEditPanel.PerformLayout();
            this.AllowConfidentialDataPanel.ResumeLayout(false);
            this.AllowConfidentialDataPanel.PerformLayout();
            this.ShowUserAssistPanel.ResumeLayout(false);
            this.ShowUserAssistPanel.PerformLayout();
            this.AutoSearchPanel.ResumeLayout(false);
            this.AutoSearchPanel.PerformLayout();
            this.RecentShownContentsPanel.ResumeLayout(false);
            this.RecentShownContentsPanel.PerformLayout();
            this.BootUpdateCheckPanel.ResumeLayout(false);
            this.BootUpdateCheckPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AllowEditLabel;
        private System.Windows.Forms.Label AllowConfidentialDataLabel;
        private System.Windows.Forms.RadioButton AllowEditRadioButton;
        private System.Windows.Forms.RadioButton DenyEditRadioButton;
        private System.Windows.Forms.Panel AllowEditPanel;
        private System.Windows.Forms.RadioButton AllowConfidentialDataRadioButton;
        private System.Windows.Forms.RadioButton DenyConfidentialDataRadioButton;
        private System.Windows.Forms.Panel AllowConfidentialDataPanel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Panel ShowUserAssistPanel;
        private System.Windows.Forms.RadioButton HideUserAssistRadioButton;
        private System.Windows.Forms.RadioButton ShowUserAssistRadioButton;
        private System.Windows.Forms.Label ShowUserAssistLabel;
        private System.Windows.Forms.Label AutoLoadProjectLabel;
        private System.Windows.Forms.TextBox SetAutoLoadProjectTextBox;
        private System.Windows.Forms.CheckBox OpenLastTimeProjectCheckBox;
        private System.Windows.Forms.Panel AutoSearchPanel;
        private System.Windows.Forms.RadioButton DenyAutoSearchRadioButton;
        private System.Windows.Forms.RadioButton AllowAutoSearchRadioButton;
        private System.Windows.Forms.Label AutoSearchLabel;
        private System.Windows.Forms.Label RecentShownContentsLabel;
        private System.Windows.Forms.Panel RecentShownContentsPanel;
        private System.Windows.Forms.RadioButton DiscardRecentShownContentsRadioButton;
        private System.Windows.Forms.RadioButton SaveRecentShownContentsRadioButton;
        private System.Windows.Forms.Label BootUpdateCheckLabel;
        private System.Windows.Forms.Panel BootUpdateCheckPanel;
        private System.Windows.Forms.RadioButton DenyBootUpdateCheckRadioButton;
        private System.Windows.Forms.RadioButton AllowBootUpdateCheckRadioButton;
    }
}