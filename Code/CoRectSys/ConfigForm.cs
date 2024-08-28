/*
ConfigForm
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Linq;

namespace CREC
{
    public partial class ConfigForm : Form
    {
        readonly string ConfigFile = "config.sys";
        string reader;
        string[] rows;
        string row;
        string[] cols;
        string ColorSetting = "Blue";
        string CurrentLanguageFileName = "";// 言語設定
        int FontsizeOffset = 0;// 文字サイズオフセット量
        int ConfigNumber;
        readonly XElement LanguageFile;// 言語ファイル
        public bool ReturnConfigSaved { get; private set; } = false;// 保存されて終了した場合はTrue

        public ConfigForm(string colorSetting, string currentLanguage, XElement languageFile, int fontsizeOffset)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            CurrentLanguageFileName = currentLanguage;
            FontsizeOffset = fontsizeOffset;
            SetColorMethod();
            LanguageFile = languageFile;
        }

        private void ConfigForm_Load(object sender, EventArgs e)// 読み込み
        {
            // 言語設定
            SetLanguage();
            reader = File.ReadAllText(ConfigFile, Encoding.GetEncoding("UTF-8"));
            string[] tmp = File.ReadAllLines(ConfigFile, Encoding.GetEncoding("UTF-8"));
            rows = reader.Trim().Replace("\r", "").Split('\n');
            // チェックボックスをデフォルト値で初期化
            AllowEditRadioButton.Checked = true;
            AllowConfidentialDataRadioButton.Checked = true;
            ShowUserAssistRadioButton.Checked = true;
            OpenLastTimeProjectCheckBox.Checked = false;
            AllowAutoSearchRadioButton.Checked = true;
            AllowBootUpdateCheckRadioButton.Checked = true;
            // config.sysから該当項目読み込み
            for (ConfigNumber = 0; ConfigNumber <= Convert.ToInt32(tmp.Length - 1); ConfigNumber++)
            {
                row = rows[ConfigNumber];
                cols = row.Split(',');
                switch (cols[0])
                {
                    case "AllowEdit":
                        if (cols[1] == "true")
                        {
                            AllowEditRadioButton.Checked = true;
                        }
                        else
                        {
                            DenyEditRadioButton.Checked = true;
                        }
                        break;
                    case "ShowConfidentialData":
                        if (cols[1] == "true")
                        {
                            AllowConfidentialDataRadioButton.Checked = true;
                        }
                        else
                        {
                            DenyConfidentialDataRadioButton.Checked = true;
                        }
                        break;
                    case "ShowUserAssistToolTips":
                        if (cols[1] == "true")
                        {
                            ShowUserAssistRadioButton.Checked = true;
                        }
                        else
                        {
                            HideUserAssistRadioButton.Checked = true;
                        }
                        break;
                    case "AutoLoadProject":
                        SetAutoLoadProjectTextBox.Text = cols[1];
                        break;
                    case "OpenLastTimeProject":
                        if (cols[1] == "true")
                        {
                            SetAutoLoadProjectTextBox.ReadOnly = true;
                            OpenLastTimeProjectCheckBox.Checked = true;
                        }
                        else
                        {
                            SetAutoLoadProjectTextBox.ReadOnly = false;
                            OpenLastTimeProjectCheckBox.Checked = false;
                        }
                        break;
                    case "AutoSearch":
                        if (cols[1] == "true")
                        {
                            AllowAutoSearchRadioButton.Checked = true;
                        }
                        else
                        {
                            DenyAutoSearchRadioButton.Checked = true;
                        }
                        break;
                    case "RecentShownContents":
                        if (cols[1] == "true")
                        {
                            SaveRecentShownContentsRadioButton.Checked = true;
                        }
                        else
                        {
                            DiscardRecentShownContentsRadioButton.Checked = true;
                        }
                        break;
                    case "BootUpdateCheck":
                        if (cols[1] == "true")
                        {
                            AllowBootUpdateCheckRadioButton.Checked = true;
                        }
                        else
                        {
                            DenyBootUpdateCheckRadioButton.Checked = true;
                        }
                        break;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)// 保存して終了
        {
            StreamWriter configfile = new StreamWriter("config.sys", false, Encoding.GetEncoding("UTF-8"));
            if (AllowEditRadioButton.Checked)
            {
                configfile.WriteLine("AllowEdit,true");
            }
            else if (DenyEditRadioButton.Checked)
            {
                configfile.WriteLine("AllowEdit,false");
            }
            if (AllowConfidentialDataRadioButton.Checked)
            {
                configfile.WriteLine("ShowConfidentialData,true");
            }
            else if (DenyConfidentialDataRadioButton.Checked)
            {
                configfile.WriteLine("ShowConfidentialData,false");
            }
            if (ShowUserAssistRadioButton.Checked)
            {
                configfile.WriteLine("ShowUserAssistToolTips,true");
            }
            else if (HideUserAssistRadioButton.Checked)
            {
                configfile.WriteLine("ShowUserAssistToolTips,false");
            }
            configfile.WriteLine("AutoLoadProject,{0}", SetAutoLoadProjectTextBox.Text);
            if (OpenLastTimeProjectCheckBox.Checked)
            {
                configfile.WriteLine("OpenLastTimeProject,true");
            }
            else
            {
                configfile.WriteLine("OpenLastTimeProject,false");
            }
            if (AllowAutoSearchRadioButton.Checked)
            {
                configfile.WriteLine("AutoSearch,true");
            }
            else if (DenyAutoSearchRadioButton.Checked)
            {
                configfile.WriteLine("AutoSearch,false");
            }
            if (SaveRecentShownContentsRadioButton.Checked)
            {
                configfile.WriteLine("RecentShownContents,true");
            }
            else if (DiscardRecentShownContentsRadioButton.Checked)
            {
                configfile.WriteLine("RecentShownContents,false");
            }
            if (AllowBootUpdateCheckRadioButton.Checked)
            {
                configfile.WriteLine("BootUpdateCheck,true");
            }
            else if (DenyBootUpdateCheckRadioButton.Checked)
            {
                configfile.WriteLine("BootUpdateCheck,false");
            }
            configfile.WriteLine("ColorSetting,{0}", ColorSetting);
            configfile.WriteLine("Language,{0}", CurrentLanguageFileName);
            configfile.WriteLine("FontsizeOffset,{0}", FontsizeOffset);
            configfile.Close();
            ReturnConfigSaved = true;
            this.Close();
        }

        private void SetColorMethod()// 色設定のメソッド
        {
            switch (ColorSetting)
            {
                case "Blue":
                    this.BackColor = Color.AliceBlue;
                    break;
                case "White":
                    this.BackColor = Color.WhiteSmoke;
                    break;
                case "Sakura":
                    this.BackColor = Color.LavenderBlush;
                    break;
                case "Green":
                    this.BackColor = Color.Honeydew;
                    break;
                default:
                    this.BackColor = Color.AliceBlue;
                    ColorSetting = "Blue";
                    break;
            }
        }

        private void OpenLastTimeProjectCheckBox_CheckedChanged(object sender, EventArgs e)// 前回のプロジェクトを開くモードのときはSetAutoLoadProjectTextBoxを編集不可にする
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

        #region 言語設定
        private void SetLanguage()// 言語ファイル（xml）を読み込んで表示する処理
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
        #endregion
    }
}