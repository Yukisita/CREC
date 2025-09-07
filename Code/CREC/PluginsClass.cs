using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using File = System.IO.File;

namespace CREC
{
    internal class PluginsClass
    {

        /// <summary>
        /// 最近実行したプラグインリストのファイルパスを取得する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <returns>ファイルパス</returns>
        public static string GetRecentPluginsFilePath(ProjectSettingValuesClass ProjectSettingValues)
        {
            if (ProjectSettingValues.ProjectSettingFilePath.Length != 0)
            {
                // projectのシステムデータフォルダ内に配置する
                return System.IO.Path.GetDirectoryName(ProjectSettingValues.ProjectSettingFilePath) +"\\"+ProjectSettingValues.Name+ "_SystemData\\RecentlyExecutedPluginList.log";
            }
            return string.Empty;
        }


        /// <summary>
        /// 最近実行したプラグインリストに追加する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="pluginName">プラグイン名</param>
        /// <param name="pluginPath">プラグインのパス</param>
        public static void AddToRecentPluginsList(ProjectSettingValuesClass ProjectSettingValues, string pluginName, string pluginPath)
        {
            try
            {
                string recentPluginsFilePath = PluginsClass.GetRecentPluginsFilePath(ProjectSettingValues);
                // projectのシステムデータフォルダが存在しない場合は作成
                string projectSystemDataFolderPath = System.IO.Path.GetDirectoryName(recentPluginsFilePath);
                if (!System.IO.Directory.Exists(projectSystemDataFolderPath))
                {
                    System.IO.Directory.CreateDirectory(projectSystemDataFolderPath);
                }

                List<string> recentPlugins = new List<string>();

                // 既存の履歴を読み込み
                if (File.Exists(recentPluginsFilePath))
                {
                    string[] existingLines = File.ReadAllLines(recentPluginsFilePath, Encoding.GetEncoding("UTF-8"));
                    foreach (string line in existingLines)
                    {
                        if (!line.Contains(pluginPath))
                        {
                            recentPlugins.Add(line);
                        }
                    }
                }

                // 新しいプラグインを先頭に追加
                recentPlugins.Insert(0, $"{pluginName},{pluginPath}");

                // 最大5件まで保持
                if (recentPlugins.Count > 5)
                {
                    recentPlugins = recentPlugins.Take(5).ToList();
                }

                // ファイルに保存
                File.WriteAllLines(recentPluginsFilePath, recentPlugins, Encoding.GetEncoding("UTF-8"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("プラグイン履歴の保存に失敗しました。\n" + ex.Message, "CREC");
            }
        }

        /// <summary>
        /// プラグインを実行する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="collectionDataValues">コレクションデータの値</param>
        /// <param name="pluginPath">プラグインのパス</param>
        /// <param name="languageData">言語データ</param>
        public static bool ExecutePlugin(
            ProjectSettingValuesClass ProjectSettingValues,
            CollectionDataValuesClass collectionDataValues,
            string pluginPath,
            XElement languageData
            )
        {
            if (ProjectSettingValues.ProjectSettingFilePath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", languageData), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = pluginPath;
                startInfo.WorkingDirectory = collectionDataValues.CollectionFolderPath;
                startInfo.UseShellExecute = false;
                Process process = Process.Start(startInfo);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
