/*
VersionInformation
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
using System.Management;
using System.Runtime.Remoting.Messaging;
//using System.Net.NetworkInformation;

namespace CoRectSys
{
    public partial class VersionInformation : Form
    {
        string SystemInformations;
        string ColorSetting = "Blue";
        float mainFormCurrentDPI = 0;// MainFormのDPI
        // 表示関係
        double CurrentDPI = 1.0;// 現在のDPI値
        public VersionInformation(string colorSetting, double MainFormCurrentDPI)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            mainFormCurrentDPI = (float)MainFormCurrentDPI;
            SetColorMethod();
        }

        private void VersionInformation_Load(object sender, EventArgs e)
        {
            GetSystemInformation();// 下の非同期処理
        }
        private async void GetSystemInformation()// 非同期でシステム情報を取得し、結果を表示
        {
            SystemInformations = await Task.Run(() =>
            {
                string temp = "";
                ManagementClass mcOS = new ManagementClass("Win32_OperatingSystem");
                ManagementObjectCollection mocOS = mcOS.GetInstances();
                foreach (ManagementObject m in mocOS)
                {
                    temp =
                        ("エディション：" + m["Caption"].ToString() + "\n"
                       + "バージョン：" + m["Version"].ToString() + "\n"
                       + "OSビルド：" + m["BuildNumber"].ToString() + "\n"
                       + "合計物理メモリ(MB)：" + (Convert.ToInt32(m["TotalVisibleMemorySize"]) / 1024).ToString() + "\n"
                        );
                }
                ManagementClass mcCPU = new ManagementClass("Win32_Processor");
                ManagementObjectCollection mocCPU = mcCPU.GetInstances();
                string CPUName = string.Empty;
                foreach (ManagementObject m in mocCPU)
                {
                    CPUName = m["Name"].ToString();
                }
                temp =
                    (temp
                    + "CPU名：" + CPUName + "\n");
                temp =
                    (temp
                     + "表示スケール：" + Convert.ToInt32(mainFormCurrentDPI * 100).ToString() + "%\n"
                      );
                return temp;
            });
            ShowSystemInformationsLabel.Text = SystemInformations;
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

        #region 画面サイズ変更時のコントロールサイズ更新処理
        private void VersionInformation_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            CurrentDPI = e.DeviceDpiNew / 96.0;
        }
        #endregion
    }
}