/*
VersionInformation
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
        public VersionInformation(string colorSetting)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
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
                float DpiScale = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX / 96 * 100);// DPI取得
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
                foreach (ManagementObject m in mocCPU)
                {
                    temp =
                         (temp
                        + "CPU名：" + m["Name"].ToString() + "\n"
                        + "表示スケール：" + Convert.ToInt32(DpiScale).ToString() + "%\n"
                         );
                }
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
            }
        }
    }
}
