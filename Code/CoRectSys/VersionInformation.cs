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
using System.Xml.Linq;

namespace CREC
{
    public partial class VersionInformation : Form
    {
        string SystemInformations;
        string ColorSetting = "Blue";
        float mainFormCurrentDPI = 0;// MainFormのDPI
        string CurrentLanguageFileName = "";// 言語設定
        // システム情報表示用の変数
        string OSEdition = "エディション：";
        string OSVersion = "バージョン：";
        string OSBuildNumber = "OSビルド：";
        string TotalVisibleMemorySize = "合計物理メモリ(MB)：";
        string CPUName = "CPU名：";
        string WindowScale = "表示スケール：";

        // 表示関係
        double CurrentDPI = 1.0;// 現在のDPI値
        public VersionInformation(string colorSetting, double MainFormCurrentDPI, string currentLanguage)
        {
            InitializeComponent();
            ColorSetting = colorSetting;
            mainFormCurrentDPI = (float)MainFormCurrentDPI;
            CurrentLanguageFileName = currentLanguage;
            SetColorMethod();
        }

        private void VersionInformation_Load(object sender, EventArgs e)
        {
            GetSystemInformation();// 下の非同期処理
            SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
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
                        (OSEdition + m["Caption"].ToString() + "\n"
                       + OSVersion + m["Version"].ToString() + "\n"
                       + OSBuildNumber + m["BuildNumber"].ToString() + "\n"
                       + TotalVisibleMemorySize + (Convert.ToInt32(m["TotalVisibleMemorySize"]) / 1024).ToString() + "\n"
                        );
                }
                ManagementClass mcCPU = new ManagementClass("Win32_Processor");
                ManagementObjectCollection mocCPU = mcCPU.GetInstances();
                string thisCPUName = string.Empty;
                foreach (ManagementObject m in mocCPU)
                {
                    thisCPUName = m["Name"].ToString();
                }
                temp =
                    (temp
                    + CPUName + thisCPUName + "\n");
                temp =
                    (temp
                     + WindowScale + Convert.ToInt32(mainFormCurrentDPI * 100).ToString() + "%\n"
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

        #region 言語設定
        private void SetLanguage(string targetLanguageFilePath)// 言語ファイル（xml）を読み込んで表示する処理
        {
            try
            {
                this.Text = LangageSettingClass.GetOtherMessage("FormName", "VersionInformation", CurrentLanguageFileName);
                XElement xElement = XElement.Load(targetLanguageFilePath);
                IEnumerable<XElement> buttonItemDataList = from item in xElement.Elements("VersionInformation").Elements("Button").Elements("item") select item;
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
                IEnumerable<XElement> labelItemDataList = from item in xElement.Elements("VersionInformation").Elements("Label").Elements("item") select item;
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
                IEnumerable<XElement> OtherItemDataList = from item in xElement.Elements("VersionInformation").Elements("Other").Elements("item") select item;
                foreach (XElement itemData in OtherItemDataList)
                {
                    try
                    {
                        switch (itemData.Element("itemname").Value)
                        {
                            case "OSEdition":
                                OSEdition = itemData.Element("itemtext").Value;
                                break;
                            case "OSVersion":
                                OSVersion = itemData.Element("itemtext").Value;
                                break;
                            case "OSBuildNumber":
                                OSBuildNumber = itemData.Element("itemtext").Value;
                                break;
                            case "TotalVisibleMemorySize":
                                TotalVisibleMemorySize = itemData.Element("itemtext").Value;
                                break;
                            case "CPUName":
                                CPUName = itemData.Element("itemtext").Value;
                                break;
                            case "WindowScale":
                                WindowScale = itemData.Element("itemtext").Value;
                                break;
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