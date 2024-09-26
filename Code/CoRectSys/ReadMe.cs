﻿/*
ReadMe
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

namespace CREC
{
    public partial class ReadMe : Form
    {
        string ColorSetting = "Blue";
        // 表示関係
        double CurrentDPI = 1.0;// 現在のDPI値
        double FirstDPI = 1.0;// 起動時の表示スケール値
        public ReadMe(string colorSetting)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            SetColorMethod();
            CurrentDPI = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;// 現在のDPI取得
            FirstDPI = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;// 起動時の表示スケール取得
        }
        private void ReadMe_Shown(object sender, EventArgs e)
        {
            SetFormLayout();
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

        private void ReadMe_DpiChanged(object sender, DpiChangedEventArgs e)// DPI変更時の処理、DPI取得
        {
            CurrentDPI = e.DeviceDpiNew / 96.0;
            SetFormLayout();
        }
        private void ReadMe_SizeChanged(object sender, EventArgs e)// Formサイズ変更時の処理
        {
            SetFormLayout();
        }
        private void SetFormLayout()// レイアウト構築処理
        {
            if (ReadMe.ActiveForm != null)
            {
                Size FormSize = ReadMe.ActiveForm.Size;// フォームサイズを取得
                ReadMeTextBox.Size = new Size(FormSize.Width - Convert.ToInt32(40 * CurrentDPI), FormSize.Height - Convert.ToInt32(60 * CurrentDPI));
                ReadMeTextBox.Location = new Point(Convert.ToInt32(10 * CurrentDPI), Convert.ToInt32(10 * CurrentDPI));
            }
        }
    }
}
