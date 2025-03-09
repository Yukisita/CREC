/*
ConfigForm
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CREC
{
    public partial class ConfigForm : Form
    {
        ConfigValuesClass ConfigValues = new ConfigValuesClass();// 設定値
        ColorValue ColorSetting = ColorValue.Blue;// 色設定
        readonly XElement LanguageFile;// 言語ファイル
        public bool ReturnConfigSaved { get; private set; } = false;// 保存されて終了した場合はTrue

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="configValues">設定値</param>
        /// <param name="colorSetting">色設定</param>
        /// <param name="languageFile">言語ファイル</param>
        public ConfigForm(ConfigValuesClass configValues, ColorValue colorSetting, XElement languageFile)
        {
            InitializeComponent();
            ConfigValues = configValues;
            ColorSetting = colorSetting;
            SetColorMethod();
            LanguageFile = languageFile;
        }

        /// <summary>
        /// フォーム読み込み時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // 言語設定
            SetLanguage();
            // 現在の設定値を反映
            if (ConfigValues.AllowEdit)
            {
                AllowEditRadioButton.Checked = true;
            }
            else
            {
                DenyEditRadioButton.Checked = true;
            }
            if (ConfigValues.ShowConfidentialData)
            {
                AllowConfidentialDataRadioButton.Checked = true;
            }
            else
            {
                DenyConfidentialDataRadioButton.Checked = true;
            }
            if (ConfigValues.ShowUserAssistToolTips)
            {
                ShowUserAssistRadioButton.Checked = true;
            }
            else
            {
                HideUserAssistRadioButton.Checked = true;
            }
            SetAutoLoadProjectTextBox.Text = ConfigValues.AutoLoadProjectPath;
            if (ConfigValues.OpenLastTimeProject)
            {
                SetAutoLoadProjectTextBox.ReadOnly = true;
                OpenLastTimeProjectCheckBox.Checked = true;
            }
            else
            {
                SetAutoLoadProjectTextBox.ReadOnly = false;
                OpenLastTimeProjectCheckBox.Checked = false;
            }
            if (ConfigValues.AutoSearch)
            {
                AllowAutoSearchRadioButton.Checked = true;
            }
            else
            {
                DenyAutoSearchRadioButton.Checked = true;
            }
            if (ConfigValues.RecentShownContents)
            {
                SaveRecentShownContentsRadioButton.Checked = true;
            }
            else
            {
                DiscardRecentShownContentsRadioButton.Checked = true;
            }
            if (ConfigValues.BootUpdateCheck)
            {
                AllowBootUpdateCheckRadioButton.Checked = true;
            }
            else
            {
                DenyBootUpdateCheckRadioButton.Checked = true;
            }
        }

        /// <summary>
        /// 保存して終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // 変更内容を反映
            if (AllowEditRadioButton.Checked)
            {
                ConfigValues.AllowEdit = true;
            }
            else if (DenyEditRadioButton.Checked)
            {
                ConfigValues.AllowEdit = false;
            }
            if (AllowConfidentialDataRadioButton.Checked)
            {
                ConfigValues.ShowConfidentialData = true;
            }
            else if (DenyConfidentialDataRadioButton.Checked)
            {
                ConfigValues.ShowConfidentialData = false;
            }
            if (ShowUserAssistRadioButton.Checked)
            {
                ConfigValues.ShowUserAssistToolTips = true;
            }
            else if (HideUserAssistRadioButton.Checked)
            {
                ConfigValues.ShowUserAssistToolTips = false;
            }
            ConfigValues.AutoLoadProjectPath = SetAutoLoadProjectTextBox.Text;
            if (OpenLastTimeProjectCheckBox.Checked)
            {
                ConfigValues.OpenLastTimeProject = true;
            }
            else
            {
                ConfigValues.OpenLastTimeProject = false;
            }
            if (AllowAutoSearchRadioButton.Checked)
            {
                ConfigValues.AutoSearch = true;
            }
            else if (DenyAutoSearchRadioButton.Checked)
            {
                ConfigValues.AutoSearch = false;
            }
            if (SaveRecentShownContentsRadioButton.Checked)
            {
                ConfigValues.RecentShownContents = true;
            }
            else if (DiscardRecentShownContentsRadioButton.Checked)
            {
                ConfigValues.RecentShownContents = false;
            }
            if (AllowBootUpdateCheckRadioButton.Checked)
            {
                ConfigValues.BootUpdateCheck = true;
            }
            else if (DenyBootUpdateCheckRadioButton.Checked)
            {
                ConfigValues.BootUpdateCheck = false;
            }
            // config.sysに保存
            ReturnConfigSaved = ConfigClass.SaveConfigValues(ConfigValues, ConfigValues.AutoLoadProjectPath);
            this.Close();
        }

        /// <summary>
        /// 色設定
        /// </summary>
        private void SetColorMethod()
        {
            switch (ColorSetting)
            {
                case ColorValue.Blue:
                    this.BackColor = Color.AliceBlue;
                    break;
                case ColorValue.White:
                    this.BackColor = Color.WhiteSmoke;
                    break;
                case ColorValue.Sakura:
                    this.BackColor = Color.LavenderBlush;
                    break;
                case ColorValue.Green:
                    this.BackColor = Color.Honeydew;
                    break;
                default:
                    this.BackColor = Color.AliceBlue;
                    ColorSetting = ColorValue.Blue;
                    break;
            }
        }

        /// <summary>
        /// 前回のプロジェクトを開くモードのときはSetAutoLoadProjectTextBoxを編集不可にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLastTimeProjectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (OpenLastTimeProjectCheckBox.Checked)
            {
                SetAutoLoadProjectTextBox.ReadOnly = true;
            }
            else
            {
                SetAutoLoadProjectTextBox.ReadOnly = false;
            }
        }

        /// <summary>
        /// 言語設定
        /// </summary>
        private void SetLanguage()
        {
            this.Text = LanguageSettingClass.GetOtherMessage("FormName", "ConfigForm", LanguageFile);
            IEnumerable<XElement> buttonItemDataList = from item in LanguageFile.Elements("ConfigForm").Elements("Button").Elements("item") select item;
            foreach (XElement itemData in buttonItemDataList)
            {
                try
                {
                    Control[] targetContrl = Controls.Find(itemData.Element("itemname").Value, true);
                    if (targetContrl.Length > 0)
                    {
                        targetContrl[0].Text = itemData.Element("itemtext").Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            IEnumerable<XElement> labelItemDataList = from item in LanguageFile.Elements("ConfigForm").Elements("Label").Elements("item") select item;
            foreach (XElement itemData in labelItemDataList)
            {
                try
                {
                    Control[] targetContrl = Controls.Find(itemData.Element("itemname").Value, true);
                    if (targetContrl.Length > 0)
                    {
                        targetContrl[0].Text = itemData.Element("itemtext").Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            IEnumerable<XElement> radioButtonItemDataList = from item in LanguageFile.Elements("ConfigForm").Elements("RadioButton").Elements("item") select item;
            foreach (XElement itemData in radioButtonItemDataList)
            {
                try
                {
                    Control[] targetContrl = Controls.Find(itemData.Element("itemname").Value, true);
                    if (targetContrl.Length > 0)
                    {
                        targetContrl[0].Text = itemData.Element("itemtext").Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            IEnumerable<XElement> checkBoxItemDataList = from item in LanguageFile.Elements("ConfigForm").Elements("CheckBox").Elements("item") select item;
            foreach (XElement itemData in checkBoxItemDataList)
            {
                try
                {
                    Control[] targetContrl = Controls.Find(itemData.Element("itemname").Value, true);
                    if (targetContrl.Length > 0)
                    {
                        targetContrl[0].Text = itemData.Element("itemtext").Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}