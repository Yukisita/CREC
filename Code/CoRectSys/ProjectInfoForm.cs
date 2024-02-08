/*
MakeNewProjectForm
Copyright (c) [2022-2024] [Yukisita Mfg.]
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ColRECt
{
    public partial class ProjectInfoForm : Form
    {
        // 共用の変数
        string TargetCRECPath = ""; //管理ファイル（.crec）のファイルパス
        string[] cols;// List等読み込み用
        string ColorSetting = "Blue";

        public ProjectInfoForm(string tmp, string colorSetting)
        {
            InitializeComponent();
            TargetCRECPath = tmp;
            ColorSetting = colorSetting;
            SetColorMethod();
            ReadProjcetInformationMethod();
        }

        private void ReadProjcetInformationMethod()// 管理ファイルを読み込んで表示
        {
            if (TargetCRECPath.Length > 0)// 編集の場合は既存の.crecを読み込み
            {
                IEnumerable<string> tmp = null;
                tmp = File.ReadLines(TargetCRECPath, Encoding.GetEncoding("UTF-8"));
                foreach (string line in tmp)
                {
                    cols = line.Split(',');
                    switch (cols[0])
                    {
                        case "projectname":
                            ProjcetNameLabel.Text = "　名　称　：" + cols[1];
                            break;
                        case "created":
                            ProjcetCreatedDateLabel.Text = "　作　成　日　：" + cols[1];
                            break;
                        case "modified":
                            ProjcetModifiedDateLabel.Text = "　更　新　日　：" + cols[1];
                            break;
                        case "accessed":
                            ProjcetAccessedDateLabel.Text = "最終アクセス日：" + cols[1];
                            break;
                    }
                }
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
