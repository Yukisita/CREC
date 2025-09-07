/*
MakeNewProjectForm
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CREC;
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace CREC
{
    public partial class ProjectInfoForm : Form
    {
        // 共用の変数
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 現在表示中のプロジェクトの設定値
        string ColorSetting = "Blue";
        readonly XElement LanguageFile;// 言語ファイル

        public ProjectInfoForm(ProjectSettingValuesClass projectSettingValues, XElement languageFile)
        {
            InitializeComponent();
            CurrentProjectSettingValues = projectSettingValues;
            ColorSetting = projectSettingValues.ColorSetting.ToString();
            LanguageFile = languageFile;
            SetColorMethod();
        }

        private void ProjectInfoForm_Load(object sender, EventArgs e)
        {
            SetLanguage();
            ReadProjcetInformationMethod();
        }

        private void ReadProjcetInformationMethod()// 管理ファイルを読み込んで表示
        {
            if (CurrentProjectSettingValues.Name.Length > 0)// 編集の場合は既存の.crecを読み込み
            {
                ProjcetNameLabel.Text = ProjcetNameLabel.Text + CurrentProjectSettingValues.Name;
                ProjcetCreatedDateLabel.Text = ProjcetCreatedDateLabel.Text + CurrentProjectSettingValues.CreatedDate;
                ProjcetModifiedDateLabel.Text = ProjcetModifiedDateLabel.Text + CurrentProjectSettingValues.ModifiedDate;
                ProjcetAccessedDateLabel.Text = ProjcetAccessedDateLabel.Text + CurrentProjectSettingValues.AccessedDate;
            }
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

        #region 言語設定
        private void SetLanguage()// 言語ファイル（xml）を読み込んで表示する処理
        {
            try
            {
                this.Text = LanguageSettingClass.GetOtherMessage("FormName", "ProjectInfoForm", LanguageFile);
                IEnumerable<XElement> labelItemDataList = from item in LanguageFile.Elements("ProjectInfoForm").Elements("Label").Elements("item") select item;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
            }
        }
        #endregion
    }
}