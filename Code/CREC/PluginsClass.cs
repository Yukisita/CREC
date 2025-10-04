/*
PluginsClass
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
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
                // プロジェクトデータフォルダ内のシステムデータフォルダに配置する
                return ProjectSettingValues.ProjectDataFolderPath + "\\" + MainForm.ProjectSystemDataFolderName + "\\RecentlyExecutedPluginList.log";
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
        /// 最近実行したプラグインリストから削除する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="pluginPath">削除対象のプラグインのパス</param>
        /// <returns></returns>
        public static bool RemoveFromRecentPluginsList(ProjectSettingValuesClass ProjectSettingValues, string pluginPath)
        {
            try
            {
                string recentPluginsFilePath = PluginsClass.GetRecentPluginsFilePath(ProjectSettingValues);
                if (!File.Exists(recentPluginsFilePath))
                {
                    return false;
                }

                string[] existingLines = File.ReadAllLines(recentPluginsFilePath, Encoding.GetEncoding("UTF-8"));
                List<string> filteredLines = new List<string>();

                foreach (string line in existingLines)
                {
                    if (!line.Contains(pluginPath))
                    {
                        filteredLines.Add(line);
                    }
                }

                File.WriteAllLines(recentPluginsFilePath, filteredLines, Encoding.GetEncoding("UTF-8"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            return true;
        }

        /// <summary>
        /// お気に入りプラグインリストのファイルパスを取得する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <returns>ファイルパス</returns>
        public static string GetFavoritePluginsFilePath(ProjectSettingValuesClass ProjectSettingValues)
        {
            if (ProjectSettingValues.ProjectSettingFilePath.Length != 0)
            {
                // プロジェクトデータフォルダ内のシステムデータフォルダに配置する
                return ProjectSettingValues.ProjectDataFolderPath + "\\" + MainForm.ProjectSystemDataFolderName + "\\FavoritePluginsList.log";
            }
            return string.Empty;
        }

        /// <summary>
        /// お気に入りプラグインリストに追加する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="pluginName">プラグイン名</param>
        /// <param name="pluginPath">プラグインのパス</param>
        /// <param name="languageData">言語データ</param>
        public static bool AddToFavoritePluginsList(
            ProjectSettingValuesClass ProjectSettingValues,
            string pluginName,
            string pluginPath,
            XElement languageData)
        {
            try
            {
                // お気に入りプラグインリストのファイルパスを取得
                string favoritePluginsFilePath = PluginsClass.GetFavoritePluginsFilePath(ProjectSettingValues);
                if (string.IsNullOrEmpty(favoritePluginsFilePath))
                {
                    return false;
                }

                // projectのシステムデータフォルダが存在しない場合は作成
                string projectSystemDataFolderPath = System.IO.Path.GetDirectoryName(favoritePluginsFilePath);
                if (!System.IO.Directory.Exists(projectSystemDataFolderPath))
                {
                    System.IO.Directory.CreateDirectory(projectSystemDataFolderPath);
                }

                List<string> favoritePlugins = new List<string>();

                // 既存のリストを読み込み
                if (File.Exists(favoritePluginsFilePath))
                {
                    string[] existingLines = File.ReadAllLines(favoritePluginsFilePath, Encoding.GetEncoding("UTF-8"));
                    foreach (string line in existingLines)
                    {
                        // 同じパスが既に存在する場合は重複登録しない
                        var parts = line.Split(new[] { ',' }, 2);
                        string existingPath = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                        if (!string.Equals(existingPath, pluginPath, StringComparison.OrdinalIgnoreCase))
                        {
                            favoritePlugins.Add(line);
                        }
                        else
                        {
                            // 既に登録済みの場合はメッセージを表示して終了
                            MessageBox.Show(
                                LanguageSettingClass.GetMessageBoxMessage("AlreadyInFavorites", "PluginsClass", languageData),
                                "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }

                // 新しいプラグインを追加
                favoritePlugins.Add($"{pluginName},{pluginPath}");

                // ファイルに保存
                File.WriteAllLines(favoritePluginsFilePath, favoritePlugins, Encoding.GetEncoding("UTF-8"));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("FavoritePluginSaveError", "PluginsClass", languageData) + "\n" + ex.Message,
                    "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// お気に入りプラグインリストから削除する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="pluginPath">削除対象のプラグインのパス</param>
        /// <param name="languageData">言語データ</param>
        /// <returns></returns>
        public static bool RemoveFromFavoritePluginsList(ProjectSettingValuesClass ProjectSettingValues, string pluginPath, XElement languageData)
        {
            try
            {
                // お気に入りプラグインリストのファイルパスを取得
                string favoritePluginsFilePath = PluginsClass.GetFavoritePluginsFilePath(ProjectSettingValues);
                if (string.IsNullOrEmpty(favoritePluginsFilePath) || !File.Exists(favoritePluginsFilePath))
                {
                    return false;
                }

                string[] existingLines = File.ReadAllLines(favoritePluginsFilePath, Encoding.GetEncoding("UTF-8"));
                List<string> filteredLines = new List<string>();

                foreach (string line in existingLines)
                {
                    // Split the line by comma and compare the second part (plugin path) exactly
                    string[] parts = line.Split(',');
                    if (parts.Length < 2 || !string.Equals(parts[1], pluginPath, StringComparison.Ordinal))
                    {
                        filteredLines.Add(line);
                    }
                }

                File.WriteAllLines(favoritePluginsFilePath, filteredLines, Encoding.GetEncoding("UTF-8"));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("FavoritePluginDeleteError", "PluginsClass", languageData) + "\n" + ex.Message,
                    "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// お気に入りプラグインリストを取得し、見つからないファイルを削除する
        /// </summary>
        /// <param name="ProjectSettingValues">プロジェクト設定値</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>お気に入りプラグインのリスト（プラグイン名,パス）</returns>
        public static List<(string name, string path)> GetFavoritePluginsList(ProjectSettingValuesClass ProjectSettingValues, XElement languageData)
        {
            List<(string name, string path)> favoritePlugins = new List<(string name, string path)>();
            string favoritePluginsFilePath = PluginsClass.GetFavoritePluginsFilePath(ProjectSettingValues);
            if (string.IsNullOrEmpty(favoritePluginsFilePath) || !File.Exists(favoritePluginsFilePath))
            {
                return favoritePlugins;
            }

            try
            {
                string[] existingLines = File.ReadAllLines(favoritePluginsFilePath, Encoding.GetEncoding("UTF-8"));
                List<string> validLines = new List<string>();
                bool needsUpdate = false;

                foreach (string line in existingLines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        string pluginPath = parts[1];
                        // ファイルが存在する場合のみリストに追加
                        if (File.Exists(pluginPath))
                        {
                            favoritePlugins.Add((parts[0], pluginPath));
                            validLines.Add(line);
                        }
                        else
                        {
                            // ファイルが存在しない場合はリストから削除
                            needsUpdate = true;
                        }
                    }
                    else
                    {
                        // フォーマットが不正な行は削除
                        needsUpdate = true;
                    }
                }

                // 見つからないファイルがあった場合、リストを更新
                if (needsUpdate)
                {
                    File.WriteAllLines(favoritePluginsFilePath, validLines, Encoding.GetEncoding("UTF-8"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("FavoritePluginListLoadError", "PluginsClass", languageData) + "\n" + ex.Message,
                    "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return favoritePlugins;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
