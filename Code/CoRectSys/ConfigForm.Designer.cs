﻿namespace CoRectSys
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
            this.label1 = new System.Windows.Forms.Label();
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
            this.AllowEditLabel.Location = new System.Drawing.Point(61, 35);
            this.AllowEditLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.AllowEditLabel.Name = "AllowEditLabel";
            this.AllowEditLabel.Size = new System.Drawing.Size(192, 51);
            this.AllowEditLabel.TabIndex = 0;
            this.AllowEditLabel.Text = "データ編集";
            // 
            // AllowConfidentialDataLabel
            // 
            this.AllowConfidentialDataLabel.AutoSize = true;
            this.AllowConfidentialDataLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowConfidentialDataLabel.Location = new System.Drawing.Point(28, 140);
            this.AllowConfidentialDataLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.AllowConfidentialDataLabel.Name = "AllowConfidentialDataLabel";
            this.AllowConfidentialDataLabel.Size = new System.Drawing.Size(260, 51);
            this.AllowConfidentialDataLabel.TabIndex = 4;
            this.AllowConfidentialDataLabel.Text = "機密データ表示";
            // 
            // AllowEditRadioButton
            // 
            this.AllowEditRadioButton.AutoSize = true;
            this.AllowEditRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowEditRadioButton.Location = new System.Drawing.Point(10, 12);
            this.AllowEditRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowEditRadioButton.Name = "AllowEditRadioButton";
            this.AllowEditRadioButton.Size = new System.Drawing.Size(115, 55);
            this.AllowEditRadioButton.TabIndex = 2;
            this.AllowEditRadioButton.TabStop = true;
            this.AllowEditRadioButton.Text = "許可";
            this.AllowEditRadioButton.UseVisualStyleBackColor = true;
            // 
            // DenyEditRadioButton
            // 
            this.DenyEditRadioButton.AutoSize = true;
            this.DenyEditRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyEditRadioButton.Location = new System.Drawing.Point(158, 12);
            this.DenyEditRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.DenyEditRadioButton.Name = "DenyEditRadioButton";
            this.DenyEditRadioButton.Size = new System.Drawing.Size(115, 55);
            this.DenyEditRadioButton.TabIndex = 3;
            this.DenyEditRadioButton.TabStop = true;
            this.DenyEditRadioButton.Text = "拒否";
            this.DenyEditRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowEditPanel
            // 
            this.AllowEditPanel.Controls.Add(this.DenyEditRadioButton);
            this.AllowEditPanel.Controls.Add(this.AllowEditRadioButton);
            this.AllowEditPanel.Location = new System.Drawing.Point(292, 16);
            this.AllowEditPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowEditPanel.Name = "AllowEditPanel";
            this.AllowEditPanel.Size = new System.Drawing.Size(287, 84);
            this.AllowEditPanel.TabIndex = 1;
            // 
            // AllowConfidentialDataRadioButton
            // 
            this.AllowConfidentialDataRadioButton.AutoSize = true;
            this.AllowConfidentialDataRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowConfidentialDataRadioButton.Location = new System.Drawing.Point(10, 12);
            this.AllowConfidentialDataRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowConfidentialDataRadioButton.Name = "AllowConfidentialDataRadioButton";
            this.AllowConfidentialDataRadioButton.Size = new System.Drawing.Size(115, 55);
            this.AllowConfidentialDataRadioButton.TabIndex = 6;
            this.AllowConfidentialDataRadioButton.TabStop = true;
            this.AllowConfidentialDataRadioButton.Text = "有効";
            this.AllowConfidentialDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // DenyConfidentialDataRadioButton
            // 
            this.DenyConfidentialDataRadioButton.AutoSize = true;
            this.DenyConfidentialDataRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyConfidentialDataRadioButton.Location = new System.Drawing.Point(158, 12);
            this.DenyConfidentialDataRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.DenyConfidentialDataRadioButton.Name = "DenyConfidentialDataRadioButton";
            this.DenyConfidentialDataRadioButton.Size = new System.Drawing.Size(115, 55);
            this.DenyConfidentialDataRadioButton.TabIndex = 7;
            this.DenyConfidentialDataRadioButton.TabStop = true;
            this.DenyConfidentialDataRadioButton.Text = "無効";
            this.DenyConfidentialDataRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowConfidentialDataPanel
            // 
            this.AllowConfidentialDataPanel.Controls.Add(this.DenyConfidentialDataRadioButton);
            this.AllowConfidentialDataPanel.Controls.Add(this.AllowConfidentialDataRadioButton);
            this.AllowConfidentialDataPanel.Location = new System.Drawing.Point(292, 124);
            this.AllowConfidentialDataPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowConfidentialDataPanel.Name = "AllowConfidentialDataPanel";
            this.AllowConfidentialDataPanel.Size = new System.Drawing.Size(287, 75);
            this.AllowConfidentialDataPanel.TabIndex = 5;
            // 
            // SaveButton
            // 
            this.SaveButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SaveButton.Location = new System.Drawing.Point(1069, 441);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(130, 65);
            this.SaveButton.TabIndex = 11;
            this.SaveButton.Text = "保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ShowUserAssistPanel
            // 
            this.ShowUserAssistPanel.Controls.Add(this.HideUserAssistRadioButton);
            this.ShowUserAssistPanel.Controls.Add(this.ShowUserAssistRadioButton);
            this.ShowUserAssistPanel.Location = new System.Drawing.Point(294, 222);
            this.ShowUserAssistPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.ShowUserAssistPanel.Name = "ShowUserAssistPanel";
            this.ShowUserAssistPanel.Size = new System.Drawing.Size(287, 75);
            this.ShowUserAssistPanel.TabIndex = 8;
            // 
            // HideUserAssistRadioButton
            // 
            this.HideUserAssistRadioButton.AutoSize = true;
            this.HideUserAssistRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.HideUserAssistRadioButton.Location = new System.Drawing.Point(158, 12);
            this.HideUserAssistRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.HideUserAssistRadioButton.Name = "HideUserAssistRadioButton";
            this.HideUserAssistRadioButton.Size = new System.Drawing.Size(115, 55);
            this.HideUserAssistRadioButton.TabIndex = 10;
            this.HideUserAssistRadioButton.TabStop = true;
            this.HideUserAssistRadioButton.Text = "無効";
            this.HideUserAssistRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShowUserAssistRadioButton
            // 
            this.ShowUserAssistRadioButton.AutoSize = true;
            this.ShowUserAssistRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ShowUserAssistRadioButton.Location = new System.Drawing.Point(10, 12);
            this.ShowUserAssistRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.ShowUserAssistRadioButton.Name = "ShowUserAssistRadioButton";
            this.ShowUserAssistRadioButton.Size = new System.Drawing.Size(115, 55);
            this.ShowUserAssistRadioButton.TabIndex = 9;
            this.ShowUserAssistRadioButton.TabStop = true;
            this.ShowUserAssistRadioButton.Text = "有効";
            this.ShowUserAssistRadioButton.UseVisualStyleBackColor = true;
            // 
            // ShowUserAssistLabel
            // 
            this.ShowUserAssistLabel.AutoSize = true;
            this.ShowUserAssistLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ShowUserAssistLabel.Location = new System.Drawing.Point(28, 238);
            this.ShowUserAssistLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.ShowUserAssistLabel.Name = "ShowUserAssistLabel";
            this.ShowUserAssistLabel.Size = new System.Drawing.Size(260, 51);
            this.ShowUserAssistLabel.TabIndex = 9;
            this.ShowUserAssistLabel.Text = "ユーザ補助表示";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(28, 334);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(430, 51);
            this.label1.TabIndex = 13;
            this.label1.Text = "自動プロジェクト読み込み";
            // 
            // SetAutoLoadProjectTextBox
            // 
            this.SetAutoLoadProjectTextBox.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SetAutoLoadProjectTextBox.Location = new System.Drawing.Point(37, 388);
            this.SetAutoLoadProjectTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.SetAutoLoadProjectTextBox.Name = "SetAutoLoadProjectTextBox";
            this.SetAutoLoadProjectTextBox.Size = new System.Drawing.Size(541, 42);
            this.SetAutoLoadProjectTextBox.TabIndex = 14;
            // 
            // OpenLastTimeProjectCheckBox
            // 
            this.OpenLastTimeProjectCheckBox.AutoSize = true;
            this.OpenLastTimeProjectCheckBox.Location = new System.Drawing.Point(37, 446);
            this.OpenLastTimeProjectCheckBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.OpenLastTimeProjectCheckBox.Name = "OpenLastTimeProjectCheckBox";
            this.OpenLastTimeProjectCheckBox.Size = new System.Drawing.Size(456, 55);
            this.OpenLastTimeProjectCheckBox.TabIndex = 15;
            this.OpenLastTimeProjectCheckBox.Text = "前回のプロジェクトを開く";
            this.OpenLastTimeProjectCheckBox.UseVisualStyleBackColor = true;
            this.OpenLastTimeProjectCheckBox.CheckedChanged += new System.EventHandler(this.OpenLastTimeProjectCheckBox_CheckedChanged);
            // 
            // AutoSearchPanel
            // 
            this.AutoSearchPanel.Controls.Add(this.DenyAutoSearchRadioButton);
            this.AutoSearchPanel.Controls.Add(this.AllowAutoSearchRadioButton);
            this.AutoSearchPanel.Location = new System.Drawing.Point(912, 16);
            this.AutoSearchPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AutoSearchPanel.Name = "AutoSearchPanel";
            this.AutoSearchPanel.Size = new System.Drawing.Size(287, 84);
            this.AutoSearchPanel.TabIndex = 17;
            // 
            // DenyAutoSearchRadioButton
            // 
            this.DenyAutoSearchRadioButton.AutoSize = true;
            this.DenyAutoSearchRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyAutoSearchRadioButton.Location = new System.Drawing.Point(158, 12);
            this.DenyAutoSearchRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.DenyAutoSearchRadioButton.Name = "DenyAutoSearchRadioButton";
            this.DenyAutoSearchRadioButton.Size = new System.Drawing.Size(115, 55);
            this.DenyAutoSearchRadioButton.TabIndex = 3;
            this.DenyAutoSearchRadioButton.TabStop = true;
            this.DenyAutoSearchRadioButton.Text = "無効";
            this.DenyAutoSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowAutoSearchRadioButton
            // 
            this.AllowAutoSearchRadioButton.AutoSize = true;
            this.AllowAutoSearchRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowAutoSearchRadioButton.Location = new System.Drawing.Point(10, 12);
            this.AllowAutoSearchRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowAutoSearchRadioButton.Name = "AllowAutoSearchRadioButton";
            this.AllowAutoSearchRadioButton.Size = new System.Drawing.Size(115, 55);
            this.AllowAutoSearchRadioButton.TabIndex = 2;
            this.AllowAutoSearchRadioButton.TabStop = true;
            this.AllowAutoSearchRadioButton.Text = "有効";
            this.AllowAutoSearchRadioButton.UseVisualStyleBackColor = true;
            // 
            // AutoSearchLabel
            // 
            this.AutoSearchLabel.AutoSize = true;
            this.AutoSearchLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AutoSearchLabel.Location = new System.Drawing.Point(705, 35);
            this.AutoSearchLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.AutoSearchLabel.Name = "AutoSearchLabel";
            this.AutoSearchLabel.Size = new System.Drawing.Size(158, 51);
            this.AutoSearchLabel.TabIndex = 16;
            this.AutoSearchLabel.Text = "自動検索";
            // 
            // RecentShownContentsLabel
            // 
            this.RecentShownContentsLabel.AutoSize = true;
            this.RecentShownContentsLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RecentShownContentsLabel.Location = new System.Drawing.Point(671, 140);
            this.RecentShownContentsLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.RecentShownContentsLabel.Name = "RecentShownContentsLabel";
            this.RecentShownContentsLabel.Size = new System.Drawing.Size(226, 51);
            this.RecentShownContentsLabel.TabIndex = 18;
            this.RecentShownContentsLabel.Text = "表示履歴保存";
            // 
            // RecentShownContentsPanel
            // 
            this.RecentShownContentsPanel.Controls.Add(this.DiscardRecentShownContentsRadioButton);
            this.RecentShownContentsPanel.Controls.Add(this.SaveRecentShownContentsRadioButton);
            this.RecentShownContentsPanel.Location = new System.Drawing.Point(912, 124);
            this.RecentShownContentsPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.RecentShownContentsPanel.Name = "RecentShownContentsPanel";
            this.RecentShownContentsPanel.Size = new System.Drawing.Size(287, 75);
            this.RecentShownContentsPanel.TabIndex = 19;
            // 
            // DiscardRecentShownContentsRadioButton
            // 
            this.DiscardRecentShownContentsRadioButton.AutoSize = true;
            this.DiscardRecentShownContentsRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DiscardRecentShownContentsRadioButton.Location = new System.Drawing.Point(158, 12);
            this.DiscardRecentShownContentsRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.DiscardRecentShownContentsRadioButton.Name = "DiscardRecentShownContentsRadioButton";
            this.DiscardRecentShownContentsRadioButton.Size = new System.Drawing.Size(115, 55);
            this.DiscardRecentShownContentsRadioButton.TabIndex = 3;
            this.DiscardRecentShownContentsRadioButton.TabStop = true;
            this.DiscardRecentShownContentsRadioButton.Text = "無効";
            this.DiscardRecentShownContentsRadioButton.UseVisualStyleBackColor = true;
            // 
            // SaveRecentShownContentsRadioButton
            // 
            this.SaveRecentShownContentsRadioButton.AutoSize = true;
            this.SaveRecentShownContentsRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SaveRecentShownContentsRadioButton.Location = new System.Drawing.Point(10, 12);
            this.SaveRecentShownContentsRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.SaveRecentShownContentsRadioButton.Name = "SaveRecentShownContentsRadioButton";
            this.SaveRecentShownContentsRadioButton.Size = new System.Drawing.Size(115, 55);
            this.SaveRecentShownContentsRadioButton.TabIndex = 2;
            this.SaveRecentShownContentsRadioButton.TabStop = true;
            this.SaveRecentShownContentsRadioButton.Text = "有効";
            this.SaveRecentShownContentsRadioButton.UseVisualStyleBackColor = true;
            // 
            // BootUpdateCheckLabel
            // 
            this.BootUpdateCheckLabel.AutoSize = true;
            this.BootUpdateCheckLabel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BootUpdateCheckLabel.Location = new System.Drawing.Point(654, 238);
            this.BootUpdateCheckLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.BootUpdateCheckLabel.Name = "BootUpdateCheckLabel";
            this.BootUpdateCheckLabel.Size = new System.Drawing.Size(260, 51);
            this.BootUpdateCheckLabel.TabIndex = 20;
            this.BootUpdateCheckLabel.Text = "起動時更新確認";
            // 
            // BootUpdateCheckPanel
            // 
            this.BootUpdateCheckPanel.Controls.Add(this.DenyBootUpdateCheckRadioButton);
            this.BootUpdateCheckPanel.Controls.Add(this.AllowBootUpdateCheckRadioButton);
            this.BootUpdateCheckPanel.Location = new System.Drawing.Point(912, 222);
            this.BootUpdateCheckPanel.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.BootUpdateCheckPanel.Name = "BootUpdateCheckPanel";
            this.BootUpdateCheckPanel.Size = new System.Drawing.Size(287, 75);
            this.BootUpdateCheckPanel.TabIndex = 21;
            // 
            // DenyBootUpdateCheckRadioButton
            // 
            this.DenyBootUpdateCheckRadioButton.AutoSize = true;
            this.DenyBootUpdateCheckRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DenyBootUpdateCheckRadioButton.Location = new System.Drawing.Point(158, 12);
            this.DenyBootUpdateCheckRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.DenyBootUpdateCheckRadioButton.Name = "DenyBootUpdateCheckRadioButton";
            this.DenyBootUpdateCheckRadioButton.Size = new System.Drawing.Size(115, 55);
            this.DenyBootUpdateCheckRadioButton.TabIndex = 3;
            this.DenyBootUpdateCheckRadioButton.TabStop = true;
            this.DenyBootUpdateCheckRadioButton.Text = "無効";
            this.DenyBootUpdateCheckRadioButton.UseVisualStyleBackColor = true;
            // 
            // AllowBootUpdateCheckRadioButton
            // 
            this.AllowBootUpdateCheckRadioButton.AutoSize = true;
            this.AllowBootUpdateCheckRadioButton.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AllowBootUpdateCheckRadioButton.Location = new System.Drawing.Point(10, 12);
            this.AllowBootUpdateCheckRadioButton.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.AllowBootUpdateCheckRadioButton.Name = "AllowBootUpdateCheckRadioButton";
            this.AllowBootUpdateCheckRadioButton.Size = new System.Drawing.Size(115, 55);
            this.AllowBootUpdateCheckRadioButton.TabIndex = 2;
            this.AllowBootUpdateCheckRadioButton.TabStop = true;
            this.AllowBootUpdateCheckRadioButton.Text = "有効";
            this.AllowBootUpdateCheckRadioButton.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1237, 542);
            this.Controls.Add(this.BootUpdateCheckPanel);
            this.Controls.Add(this.BootUpdateCheckLabel);
            this.Controls.Add(this.RecentShownContentsPanel);
            this.Controls.Add(this.RecentShownContentsLabel);
            this.Controls.Add(this.AutoSearchPanel);
            this.Controls.Add(this.AutoSearchLabel);
            this.Controls.Add(this.SetAutoLoadProjectTextBox);
            this.Controls.Add(this.label1);
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
            this.Margin = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConfigForm";
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
        private System.Windows.Forms.Label label1;
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