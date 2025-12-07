/*
InventoryDataClass
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace CREC
{
    /// <summary>
    /// 在庫操作の種類
    /// </summary>
    public enum InventoryOperationType
    {
        EntryOperation,   // 入庫
        ExitOperation,     // 出庫
        Stocktaking        // 棚卸
    }

    /// <summary>
    /// 在庫メタデータ
    /// </summary>
    [DataContract]
    public class InventoryMetaData
    {
        [DataMember(Name = "collectionId")]
        public string CollectionId { get; set; }

        public InventoryMetaData()
        {
            CollectionId = string.Empty;
        }

        public InventoryMetaData(string collectionId)
        {
            CollectionId = collectionId ?? string.Empty;
        }
    }

    /// <summary>
    /// 在庫管理設定
    /// </summary>
    [DataContract]
    public class InventoryOperationSetting
    {
        [DataMember(Name = "safetyStock")]
        public int? SafetyStock { get; set; }

        [DataMember(Name = "reorderPoint")]
        public int? ReorderPoint { get; set; }

        [DataMember(Name = "maximumLevel")]
        public int? MaximumLevel { get; set; }

        public InventoryOperationSetting()
        {
            SafetyStock = null;
            ReorderPoint = null;
            MaximumLevel = null;
        }

        public InventoryOperationSetting(int? safetyStock, int? reorderPoint, int? maximumLevel)
        {
            SafetyStock = safetyStock;
            ReorderPoint = reorderPoint;
            MaximumLevel = maximumLevel;
        }
    }

    /// <summary>
    /// 在庫操作レコード (JSON用)
    /// </summary>
    [DataContract]
    public class InventoryOperationRecord
    {
        [DataMember(Name = "dateTime")]
        public string DateTime { get; set; }

        [DataMember(Name = "operationType")]
        public InventoryOperationType OperationType { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "note")]
        public string Note { get; set; }

        public InventoryOperationRecord()
        {
            DateTime = string.Empty;
            OperationType = InventoryOperationType.Stocktaking;
            Quantity = 0;
            Note = string.Empty;
        }

        public InventoryOperationRecord(string dateTime, InventoryOperationType operationType, int quantity, string note)
        {
            DateTime = dateTime ?? string.Empty;
            OperationType = operationType;
            Quantity = quantity;
            Note = note ?? string.Empty;
        }
    }

    /// <summary>
    /// 在庫管理データ全体 (JSON用)
    /// </summary>
    [DataContract]
    public class InventoryData
    {
        [DataMember(Name = "metaData")]
        public InventoryMetaData MetaData { get; set; }

        [DataMember(Name = "setting")]
        public InventoryOperationSetting Setting { get; set; }

        [DataMember(Name = "operations")]
        public List<InventoryOperationRecord> Operations { get; set; }

        public InventoryData()
        {
            MetaData = new InventoryMetaData();
            Setting = new InventoryOperationSetting();
            Operations = new List<InventoryOperationRecord>();
        }

        /// <summary>
        /// 現在の在庫数を計算
        /// </summary>
        public int CalculateCurrentInventory()
        {
            int inventory = 0;
            if (Operations != null)
            {
                foreach (var op in Operations)
                {
                    inventory += op.Quantity;
                }
            }
            return inventory;
        }

        /// <summary>
        /// 在庫状況を取得
        /// </summary>
        public InventoryStatus GetInventoryStatus()
        {
            int count = CalculateCurrentInventory();

            // 在庫管理の設定値読み込み
            int? safetyStock = Setting.SafetyStock;
            int? reorderPoint = Setting.ReorderPoint;
            int? maximumLevel = Setting.MaximumLevel;

            if (!safetyStock.HasValue && !reorderPoint.HasValue && !maximumLevel.HasValue)
            {
                return InventoryStatus.NotSet;
            }

            // 在庫切れ（0以下）
            if (count <= 0)
            {
                return InventoryStatus.StockOut;
            }
            // 在庫不足（安全在庫数未満）
            else if (safetyStock.HasValue && count < safetyStock.Value)
            {
                return InventoryStatus.UnderStocked;
            }
            // 在庫不足（安全在庫数以上、発注点以下）
            else if (safetyStock.HasValue && reorderPoint.HasValue &&
                     safetyStock.Value <= count && count <= reorderPoint.Value)
            {
                return InventoryStatus.UnderStocked;
            }
            // 在庫適正（発注点以上、最大在庫数以下）
            else if (reorderPoint.HasValue && maximumLevel.HasValue &&
                     reorderPoint.Value <= count && count <= maximumLevel.Value)
            {
                return InventoryStatus.Appropriate;
            }
            // 在庫過剰（最大在庫数超過）
            else if (maximumLevel.HasValue && maximumLevel.Value < count)
            {
                return InventoryStatus.OverStocked;
            }

            return InventoryStatus.Appropriate;
        }
    }

    /// <summary>
    /// 在庫データ入出力クラス
    /// </summary>
    public static class InventoryDataIO
    {
        // ファイル名定義
        public const string JsonFileName = "inventory.json";
        public const string LegacyCsvFileName = "inventory.inv";

        /// <summary>
        /// 在庫データをJSONファイルとして保存
        /// </summary>
        public static bool SaveInventoryData(string collectionFolderPath, InventoryData data)
        {
            if (string.IsNullOrEmpty(collectionFolderPath) || data == null)
            {
                return false;
            }

            try
            {
                string filePath = Path.Combine(collectionFolderPath, "SystemData", JsonFileName);
                var serializer = new DataContractJsonSerializer(typeof(InventoryData));

                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, data);
                    string json = Encoding.UTF8.GetString(stream.ToArray());

                    // インデント付きで整形
                    json = FormatJson(json);

                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 在庫データを読み込み
        /// </summary>
        /// <param name="collectionFolderPath">コレクションフォルダパス</param>
        /// <param name="deleteLegacyInventoryFile">レガシー在庫ファイル削除フラグ</param>
        /// <returns>在庫データ</returns>
        public static InventoryData LoadInventoryData(string collectionFolderPath, ref bool? deleteLegacyInventoryFile)
        {
            if (string.IsNullOrEmpty(collectionFolderPath))
            {
                return null;
            }

            // JSONファイルを読み込み
            string jsonPath = Path.Combine(collectionFolderPath, "SystemData", JsonFileName);
            if (!File.Exists(jsonPath))
            {
                // レガシーCSVファイル
                if (MigrateLegacyCsvToJson(collectionFolderPath, ref deleteLegacyInventoryFile))
                {
                    return LoadFromJson(jsonPath);
                }
                else
                {
                    return null; // JSONもCSVも存在しない場合はnullを返す
                }
            }

            return LoadFromJson(jsonPath);
        }

        /// <summary>
        /// 在庫データを読み込み（移行なし）
        /// </summary>
        /// <param name="collectionFolderPath">コレクションフォルダパス</param>
        /// <returns>在庫データ</returns>
        public static InventoryData LoadInventoryData(string collectionFolderPath)
        {
            bool? deleteLegacyInventoryFile = false;
            return LoadInventoryData(collectionFolderPath, ref deleteLegacyInventoryFile);
        }

        /// <summary>
        /// 在庫管理ファイルが存在するかチェック (JSON または レガシーCSV)
        /// </summary>
        public static bool InventoryFileExists(string collectionFolderPath)
        {
            if (string.IsNullOrEmpty(collectionFolderPath))
            {
                return false;
            }

            string jsonPath = Path.Combine(collectionFolderPath, "SystemData", JsonFileName);
            string csvPath = Path.Combine(collectionFolderPath, LegacyCsvFileName);

            return File.Exists(jsonPath) || File.Exists(csvPath);
        }

        /// <summary>
        /// JSONファイルから在庫データを読み込み
        /// </summary>
        private static InventoryData LoadFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var serializer = new DataContractJsonSerializer(typeof(InventoryData));

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return (InventoryData)serializer.ReadObject(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// レガシーCSVファイル(.inv)から在庫データを読み込み
        /// </summary>
        private static InventoryData LoadFromLegacyCsv(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                if (lines.Length == 0)
                {
                    return null;
                }

                var data = new InventoryData();

                // 1行目: CollectionID,SafetyStock,ReorderPoint,MaximumLevel
                // CollectionIDは規定値から取得
                data.MetaData.CollectionId = Path.GetFileName(Path.GetDirectoryName(filePath));

                string[] headerCols = lines[0].Split(',');

                // InventoryOperationSetting のインスタンスを作成
                var setting = new InventoryOperationSetting();
                if (headerCols.Length >= 2 && !string.IsNullOrEmpty(headerCols[1]))
                {
                    if (int.TryParse(headerCols[1], out int safetyStock))
                    {
                        setting.SafetyStock = safetyStock;
                    }
                }
                if (headerCols.Length >= 3 && !string.IsNullOrEmpty(headerCols[2]))
                {
                    if (int.TryParse(headerCols[2], out int reorderPoint))
                    {
                        setting.ReorderPoint = reorderPoint;
                    }
                }
                if (headerCols.Length >= 4 && !string.IsNullOrEmpty(headerCols[3]))
                {
                    if (int.TryParse(headerCols[3], out int maxLevel))
                    {
                        setting.MaximumLevel = maxLevel;
                    }
                }
                // 設定を InventoryData の Setting リストに追加
                data.Setting = setting;

                // 2行目以降: DateTime,OperationType,Quantity,Note
                for (int i = lines.Length - 1; i >= 1; i--)// 最新が下になるよう逆順で追加
                {
                    string[] cols = lines[i].Split(',');
                    if (cols.Length >= 3)
                    {
                        string dateTime = cols[0];
                        InventoryOperationType opType = NormalizeOperationType(cols[1]);
                        int quantity = 0;
                        if (int.TryParse(cols[2], out int qty))
                        {
                            quantity = qty;
                        }
                        string note = cols.Length >= 4 ? cols[3] : string.Empty;

                        data.Operations.Add(new InventoryOperationRecord(dateTime, opType, quantity, note));
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// レガシーの日本語操作タイプを英語に正規化、英語での誤字を修正
        /// </summary>
        private static InventoryOperationType NormalizeOperationType(string operationType)
        {
            switch (operationType)
            {
                case "入庫":
                case "EntryOperation":
                case "EntoryOperation": // 誤字の修正
                    return InventoryOperationType.EntryOperation;
                case "出庫":
                case "ExitOperation":
                    return InventoryOperationType.ExitOperation;
                case "棚卸":
                case "Stocktaking":
                    return InventoryOperationType.Stocktaking;
                default:
                    return InventoryOperationType.Stocktaking;
            }
        }

        /// <summary>
        /// レガシーCSVファイルをJSONに移行
        /// </summary>
        /// <param name="collectionFolderPath">コレクションフォルダパス</param>
        /// <param name="deleteLegacyCsv">レガシーCSVファイルを削除するかどうか</param>
        public static bool MigrateLegacyCsvToJson(string collectionFolderPath, ref bool? deleteLegacyInventoryFile)
        {
            if (string.IsNullOrEmpty(collectionFolderPath))
            {
                return false;
            }

            string csvPath = Path.Combine(collectionFolderPath, LegacyCsvFileName);
            string jsonPath = Path.Combine(collectionFolderPath, "SystemData", JsonFileName);

            // JSONファイルが既に存在する場合は移行不要
            if (File.Exists(jsonPath))
            {
                return true;
            }

            // CSVファイルが存在しない場合は移行対象がないため移行処理を停止する
            if (!File.Exists(csvPath))
            {
                return false;
            }

            // deleteLegacyInventoryFile が null の場合は設定値を取得
            if (deleteLegacyInventoryFile == null)
            {
                // メッセージボックスを表示して、設定
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(
                    "在庫管理ファイルを新バージョンに移行します。\n\n" +
                    "旧バージョンの在庫管理ファイルを削除しますか？",
                    "CREC",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);
                deleteLegacyInventoryFile = (result == System.Windows.Forms.DialogResult.Yes);
            }

            try
            {
                // CSVから読み込み
                InventoryData data = LoadFromLegacyCsv(csvPath);
                if (data == null)
                {
                    return false;
                }

                // 日時をUTCかつISO 8601形式、Offsetを明記する形に変換、元データはローカルタイムと仮定
                // 元データの形式はStringの"yyyy/MM/dd HH:mm:ss"
                foreach (var record in data.Operations)
                {
                    if (DateTime.TryParseExact(
                        record.DateTime,
                        "yyyy/MM/dd HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AssumeLocal,
                        out DateTime parsedDateTime))
                    {
                        DateTimeOffset dto = new DateTimeOffset(parsedDateTime.ToUniversalTime());
                        record.DateTime = dto.ToString("o");// ISO 8601形式
                    }
                    else
                    {
                        MessageBox.Show(
                            $"在庫管理ファイルの日時解析に失敗しました。\n" +
                            $"不正な日時データ: {record.DateTime}\n" +
                            $"現在のUTC日時を設定します。",
                            "CREC",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        // 解析に失敗した場合は現在のUTC日時を設定
                        DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
                        record.DateTime = dto.ToString("o");// ISO 8601形式
                    }
                }

                // JSONで保存
                if (!SaveInventoryData(collectionFolderPath, data))
                {
                    return false;
                }

                // レガシーCSVファイルを削除
                if (deleteLegacyInventoryFile == true)
                {
                    File.Delete(csvPath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 新規在庫管理ファイルを作成
        /// </summary>
        public static bool CreateNewInventoryFile(string collectionFolderPath, string collectionId)
        {
            if (string.IsNullOrEmpty(collectionFolderPath) || string.IsNullOrEmpty(collectionId))
            {
                return false;
            }

            var data = new InventoryData();
            data.MetaData.CollectionId = collectionId;
            return SaveInventoryData(collectionFolderPath, data);
        }

        /// <summary>
        /// 在庫操作を追加
        /// </summary>
        /// <param name="collectionFolderPath">コレクションフォルダパス</param>
        /// <param name="record">在庫操作レコード</param>
        public static bool AddInventoryOperation(string collectionFolderPath, InventoryOperationRecord record)
        {
            if (string.IsNullOrEmpty(collectionFolderPath) || record == null)
            {
                return false;
            }

            InventoryData data = LoadInventoryData(collectionFolderPath);
            if (data == null)
            {
                return false;
            }

            // Operations が null の場合に備えて初期化
            if (data.Operations == null)
            {
                data.Operations = new List<InventoryOperationRecord>();
            }

            // 新しい操作をリストの末尾に追加
            data.Operations.Add(record);

            return SaveInventoryData(collectionFolderPath, data);
        }

        /// <summary>
        /// 適正在庫設定を更新
        /// </summary>
        public static bool UpdateProperInventorySettings(string collectionFolderPath, InventoryData data)
        {
            if (string.IsNullOrEmpty(collectionFolderPath) || data == null)
            {
                return false;
            }

            // data.Settingのそれぞれの値が負でないことを確認し、負の場合はnullに設定
            if (data.Setting.SafetyStock.HasValue && data.Setting.SafetyStock < 0)
            {
                data.Setting.SafetyStock = null;
            }
            if (data.Setting.ReorderPoint.HasValue && data.Setting.ReorderPoint < 0)
            {
                data.Setting.ReorderPoint = null;
            }
            if (data.Setting.MaximumLevel.HasValue && data.Setting.MaximumLevel < 0)
            {
                data.Setting.MaximumLevel = null;
            }

            return SaveInventoryData(collectionFolderPath, data);
        }

        /// <summary>
        /// JSONを簡易的に整形 (読みやすさのため)
        /// </summary>
        private static string FormatJson(string json)
        {
            // DataContractJsonSerializerは圧縮されたJSONを出力するため、
            // 読みやすさのために簡単な整形を行う
            var sb = new StringBuilder();
            int indentLevel = 0;
            bool inString = false;
            bool escapeNext = false;

            foreach (char c in json)
            {
                if (escapeNext)
                {
                    sb.Append(c);
                    escapeNext = false;
                    continue;
                }

                if (c == '\\')
                {
                    sb.Append(c);
                    escapeNext = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = !inString;
                    sb.Append(c);
                    continue;
                }

                if (inString)
                {
                    sb.Append(c);
                    continue;
                }

                switch (c)
                {
                    case '{':
                    case '[':
                        sb.Append(c);
                        sb.AppendLine();
                        indentLevel++;
                        sb.Append(new string('\t', indentLevel));
                        break;
                    case '}':
                    case ']':
                        sb.AppendLine();
                        indentLevel--;
                        sb.Append(new string('\t', indentLevel));
                        sb.Append(c);
                        break;
                    case ',':
                        sb.Append(c);
                        sb.AppendLine();
                        sb.Append(new string('\t', indentLevel));
                        break;
                    case ':':
                        sb.Append(c);
                        sb.Append(' ');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
