/*
MainForm
Copyright (c) [2022-2026] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Windows.Forms;

namespace CREC
{
    public class ConfigValuesClass
    {
        /// <summary>
        /// 編集許可
        /// </summary>
        public bool AllowEdit { get; set; } = true;
        /// <summary>
        /// ID編集許可
        /// </summary>
        public bool AllowEditID { get; set; } = false;
        /// <summary>
        /// 秘密データ表示許可
        /// </summary>
        public bool ShowConfidentialData { get; set; } = true;
        /// <summary>
        /// ユーザーアシストツールチップ表示
        /// </summary>
        public bool ShowUserAssistToolTips { get; set; } = true;
        /// <summary>
        /// 自動検索
        /// </summary>
        public bool AutoSearch { get; set; } = true;
        /// <summary>
        /// 最近表示したコンテンツ
        /// </summary>
        public bool RecentShownContents { get; set; } = true;
        /// <summary>
        /// 起動時アップデートチェック
        /// </summary>
        public bool BootUpdateCheck { get; set; } = true;
        /// <summary>
        /// 自動読み込みプロジェクトパス
        /// </summary>
        public string AutoLoadProjectPath { get; set; } = string.Empty;
        /// <summary>
        /// 前回開いたプロジェクトを開く
        /// </summary>
        public bool OpenLastTimeProject { get; set; } = false;
        /// <summary>
        /// 文字サイズオフセット量
        /// </summary>
        public int FontsizeOffset { get; set; } = 0;
        /// <summary>
        /// 言語ファイル名
        /// </summary>
        public string LanguageFileName { get; set; } = "Japanese";

    }

    public class ConfigClass
    {
        /// <summary>
        /// config.sysの読み込み
        /// </summary>
        /// <param name="configValues">config値</param>
        /// <param name="TargetCRECPath">現在開いているコレクションのパス</param>
        /// <returns></returns>
        public static bool LoadConfigValues(ref ConfigValuesClass configValues, ref ProjectSettingValuesClass currentProjectSettingValues)
        {
            var loadingConfigValues = new ConfigValuesClass();// 読み込み用の一時変数
            // 指定されていなかった場合のために初期化
            if (File.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + "config.sys"))
            {
                try
                {
                    IEnumerable<string> tmp = null;
                    tmp = File.ReadLines(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + "config.sys", Encoding.GetEncoding("UTF-8"));
                    foreach (string line in tmp)
                    {
                        string[] cols = line.Split(',');
                        switch (cols[0])
                        {
                            case "AllowEdit":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.AllowEdit = true;
                                }
                                else
                                {
                                    loadingConfigValues.AllowEdit = false;
                                }
                                break;
                            case "ShowConfidentialData":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.ShowConfidentialData = true;
                                }
                                else
                                {
                                    loadingConfigValues.ShowConfidentialData = false;
                                }
                                break;
                            case "ShowUserAssistToolTips":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.ShowUserAssistToolTips = true;
                                }
                                else
                                {
                                    loadingConfigValues.ShowUserAssistToolTips = false;
                                }
                                break;
                            case "AutoLoadProject":
                                if (File.Exists(cols[1]))
                                {
                                    if (currentProjectSettingValues.ProjectSettingFilePath.Length == 0)
                                    {
                                        currentProjectSettingValues.ProjectSettingFilePath = cols[1];//CREC起動時のみ読み込み
                                    }
                                    loadingConfigValues.AutoLoadProjectPath = cols[1];
                                }
                                else if (cols[1].Length == 0)
                                {
                                    loadingConfigValues.AutoLoadProjectPath = string.Empty;
                                }
                                else
                                {
                                    MessageBox.Show("自動読み込み設定されたプロジェクトが見つかりません。", "CREC");
                                    currentProjectSettingValues.ProjectSettingFilePath = string.Empty;
                                    loadingConfigValues.AutoLoadProjectPath = string.Empty;
                                }
                                break;
                            case "OpenLastTimeProject":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.OpenLastTimeProject = true;
                                }
                                else
                                {
                                    loadingConfigValues.OpenLastTimeProject = false;
                                }
                                break;
                            case "AutoSearch":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.AutoSearch = true;
                                }
                                else
                                {
                                    loadingConfigValues.AutoSearch = false;
                                }
                                break;
                            case "RecentShownContents":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.RecentShownContents = true;
                                }
                                else
                                {
                                    loadingConfigValues.RecentShownContents = false;
                                }
                                break;
                            case "BootUpdateCheck":
                                if (cols[1] == "true")
                                {
                                    loadingConfigValues.BootUpdateCheck = true;
                                }
                                else
                                {
                                    loadingConfigValues.BootUpdateCheck = false;
                                }
                                break;
                            case "Language":
                                if (cols[1].Length == 0)
                                {
                                    loadingConfigValues.LanguageFileName = "Japanese.xml";
                                }
                                else
                                {
                                    loadingConfigValues.LanguageFileName = cols[1];
                                }
                                break;
                            case "FontsizeOffset":
                                if (cols[1].Length == 0)
                                {
                                    loadingConfigValues.FontsizeOffset = 0;
                                }
                                else
                                {
                                    loadingConfigValues.FontsizeOffset = Convert.ToInt32(cols[1]);
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Config.sysの読み込みに失敗しました(´・ω・｀)\n" + ex.Message, "CREC");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("設定ファイルが見つかりません。\nデフォルト設定で起動します。", "CREC");
                return SaveConfigValues(loadingConfigValues, string.Empty);// Config.sysの作成
            }
            configValues = loadingConfigValues;// 読み込み用の一時変数を反映する
            return true;
        }

        /// <summary>
        /// config.sysの保存
        /// </summary>
        /// <param name="configValues">config値</param>
        /// <param name="CurrentCRECPath">現在開いているコレクションのCRECパス</param>
        /// <returns></returns>
        public static bool SaveConfigValues(ConfigValuesClass configValues, string CurrentCRECPath)
        {
            StreamWriter configfile = new StreamWriter(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + "config.sys", false, Encoding.GetEncoding("UTF-8"));
            try
            {
                if (configValues.AllowEdit == true)
                {
                    configfile.WriteLine("AllowEdit,true");
                }
                else
                {
                    configfile.WriteLine("AllowEdit,false");
                }
                if (configValues.ShowConfidentialData == true)
                {
                    configfile.WriteLine("ShowConfidentialData,true");
                }
                else
                {
                    configfile.WriteLine("ShowConfidentialData,false");
                }
                if (configValues.ShowUserAssistToolTips == true)
                {
                    configfile.WriteLine("ShowUserAssistToolTips,true");
                }
                else
                {
                    configfile.WriteLine("ShowUserAssistToolTips,false");
                }
                if (configValues.OpenLastTimeProject == true)
                {
                    configfile.WriteLine("AutoLoadProject,{0}", CurrentCRECPath);
                    configfile.WriteLine("OpenLastTimeProject,true");
                }
                else
                {
                    configfile.WriteLine("AutoLoadProject,{0}", configValues.AutoLoadProjectPath);
                    configfile.WriteLine("OpenLastTimeProject,false");
                }
                if (configValues.AutoSearch == true)
                {
                    configfile.WriteLine("AutoSearch,true");
                }
                else
                {
                    configfile.WriteLine("AutoSearch,false");
                }
                if (configValues.RecentShownContents == true)
                {
                    configfile.WriteLine("RecentShownContents,true");
                }
                else
                {
                    configfile.WriteLine("RecentShownContents,false");
                }
                if (configValues.BootUpdateCheck == true)
                {
                    configfile.WriteLine("BootUpdateCheck,true");
                }
                else
                {
                    configfile.WriteLine("BootUpdateCheck,false");
                }
                configfile.WriteLine("Language,{0}", configValues.LanguageFileName);
                configfile.WriteLine("FontsizeOffset,{0}", configValues.FontsizeOffset);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config.sysの作成に失敗しました(´・ω・｀)\n" + ex.Message, "CREC");
            }
            finally
            {
                configfile.Close();
            }
            return true;
        }
    }
}