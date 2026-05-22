/*
Program
Copyright (c) [2022-2026] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
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
        /// コレクション一覧でのUUID列幅自動調整フラグ
        /// </summary>
        public bool CollectionListUUIDAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧での管理コード列幅自動調整フラグ
        /// </summary>
        public bool CollectionListManagementCodeAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧での名前列幅自動調整フラグ
        /// </summary>
        public bool CollectionListNameAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧での登録日列幅自動調整フラグ
        /// </summary>
        public bool CollectionListRegistrationDateAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのカテゴリ列幅自動調整フラグ
        /// </summary>
        public bool CollectionListCategoryAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ1列幅自動調整フラグ
        /// </summary>
        public bool CollectionListFirstTagAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ2列幅自動調整フラグ
        /// </summary>
        public bool CollectionListSecondTagAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧でのタグ3列幅自動調整フラグ
        /// </summary>
        public bool CollectionListThirdTagAutoWidth { get; set; } = true;
        /// <summary>
        /// コレクション一覧での在庫情報列幅自動調整フラグ
        /// </summary>
        public bool CollectionListInventoryInformationAutoWidth { get; set; } = true;
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
        /// データ監視の間隔(秒)
        /// </summary>
        public int DataCheckInterval { get; set; } = 10;
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
            if (!File.Exists(projectSettingValues.ProjectSettingFilePath))
            {
                MessageBox.Show("プロジェクトファイルが見つかりませんでした。", "CREC");
                return false;
            }
            string jsonContent;
            try
            {
                jsonContent = File.ReadAllText(projectSettingValues.ProjectSettingFilePath, Encoding.GetEncoding("UTF-8"));
            }
            catch
            {
                MessageBox.Show("プロジェクトファイルの読み込みに失敗しました。", "CREC");
                return false;
            }
            return LoadProjectSettingFromJson(jsonContent, ref projectSettingValues);
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
            // 最終更新日を更新する場合は現在時刻を設定
            if (updateModifiedDate == true)
            {
                projectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");// 現在時刻を取得して最終更新日として設定
            }
            try
            {
                streamWriter = new StreamWriter(projectSettingValues.ProjectSettingFilePath, false, Encoding.GetEncoding("UTF-8"));
                streamWriter.Write(BuildProjectSettingJson(projectSettingValues));
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

        // ────────────────────────────────────────────────────────────────────────────
        // JSON シリアライズ / デシリアライズ ヘルパー
        // ────────────────────────────────────────────────────────────────────────────

        /// <summary>JSON 用に文字列をエスケープして二重引用符で囲んで返す。null の場合は "null" を返す。</summary>
        private static string JsonStr(string value)
        {
            if (value == null) return "null";
            var sb = new StringBuilder();
            sb.Append('"');
            foreach (char c in value)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:
                        if (c < 0x20)
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        /// <summary>bool 値を JSON リテラル ("true" / "false") に変換する。</summary>
        private static string JsonBool(bool value) => value ? "true" : "false";

        /// <summary>int 値を JSON 数値文字列に変換する。</summary>
        private static string JsonInt(int value) => value.ToString();

        /// <summary>null 許容 int 値を JSON 数値または "null" に変換する。</summary>
        private static string JsonIntOrNull(int? value) => value.HasValue ? value.Value.ToString() : "null";

        /// <summary>
        /// 親要素から指定名の子要素の値を取得する。
        /// 要素が存在しないか JSON null の場合は null を返す。
        /// </summary>
        private static string GetJsonElementValue(XElement parent, string name)
        {
            XElement el = parent?.Element(name);
            if (el == null) return null;
            if (el.Attribute("type")?.Value == "null") return null;
            return el.Value;
        }

        /// <summary>親要素から指定名の子要素を bool として取得する。</summary>
        private static bool GetJsonElementBool(XElement parent, string name, bool defaultValue = false)
        {
            string val = GetJsonElementValue(parent, name);
            if (val == null) return defaultValue;
            return val.ToLowerInvariant() == "true";
        }

        /// <summary>親要素から指定名の子要素を int として取得する。</summary>
        private static int GetJsonElementInt(XElement parent, string name, int defaultValue = 0)
        {
            string val = GetJsonElementValue(parent, name);
            if (val == null) return defaultValue;
            if (int.TryParse(val, out int result)) return result;
            return defaultValue;
        }

        /// <summary>
        /// JSON フォーマットのプロジェクトファイルを読み込む。
        /// </summary>
        private static bool LoadProjectSettingFromJson(string jsonContent, ref ProjectSettingValuesClass projectSettingValues)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(jsonContent);
                XDocument doc;
                using (var ms = new MemoryStream(bytes))
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(ms, new XmlDictionaryReaderQuotas()))
                {
                    doc = XDocument.Load(reader);
                }

                XElement root = doc.Root;
                XElement ps   = root?.Element("projectSettings");
                XElement bs   = root?.Element("backupSettings");
                XElement os   = root?.Element("outputSettings");
                XElement ds   = root?.Element("displaySettings");
                XElement ls   = root?.Element("labelSettings");
                XElement ss   = root?.Element("searchSettings");
                XElement lvs  = root?.Element("listVisibilitySettings");
                XElement laws = root?.Element("listAutoWidthSettings");
                XElement bhs  = root?.Element("behaviorSettings");

                var result = new ProjectSettingValuesClass();
                result.ProjectSettingFilePath = projectSettingValues.ProjectSettingFilePath;

                // projectSettings
                if (ps != null)
                {
                    result.Name                      = GetJsonElementValue(ps, "projectName")      ?? string.Empty;
                    result.ProjectDataFolderPath     = GetJsonElementValue(ps, "projectLocation")  ?? string.Empty;
                    result.ProjectBackupFolderPath   = GetJsonElementValue(ps, "backupLocation")   ?? string.Empty;
                    result.ListOutputPath            = GetJsonElementValue(ps, "listOutputLocation") ?? string.Empty;
                    result.CreatedDate               = GetJsonElementValue(ps, "created")          ?? string.Empty;
                    result.ModifiedDate              = GetJsonElementValue(ps, "modified")         ?? string.Empty;
                    // アクセス日時はロード時に現在時刻で更新する
                    result.AccessedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                }

                // backupSettings
                if (bs != null)
                {
                    XElement autoBackup = bs.Element("autoBackup");
                    if (autoBackup != null)
                    {
                        result.StartUpBackUp = GetJsonElementBool(autoBackup, "startUp", false);
                        result.CloseBackUp   = GetJsonElementBool(autoBackup, "close",   false);
                        result.EditBackUp    = GetJsonElementBool(autoBackup, "edit",    false);
                    }

                    result.BackupCompressionType = (BackupCompressionType)GetJsonElementInt(bs, "backupCompressionType", 1);

                    string maxParallel = GetJsonElementValue(bs, "maxDegreeOfBackUpProcessParallelism");
                    if (maxParallel != null && int.TryParse(maxParallel, out int maxP))
                        result.MaxDegreeOfBackUpProcessParallelism = maxP;
                    else
                        result.MaxDegreeOfBackUpProcessParallelism = null;

                    int maxCount = GetJsonElementInt(bs, "maxBackupCount", 256);
                    result.MaxBackupCount = maxCount >= 1 ? maxCount : 256;
                }

                // outputSettings
                if (os != null)
                {
                    XElement autoListOutput = os.Element("autoListOutput");
                    if (autoListOutput != null)
                    {
                        result.StartUpListOutput = GetJsonElementBool(autoListOutput, "startUp", false);
                        result.CloseListOutput   = GetJsonElementBool(autoListOutput, "close",   false);
                        result.EditListOutput    = GetJsonElementBool(autoListOutput, "edit",    false);
                    }

                    result.OpenListAfterOutput = GetJsonElementBool(os, "openListAfterOutput", false);

                    string format = GetJsonElementValue(os, "listOutputFormat");
                    result.ListOutputFormat = format == "TSV" ? ListOutputFormat.TSV : ListOutputFormat.CSV;
                }

                // displaySettings
                if (ds != null)
                {
                    result.ColorSetting          = (ColorValue)GetJsonElementInt(ds, "color", 0);
                    result.ManagementCodeAutoFill = GetJsonElementBool(ds, "autoMCFill", true);
                }

                // labelSettings
                if (ls != null)
                {
                    XElement objectName = ls.Element("objectName");
                    if (objectName != null)
                    {
                        result.CollectionNameLabel   = GetJsonElementValue(objectName, "displayName") ?? "Name";
                        result.CollectionNameVisible = GetJsonElementBool(objectName, "enabled", true);
                    }
                    XElement id = ls.Element("id");
                    if (id != null)
                    {
                        result.UUIDLabel   = GetJsonElementValue(id, "displayName") ?? "UUID";
                        result.UUIDVisible = GetJsonElementBool(id, "enabled", true);
                    }
                    XElement mc = ls.Element("mc");
                    if (mc != null)
                    {
                        result.ManagementCodeLabel   = GetJsonElementValue(mc, "displayName") ?? "Mgmt. code";
                        result.ManagementCodeVisible = GetJsonElementBool(mc, "enabled", true);
                    }
                    XElement registrationDate = ls.Element("registrationDate");
                    if (registrationDate != null)
                    {
                        result.RegistrationDateLabel   = GetJsonElementValue(registrationDate, "displayName") ?? "Registration Date";
                        result.RegistrationDateVisible = GetJsonElementBool(registrationDate, "enabled", true);
                    }
                    XElement category = ls.Element("category");
                    if (category != null)
                    {
                        result.CategoryLabel   = GetJsonElementValue(category, "displayName") ?? "Category";
                        result.CategoryVisible = GetJsonElementBool(category, "enabled", true);
                    }
                    XElement tag1 = ls.Element("tag1");
                    if (tag1 != null)
                    {
                        result.FirstTagLabel   = GetJsonElementValue(tag1, "displayName") ?? "Tag1";
                        result.FirstTagVisible = GetJsonElementBool(tag1, "enabled", true);
                    }
                    XElement tag2 = ls.Element("tag2");
                    if (tag2 != null)
                    {
                        result.SecondTagLabel   = GetJsonElementValue(tag2, "displayName") ?? "Tag2";
                        result.SecondTagVisible = GetJsonElementBool(tag2, "enabled", true);
                    }
                    XElement tag3 = ls.Element("tag3");
                    if (tag3 != null)
                    {
                        result.ThirdTagLabel   = GetJsonElementValue(tag3, "displayName") ?? "Tag3";
                        result.ThirdTagVisible = GetJsonElementBool(tag3, "enabled", true);
                    }
                    XElement realLocation = ls.Element("realLocation");
                    if (realLocation != null)
                    {
                        result.RealLocationLabel   = GetJsonElementValue(realLocation, "displayName") ?? "Real location";
                        result.RealLocationVisible = GetJsonElementBool(realLocation, "enabled", true);
                    }
                    XElement dataLocation = ls.Element("dataLocation");
                    if (dataLocation != null)
                    {
                        result.DataLocationLabel   = GetJsonElementValue(dataLocation, "displayName") ?? "Data location";
                        result.DataLocationVisible = GetJsonElementBool(dataLocation, "enabled", true);
                    }
                }

                // searchSettings
                if (ss != null)
                {
                    result.SearchOptionNumber = GetJsonElementInt(ss, "searchOptionNumber", 0);
                    result.SearchMethodNumber = GetJsonElementInt(ss, "searchMethodNumber", 0);
                }

                // listVisibilitySettings
                if (lvs != null)
                {
                    result.CollectionListUUIDVisible                 = GetJsonElementBool(lvs, "id",                  true);
                    result.CollectionListManagementCodeVisible       = GetJsonElementBool(lvs, "mc",                  true);
                    result.CollectionListNameVisible                 = GetJsonElementBool(lvs, "objectName",          true);
                    result.CollectionListRegistrationDateVisible     = GetJsonElementBool(lvs, "registrationDate",    true);
                    result.CollectionListCategoryVisible             = GetJsonElementBool(lvs, "category",            true);
                    result.CollectionListFirstTagVisible             = GetJsonElementBool(lvs, "tag1",                true);
                    result.CollectionListSecondTagVisible            = GetJsonElementBool(lvs, "tag2",                true);
                    result.CollectionListThirdTagVisible             = GetJsonElementBool(lvs, "tag3",                true);
                    result.CollectionListInventoryInformationVisible = GetJsonElementBool(lvs, "inventoryInformation", true);
                }

                // listAutoWidthSettings
                if (laws != null)
                {
                    result.CollectionListUUIDAutoWidth                 = GetJsonElementBool(laws, "id",                  true);
                    result.CollectionListManagementCodeAutoWidth       = GetJsonElementBool(laws, "mc",                  true);
                    result.CollectionListNameAutoWidth                 = GetJsonElementBool(laws, "name",                 true);
                    result.CollectionListRegistrationDateAutoWidth     = GetJsonElementBool(laws, "registrationDate",    true);
                    result.CollectionListCategoryAutoWidth             = GetJsonElementBool(laws, "category",             true);
                    result.CollectionListFirstTagAutoWidth             = GetJsonElementBool(laws, "tag1",                 true);
                    result.CollectionListSecondTagAutoWidth            = GetJsonElementBool(laws, "tag2",                 true);
                    result.CollectionListThirdTagAutoWidth             = GetJsonElementBool(laws, "tag3",                 true);
                    result.CollectionListInventoryInformationAutoWidth = GetJsonElementBool(laws, "inventoryInformation", true);
                }

                // behaviorSettings
                if (bhs != null)
                {
                    result.SleepMode               = (SleepMode)GetJsonElementInt(bhs, "sleepMode",              0);
                    result.DataCheckInterval        = GetJsonElementInt(bhs, "dataCheckInterval",                10);
                    result.CollectionListAutoUpdate = GetJsonElementBool(bhs, "collectionListAutoUpdate",         false);
                }

                CheckListVisibleColumnExist(ref result);
                projectSettingValues = result;
                return true;
            }
            catch
            {
                MessageBox.Show("プロジェクトファイルの読み込みに失敗しました。", "CREC");
                return false;
            }
        }

        /// <summary>
        /// プロジェクト設定を JSON 文字列に変換する。
        /// </summary>
        private static string BuildProjectSettingJson(ProjectSettingValuesClass p)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"projectSettings\": {");
            sb.AppendLine($"    \"projectName\": {JsonStr(p.Name)},");
            sb.AppendLine($"    \"projectLocation\": {JsonStr(p.ProjectDataFolderPath)},");
            sb.AppendLine($"    \"backupLocation\": {JsonStr(p.ProjectBackupFolderPath)},");
            sb.AppendLine($"    \"listOutputLocation\": {JsonStr(p.ListOutputPath)},");
            sb.AppendLine($"    \"created\": {JsonStr(p.CreatedDate)},");
            sb.AppendLine($"    \"modified\": {JsonStr(p.ModifiedDate)},");
            sb.AppendLine($"    \"accessed\": {JsonStr(p.AccessedDate)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"backupSettings\": {");
            sb.AppendLine("    \"autoBackup\": {");
            sb.AppendLine($"      \"startUp\": {JsonBool(p.StartUpBackUp)},");
            sb.AppendLine($"      \"close\": {JsonBool(p.CloseBackUp)},");
            sb.AppendLine($"      \"edit\": {JsonBool(p.EditBackUp)}");
            sb.AppendLine("    },");
            sb.AppendLine($"    \"backupCompressionType\": {JsonInt((int)p.BackupCompressionType)},");
            sb.AppendLine($"    \"maxDegreeOfBackUpProcessParallelism\": {JsonIntOrNull(p.MaxDegreeOfBackUpProcessParallelism)},");
            sb.AppendLine($"    \"maxBackupCount\": {JsonInt(p.MaxBackupCount)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"outputSettings\": {");
            sb.AppendLine("    \"autoListOutput\": {");
            sb.AppendLine($"      \"startUp\": {JsonBool(p.StartUpListOutput)},");
            sb.AppendLine($"      \"close\": {JsonBool(p.CloseListOutput)},");
            sb.AppendLine($"      \"edit\": {JsonBool(p.EditListOutput)}");
            sb.AppendLine("    },");
            sb.AppendLine($"    \"openListAfterOutput\": {JsonBool(p.OpenListAfterOutput)},");
            sb.AppendLine($"    \"listOutputFormat\": {JsonStr(p.ListOutputFormat.ToString())}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"displaySettings\": {");
            sb.AppendLine($"    \"color\": {JsonInt((int)p.ColorSetting)},");
            sb.AppendLine($"    \"autoMCFill\": {JsonBool(p.ManagementCodeAutoFill)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"labelSettings\": {");
            sb.AppendLine("    \"objectName\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.CollectionNameLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.CollectionNameVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"id\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.UUIDLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.UUIDVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"mc\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.ManagementCodeLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.ManagementCodeVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"registrationDate\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.RegistrationDateLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.RegistrationDateVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"category\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.CategoryLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.CategoryVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"tag1\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.FirstTagLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.FirstTagVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"tag2\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.SecondTagLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.SecondTagVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"tag3\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.ThirdTagLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.ThirdTagVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"realLocation\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.RealLocationLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.RealLocationVisible)}");
            sb.AppendLine("    },");
            sb.AppendLine("    \"dataLocation\": {");
            sb.AppendLine($"      \"displayName\": {JsonStr(p.DataLocationLabel)},");
            sb.AppendLine($"      \"enabled\": {JsonBool(p.DataLocationVisible)}");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"searchSettings\": {");
            sb.AppendLine($"    \"searchOptionNumber\": {JsonInt(p.SearchOptionNumber)},");
            sb.AppendLine($"    \"searchMethodNumber\": {JsonInt(p.SearchMethodNumber)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"listVisibilitySettings\": {");
            sb.AppendLine($"    \"id\": {JsonBool(p.CollectionListUUIDVisible)},");
            sb.AppendLine($"    \"mc\": {JsonBool(p.CollectionListManagementCodeVisible)},");
            sb.AppendLine($"    \"objectName\": {JsonBool(p.CollectionListNameVisible)},");
            sb.AppendLine($"    \"registrationDate\": {JsonBool(p.CollectionListRegistrationDateVisible)},");
            sb.AppendLine($"    \"category\": {JsonBool(p.CollectionListCategoryVisible)},");
            sb.AppendLine($"    \"tag1\": {JsonBool(p.CollectionListFirstTagVisible)},");
            sb.AppendLine($"    \"tag2\": {JsonBool(p.CollectionListSecondTagVisible)},");
            sb.AppendLine($"    \"tag3\": {JsonBool(p.CollectionListThirdTagVisible)},");
            sb.AppendLine($"    \"inventoryInformation\": {JsonBool(p.CollectionListInventoryInformationVisible)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"listAutoWidthSettings\": {");
            sb.AppendLine($"    \"id\": {JsonBool(p.CollectionListUUIDAutoWidth)},");
            sb.AppendLine($"    \"mc\": {JsonBool(p.CollectionListManagementCodeAutoWidth)},");
            sb.AppendLine($"    \"name\": {JsonBool(p.CollectionListNameAutoWidth)},");
            sb.AppendLine($"    \"registrationDate\": {JsonBool(p.CollectionListRegistrationDateAutoWidth)},");
            sb.AppendLine($"    \"category\": {JsonBool(p.CollectionListCategoryAutoWidth)},");
            sb.AppendLine($"    \"tag1\": {JsonBool(p.CollectionListFirstTagAutoWidth)},");
            sb.AppendLine($"    \"tag2\": {JsonBool(p.CollectionListSecondTagAutoWidth)},");
            sb.AppendLine($"    \"tag3\": {JsonBool(p.CollectionListThirdTagAutoWidth)},");
            sb.AppendLine($"    \"inventoryInformation\": {JsonBool(p.CollectionListInventoryInformationAutoWidth)}");
            sb.AppendLine("  },");
            sb.AppendLine("  \"behaviorSettings\": {");
            sb.AppendLine($"    \"sleepMode\": {JsonInt((int)p.SleepMode)},");
            sb.AppendLine($"    \"dataCheckInterval\": {JsonInt(p.DataCheckInterval)},");
            sb.AppendLine($"    \"collectionListAutoUpdate\": {JsonBool(p.CollectionListAutoUpdate)}");
            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
