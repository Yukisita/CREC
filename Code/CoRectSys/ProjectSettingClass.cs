/*
Program
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;

namespace CREC
{
    /// <summary>
    /// バックアップ時のファイル圧縮方法
    /// </summary>
    public enum BackupCompressionType
    {
        NoCompress, // 圧縮なし
        Zip,        // Zip圧縮
    }

    /// <summary>
    /// リスト出力時のフォーマット
    /// </summary>
    public enum ListOutputFormat
    {
        CSV,
        TSV
    }

    /// <summary>
    /// 色設定値
    /// </summary>
    public enum ColorValue
    {
        Blue,
        White,
        Sakura,
        Green
    }

    /// <summary>
    /// SleepMode設定
    /// </summary>
    public enum SleepMode
    {
        Deep,
        Normal,
        Disable
    }

    public class ProjectSettingValuesClass
    {
        /// <summary>
        /// プロジェクトセッティングファイルのパス
        /// </summary>
        public string ProjectSettingFilePath { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクトデータ保存場所のパス
        /// </summary>
        public string ProjectDataFolderPath { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクトデータのバックアップ場所のパス
        /// </summary>
        public string ProjectBackupFolderPath { get; set; } = string.Empty;
        /// <summary>
        /// 起動時の自動バックアップ
        /// </summary>
        public bool StartUpBackUp { get; set; } = false;
        /// <summary>
        /// アプリケーション終了時の自動バックアップ
        /// </summary>
        public bool CloseBackUp { get; set; } = false;
        /// <summary>
        /// データ編集後の自動バックアップ
        /// </summary>
        public bool EditBackUp { get; set; } = false;
        /// <summary>
        /// バックアップ時の並列処理の最大数
        /// </summary>
        public int? MaxDegreeOfBackUpProcessParallelism { get; set; } = null; // バックアップ時の並列処理の最大数。nullの場合はデフォルト値を使用。
        /// <summary>
        /// バックアップのデータ圧縮方法
        /// </summary>
        public BackupCompressionType BackupCompressionType { get; set; } = BackupCompressionType.Zip;
        /// <summary>
        /// リスト出力フォルダのパス
        /// </summary>
        public string ListOutputPath { get; set; } = string.Empty;
        /// <summary>
        /// 起動時の自動リスト出力
        /// </summary>
        public bool StartUpListOutput { get; set; } = false;
        /// <summary>
        /// アプリケーション終了時の自動リスト出力
        /// </summary>
        public bool CloseListOutput { get; set; } = false;
        /// <summary>
        /// データ編集後の自動リスト出力
        /// </summary>
        public bool EditListOutput { get; set; } = false;
        /// <summary>
        /// リスト出力後にファイルを開くか設定
        /// </summary>
        public bool OpenListAfterOutput { get; set; } = false;
        /// <summary>
        /// リスト出力時のフォーマット
        /// </summary>
        public ListOutputFormat ListOutputFormat { get; set; } = ListOutputFormat.CSV;
        /// <summary>
        /// プロジェクト作成日
        /// </summary>
        public string CreatedDate { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクト最終編集日
        /// </summary>
        public string ModifiedDate { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクト最終アクセス日
        /// </summary>
        public string AccessedDate { get; set; } = string.Empty;
        /// <summary>
        /// プロジェクトの色設定
        /// </summary>
        public ColorValue ColorSetting { get; set; } = ColorValue.Blue;
        /// <summary>
        /// コレクションの名称ラベル
        /// </summary>
        public string CollectionNameLabel { get; set; } = "Name";
        /// <summary>
        /// コレクションの名称表示・非表示フラグ
        /// </summary>
        public bool CollectionNameVisible { get; set; } = true;
        /// <summary>
        /// コレクションのUUIDのラベル
        /// </summary>
        public string UUIDLabel { get; set; } = "UUID";
        /// <summary>
        /// コレクションのUUIDの表示・非表示フラグ
        /// </summary>
        public bool UUIDVisible { get; set; } = true;
        /// <summary>
        /// コレクションの管理コードのラベル
        /// </summary>
        public string ManagementCodeLabel { get; set; } = "Mgmt. code";
        /// <summary>
        /// コレクションの管理コードの表示・非表示フラグ
        /// </summary>
        public bool ManagementCodeVisible { get; set; } = true;
        /// <summary>
        /// コレクションの管理コードの自動入力有効・無効フラグ
        /// </summary>
        public bool ManagementCodeAutoFill { get; set; } = true;
        /// <summary>
        /// コレクションの登録日
        /// </summary>
        public string RegistrationDateLabel { get; set; } = "Registration Date";
        /// <summary>
        /// コレクションの登録日の表示・非表示フラグ
        /// </summary>
        public bool RegistrationDateVisible { get; set; } = true;
        /// <summary>
        /// コレクションのカテゴリのラベル
        /// </summary>
        public string CategoryLabel { get; set; } = "Category";
        /// <summary>
        /// コレクションのカテゴリの表示・非表示フラグ
        /// </summary>
        public bool CategoryVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ1のラベル
        /// </summary>
        public string FirstTagLabel { get; set; } = "Tag1";
        /// <summary>
        /// コレクションのタグ1の表示・非表示フラグ
        /// </summary>
        public bool FirstTagVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ2のラベル
        /// </summary>
        public string SecondTagLabel { get; set; } = "Tag2";
        /// <summary>
        /// コレクションのタグ2の表示・非表示フラグ
        /// </summary>
        public bool SecondTagVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ3のラベル
        /// </summary>
        public string ThirdTagLabel { get; set; } = "Tag3";
        /// <summary>
        /// コレクションのタグ3の表示・非表示フラグ
        /// </summary>
        public bool ThirdTagVisible { get; set; } = true;
        /// <summary>
        /// コレクションの現物保管場所のラベル
        /// </summary>
        public string RealLocationLabel { get; set; } = "Real location";
        /// <summary>
        /// コレクションの現物保管場所のラベルの表示・非表示フラグ
        /// </summary>
        public bool RealLocationVisible { get; set; } = true;
        /// <summary>
        /// コレクションのデータ保管場所のラベル
        /// </summary>
        public string DataLocationLabel { get; set; } = "Data location";
        /// <summary>
        /// コレクションのデータ保管場場所のラベルの表示・非表示フラグ
        /// </summary>
        public bool DataLocationVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのUUID列表示・非表示フラグ
        /// </summary>
        public bool CollectionListUUIDVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧での管理コード列表示・非表示フラグ
        /// </summary>
        public bool CollectionListManagementCodeVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧での名称列表示・非表示フラグ
        /// </summary>
        public bool CollectionListNameVisible { get; set; } = true;
        /// <summary>
        ///  コレクション一覧での登録日表示・非表示フラグ
        /// </summary>
        public bool CollectionListRegistrationDateVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのカテゴリ表示・非表示フラグ
        /// </summary>
        public bool CollectionListCategoryVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ1表示・非表示フラグ
        /// </summary>
        public bool CollectionListFirstTagVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ2表示・非表示フラグ
        /// </summary>
        public bool CollectionListSecondTagVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ3表示・非表示フラグ
        /// </summary>
        public bool CollectionListThirdTagVisible { get; set; } = true;
        /// <summary>
        /// コレクション一覧での在庫情報表示・非表示フラグ
        /// </summary>
        public bool CollectionListInventoryInformationVisible { get; set; } = true;
        /// <summary>
        /// 検索対象の番号
        /// </summary>
        public int SearchOptionNumber { get; set; } = 0;
        /// <summary>
        /// 検索方法の番号
        /// </summary>
        public int SearchMethodNumber { get; set; } = 0;
        /// <summary>
        /// SleepModeの設定
        /// </summary>
        public SleepMode SleepMode { get; set; } = SleepMode.Deep;
        /// <summary>
        /// データ監視の間隔
        /// </summary>
        public int DataCheckInterval { get; set; } = 100;
        /// <summary>
        /// バックアップ保持数（各コレクションの最大バックアップ数）
        /// </summary>
        public int MaxBackupCount { get; set; } = 256;
        /// <summary>
        /// コレクションリストの自動更新設定
        /// </summary>
        public bool CollectionListAutoUpdate { get; set; } = false;
    }

    public class ProjectSettingClass
    {
        /// <summary>
        /// プロジェクトファイル読み込み処理
        /// </summary>
        /// <param name="projectSettingValues">読み込むプロジェクトの設定値、参照渡し</param>
        /// <param name="path">読み込むプロジェクトファイルのパス</param>
        /// <returns>読み込み成功：true、読み込み失敗：false</returns>
        public static bool LoadProjectSetting(ref ProjectSettingValuesClass projectSettingValues)
        {
            var loadingProjectSettingValues = new ProjectSettingValuesClass();// 読み込んだ設定値を一時保存する変数
            // 初期化
            IEnumerable<string> lines;
            if (File.Exists(projectSettingValues.ProjectSettingFilePath))
            {
                try
                {
                    lines = File.ReadLines(projectSettingValues.ProjectSettingFilePath, Encoding.GetEncoding("UTF-8"));
                }
                catch
                {
                    MessageBox.Show("プロジェクトファイルの読み込みに失敗しました。", "CREC");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("プロジェクトファイルが見つかりませんでした。", "CREC");
                return false;
            }
            loadingProjectSettingValues.ProjectSettingFilePath = projectSettingValues.ProjectSettingFilePath;
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                switch (cols[0])
                {
                    case "projectname":
                        loadingProjectSettingValues.Name = cols[1];
                        break;
                    case "projectlocation":
                        loadingProjectSettingValues.ProjectDataFolderPath = cols[1];
                        break;
                    case "backuplocation":
                        loadingProjectSettingValues.ProjectBackupFolderPath = cols[1];
                        break;
                    case "autobackup":
                        if (cols[1].Contains("S"))
                        {
                            loadingProjectSettingValues.StartUpBackUp = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.StartUpBackUp = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            loadingProjectSettingValues.CloseBackUp = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.CloseBackUp = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            loadingProjectSettingValues.EditBackUp = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.EditBackUp = false;
                        }
                        break;
                    case "BackupCompressionType":
                        try
                        {
                            loadingProjectSettingValues.BackupCompressionType = (CREC.BackupCompressionType)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.BackupCompressionType = (CREC.BackupCompressionType)1;
                        }
                        break;
                    case "MaxDegreeOfBackUpProcessParallelism":
                        try
                        {
                            if (cols[1] == "null")
                            {
                                loadingProjectSettingValues.MaxDegreeOfBackUpProcessParallelism = null;
                            }
                            else
                            {
                                loadingProjectSettingValues.MaxDegreeOfBackUpProcessParallelism = Convert.ToInt32(cols[1]);
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.MaxDegreeOfBackUpProcessParallelism = null;
                        }
                        break;
                    case "MaxBackupCount":
                        try
                        {
                            int maxBackupCount = Convert.ToInt32(cols[1]);
                            loadingProjectSettingValues.MaxBackupCount = maxBackupCount >= 1 ? maxBackupCount : 256;
                        }
                        catch
                        {
                            loadingProjectSettingValues.MaxBackupCount = 256;
                        }
                        break;
                    case "Listoutputlocation":
                        loadingProjectSettingValues.ListOutputPath = cols[1];
                        break;
                    case "autoListoutput":
                        if (cols[1].Contains("S"))
                        {
                            loadingProjectSettingValues.StartUpListOutput = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.StartUpListOutput = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            loadingProjectSettingValues.CloseListOutput = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.CloseListOutput = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            loadingProjectSettingValues.EditListOutput = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.EditListOutput = false;
                        }
                        break;
                    case "openListafteroutput":
                        if (cols[1].Contains("O"))
                        {
                            loadingProjectSettingValues.OpenListAfterOutput = true;
                        }
                        else
                        {
                            loadingProjectSettingValues.OpenListAfterOutput = false;
                        }
                        break;
                    case "ListOutputFormat":
                        if (cols[1] == "CSV")
                        {
                            loadingProjectSettingValues.ListOutputFormat = ListOutputFormat.CSV;
                        }
                        else if (cols[1] == "TSV")
                        {
                            loadingProjectSettingValues.ListOutputFormat = ListOutputFormat.TSV;
                        }
                        break;
                    case "created":
                        loadingProjectSettingValues.CreatedDate = cols[1];
                        break;
                    case "modified":
                        loadingProjectSettingValues.ModifiedDate = cols[1];
                        break;
                    case "accessed":
                        // 現在時刻を取得 
                        DateTime dateTime = DateTime.Now;
                        loadingProjectSettingValues.AccessedDate = dateTime.ToString("yyyy/MM/dd hh:mm:ss");
                        break;
                    case "Color":
                        try
                        {
                            loadingProjectSettingValues.ColorSetting = (ColorValue)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.ColorSetting = ColorValue.Blue;
                        }
                        break;
                    case "ShowObjectNameLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.CollectionNameLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.CollectionNameLabel = "Name";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.CollectionNameVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.CollectionNameVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.CollectionNameVisible = true;
                        }
                        break;
                    case "ShowIDLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.UUIDLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.UUIDLabel = "UUID";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.UUIDVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.UUIDVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.UUIDVisible = true;
                        }
                        break;
                    case "ShowMCLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.ManagementCodeLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.ManagementCodeLabel = "管理コード";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.ManagementCodeVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.ManagementCodeVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.ManagementCodeVisible = true;
                        }
                        break;
                    case "ShowRegistrationDateLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.RegistrationDateLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.RegistrationDateLabel = "登録日";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.RegistrationDateVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.RegistrationDateVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.RegistrationDateVisible = true;
                        }
                        break;
                    case "AutoMCFill":
                        try
                        {
                            if (cols[1] == "f")
                            {
                                loadingProjectSettingValues.ManagementCodeAutoFill = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.ManagementCodeAutoFill = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.ManagementCodeAutoFill = true;
                        }
                        break;
                    case "ShowCategoryLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.CategoryLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.CategoryLabel = "カテゴリ";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.CategoryVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.CategoryVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.CategoryVisible = true;
                        }
                        break;
                    case "Tag1Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.FirstTagLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.FirstTagLabel = "タグ１";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.FirstTagVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.FirstTagVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.FirstTagVisible = true;
                        }
                        break;
                    case "Tag2Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.SecondTagLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.SecondTagLabel = "タグ２";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.SecondTagVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.SecondTagVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.SecondTagVisible = true;
                        }
                        break;
                    case "Tag3Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.ThirdTagLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.ThirdTagLabel = "タグ３";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.ThirdTagVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.ThirdTagVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.ThirdTagVisible = true;
                        }
                        break;
                    case "ShowRealLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.RealLocationLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.RealLocationLabel = "現物保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.RealLocationVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.RealLocationVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.RealLocationVisible = true;
                        }
                        break;
                    case "ShowDataLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                loadingProjectSettingValues.DataLocationLabel = cols[1];
                            }
                            else
                            {
                                loadingProjectSettingValues.DataLocationLabel = "データ保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                loadingProjectSettingValues.DataLocationVisible = false;
                            }
                            else
                            {
                                loadingProjectSettingValues.DataLocationVisible = true;
                            }
                        }
                        catch
                        {
                            loadingProjectSettingValues.DataLocationVisible = true;
                        }
                        break;
                    case "IDListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListUUIDVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListUUIDVisible = true;
                        }
                        break;
                    case "MCListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListManagementCodeVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListManagementCodeVisible = true;
                        }
                        break;
                    case "ObjectNameListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListNameVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListNameVisible = true;
                        }
                        break;
                    case "RegistrationDateListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListRegistrationDateVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListRegistrationDateVisible = true;
                        }
                        break;
                    case "CategoryListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListCategoryVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListCategoryVisible = true;
                        }
                        break;
                    case "Tag1ListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListFirstTagVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListFirstTagVisible = true;
                        }
                        break;
                    case "Tag2ListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListSecondTagVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListSecondTagVisible = true;
                        }
                        break;
                    case "Tag3ListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListThirdTagVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListThirdTagVisible = true;
                        }
                        break;
                    case "InventoryInformationListVisible":
                        if (cols[1] == "false")
                        {
                            loadingProjectSettingValues.CollectionListInventoryInformationVisible = false;
                        }
                        else
                        {
                            loadingProjectSettingValues.CollectionListInventoryInformationVisible = true;
                        }
                        break;
                    case "SearchOptionNumber":
                        try
                        {
                            loadingProjectSettingValues.SearchOptionNumber = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.SearchOptionNumber = 0;
                        }
                        break;
                    case "SearchMethodNumber":
                        try
                        {
                            loadingProjectSettingValues.SearchMethodNumber = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.SearchMethodNumber = 0;
                        }
                        break;
                    case "SleepMode":
                        try
                        {
                            loadingProjectSettingValues.SleepMode = (CREC.SleepMode)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.SleepMode = (CREC.SleepMode)0;
                        }
                        break;
                    case "DataCheckInterval":
                        try
                        {
                            loadingProjectSettingValues.DataCheckInterval = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            loadingProjectSettingValues.DataCheckInterval = 100;
                        }
                        break;
                    case "CollectionListAutoUpdate":
                        try
                        {
                            loadingProjectSettingValues.CollectionListAutoUpdate = cols[1] == "true";
                        }
                        catch
                        {
                            loadingProjectSettingValues.CollectionListAutoUpdate = false;
                        }
                        break;
                }
            }
            CheckListVisibleColumnExist(ref loadingProjectSettingValues);
            projectSettingValues = loadingProjectSettingValues;// 読み込んだ設定値を渡す
            return true;
        }

        /// <summary>
        /// プロジェクトファイル保存
        /// </summary>
        /// <param name="projectSettingValues">保存するプロジェクトの設定値</param>
        /// <param name="updateModifiedDate">最終更新日を更新するかどうか</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>保存成功：true、保存失敗：false</returns>
        public static bool SaveProjectSetting(
            ref ProjectSettingValuesClass projectSettingValues,
            bool updateModifiedDate,
            XElement languageData)
        {
            bool returnValue = false;
            if (projectSettingValues.ProjectSettingFilePath.Length == 0)// pathが指定されているか確認
            {
                MessageBox.Show("保存先が指定されていません。", "CREC");
                return false;
            }
            // プロジェクトファイル名と名前が一致しているか確認
            if (Path.GetFileNameWithoutExtension(projectSettingValues.ProjectSettingFilePath) != projectSettingValues.Name)
            {
                // 一致していない場合は警告を表示して保存するか確認
                MessageBoxResult result = MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("ProjectNameMatchError", "ProjectSettingClass", languageData),
                    "CREC",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            StreamWriter streamWriter = null; // 修正: 変数を初期化
            try
            {
                streamWriter = new StreamWriter(projectSettingValues.ProjectSettingFilePath, false, Encoding.GetEncoding("UTF-8"));
                streamWriter.WriteLine("{0},{1}", "projectname", projectSettingValues.Name);
                streamWriter.WriteLine("{0},{1}", "projectlocation", projectSettingValues.ProjectDataFolderPath);
                streamWriter.WriteLine("{0},{1}", "backuplocation", projectSettingValues.ProjectBackupFolderPath);
                streamWriter.Write("autobackup,");
                if (projectSettingValues.StartUpBackUp == true)
                {
                    streamWriter.Write("S");
                }
                if (projectSettingValues.CloseBackUp == true)
                {
                    streamWriter.Write("C");
                }
                if (projectSettingValues.EditBackUp == true)
                {
                    streamWriter.Write("E");
                }
                streamWriter.Write('\n');
                streamWriter.WriteLine("BackupCompressionType,{0}", (int)projectSettingValues.BackupCompressionType);
                if (projectSettingValues.MaxDegreeOfBackUpProcessParallelism == null)
                {
                    streamWriter.WriteLine("MaxDegreeOfBackUpProcessParallelism,null");
                }
                else
                {
                    streamWriter.WriteLine("MaxDegreeOfBackUpProcessParallelism,{0}", projectSettingValues.MaxDegreeOfBackUpProcessParallelism);
                }
                streamWriter.WriteLine("MaxBackupCount,{0}", (int)projectSettingValues.MaxBackupCount);
                streamWriter.WriteLine("{0},{1}", "Listoutputlocation", projectSettingValues.ListOutputPath);
                streamWriter.Write("autoListoutput,");
                if (projectSettingValues.StartUpListOutput == true)
                {
                    streamWriter.Write("S");
                }
                if (projectSettingValues.CloseListOutput == true)
                {
                    streamWriter.Write("C");
                }
                if (projectSettingValues.EditListOutput == true)
                {
                    streamWriter.Write("E");
                }
                streamWriter.Write('\n');
                streamWriter.Write("openListafteroutput,");
                if (projectSettingValues.OpenListAfterOutput == true)
                {
                    streamWriter.Write("O");
                }
                streamWriter.Write('\n');
                streamWriter.WriteLine("ListOutputFormat,{0}", projectSettingValues.ListOutputFormat.ToString());
                streamWriter.WriteLine("{0},{1}", "created", projectSettingValues.CreatedDate);
                // 最終更新日を更新する場合は現在時刻を設定
                if (updateModifiedDate == true)
                {
                    projectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");// 現在時刻を取得して最終更新日として設定
                }
                streamWriter.WriteLine("{0},{1}", "modified", projectSettingValues.ModifiedDate);// 現在時刻を記録
                streamWriter.WriteLine("{0},{1}", "accessed", projectSettingValues.AccessedDate);
                streamWriter.WriteLine("Color,{0}", (int)projectSettingValues.ColorSetting);
                if (projectSettingValues.CollectionNameVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowObjectNameLabel", projectSettingValues.CollectionNameLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowObjectNameLabel", projectSettingValues.CollectionNameLabel, "f");
                }
                if (projectSettingValues.UUIDVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowIDLabel", projectSettingValues.UUIDLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowIDLabel", projectSettingValues.UUIDLabel, "f");
                }
                if (projectSettingValues.ManagementCodeVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowMCLabel", projectSettingValues.ManagementCodeLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowMCLabel", projectSettingValues.ManagementCodeLabel, "f");
                }
                if (projectSettingValues.RegistrationDateVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowRegistrationDateLabel", projectSettingValues.RegistrationDateLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowRegistrationDateLabel", projectSettingValues.RegistrationDateLabel, "f");
                }
                if (projectSettingValues.ManagementCodeAutoFill == true)
                {
                    streamWriter.Write("AutoMCFill,t\n");
                }
                else
                {
                    streamWriter.Write("AutoMCFill,f\n");
                }
                if (projectSettingValues.CategoryVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowCategoryLabel", projectSettingValues.CategoryLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowCategoryLabel", projectSettingValues.CategoryLabel, "f");
                }
                if (projectSettingValues.FirstTagVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag1Name", projectSettingValues.FirstTagLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag1Name", projectSettingValues.FirstTagLabel, "f");
                }
                if (projectSettingValues.SecondTagVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag2Name", projectSettingValues.SecondTagLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag2Name", projectSettingValues.SecondTagLabel, "f");
                }
                if (projectSettingValues.ThirdTagVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag3Name", projectSettingValues.ThirdTagLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "Tag3Name", projectSettingValues.ThirdTagLabel, "f");
                }
                if (projectSettingValues.RealLocationVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowRealLocationLabel", projectSettingValues.RealLocationLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowRealLocationLabel", projectSettingValues.RealLocationLabel, "f");
                }
                if (projectSettingValues.DataLocationVisible == true)
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowDataLocationLabel", projectSettingValues.DataLocationLabel, "t");
                }
                else
                {
                    streamWriter.WriteLine("{0},{1},{2}", "ShowDataLocationLabel", projectSettingValues.DataLocationLabel, "f");
                }
                streamWriter.WriteLine("{0},{1}", "SearchOptionNumber", projectSettingValues.SearchOptionNumber.ToString());
                streamWriter.WriteLine("{0},{1}", "SearchMethodNumber", projectSettingValues.SearchMethodNumber.ToString());
                if (projectSettingValues.CollectionListUUIDVisible == true)
                {
                    streamWriter.WriteLine("IDListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("IDListVisible,false");
                }
                if (projectSettingValues.CollectionListManagementCodeVisible == true)
                {
                    streamWriter.WriteLine("MCListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("MCListVisible,false");
                }
                if (projectSettingValues.CollectionListNameVisible == true)
                {
                    streamWriter.WriteLine("ObjectNameListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("ObjectNameListVisible,false");
                }
                if (projectSettingValues.CollectionListRegistrationDateVisible == true)
                {
                    streamWriter.WriteLine("RegistrationDateListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("RegistrationDateListVisible,false");
                }
                if (projectSettingValues.CollectionListCategoryVisible == true)
                {
                    streamWriter.WriteLine("CategoryListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("CategoryListVisible,false");
                }
                if (projectSettingValues.CollectionListFirstTagVisible == true)
                {
                    streamWriter.WriteLine("Tag1ListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("Tag1ListVisible,false");
                }
                if (projectSettingValues.CollectionListSecondTagVisible == true)
                {
                    streamWriter.WriteLine("Tag2ListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("Tag2ListVisible,false");
                }
                if (projectSettingValues.CollectionListThirdTagVisible == true)
                {
                    streamWriter.WriteLine("Tag3ListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("Tag3ListVisible,false");
                }
                if (projectSettingValues.CollectionListInventoryInformationVisible == true)
                {
                    streamWriter.WriteLine("InventoryInformationListVisible,true");
                }
                else
                {
                    streamWriter.WriteLine("InventoryInformationListVisible,false");
                }
                streamWriter.WriteLine("SleepMode,{0}", (int)projectSettingValues.SleepMode);
                streamWriter.WriteLine("DataCheckInterval,{0}", (int)projectSettingValues.DataCheckInterval);
                streamWriter.WriteLine("CollectionListAutoUpdate,{0}", projectSettingValues.CollectionListAutoUpdate ? "true" : "false");
                returnValue = true;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合は再起処理するユーザーに尋ねる。
                if (MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("ProjectSettingFileSaveError", "ProjectSettingClass", languageData) + "\n" + ex.Message,
                    "CREC",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                    }
                    return SaveProjectSetting(ref projectSettingValues, updateModifiedDate, languageData); // 再試行
                }
                else
                {
                    returnValue = false;
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
            return returnValue;
        }

        /// <summary>
        /// １列以上はコレクション一覧で表示されるようにする
        /// </summary>
        public static void CheckListVisibleColumnExist(ref ProjectSettingValuesClass projectSettingValues)
        {
            if (projectSettingValues.CollectionListUUIDVisible == false
                && projectSettingValues.CollectionListManagementCodeVisible == false
                && projectSettingValues.CollectionListNameVisible == false
                && projectSettingValues.CollectionListRegistrationDateVisible == false
                && projectSettingValues.CollectionListCategoryVisible == false
                && projectSettingValues.CollectionListFirstTagVisible == false
                && projectSettingValues.CollectionListSecondTagVisible == false
                && projectSettingValues.CollectionListThirdTagVisible == false
                && projectSettingValues.CollectionListInventoryInformationVisible == false)
            {
                MessageBox.Show("全項目が非表示状態に設定されています。システム上IDのみ表示します。", "CREC");
                projectSettingValues.CollectionListUUIDVisible = true;
            }
        }
    }
}