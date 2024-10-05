/*
MakeNewProjectForm
Copyright (c) [2022-2024] [S.Yukisita]
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

namespace ColRECt
{
    public partial class ProjectInfoForm : Form
    {
        // 共用の変数
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 現在表示中のプロジェクトの設定値
        string ColorSetting = "Blue";

        public ProjectInfoForm(ProjectSettingValuesClass projectSettingValues)
        {
            InitializeComponent();
            CurrentProjectSettingValues = projectSettingValues;
            ColorSetting = projectSettingValues.ColorSetting.ToString();
            SetColorMethod();
            ReadProjcetInformationMethod();
        }

        private void ReadProjcetInformationMethod()// 管理ファイルを読み込んで表示
        {
            if (CurrentProjectSettingValues.Name.Length > 0)// 編集の場合は既存の.crecを読み込み
            {
                ProjcetNameLabel.Text = "　名　称　：" + CurrentProjectSettingValues.Name;
                ProjcetCreatedDateLabel.Text = "　作　成　日　：" + CurrentProjectSettingValues.CreatedDate;
                ProjcetModifiedDateLabel.Text = "　更　新　日　：" + CurrentProjectSettingValues.ModifiedDate;
                ProjcetAccessedDateLabel.Text = "最終アクセス日：" + CurrentProjectSettingValues.AccessedDate;
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
    }
}