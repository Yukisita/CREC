/*
ConfigForm
Copyright (c) [2022-2023] [Yukisita Mfg.]
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

namespace CoRectSys
{
    public partial class ConfigForm : Form
    {
        string ConfigFile = "config.sys";
        string reader;
        string[] rows;
        string row;
        string[] cols;
        string ColorSetting = "Blue";
        int ConfigNumber;

        public ConfigForm(string colorSetting)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            SetColorMethod();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            reader = File.ReadAllText(ConfigFile, Encoding.GetEncoding("UTF-8"));
            string[] tmp = File.ReadAllLines(ConfigFile, Encoding.GetEncoding("UTF-8"));
            rows = reader.Trim().Replace("\r", "").Split('\n');
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
                        else if (cols[1] == "false") 
                        {
                            DenyEditRadioButton.Checked = true;
                        }
                        break;
                    case "ShowConfidentialData":
                        if (cols[1] == "true")
                        {
                            AllowConfidentialDataRadioButton.Checked = true;
                        }
                        else if (cols[1] == "false")
                        {
                            DenyConfidentialDataRadioButton.Checked = true;
                        }
                        break;
                    case "ShowUserAssistToolTips":
                        if (cols[1] == "true")
                        {
                            ShowUserAssistRadioButton.Checked = true;
                        }
                        else if (cols[1] == "false")
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
                }
            }
        }// config.sysを読み込み

        private void SaveButton_Click(object sender, EventArgs e)// 保存して終了
        {
            StreamWriter configfile = new StreamWriter("config.sys", false, Encoding.GetEncoding("UTF-8"));
            if(AllowEditRadioButton.Checked)
            {
                configfile.WriteLine("AllowEdit,true");
            }
            else if(DenyEditRadioButton.Checked)
            {
                configfile.WriteLine("AllowEdit,false");
            }
            if(AllowConfidentialDataRadioButton.Checked)
            {
                configfile.WriteLine("ShowConfidentialData,true");
            }
            else if(DenyConfidentialDataRadioButton.Checked)
            {
                configfile.WriteLine("ShowConfidentialData,false");
            }
            if(ShowUserAssistRadioButton.Checked) 
            {
                configfile.WriteLine("ShowUserAssistToolTips,true");
            }
            else if(HideUserAssistRadioButton.Checked)
            {
                configfile.WriteLine("ShowUserAssistToolTips,false");
            }
            configfile.WriteLine("AutoLoadProject,{0}",SetAutoLoadProjectTextBox.Text);
            if(OpenLastTimeProjectCheckBox.Checked)
            {
                configfile.WriteLine("OpenLastTimeProject,true");
            }
            else
            {
                configfile.WriteLine("OpenLastTimeProject,false");
            }
            configfile.Close();
            this.Close();
        }
    
        private void  SetColorMethod()// 色設定のメソッド
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
            if(OpenLastTimeProjectCheckBox.Checked) 
            {
                SetAutoLoadProjectTextBox.ReadOnly = true;
            }
            else
            {
                SetAutoLoadProjectTextBox.ReadOnly = false;
            }
        }
    }
}
