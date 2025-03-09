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
    public enum CompressType
    {
        SingleFile,
        ParData,
        NoCompress
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
        /// 圧縮方法
        /// </summary>
        public CompressType CompressType { get; set; } = CompressType.NoCompress;
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
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                switch (cols[0])
                {
                    case "projectname":
                        projectSettingValues.Name = cols[1];
                        break;
                    case "projectlocation":
                        projectSettingValues.ProjectDataFolderPath = cols[1];
                        break;
                    case "backuplocation":
                        projectSettingValues.ProjectBackupFolderPath = cols[1];
                        break;
                    case "autobackup":
                        if (cols[1].Contains("S"))
                        {
                            projectSettingValues.StartUpBackUp = true;
                        }
                        else
                        {
                            projectSettingValues.StartUpBackUp = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            projectSettingValues.CloseBackUp = true;
                        }
                        else
                        {
                            projectSettingValues.CloseBackUp = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            projectSettingValues.EditBackUp = true;
                        }
                        else
                        {
                            projectSettingValues.EditBackUp = false;
                        }
                        break;
                    case "CompressType":
                        try
                        {
                            projectSettingValues.CompressType = (CREC.CompressType)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.CompressType = (CREC.CompressType)1;
                        }
                        break;
                    case "Listoutputlocation":
                        projectSettingValues.ListOutputPath = cols[1];
                        break;
                    case "autoListoutput":
                        if (cols[1].Contains("S"))
                        {
                            projectSettingValues.StartUpListOutput = true;
                        }
                        else
                        {
                            projectSettingValues.StartUpListOutput = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            projectSettingValues.CloseListOutput = true;
                        }
                        else
                        {
                            projectSettingValues.CloseListOutput = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            projectSettingValues.EditListOutput = true;
                        }
                        else
                        {
                            projectSettingValues.EditListOutput = false;
                        }
                        break;
                    case "openListafteroutput":
                        if (cols[1].Contains("O"))
                        {
                            projectSettingValues.OpenListAfterOutput = true;
                        }
                        else
                        {
                            projectSettingValues.OpenListAfterOutput = false;
                        }
                        break;
                    case "ListOutputFormat":
                        if (cols[1] == "CSV")
                        {
                            projectSettingValues.ListOutputFormat = ListOutputFormat.CSV;
                        }
                        else if (cols[1] == "TSV")
                        {
                            projectSettingValues.ListOutputFormat = ListOutputFormat.TSV;
                        }
                        break;
                    case "created":
                        projectSettingValues.CreatedDate = cols[1];
                        break;
                    case "modified":
                        projectSettingValues.ModifiedDate = cols[1];
                        break;
                    case "accessed":
                        // 現在時刻を取得 
                        DateTime dateTime = DateTime.Now;
                        projectSettingValues.AccessedDate = dateTime.ToString("yyyy/MM/dd hh:mm:ss");
                        break;
                    case "Color":
                        try
                        {
                            projectSettingValues.ColorSetting = (ColorValue)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.ColorSetting = ColorValue.Blue;
                        }
                        break;
                    case "ShowObjectNameLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.CollectionNameLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.CollectionNameLabel = "Name";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.CollectionNameVisible = false;
                            }
                            else
                            {
                                projectSettingValues.CollectionNameVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.CollectionNameVisible = true;
                        }
                        break;
                    case "ShowIDLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.UUIDLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.UUIDLabel = "UUID";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.UUIDVisible = false;
                            }
                            else
                            {
                                projectSettingValues.UUIDVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.UUIDVisible = true;
                        }
                        break;
                    case "ShowMCLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.ManagementCodeLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.ManagementCodeLabel = "管理コード";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.ManagementCodeVisible = false;
                            }
                            else
                            {
                                projectSettingValues.ManagementCodeVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.ManagementCodeVisible = true;
                        }
                        break;
                    case "ShowRegistrationDateLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.RegistrationDateLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.RegistrationDateLabel = "登録日";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.RegistrationDateVisible = false;
                            }
                            else
                            {
                                projectSettingValues.RegistrationDateVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.RegistrationDateVisible = true;
                        }
                        break;
                    case "AutoMCFill":
                        try
                        {
                            if (cols[1] == "f")
                            {
                                projectSettingValues.ManagementCodeAutoFill = false;
                            }
                            else
                            {
                                projectSettingValues.ManagementCodeAutoFill = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.ManagementCodeAutoFill = true;
                        }
                        break;
                    case "ShowCategoryLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.CategoryLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.CategoryLabel = "カテゴリ";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.CategoryVisible = false;
                            }
                            else
                            {
                                projectSettingValues.CategoryVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.CategoryVisible = true;
                        }
                        break;
                    case "Tag1Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.FirstTagLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.FirstTagLabel = "タグ１";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.FirstTagVisible = false;
                            }
                            else
                            {
                                projectSettingValues.FirstTagVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.FirstTagVisible = true;
                        }
                        break;
                    case "Tag2Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.SecondTagLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.SecondTagLabel = "タグ２";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.SecondTagVisible = false;
                            }
                            else
                            {
                                projectSettingValues.SecondTagVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.SecondTagVisible = true;
                        }
                        break;
                    case "Tag3Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.ThirdTagLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.ThirdTagLabel = "タグ３";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.ThirdTagVisible = false;
                            }
                            else
                            {
                                projectSettingValues.ThirdTagVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.ThirdTagVisible = true;
                        }
                        break;
                    case "ShowRealLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.RealLocationLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.RealLocationLabel = "現物保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.RealLocationVisible = false;
                            }
                            else
                            {
                                projectSettingValues.RealLocationVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.RealLocationVisible = true;
                        }
                        break;
                    case "ShowDataLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                projectSettingValues.DataLocationLabel = cols[1];
                            }
                            else
                            {
                                projectSettingValues.DataLocationLabel = "データ保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                projectSettingValues.DataLocationVisible = false;
                            }
                            else
                            {
                                projectSettingValues.DataLocationVisible = true;
                            }
                        }
                        catch
                        {
                            projectSettingValues.DataLocationVisible = true;
                        }
                        break;
                    case "IDListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListUUIDVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListUUIDVisible = true;
                        }
                        break;
                    case "MCListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListManagementCodeVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListManagementCodeVisible = true;
                        }
                        break;
                    case "ObjectNameListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListNameVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListNameVisible = true;
                        }
                        break;
                    case "RegistrationDateListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListRegistrationDateVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListRegistrationDateVisible = true;
                        }
                        break;
                    case "CategoryListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListCategoryVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListCategoryVisible = true;
                        }
                        break;
                    case "Tag1ListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListFirstTagVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListFirstTagVisible = true;
                        }
                        break;
                    case "Tag2ListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListSecondTagVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListSecondTagVisible = true;
                        }
                        break;
                    case "Tag3ListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListThirdTagVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListThirdTagVisible = true;
                        }
                        break;
                    case "InventoryInformationListVisible":
                        if (cols[1] == "false")
                        {
                            projectSettingValues.CollectionListInventoryInformationVisible = false;
                        }
                        else
                        {
                            projectSettingValues.CollectionListInventoryInformationVisible = true;
                        }
                        break;
                    case "SearchOptionNumber":
                        try
                        {
                            projectSettingValues.SearchOptionNumber = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.SearchOptionNumber = 0;
                        }
                        break;
                    case "SearchMethodNumber":
                        try
                        {
                            projectSettingValues.SearchMethodNumber = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.SearchMethodNumber = 0;
                        }
                        break;
                    case "SleepMode":
                        try
                        {
                            projectSettingValues.SleepMode = (CREC.SleepMode)Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.SleepMode = (CREC.SleepMode)0;
                        }
                        break;
                    case "DataCheckInterval":
                        try
                        {
                            projectSettingValues.DataCheckInterval = Convert.ToInt32(cols[1]);
                        }
                        catch
                        {
                            projectSettingValues.DataCheckInterval = 100;
                        }
                        break;

                }
            }
            CheckListVisibleColumnExist(ref projectSettingValues);
            return true;
        }

        /// <summary>
        /// プロジェクトファイル保存
        /// </summary>
        /// <param name="projectSettingValues">保存するプロジェクトの設定値</param>
        /// <param name="path">保存するプロジェクトファイルのパス</param>
        /// <returns>保存成功：true、保存失敗：false</returns>
        public static bool SaveProjectSetting(ProjectSettingValuesClass projectSettingValues,  XElement languageData)
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
                MessageBoxResult result = MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("ProjectNameMatchError", "ProjectSettingClass", languageData), "CREC", MessageBoxButton.YesNo);
                 if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            StreamWriter streamWriter;
            streamWriter = new StreamWriter(projectSettingValues.ProjectSettingFilePath, false, Encoding.GetEncoding("UTF-8"));
            try
            {
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
                streamWriter.WriteLine("CompressType,{0}", (int)projectSettingValues.CompressType);
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
                streamWriter.WriteLine("{0},{1}", "modified", projectSettingValues.ModifiedDate);
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
                returnValue = true;
            }
            catch
            {
                returnValue = false;
            }
            finally
            {
                streamWriter.Close();
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