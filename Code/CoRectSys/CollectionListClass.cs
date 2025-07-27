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
        ///  CollectionDataTableから検索
        /// </summary>
        /// <param name="SearchedCollectionList">検索対象のリスト</param>
        /// <param name="currentProjectSettingValues">現在のプロジェクト設定値</param>
        /// <param name="searchKeyWords">検索キーワード</param>
        /// <param name="searchOption">検索オプション</param>
        /// <param name="searchMethod">検索方法</param>
        /// <param name="LanguageData">言語情報</param>
        /// <returns></returns>
        public static void SearchCollectionFromList(
            ref List<CollectionDataValuesClass> SearchedCollectionList,
            ProjectSettingValuesClass currentProjectSettingValues,
            string searchKeyWords,
            int searchOption,
            int searchMethod,
            XElement LanguageData)
        {
            // 検索欄が空かつ在庫検索以外の場合はそのままとする
            if (searchKeyWords.Length == 0 && searchOption != 8)
            {
                return;
            }

            var loadingCollectionList = new List<CollectionDataValuesClass>();// 読み込んだコレクションリストを格納するリスト
            foreach (var item in SearchedCollectionList)
            {
                if (SearchMethod(
                    item,
                    currentProjectSettingValues,
                    searchKeyWords,
                    searchOption,
                    searchMethod))
                {
                    loadingCollectionList.Add(item);
                }
            }
            // 検索結果をSearchedCollectionListに複製
            SearchedCollectionList = new List<CollectionDataValuesClass>(loadingCollectionList);
            // loadingCollectionListをメモリから解放
            loadingCollectionList.Clear();
        }

        /// <summary>
        /// 検索窓の入力内容とキーワードが指定の検索方法で一致するか判定
        /// </summary>
        /// <param name="item">検索対象のコレクション</param>
        /// <param name="currentProjectSettingValues">現在のプロジェクト設定値</param>
        /// <param name="searchKeyWords">検索ワード</param>
        /// <param name="SearchOption">検索オプション</param>
        /// <param name="SearchMethod">検索方法</param>
        /// <returns>検索Hit：true、それ以外：false</returns>
        private static bool SearchMethod(
            CollectionDataValuesClass item,
            ProjectSettingValuesClass currentProjectSettingValues,
            string searchKeyWords,
            int SearchOption,
            int SearchMethod)
        {
            // 検索対象の文字列を取得（配列とする）
            string[] TargetWords = Array.Empty<string>();
            switch (SearchOption)
            {
                case 0:
                    // currentProjectSettingValuesで表示状態のものを検索対象として追加する
                    if (currentProjectSettingValues.CollectionListUUIDVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionID;
                    }
                    if (currentProjectSettingValues.CollectionListManagementCodeVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionMC;
                    }
                    if (currentProjectSettingValues.CollectionListNameVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionName;
                    }
                    if (currentProjectSettingValues.CollectionListCategoryVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionCategory;
                    }
                    if (currentProjectSettingValues.CollectionListFirstTagVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionTag1;
                    }
                    if (currentProjectSettingValues.CollectionListSecondTagVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionTag2;
                    }
                    if (currentProjectSettingValues.CollectionListThirdTagVisible)
                    {
                        Array.Resize(ref TargetWords, TargetWords.Length + 1);
                        TargetWords[TargetWords.Length - 1] = item.CollectionTag3;
                    }
                    break;
                case 1:
                    TargetWords = new[] { item.CollectionID };
                    break;
                case 2:
                    TargetWords = new[] { item.CollectionMC };
                    break;
                case 3:
                    TargetWords = new[] { item.CollectionName };
                    break;
                case 4:
                    TargetWords = new[] { item.CollectionCategory };
                    break;
                case 5:
                    TargetWords = new[] { item.CollectionTag1 };
                    break;
                case 6:
                    TargetWords = new[] { item.CollectionTag2 };
                    break;
                case 7:
                    TargetWords = new[] { item.CollectionTag3 };
                    break;
                case 8:// 在庫状況検索用
                    break;
            }

            //検索方法に合わせて検索
            switch (SearchMethod)
            {
                case 0:
                    if (SearchOption == 8)
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
                        // 前方一致検索
                        return TargetWords.Any(word => word.StartsWith(searchKeyWords));
                    }
                case 1:
                    if (SearchOption == 8)
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
                        // 部分一致検索
                        return TargetWords.Any(word => word.Contains(searchKeyWords));
                    }
                case 2:
                    if (SearchOption == 8)
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
                        // 後方一致検索
                        return TargetWords.Any(word => word.EndsWith(searchKeyWords));
                    }
                case 3:
                    if (SearchOption == 8)
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
                        // 完全一致検索
                        return TargetWords.Any(word => word == searchKeyWords);
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
                ListOutputMethod(currentProjectSettingValues, listOutputPath, collectionList, languageData);
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
                    System.Diagnostics.Process.Start(listOutputPath + "\\InventoryOutput.csv");
                }
                else if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.TSV)
                {
                    System.Diagnostics.Process.Start(listOutputPath + "\\InventoryOutput.tsv");
                }
            }
            return true;
        }

        /// <summary>
        /// 一覧出力する処理
        /// </summary>
        /// <param name="currentProjectSettingValues">プロジェクト設定値</param>
        /// <param name="listOutputPath">出力パス</param>
        /// <param name="collectionList">リスト出力するコレクションのリスト</param>
        private static void ListOutputMethod(ProjectSettingValuesClass currentProjectSettingValues, string listOutputPath, List<CollectionDataValuesClass> collectionList, XElement languageData)
        {
            StreamWriter streamWriter;
            // ヘッダ書き込み
            if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV)
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
                    if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV)
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
                if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV)
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
                if (currentProjectSettingValues.ListOutputFormat == CREC.ListOutputFormat.CSV)
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
