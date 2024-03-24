/*
UpdateHistory
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoRectSys
{
    public partial class UpdateHistory : Form
    {
        string ColorSetting = "Blue";
        public UpdateHistory(string colorSetting)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            SetColorMethod();
        }
        private void UpdateHistory_Shown(object sender, EventArgs e)// ウインドウ表示後の処理
        {
            // 一番下までスクロール
            UpdateHistoryTextBox.SelectionStart = UpdateHistoryTextBox.Text.Length;
            UpdateHistoryTextBox.ScrollToCaret();
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