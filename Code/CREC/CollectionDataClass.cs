/*
Program
Copyright (c) [2022-2026] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace CREC
{
    /// <summary>
    /// 在庫状況の種類
    /// </summary>
    public enum InventoryStatus
    {
        StockOut,// 在庫切れ
        UnderStocked,// 在庫不足
        Appropriate,// 在庫適正
        OverStocked,// 在庫過剰
        NotSet// 未設定
    }

    /// <summary>
    /// 詳細表示中のコレクションの状態
    /// </summary>
    public enum CollectionOperationStatus
    {
        Watching,// 閲覧中
        Editing,// 編集中
        EditRequesting,// 編集リクエスト中
    }

    /// <summary>
    /// JSON形式のIndexファイルのsystemDataセクション
    /// </summary>
    [DataContract]
    public class IndexJsonSystemData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        
        [DataMember(Name = "systemCreateDate")]
        public string SystemCreateDate { get; set; }
    }

    /// <summary>
    /// JSON形式のIndexファイルのvaluesセクション
    /// </summary>
    [DataContract]
    public class IndexJsonValues
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "managementCode")]
        public string ManagementCode { get; set; }
        
        [DataMember(Name = "registrationDate")]
        public string RegistrationDate { get; set; }
        
        [DataMember(Name = "category")]
        public string Category { get; set; }
        
        [DataMember(Name = "firstTag")]
        public string FirstTag { get; set; }
        
        [DataMember(Name = "secondTag")]
        public string SecondTag { get; set; }
        
        [DataMember(Name = "thirdTag")]
        public string ThirdTag { get; set; }
        
        [DataMember(Name = "location")]
        public string Location { get; set; }
    }

    /// <summary>
    /// JSON形式のIndexファイル
    /// </summary>
    [DataContract]
    public class IndexJsonFormat
    {
        [DataMember(Name = "systemData")]
        public IndexJsonSystemData SystemData { get; set; }
        
        [DataMember(Name = "values")]
        public IndexJsonValues Values { get; set; }
    }

    /// <summary>
    /// 各コレクションのデータを保持するクラス
    /// </summary>
    public class CollectionDataValuesClass
    {
        private string collectionFolderPath = string.Empty;
        /// <summary>
        /// CollectionFolderPath: Collection folder path
        /// </summary>
        public string CollectionFolderPath
        {
            get
            {
                return collectionFolderPath;
            }
            set
            {
                collectionFolderPath = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionFolderPath.Length == 0)
                {
                    collectionFolderPath = " - ";
                }
            }
        }

        private string collectionName = string.Empty;
        /// <summary>
        /// CollectionName: Collection name
        /// </summary>
        public string CollectionName
        {
            get
            {
                return collectionName;
            }
            set
            {
                collectionName = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionName.Length == 0)
                {
                    collectionName = " - ";
                }
            }
        }

        private string collectionID = string.Empty;
        /// <summary>
        /// CollectionID: Collection ID
        /// </summary>
        public string CollectionID
        {
            get
            {
                return collectionID;
            }
            set
            {
                collectionID = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionID.Length == 0)
                {
                    collectionID = " - ";
                }
            }
        }

        private string collectionMC = string.Empty;
        /// <summary>
        /// CollectionMC: Collection management code
        /// </summary>
        public string CollectionMC
        {
            get
            {
                return collectionMC;
            }
            set
            {
                collectionMC = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionMC.Length == 0)
                {
                    collectionMC = " - ";
                }
            }
        }

        private string collectionRegistrationDate = string.Empty;
        /// <summary>
        /// CollectionRegistrationDate: Collection registration date
        /// </summary>
        public string CollectionRegistrationDate
        {
            get
            {
                return collectionRegistrationDate;
            }
            set
            {
                collectionRegistrationDate = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionRegistrationDate.Length == 0)
                {
                    collectionRegistrationDate = " - ";
                }
            }
        }

        private string collectionCategory = string.Empty;
        /// <summary>
        /// CollectionType: Collection type
        /// </summary>
        public string CollectionCategory
        {
            get
            {
                return collectionCategory;
            }
            set
            {
                collectionCategory = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionCategory.Length == 0)
                {
                    collectionCategory = " - ";
                }
            }
        }

        private string collectionTag1 = string.Empty;
        /// <summary>
        /// CollectionTag1: Collection tag1
        /// </summary>
        public string CollectionTag1
        {
            get
            {
                return collectionTag1;
            }
            set
            {
                collectionTag1 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag1.Length == 0)
                {
                    collectionTag1 = " - ";
                }
            }
        }

        private string collectionTag2 = string.Empty;
        /// <summary>
        /// CollectionTag2: Collection tag2
        /// </summary>
        public string CollectionTag2
        {
            get
            {
                return collectionTag2;
            }
            set
            {
                collectionTag2 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag2.Length == 0)
                {
                    collectionTag2 = " - ";
                }
            }
        }

        private string collectionTag3 = string.Empty;
        /// <summary>
        /// CollectionTag3: Collection tag3
        /// </summary>
        public string CollectionTag3
        {
            get
            {
                return collectionTag3;
            }
            set
            {
                collectionTag3 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag3.Length == 0)
                {
                    collectionTag3 = " - ";
                }
            }
        }

        private string collectionRealLocation = string.Empty;
        /// <summary>
        /// CollectionRealLocation: Collection real location
        /// </summary>
        public string CollectionRealLocation
        {
            get
            {
                return collectionRealLocation;
            }
            set
            {
                collectionRealLocation = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionRealLocation.Length == 0)
                {
                    collectionRealLocation = " - ";
                }
            }
        }

        private int? collectionCurrentInventory = null;
        /// <summary>
        /// CollectionCurrentInventory: Collection's current inventory
        /// </summary>
        public int? CollectionCurrentInventory
        {
            get
            {
                return collectionCurrentInventory;
            }
            set
            {
                collectionCurrentInventory = value;
            }
        }

        private InventoryStatus collectionInventoryStatus = InventoryStatus.NotSet;
        public InventoryStatus CollectionInventoryStatus
        {
            get
            {
                return collectionInventoryStatus;
            }
            set
            {
                collectionInventoryStatus = value;
            }
        }
    }

    public class CollectionDataClass
    {
        // 定数
        const string BackupLogFolderName = "!BackupLog";// バックアップログのフォルダ名を定義

        /// <summary>
        /// JSON形式でIndexファイルを読み込む
        /// </summary>
        /// <param name="jsonFilePath">JSONファイルのパス</param>
        /// <returns>読み込んだIndexデータ</returns>
        private static IndexJsonFormat LoadIndexJsonFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var serializer = new DataContractJsonSerializer(typeof(IndexJsonFormat));

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    // JSONデータをデシリアライズ
                    IndexJsonFormat indexData = (IndexJsonFormat)serializer.ReadObject(stream);
                    // indexData.values.RegistrationDateをローカルタイムゾーンのISO8601形式に変換
                    if (indexData != null && indexData.Values != null)
                    {
                        // RegistrationDateがnullまたは空文字列の場合はそのまま返す
                        if (string.IsNullOrEmpty(indexData.Values.RegistrationDate))
                        {
                            indexData.Values.RegistrationDate = " - ";
                        }
                        else
                        {
                            // RegistrationDateをローカル時刻に変換
                            DateTimeOffset dto;
                            if (DateTimeOffset.TryParse(indexData.Values.RegistrationDate, out dto))
                            {
                                // ローカルタイムゾーンのISO8601形式に変換
                                indexData.Values.RegistrationDate = dto.ToLocalTime().ToString("o");
                            }
                        }
                    }
                    return indexData;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 登録日をISO8601形式に変換する
        /// </summary>
        /// <param name="registrationDate">元の登録日文字列</param>
        /// <returns>ISO8601形式の日時文字列</returns>
        private static string ConvertRegistrationDateToIso8601(string registrationDate)
        {
            // 空または既定値の場合はそのまま返す
            if (string.IsNullOrEmpty(registrationDate) || registrationDate == " - ")
            {
                return registrationDate;
            }

            // 複数の日時フォーマットを試行
            string[] formats = {
                "yyyy/MM/dd_HH:mm:ss.f",  // コレクション登録日の形式
                "yyyy/MM/dd_HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss.f",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy/MM/dd",// 日付のみ
            };

            foreach (string format in formats)
            {
                if (DateTime.TryParseExact(
                    registrationDate,
                    format,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeLocal,
                    out DateTime parsedDateTime))
                {
                    // UTCに変換してISO8601形式で返す
                    DateTimeOffset dto = new DateTimeOffset(parsedDateTime.ToUniversalTime());
                    return dto.ToString("o");
                }
            }

            // 解析に失敗した場合は元の文字列をそのまま返す
            return registrationDate;
        }

        /// <summary>
        /// index.txtからindex.jsonへ変換する
        /// </summary>
        /// <param name="CollectionFolderPath">コレクションフォルダのパス</param>
        /// <param name="deleteOldFile">変換後に古いファイルを削除するか</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>変換に成功した場合はtrue</returns>
        private static bool MigrateIndexTxtToJson(string CollectionFolderPath, ref bool? deleteLegacyIndexFile, XElement languageData)
        {
            // 削除フラグが未設定の場合、ユーザーに確認
            if (deleteLegacyIndexFile == null)
            {
                DialogResult deleteOldFile = MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("IndexFileMigrationDeleteOldFile", "CollectionDataClass", languageData),
                    "CREC",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                switch (deleteOldFile)
                {
                    case DialogResult.Yes:
                        deleteLegacyIndexFile = true;
                        break;
                    case DialogResult.No:
                        deleteLegacyIndexFile = false;
                        break;
                }
            }

            try
            {
                string txtPath = CollectionFolderPath + @"\index.txt";

                if (!File.Exists(txtPath))
                {
                    return false;
                }

                // index.txtを読み込む
                var loadingData = new CollectionDataValuesClass();
                loadingData.CollectionID = new DirectoryInfo(CollectionFolderPath).Name;
                loadingData.CollectionName = " - ";
                loadingData.CollectionMC = " - ";
                loadingData.CollectionRegistrationDate = " - ";
                loadingData.CollectionCategory = " - ";
                loadingData.CollectionTag1 = " - ";
                loadingData.CollectionTag2 = " - ";
                loadingData.CollectionTag3 = " - ";
                loadingData.CollectionRealLocation = " - ";

                string[] lines = File.ReadAllLines(txtPath, Encoding.GetEncoding("UTF-8"));
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length < 2)
                        continue;

                    switch (parts[0])
                    {
                        case "名称":
                            if (line.Length > 3)
                                loadingData.CollectionName = line.Substring(3);
                            break;
                        case "ID":
                            if (line.Length > 3)
                                loadingData.CollectionID = line.Substring(3);
                            break;
                        case "MC":
                            if (line.Length > 3)
                                loadingData.CollectionMC = line.Substring(3);
                            break;
                        case "登録日":
                            if (line.Length > 4)
                                loadingData.CollectionRegistrationDate = ConvertRegistrationDateToIso8601(line.Substring(4));
                            break;
                        case "カテゴリ":
                            if (line.Length > 5)
                                loadingData.CollectionCategory = line.Substring(5);
                            break;
                        case "タグ1":
                            if (line.Length > 4)
                                loadingData.CollectionTag1 = line.Substring(4);
                            break;
                        case "タグ2":
                            if (line.Length > 4)
                                loadingData.CollectionTag2 = line.Substring(4);
                            break;
                        case "タグ3":
                            if (line.Length > 4)
                                loadingData.CollectionTag3 = line.Substring(4);
                            break;
                        case "場所1(Real)":
                            if (parts.Length > 1)
                                loadingData.CollectionRealLocation = parts[1];
                            break;
                    }
                }

                if(!SaveCollectionIndexData(CollectionFolderPath, loadingData, languageData))
                {
                    return false;
                }

                // 古いファイルを削除するか確認
                if (deleteLegacyIndexFile == true)
                {
                    File.Delete(txtPath);
                }
                else
                {
                    string backupPath = CollectionFolderPath + @"\index_prevFormat.txt";
                    // 既存バックアップファイルがあっても上書きでコピーし、その後元ファイルを削除する
                    File.Copy(txtPath, backupPath, true);
                    File.Delete(txtPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// コレクションのデータ読み込み
        /// </summary>
        /// <param name="CollectionFolderPath">読み込み対象のコレクションのフォルダ</param>
        /// <param name="CollectionDataValues">読み込み対象のコレクションのデータ（参照渡し）</param>
        /// <param name="restoreIndexFileIfNotExist">Indexファイルが存在しない場合にバックアップから復元するかどうか</param>
        /// <param name="deleteLegacyIndexFile">古いIndexファイルを削除するかどうか</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>読み込んだコレクションのデータ</returns>
        public static bool LoadCollectionIndexData(
            string CollectionFolderPath,
            ref CollectionDataValuesClass CollectionDataValues,
            bool restoreIndexFileIfNotExist,
            ref bool? deleteLegacyIndexFile,
            XElement languageData)
        {
            var loadingCollectionDataValues = new CollectionDataValuesClass();// 読み込んだデータを一時的に保存する変数
            if (CollectionFolderPath.Length == 0)// コレクションのパスが指定されていない場合
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CollectionNotAssigned", "CollectionDataClass", languageData), "CREC");
                return false;
            }

            if (!System.IO.Directory.Exists(CollectionFolderPath))// コレクションのフォルダが存在しない場合
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CollectionNotExist", "CollectionDataClass", languageData), "CREC");
                return false;
            }
            loadingCollectionDataValues.CollectionFolderPath = CollectionFolderPath;
            try
            {
                string jsonFilePath = CollectionFolderPath + @"\SystemData\index.json";
                string txtFilePath = CollectionFolderPath + @"\index.txt";
                CollectionDataValues.CollectionFolderPath = CollectionFolderPath;

                // JSON形式のファイルを確認
                if (System.IO.File.Exists(jsonFilePath))
                {
                    // JSON形式のファイルが存在する場合は読み込む
                    IndexJsonFormat jsonData = LoadIndexJsonFile(jsonFilePath);
                    if (jsonData != null && jsonData.SystemData != null && jsonData.Values != null)
                    {
                        loadingCollectionDataValues.CollectionID = jsonData.SystemData.Id ?? " - ";
                        loadingCollectionDataValues.CollectionName = jsonData.Values.Name ?? " - ";
                        loadingCollectionDataValues.CollectionMC = jsonData.Values.ManagementCode ?? " - ";
                        loadingCollectionDataValues.CollectionRegistrationDate = jsonData.Values.RegistrationDate ?? " - ";
                        loadingCollectionDataValues.CollectionCategory = jsonData.Values.Category ?? " - ";
                        loadingCollectionDataValues.CollectionTag1 = jsonData.Values.FirstTag ?? " - ";
                        loadingCollectionDataValues.CollectionTag2 = jsonData.Values.SecondTag ?? " - ";
                        loadingCollectionDataValues.CollectionTag3 = jsonData.Values.ThirdTag ?? " - ";
                        loadingCollectionDataValues.CollectionRealLocation = jsonData.Values.Location ?? " - ";
                        CollectionDataValues = loadingCollectionDataValues;
                    }
                    return true;
                }
                // JSON形式のファイルが存在しない場合、index.txtを確認（後方互換用）
                else if (System.IO.File.Exists(txtFilePath))
                {
                    if (!MigrateIndexTxtToJson(CollectionFolderPath, ref deleteLegacyIndexFile, languageData))
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileMigrationFailed", "CollectionDataClass", languageData), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // 移行後、再度読み込みを試みる
                    return LoadCollectionIndexData(CollectionFolderPath, ref CollectionDataValues, false, ref deleteLegacyIndexFile, languageData);
                }
                // Indexファイルが存在せず、復元処理有効な場合
                else if (restoreIndexFileIfNotExist)
                {
                    // バックアップから復元または再生成する
                    if (!CollectionIndexRecovery_IndexFileNotFound(CollectionFolderPath, languageData))
                    {
                        return false;
                    }
                    // 復元後、再度読み込みを試みる
                    return LoadCollectionIndexData(CollectionFolderPath, ref CollectionDataValues, false, ref deleteLegacyIndexFile, languageData);
                }
                // 防御的処理：Indexファイルが存在しない場合
                else
                {
                    loadingCollectionDataValues.CollectionID = new DirectoryInfo(CollectionFolderPath).Name; // IDはフォルダ名
                    loadingCollectionDataValues.CollectionName = " - "; // 名称は空欄
                    loadingCollectionDataValues.CollectionMC = " - "; // MCは空欄
                    loadingCollectionDataValues.CollectionRegistrationDate = " - "; // 登録日は空欄
                    loadingCollectionDataValues.CollectionCategory = " - "; // カテゴリは空欄
                    loadingCollectionDataValues.CollectionTag1 = " - "; // タグ1は空欄
                    loadingCollectionDataValues.CollectionTag2 = " - "; // タグ2は空欄
                    loadingCollectionDataValues.CollectionTag3 = " - "; // タグ3は空欄
                    loadingCollectionDataValues.CollectionRealLocation = " - "; // 場所1(Real)は空欄
                    loadingCollectionDataValues.CollectionCurrentInventory = null; // 在庫数は未設定
                    loadingCollectionDataValues.CollectionInventoryStatus = InventoryStatus.NotSet; // 在庫状況は未設定
                    CollectionDataValues = loadingCollectionDataValues;// 読み込んだデータを返す
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileReadFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return CollectionIndexRecovery_IndexFileNotFound(CollectionFolderPath, languageData);
            }
        }

        /// <summary>
        /// コレクションのIndexファイル読み込み
        /// </summary>
        /// <param name="CollectionFolderPath">対象のコレクションフォルダパス</param>
        /// <param name="CollectionDataValues">対象のコレクションデータ</param>
        /// <param name="restoreIndexFileIfNotExist">Indexファイルが存在しない場合に復元するかどうか</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>読み込んだコレクションのデータ</returns>
        public static bool LoadCollectionIndexData(
            string CollectionFolderPath,
            ref CollectionDataValuesClass CollectionDataValues,
            bool restoreIndexFileIfNotExist,
            XElement languageData)
        {
            bool? deleteLegacyIndexFile = null;
            return LoadCollectionIndexData(CollectionFolderPath, ref CollectionDataValues, restoreIndexFileIfNotExist, ref deleteLegacyIndexFile, languageData);
        }

        /// <summary>
        /// コレクションの在庫情報読み込み
        /// </summary>
        /// <param name="CollectionFolderPath">対象のコレクションフォルダパス</param>
        /// <param name="CollectionDataValues">対象のコレクションデータ</param>
        /// <param name="deleteLegacyInventoryFile">レガシー在庫ファイル削除フラグ</param>
        /// <param name="languageData">言語ファイル</param>
        /// <returns></returns>
        public static bool LoadCollectionInventoryData(
            string CollectionFolderPath,
            ref CollectionDataValuesClass CollectionDataValues,
            ref bool? deleteLegacyInventoryFile,
            XElement languageData)
        {
            var loadingCollectionDataValues = new CollectionDataValuesClass();

            // InventoryDataIOを使用して在庫データを読み込み
            InventoryData inventoryData = InventoryDataIO.LoadInventoryData(CollectionFolderPath, ref deleteLegacyInventoryFile);
            if (inventoryData == null)
            {
                return false;
            }

            try
            {
                // 在庫数と在庫状況を計算
                int count = inventoryData.CalculateCurrentInventory();
                loadingCollectionDataValues.CollectionCurrentInventory = count;
                loadingCollectionDataValues.CollectionInventoryStatus = inventoryData.GetInventoryStatus();
            }
            catch
            {
                loadingCollectionDataValues.CollectionCurrentInventory = null;
                loadingCollectionDataValues.CollectionInventoryStatus = InventoryStatus.NotSet;
            }

            CollectionDataValues.CollectionInventoryStatus = loadingCollectionDataValues.CollectionInventoryStatus;
            CollectionDataValues.CollectionCurrentInventory = loadingCollectionDataValues.CollectionCurrentInventory;
            return true;
        }

        /// <summary>
        /// コレクションのIndexファイルが見つからない場合の処理
        /// </summary>
        /// <param name="CollectionFolderPath">対象のコレクションフォルダパス</param>
        /// <param name="languageData">言語データ</param>
        /// <returns></returns>
        private static bool CollectionIndexRecovery_IndexFileNotFound(string CollectionFolderPath, XElement languageData)
        {
            try
            {
                // バックアップデータを探索
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupData", "CollectionDataClass", languageData), "CREC");
                
                // バックアップを確認
                if (System.IO.File.Exists(CollectionFolderPath + @"\SystemData\index_old.json"))
                {
                    try
                    {
                        File.Copy(CollectionFolderPath + "\\SystemData\\index_old.json", CollectionFolderPath + "\\SystemData\\index.json", true);
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataSuccessed", "CollectionDataClass", languageData), "CREC");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    }
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataBackupDataNotFound", "CollectionDataClass", languageData), "CREC");
                }
                
                // Indexを再生成する
                CollectionDataValuesClass RecoveryCollectionDataValues = new CollectionDataValuesClass();
                RecoveryCollectionDataValues.CollectionID = new DirectoryInfo(CollectionFolderPath).Name; // IDはフォルダ名
                return SaveCollectionIndexData(CollectionFolderPath, RecoveryCollectionDataValues, languageData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoreFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return false;
            }
        }

        /// <summary>
        /// コレクションのデータ保存
        /// </summary>
        /// <param name="CollectionFolderPath">保存対象のコレクションのフォルダ</param>
        /// <param name="CollectionDataValues">保存対象のコレクションのデータ</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>成功したらtrue</returns>
        public static bool SaveCollectionIndexData(string CollectionFolderPath, CollectionDataValuesClass CollectionDataValues, XElement languageData)
        {
            try
            {
                string indexFilePath = CollectionFolderPath + @"\SystemData\index.json";
                string systemDataFolder = CollectionFolderPath + @"\SystemData";

                // SystemDataフォルダが存在しない場合は作成
                if (!Directory.Exists(systemDataFolder))
                {
                    Directory.CreateDirectory(systemDataFolder);
                }

                // 既存のJSONファイルがあれば、systemDataを保持する
                string existingId = CollectionDataValues.CollectionID;
                string existingSystemCreateDate = new DateTimeOffset(DateTime.UtcNow).ToString("o");
                if (File.Exists(indexFilePath))
                {
                    IndexJsonFormat existingData = LoadIndexJsonFile(indexFilePath);
                    if (existingData != null && existingData.SystemData != null)
                    {
                        existingId = existingData.SystemData.Id ?? CollectionDataValues.CollectionID;
                        existingSystemCreateDate = existingData.SystemData.SystemCreateDate ?? new DateTimeOffset(DateTime.UtcNow).ToString("o");
                    }
                }

                // ローカル時刻を含むCollectionDataValues.CollectionRegistrationDateをUTCに変換
                string utcRegistrationDate = CollectionDataValues.CollectionRegistrationDate;
                if (!string.IsNullOrEmpty(CollectionDataValues.CollectionRegistrationDate) 
                    && CollectionDataValues.CollectionRegistrationDate != " - ")
                {
                    DateTimeOffset dto;
                    if (DateTimeOffset.TryParse(CollectionDataValues.CollectionRegistrationDate, out dto))
                    {
                        // UTC時刻のISO8601形式に変換
                        utcRegistrationDate = dto.ToUniversalTime().ToString("o");
                    }
                }

                // JSON形式に変換
                IndexJsonFormat jsonData = new IndexJsonFormat
                {
                    SystemData = new IndexJsonSystemData
                    {
                        Id = existingId,
                        SystemCreateDate = existingSystemCreateDate
                    },
                    Values = new IndexJsonValues
                    {
                        Name = CollectionDataValues.CollectionName,
                        ManagementCode = CollectionDataValues.CollectionMC,
                        RegistrationDate = utcRegistrationDate,
                        Category = CollectionDataValues.CollectionCategory,
                        FirstTag = CollectionDataValues.CollectionTag1,
                        SecondTag = CollectionDataValues.CollectionTag2,
                        ThirdTag = CollectionDataValues.CollectionTag3,
                        Location = CollectionDataValues.CollectionRealLocation
                    }
                };

                // JSON形式で保存
                var serializer = new DataContractJsonSerializer(typeof(IndexJsonFormat));
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, jsonData);
                    string json = Encoding.UTF8.GetString(stream.ToArray());
                    File.WriteAllText(indexFilePath, json, Encoding.UTF8);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileSaveFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return false;
            }
        }

        /// <summary>
        /// コレクションのデータフォルダを開く
        /// </summary>
        /// <param name="CollectionDataValues">対象のコレクションデータ</param>
        /// <param name="languageData">言語ファイル</param>
        /// <returns></returns>
        public static bool OpenCollectionDataFolder(CollectionDataValuesClass CollectionDataValues, XElement languageData)
        {
            try
            {
                System.Diagnostics.Process.Start(CollectionDataValues.CollectionFolderPath + "\\data");
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
                return false;
            }
            return true;
        }

        /// <summary>
        /// コレクションIndexのデータバックアップ
        /// </summary>
        /// <param name="CollectionFolderPath">コレクションのフォルダパス</param>
        /// <param name="CollectionDataValues">コレクションのデータ</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>バックアップ成功若しくはバックアップ対象なしの場合はTrue</returns>
        public static bool BackupCollectionIndexData(
            string CollectionFolderPath,
            CollectionDataValuesClass CollectionDataValues,
            XElement languageData)
        {
            string indexFilePath = CollectionFolderPath + @"\SystemData\index.json";
            if (System.IO.File.Exists(indexFilePath))// JSON形式のファイルが存在する場合はバックアップを作成
            {
                try
                {
                    File.Copy(indexFilePath, CollectionFolderPath + "\\SystemData\\index_old.json", true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileBackupFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// コレクションのデータ削除
        /// </summary>
        /// <param name="CollectionFolderPath"></param>
        /// <param name="CollectionDataValues"></param>
        /// <param name="languageData"></param>
        /// <returns></returns>
        public static bool DeleteCollectionData(
            CollectionDataValuesClass CollectionDataValues,
            XElement languageData)
        {
            FileOperationClass.AddBlankFile(CollectionDataValues.CollectionFolderPath + "\\SystemData\\DEL");// 削除タグを作成

            int tryCount = 0;// 試行回数
            while (tryCount < 5)
            {
                try
                {
                    Directory.Delete(CollectionDataValues.CollectionFolderPath, recursive: true);// コレクションフォルダを破棄
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("データの削除に失敗しました。再試行します。\n" + ex.Message, "CREC");
                }
                tryCount++;
            }
            return true;
        }

        /// <summary>
        /// サムネイルを選択して生成
        /// </summary>
        /// <param name="CollectionFolderPath">コレクションのフォルダパス</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>サムネイル生成に成功したらTrue、その他False</returns>
        public static bool MakeTumbnailPicture(string CollectionFolderPath, XElement languageData)
        {
            // サムネイルとする画像を選択
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = CollectionFolderPath + "\\pictures";
            openFolderDialog.Title = LanguageSettingClass.GetOtherMessage("SelectThumbnail", "CollectionDataClass", languageData);
            openFolderDialog.Filter = "JPEG|*.jpg;*.jpeg;*.jfif;*.jpe" + "|PNG|*.png" + "|GIF|*.gif" + "|ICO|*.ico";
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// 画像読み込み成功
            {
                try
                {
                    // 画像最大サイズを400x400に設定
                    int TargetWidth = 400;
                    int TargetHeight = 400;
                    Bitmap TargetBitmap = new Bitmap(openFolderDialog.FileName);
                    // 画像サイズ設定
                    if (Math.Max(TargetBitmap.Width, TargetBitmap.Height) < 400)// 画像サイズが元々規定サイズ以内だった場合
                    {
                        TargetWidth = TargetBitmap.Width;
                        TargetHeight = TargetBitmap.Height;
                    }
                    else// 画像圧縮処理
                    {
                        // 長辺を取得してサイズを決定
                        if (TargetBitmap.Width > TargetBitmap.Height)// 横長画像
                        {
                            TargetWidth = 400;
                            TargetHeight = (int)(400.0 * TargetBitmap.Height / TargetBitmap.Width);
                        }
                        else if (TargetBitmap.Width < TargetBitmap.Height)// 縦長画像
                        {
                            TargetHeight = 400;
                            TargetWidth = (int)(400.0 * TargetBitmap.Width / TargetBitmap.Height);
                        }
                    }
                    Bitmap ConvertedBitmap = new Bitmap(TargetWidth, TargetHeight);
                    // DPI設定
                    if (Math.Max(TargetBitmap.HorizontalResolution, TargetBitmap.VerticalResolution) <= 72)
                    {
                        ConvertedBitmap.SetResolution(TargetBitmap.HorizontalResolution, TargetBitmap.VerticalResolution);
                    }
                    else
                    {
                        ConvertedBitmap.SetResolution(72.0F, 72.0F);
                    }
                    // エンコーダ設定
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L);
                    ImageCodecInfo imageCodecInfos = null;
                    foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageEncoders())
                    {
                        if (imageCodecInfo.FormatID == ImageFormat.Png.Guid)
                        {
                            imageCodecInfos = imageCodecInfo;
                            break;
                        }
                    }
                    // 変換＆保存
                    using (Graphics g = Graphics.FromImage(ConvertedBitmap))
                    {
                        g.DrawImage(TargetBitmap, 0, 0, TargetWidth, TargetHeight);
                    }
                    ConvertedBitmap.Save(CollectionFolderPath + "\\SystemData\\NewThumbnail.png", imageCodecInfos, encoderParameters);
                    ConvertedBitmap.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetOtherMessage("ThumbnailSetFaild", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    return false;
                }
            }
            else// 画像が選択されなかった場合
            {
                return false;
            }
        }

        /// <summary>
        /// 在庫状況を文字列に変換
        /// </summary>
        /// <param name="inventoryStatus">在庫状況（enum）</param>
        /// <returns>在庫状況（string）</returns>
        public static string InventoryStatusToString(InventoryStatus inventoryStatus, XElement languageData)
        {
            switch (inventoryStatus)
            {
                case InventoryStatus.StockOut:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_StockOut", "CollectionDataClass", languageData);
                case InventoryStatus.UnderStocked:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_UnderStocked", "CollectionDataClass", languageData);
                case InventoryStatus.Appropriate:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_Appropriate", "CollectionDataClass", languageData);
                case InventoryStatus.OverStocked:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_OverStocked", "CollectionDataClass", languageData);
                case InventoryStatus.NotSet:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_NotSet", "CollectionDataClass", languageData);
                default:
                    return LanguageSettingClass.GetOtherMessage("InventoryStatus_NotSet", "CollectionDataClass", languageData);
            }
        }

        /// <summary>
        /// キャッシュディレクトリを削除する
        /// </summary>
        /// <param name="cacheDirectoryPath">削除対象のキャッシュディレクトリパス</param>
        private static void CleanupBackupCacheDirectory(string cacheDirectoryPath)
        {
            if (Directory.Exists(cacheDirectoryPath))
            {
                try
                {
                    Directory.Delete(cacheDirectoryPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                    "キャッシュフォルダの削除に失敗しました。\n" + ex.Message,
                    "CREC",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// バックアップログを出力する
        /// </summary>
        /// <param name="backupFolderPath">バックアップフォルダのパス</param>
        /// <param name="collectionId">コレクションのUUID</param>
        /// <param name="isSuccess">バックアップ成功フラグ</param>
        public static void WriteBackupLog(
            string backupFolderPath,
            ConcurrentBag<(string collectionId, bool isSuccess)> backupLog,
            XElement languageData)
        {
            if (backupLog.Count == 0)
            {
                return; // バックアップリストが空の場合はログ出力しない
            }
            try
            {
                // BackupLogフォルダを作成(名前順で冒頭に表示されるよう、フォルダ名に感嘆符を付ける)
                string backupLogFolderPath = System.IO.Path.Combine(backupFolderPath, BackupLogFolderName);
                if (!Directory.Exists(backupLogFolderPath))
                {
                    Directory.CreateDirectory(backupLogFolderPath);
                }

                // ログファイル名：BackupLog_yyyyMMdd-hhmmssfff.txt
                string logName = $"BackupLog_{DateTime.Now:yyyyMMdd-HHmmssfff}";
                string logFilePath = System.IO.Path.Combine(backupLogFolderPath, logName);

                // ログファイルに追記
                using (StreamWriter writer = new StreamWriter(logFilePath + ".txt", true, Encoding.GetEncoding("UTF-8")))
                {
                    writer.WriteLine(logName);// logNameを書き込む
                    // バックアップに失敗したコレクションをログ出力
                    foreach (var (id, isSuccess) in backupLog)
                    {
                        if (!isSuccess)
                        {
                            string logEntry = $"{id}, Fail";
                            writer.WriteLine(logEntry);
                        }
                    }
                    // バックアップに成功したコレクションをログ出力
                    foreach (var (id, isSuccess) in backupLog)
                    {
                        if (isSuccess)
                        {
                            string logEntry = $"{id}, Success";
                            writer.WriteLine(logEntry);
                        }
                    }
                    writer.WriteLine(string.Empty);// 改行を追加
                }
            }
            catch (Exception ex)
            {
                // ログ出力失敗をメッセージボックスで表示
                MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("BackupLogOutputFailed", "CollectionDataClass", languageData) + ex.Message,
                    "CREC",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
        }

        /// <summary>
        /// コレクションのバックアップ
        /// </summary>
        /// <param name="projectSettingValues">プロジェクト設定値</param>
        /// <param name="collectionDataValues">コレクションデータ値</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>バックアップ結果</returns>
        public static bool BackupCollection(
            ProjectSettingValuesClass projectSettingValues,
            CollectionDataValuesClass collectionDataValues,
            XElement languageData
            )
        {
            // バックアップ場所が指定されていない場合はエラー
            if (string.IsNullOrEmpty(projectSettingValues.ProjectBackupFolderPath))
            {
                MessageBox.Show("バックアップ先が指定されていません。", "CREC");
                return false;
            }

            // プロジェクトバックアップフォルダの存在確認、なければ作成
            if (!Directory.Exists(projectSettingValues.ProjectBackupFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(projectSettingValues.ProjectBackupFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("バックアップフォルダの作成に失敗しました。\n" + ex.Message, "CREC");
                    return false;
                }
            }

            // コレクションバックアップフォルダの存在確認、なければ作成
            string backupFolderPath = System.IO.Path.Combine(projectSettingValues.ProjectBackupFolderPath, collectionDataValues.CollectionID);
            if (!Directory.Exists(backupFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(backupFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("コレクションバックアップフォルダの作成に失敗しました。\n" + ex.Message, "CREC");
                    return false;
                }
            }

            // バックアップデータの名称用に現在日時をミリ秒まで取得
            string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            // コレクションをローカルに複製（キャッシュ用）
            // キャッシュのパスは "backuptmp\UUID\
            string cacheUUID = Guid.NewGuid().ToString("N"); // UUIDを生成
            string cacheDirectoryPath = "backuptmp\\" + cacheUUID;

            try
            {
                FileSystem.CopyDirectory(
                    collectionDataValues.CollectionFolderPath,// コレクションのフォルダパス
                    cacheDirectoryPath + "\\" + collectionDataValues.CollectionID,// 一時的なバックアップフォルダ
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,// エラーダイアログのみ表示
                    Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing// キャンセルオプションは何もしない
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show("コレクションのバックアップ用キャッシュ作成に失敗しました。\n" + ex.Message, "CREC");
                CleanupBackupCacheDirectory(cacheDirectoryPath);
                return false;
            }

            // バックアップ時の圧縮有無で処理切り替え
            switch (projectSettingValues.BackupCompressionType)
            {
                case BackupCompressionType.NoCompress:
                    // 圧縮なしの場合はそのまま移動
                    try
                    {
                        FileSystem.CopyDirectory(
                            cacheDirectoryPath + "\\" + collectionDataValues.CollectionID,// 一時的なバックアップフォルダ
                            backupFolderPath + "\\" + currentDateTime,// バックアップ先フォルダ
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,// エラーダイアログのみ表示
                            Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing// キャンセルオプションは何もしない
                            );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("コレクションのバックアップに失敗しました。\n" + ex.Message, "CREC");
                        CleanupBackupCacheDirectory(cacheDirectoryPath);
                        return false;
                    }
                    break;
                case BackupCompressionType.Zip:
                    // 圧縮ありの場合はbackuptemp内でZipファイルに圧縮
                    try
                    {
                        ZipFile.CreateFromDirectory(
                            cacheDirectoryPath + "\\" + collectionDataValues.CollectionID,// 一時的なバックアップフォルダ
                            cacheDirectoryPath + "\\" + collectionDataValues.CollectionID + ".zip",// バックアップ先Zipファイル
                            CompressionLevel.Optimal,// 圧縮レベルは最適化
                            false// サブディレクトリは含めない
                            );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("コレクションのバックアップデータ圧縮に失敗しました。\n" + ex.Message, "CREC");
                        CleanupBackupCacheDirectory(cacheDirectoryPath);
                        return false;
                    }

                    // 指定のフォルダに圧縮したZipファイルを移動
                    try
                    {
                        FileSystem.MoveFile(
                            cacheDirectoryPath + "\\" + collectionDataValues.CollectionID + ".zip",// 一時的なZipファイル
                            backupFolderPath + "\\" + currentDateTime + ".zip",// バックアップ先Zipファイル
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,// エラーダイアログのみ表示
                            Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing// キャンセルオプションは何もしない
                            );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("コレクションのバックアップZipファイル移動に失敗しました。\n" + ex.Message, "CREC");
                        CleanupBackupCacheDirectory(cacheDirectoryPath);
                        return false;
                    }
                    break;
                default:
                    MessageBox.Show("不明な圧縮設定です。", "CREC");
                    CleanupBackupCacheDirectory(cacheDirectoryPath);
                    return false;
            }

            CleanupBackupCacheDirectory(cacheDirectoryPath);

            ManageBackupCount(projectSettingValues, backupFolderPath);// バックアップ数管理処理

            return true;
        }

        /// <summary>
        /// 指定したコレクションのバックアップ数を管理し、古いバックアップを削除する
        /// </summary>
        /// <param name="projectSettingValues">プロジェクト設定値</param>
        /// <param name="backupFolderPath">バックアップフォルダのパス</param>
        private static void ManageBackupCount(
            ProjectSettingValuesClass projectSettingValues,
            string backupFolderPath)
        {
            try
            {
                // バックアップフォルダが存在しない場合は何もしない
                if (!Directory.Exists(backupFolderPath))
                {
                    return;
                }

                List<FileSystemInfo> backedUpCollectionItems = new List<FileSystemInfo>();// バックアップファイル/フォルダのリストを取得

                // 圧縮設定に応じてファイルまたはフォルダを取得
                // フォルダバックアップを取得
                DirectoryInfo backupDirectoryInfo = new DirectoryInfo(backupFolderPath);
                backedUpCollectionItems.AddRange(backupDirectoryInfo.GetDirectories());
                // 圧縮バックアップを取得（.zipファイル）
                backedUpCollectionItems.AddRange(backupDirectoryInfo.GetFiles("*.zip"));

                backedUpCollectionItems.Sort((x, y) => x.CreationTime.CompareTo(y.CreationTime));// 作成日時順でソート（古い順）

                // バックアップ上限数を取得(パラメータチェック含む)
                int maxBackupCount
                        = projectSettingValues.MaxBackupCount
                        = Math.Max(projectSettingValues.MaxBackupCount, 1);// バックアップ上限数が1未満の場合は1に設定

                // 設定されたバックアップ上限数を超えている場合、古いものから削除
                while (backedUpCollectionItems.Count > maxBackupCount)
                {
                    FileSystemInfo oldestBackedUpCollectionItem = backedUpCollectionItems[0];// 最も古いバックアップをリストから取得
                    backedUpCollectionItems.RemoveAt(0);// 最も古いバックアップをリストから削除

                    try
                    {
                        if (oldestBackedUpCollectionItem is DirectoryInfo)// フォルダの場合
                        {
                            Directory.Delete(oldestBackedUpCollectionItem.FullName, true);
                        }
                        else if (oldestBackedUpCollectionItem is FileInfo)// ファイルの場合
                        {
                            File.Delete(oldestBackedUpCollectionItem.FullName);
                        }
                    }
                    catch (Exception ex)// 個別のバックアップ削除失敗は警告のみ表示して終了
                    {
                        MessageBox.Show($"古いバックアップの削除に失敗しました: {oldestBackedUpCollectionItem.Name}\n{ex.Message}", "CREC");
                        return;
                    }
                }

            }
            catch (Exception ex)// バックアップ管理処理の失敗は警告のみ表示して終了
            {
                MessageBox.Show($"バックアップ数管理処理でエラーが発生しました: {ex.Message}", "CREC");
            }
        }

        /// <summary>
        /// プロジェクトの全データをバックアップ（並列数を任意または自動で切り替え可能）
        /// </summary>
        /// <param name="projectSettingValues">プロジェクト設定値</param>
        /// <param name="backUpProgressReport">進捗報告用（完了数、総数）</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>成功：true / 失敗：false</returns>
        public static bool BackupProjectData(
            ProjectSettingValuesClass projectSettingValues,
            IProgress<(int completed, int total)> backUpProgressReport,
            XElement languageData
            )
        {
            // バックアップ対象及び総コレクション数を取得
            DirectoryInfo[] subFolderArray = null;
            try
            {
                DirectoryInfo di = new DirectoryInfo(projectSettingValues.ProjectDataFolderPath);
                subFolderArray = di.EnumerateDirectories("*").Where(folder => folder.Name != MainForm.ProjectSystemDataFolderName).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageSettingClass.GetMessageBoxMessage("FailedToRetrieveCollectionFolder", "CollectionDataClass", languageData) + ex.Message,
                    "CREC",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                return false;
            }
            int totalCollections = subFolderArray.Length;// 総コレクション数を取得

            // 進捗初期化
            int backedUpCount = 0;// バックアップ完了数を初期化
            backUpProgressReport?.Report((backedUpCount, totalCollections));// 進捗を初期化

            // 並列処理用リスト
            // IDと成功・失敗のBool値を格納する
            var backupLog = new System.Collections.Concurrent.ConcurrentBag<(string id, bool isSuccess)>();
            ParallelOptions options = new ParallelOptions();

            // MaxDegreeOfBackUpProcessParallelismがnullまたは0以下の場合は自動（デフォルト）
            if (projectSettingValues.MaxDegreeOfBackUpProcessParallelism.HasValue
                && projectSettingValues.MaxDegreeOfBackUpProcessParallelism.Value > 0)
            {
                options.MaxDegreeOfParallelism = projectSettingValues.MaxDegreeOfBackUpProcessParallelism.Value;
            }
            // 論理CPUコア数より設定値が多い場合は制限する
            if (options.MaxDegreeOfParallelism > Environment.ProcessorCount)
            {
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            }

            // 非同期でコレクションのバックアップを実行
            System.Threading.Tasks.Parallel.ForEach(subFolderArray, options, subFolder =>
            {
                // Index読み込み
                CollectionDataValuesClass collectionDataValues = new CollectionDataValuesClass();
                if (!CollectionDataClass.LoadCollectionIndexData(subFolder.FullName, ref collectionDataValues, false, languageData))
                {
                    // 読み込み失敗した場合はコレクションIDをリストに追加
                    backupLog.Add((collectionDataValues.CollectionID, false));
                    return; // 次のコレクションへ
                }

                bool backupResult = BackupCollection(projectSettingValues, collectionDataValues, languageData);// バックアップ実行
                backupLog.Add((collectionDataValues.CollectionID, backupResult));// バックアップ結果をリストに追加

                // 進捗更新
                int currentbackedUpCount = System.Threading.Interlocked.Increment(ref backedUpCount);// 完了数をインクリメント
                backUpProgressReport?.Report((currentbackedUpCount, totalCollections));// 進捗を報告
            });

            WriteBackupLog(projectSettingValues.ProjectBackupFolderPath, backupLog, languageData);// バックアップ失敗のログ出力

            // バックアップ失敗したコレクションのリストがある場合は結果をfalseに設定
            if (backupLog.Any(x => !x.isSuccess))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// プロジェクトの全データを非同期処理でバックアップ（並列数を任意または自動で切り替え可能）
        /// </summary>
        /// <param name="projectSettingValues">プロジェクト設定値</param>
        /// <param name="backUpProgressReport">進捗報告用（完了数、総数）</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>成功：true / 失敗：false</returns>
        public static async Task<bool> BackupProjectDataAsync(
            ProjectSettingValuesClass projectSettingValues,
            IProgress<(int completed, int total)> backUpProgressReport,
            XElement languageData
            )
        {
            return await Task.Run(() => BackupProjectData(
                projectSettingValues,
                backUpProgressReport,
                languageData));
        }

        /// <summary>
        /// 存在しないコレクションのバックアップフォルダを削除する
        /// </summary>
        /// <param name="projectSettingValues">プロジェクト設定値</param>
        /// <param name="allCollectionList">現在のコレクションリスト</param>
        /// <param name="deletedBackupData">削除したデータを戻すための参照渡し引数</param>
        /// <returns>成功：true / 失敗：false</returns>
        public static bool CleanupDeletedCollectionBackupFolders(
            ProjectSettingValuesClass projectSettingValues,
            List<CollectionDataValuesClass> allCollectionList,
            ref int deletedBackupData)
        {
            deletedBackupData = 0;// 削除したデータ数を初期化

            // バックアップフォルダが存在するか確認
            if (string.IsNullOrEmpty(projectSettingValues.ProjectBackupFolderPath))
            {
                return false;
            }

            if (!Directory.Exists(projectSettingValues.ProjectBackupFolderPath))
            {
                return true;
            }

            try
            {
                // 現在のコレクションのUUIDリストを作成
                var currentCollectionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var collection in allCollectionList)
                {
                    currentCollectionIds.Add(collection.CollectionID);
                }

                // バックアップフォルダ内のすべてのサブフォルダを取得
                string[] backupFolders = Directory.GetDirectories(projectSettingValues.ProjectBackupFolderPath);
                // バックアップログフォルダは削除対象外
                backupFolders = backupFolders.Where(folder => System.IO.Path.GetFileName(folder) != BackupLogFolderName).ToArray();

                // 各バックアップフォルダをチェック
                foreach (string backupFolder in backupFolders)
                {
                    string folderName = System.IO.Path.GetFileName(backupFolder);

                    // 現在のコレクションリストに存在しないUUIDの場合は削除
                    if (!currentCollectionIds.Contains(folderName))
                    {
                        try
                        {
                            Directory.Delete(backupFolder, recursive: true);
                            deletedBackupData++;
                        }
                        catch (Exception ex)
                        {
                            // 個別のフォルダ削除に失敗してもログに記録するだけで処理を継続
                            System.Diagnostics.Debug.WriteLine($"Failed to delete backup folder: {backupFolder}, Error: {ex.Message}");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合はfalseを返す
                System.Diagnostics.Debug.WriteLine($"Cleanup failed: {ex.Message}");
                return false;
            }
        }
    }
}
