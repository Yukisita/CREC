/*
Program
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CREC
{
    public class CollectionListClass
    {
        /// <summary>
        /// Collectionを読み込み、CollectionDataTableに追加する
        /// </summary>
        /// <param name="CurrentProjectSettingValues"></param>
        /// <param name="searchKeyWords"></param>
        /// <param name="CollectionDataTable"></param>
        /// <param name="LanguageData"></param>
        /// <returns></returns>
        public static bool LoadCollectionList(ProjectSettingValuesClass CurrentProjectSettingValues, ref List<CollectionDataValuesClass> AllCollectionList, XElement LanguageData)
        {
            // 読み込んだコレクションリストを格納するリスト
            var loadingCollectionList = new List<CollectionDataValuesClass>();
            // 指定されたフォルダ内を探索
            DirectoryInfo di = new DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
            IEnumerable<DirectoryInfo> subFolders = di.EnumerateDirectories("*");
            foreach (DirectoryInfo subFolder in subFolders)
            {
                // Index読み込み
                CollectionDataValuesClass item = new CollectionDataValuesClass();
                CollectionDataClass.LoadCollectionIndexData(subFolder.FullName, ref item, LanguageData);
                // 在庫状況読み込み
                CollectionDataClass.LoadCollectionInventoryData(subFolder.FullName, ref item, LanguageData);
                // AllCollectionListに追加
                loadingCollectionList.Add(item);
            }
            AllCollectionList = loadingCollectionList;// AllCollectionListに読み込んだデータリストを反映
            return true;
        }

        /// <summary>
        /// CollectionDataTableから検索
        /// </summary>
        /// <param name="CollectionDataTable"></param>
        /// <param name="LanguageData"></param>
        /// <returns></returns>
        public static bool SearchCollectionFromList(ref List<CollectionDataValuesClass> SearchedCollectionList, string searchKeyWords, int searchOption, int searchMethod, XElement LanguageData)
        {
            // 読み込んだコレクションリストを格納するリスト
            var loadingCollectionList = new List<CollectionDataValuesClass>();
            foreach (var item in SearchedCollectionList)
            {
                //dataGridViewに追加、検索欄に文字が入力されている場合は絞り込み
                if (searchKeyWords.Length == 0)
                {
                    if (searchOption == 7)
                    {
                        if (SearchMethod(item, searchKeyWords, searchOption, searchMethod) == true)
                        {
                            loadingCollectionList.Add(item);
                        }
                    }
                    else
                    {
                        loadingCollectionList.Add(item);
                    }
                }
                else if (searchKeyWords.Length >= 1)
                {
                    if (SearchMethod(item, searchKeyWords, searchOption, searchMethod) == true)
                    {
                        loadingCollectionList.Add(item);
                    }
                }
            }
            SearchedCollectionList = loadingCollectionList;// SearchCollectionListに読み込んだデータリストを反映
            return true;
        }

        /// <summary>
        /// 検索窓の入力内容とキーワードが指定の検索方法で一致するか判定
        /// </summary>
        /// <param name="item">検索対象のコレクション</param>
        /// <param name="searchKeyWords">検索ワード</param>
        /// <param name="SearchOption"></param>
        /// <param name="SearchMethod"></param>
        /// <returns>検索Hit：true、それ以外：false</returns>
        private static bool SearchMethod(CollectionDataValuesClass item, string searchKeyWords, int SearchOption, int SearchMethod)
        {
            string TargetWords = string.Empty;
            switch (SearchOption)
            {
                case 0:
                    TargetWords = item.CollectionID;
                    break;
                case 1:
                    TargetWords = item.CollectionMC;
                    break;
                case 2:
                    TargetWords = item.CollectionName;
                    break;
                case 3:
                    TargetWords = item.CollectionCategory;
                    break;
                case 4:
                    TargetWords = item.CollectionTag1;
                    break;
                case 5:
                    TargetWords = item.CollectionTag2;
                    break;
                case 6:
                    TargetWords = item.CollectionTag3;
                    break;
                case 7:
                    break;
            }
            switch (SearchMethod)
            {
                case 0:
                    if (SearchOption == 7)
                    {
                        if (item.CollectionInventoryStatus == InventoryStatus.StockOut)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (TargetWords.StartsWith(searchKeyWords))// 前方一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 1:
                    if (SearchOption == 7)
                    {
                        if (item.CollectionInventoryStatus == InventoryStatus.UnderStocked)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (TargetWords.Contains(searchKeyWords))// 部分一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 2:
                    if (SearchOption == 7)
                    {
                        if (item.CollectionInventoryStatus == InventoryStatus.Appropriate)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (TargetWords.EndsWith(searchKeyWords))// 後方一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 3:
                    if (SearchOption == 7)
                    {
                        if (item.CollectionInventoryStatus == InventoryStatus.OverStocked)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (TargetWords == searchKeyWords)// 完全一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    return false;
            }
        }

        /// <summary>
        /// コレクション一覧出力処理
        /// </summary>
        /// <param name="currentProjectSettingValues"></param>
        /// <param name="collectionList"></param>
        /// <param name="languageData"></param>
        /// <returns>出力成功:true、出力失敗:false</returns>
        public static bool OutputCollectionList(ProjectSettingValuesClass currentProjectSettingValues, List<CollectionDataValuesClass> collectionList, XElement languageData)
        {
            // ファイルが開いているか確認
            if (currentProjectSettingValues.ProjectSettingFilePath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "CollectionListClass", languageData), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            // ファイル出力先を設定
            string listOutputPath;
            if (Directory.Exists(currentProjectSettingValues.ListOutputPath))
            {
                listOutputPath = currentProjectSettingValues.ListOutputPath;
            }
            else
            {
                listOutputPath = currentProjectSettingValues.ProjectDataFolderPath;
            }
            // 出力形式に合わせて出力
            if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV || currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.TSV)
            {
                ListOutputMethod(currentProjectSettingValues, listOutputPath, currentProjectSettingValues.ListOutputFormat, collectionList, languageData);
            }
            else
            {
                MessageBox.Show("値が不正です。", "CREC");
                return false;
            }
            // 出力後の描画処理
            if (currentProjectSettingValues.OpenListAfterOutput == true)
            {
                if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(listOutputPath + "\\InventoryOutput.csv");
                }
                else if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.TSV)
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(listOutputPath + "\\InventoryOutput.tsv");
                }
            }
            return true;
        }

        /// <summary>
        /// 一覧出力する処理
        /// </summary>
        /// <param name="currentProjectSettingValues">プロジェクト設定値</param>
        /// <param name="listOutputPath">出力パス</param>
        /// <param name="listOutputFormat">フォーマット</param>
        /// <param name="collectionList">リスト出力するコレクションのリスト</param>
        private static void ListOutputMethod(ProjectSettingValuesClass currentProjectSettingValues, string listOutputPath, CREC.ListOutputFormat listOutputFormat, List<CollectionDataValuesClass> collectionList, XElement languageData)
        {
            StreamWriter streamWriter;
            // ヘッダ書き込み
            if (listOutputFormat == CREC.ListOutputFormat.CSV)
            {
                streamWriter = new StreamWriter(listOutputPath + "\\InventoryOutput.csv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            currentProjectSettingValues.DataLocationLabel,
                            currentProjectSettingValues.UUIDLabel,
                            currentProjectSettingValues.ManagementCodeLabel,
                            currentProjectSettingValues.CollectionNameLabel,
                            currentProjectSettingValues.RegistrationDateLabel,
                            currentProjectSettingValues.CategoryLabel,
                            currentProjectSettingValues.FirstTagLabel,
                            currentProjectSettingValues.SecondTagLabel,
                            currentProjectSettingValues.ThirdTagLabel,
                            "在庫数",
                            "在庫状況");
            }
            else
            {
                streamWriter = new StreamWriter(listOutputPath + "\\InventoryOutput.tsv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine(
                            "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                            currentProjectSettingValues.DataLocationLabel,
                            currentProjectSettingValues.UUIDLabel,
                            currentProjectSettingValues.ManagementCodeLabel,
                            currentProjectSettingValues.CollectionNameLabel,
                            currentProjectSettingValues.RegistrationDateLabel,
                            currentProjectSettingValues.CategoryLabel,
                            currentProjectSettingValues.FirstTagLabel,
                            currentProjectSettingValues.SecondTagLabel,
                            currentProjectSettingValues.ThirdTagLabel,
                            "在庫数",
                            "在庫状況");
            }

            try
            {
                foreach (var thisCollectionDataValues in collectionList)
                {
                    // ファイル書き込み
                    if (listOutputFormat == CREC.ListOutputFormat.CSV)
                    {
                        streamWriter.WriteLine(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            thisCollectionDataValues.CollectionFolderPath,
                            thisCollectionDataValues.CollectionID,
                            thisCollectionDataValues.CollectionMC,
                            thisCollectionDataValues.CollectionName,
                            thisCollectionDataValues.CollectionRegistrationDate,
                            thisCollectionDataValues.CollectionCategory,
                            thisCollectionDataValues.CollectionTag1,
                            thisCollectionDataValues.CollectionTag2,
                            thisCollectionDataValues.CollectionTag3,
                            thisCollectionDataValues.CollectionCurrentInventory,
                            thisCollectionDataValues.CollectionInventoryStatus);
                    }
                    else
                    {
                        streamWriter.WriteLine(
                            "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                            thisCollectionDataValues.CollectionFolderPath,
                            thisCollectionDataValues.CollectionID,
                            thisCollectionDataValues.CollectionMC,
                            thisCollectionDataValues.CollectionName,
                            thisCollectionDataValues.CollectionRegistrationDate,
                            thisCollectionDataValues.CollectionCategory,
                            thisCollectionDataValues.CollectionTag1,
                            thisCollectionDataValues.CollectionTag2,
                            thisCollectionDataValues.CollectionTag3,
                            thisCollectionDataValues.CollectionCurrentInventory,
                            thisCollectionDataValues.CollectionInventoryStatus);
                    }
                }
                // 正常出力完了のメッセージ表示
                if (listOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputFinish", "CollectionListClass", languageData) + "\n" + listOutputPath + "\\InventoryOutput.csv", "CREC");
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputFinish", "CollectionListClass", languageData) + "\n" + listOutputPath + "\\InventoryOutput.tsv", "CREC");
                }
            }
            catch (Exception ex)
            {
                // エラーメッセージ表示
                if (listOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputError", "CollectionListClass", languageData) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputError", "CollectionListClass", languageData) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                streamWriter.Close();
            }
        }
    }
}
