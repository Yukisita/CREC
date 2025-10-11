/*
CREC Web Viewer - Collection Data Model
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

namespace CREC_WebViewer.Models
{
    /// <summary>
    /// 在庫状況の種類
    /// </summary>
    public enum InventoryStatus
    {
        StockOut,        // 在庫切れ
        UnderStocked,    // 在庫不足
        Appropriate,     // 在庫適正
        OverStocked,     // 在庫過剰
        NotSet           // 未設定
    }

    /// <summary>
    /// コレクションデータクラス - CRECのCollectionDataValuesClassに対応
    /// </summary>
    public class CollectionData
    {
        /// <summary>
        /// コレクションフォルダパス
        /// </summary>
        public string CollectionFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// コレクション名
        /// </summary>
        public string CollectionName { get; set; } = string.Empty;

        /// <summary>
        /// コレクションID
        /// </summary>
        public string CollectionID { get; set; } = string.Empty;

        /// <summary>
        /// 管理コード
        /// </summary>
        public string CollectionMC { get; set; } = string.Empty;

        /// <summary>
        /// 登録日
        /// </summary>
        public string CollectionRegistrationDate { get; set; } = string.Empty;

        /// <summary>
        /// カテゴリ
        /// </summary>
        public string CollectionCategory { get; set; } = string.Empty;

        /// <summary>
        /// タグ1
        /// </summary>
        public string CollectionTag1 { get; set; } = string.Empty;

        /// <summary>
        /// タグ2
        /// </summary>
        public string CollectionTag2 { get; set; } = string.Empty;

        /// <summary>
        /// タグ3
        /// </summary>
        public string CollectionTag3 { get; set; } = string.Empty;

        /// <summary>
        /// 場所(Real)
        /// </summary>
        public string CollectionRealLocation { get; set; } = string.Empty;

        /// <summary>
        /// 現在の在庫数
        /// </summary>
        public int? CollectionCurrentInventory { get; set; } = null;

        /// <summary>
        /// 在庫状況
        /// </summary>
        public InventoryStatus CollectionInventoryStatus { get; set; } = InventoryStatus.NotSet;

        /// <summary>
        /// サムネイル画像パス（相対パス）
        /// </summary>
        public string? ThumbnailPath { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 画像ファイルリスト
        /// </summary>
        public List<string> ImageFiles { get; set; } = new List<string>();

        /// <summary>
        /// その他ファイルリスト
        /// </summary>
        public List<string> OtherFiles { get; set; } = new List<string>();
    }

    /// <summary>
    /// 検索条件クラス
    /// </summary>
    public class SearchCriteria
    {
        public string? SearchText { get; set; }
        public string? Category { get; set; }
        public string? Tag1 { get; set; }
        public string? Tag2 { get; set; }
        public string? Tag3 { get; set; }
        public InventoryStatus? InventoryStatus { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// 検索結果クラス
    /// </summary>
    public class SearchResult
    {
        public List<CollectionData> Collections { get; set; } = new List<CollectionData>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}