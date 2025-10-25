/*
CREC Web Viewer - Data Reader Service
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using System.Text;
using CREC_WebViewer.Models;

namespace CREC_WebViewer.Services
{
    /// <summary>
    /// CRECデータ読み込みサービス
    /// </summary>
    public class CrecDataService
    {
        private readonly ILogger<CrecDataService> _logger;
        private readonly string _dataFolderPath;
        private readonly List<CollectionData> _collectionsCache = new();
        private DateTime _lastCacheUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

        public CrecDataService(ILogger<CrecDataService> logger, IConfiguration configuration)
        {
            _logger = logger;
            // プラグインとして実行される場合、WorkingDirectoryがデータフォルダに設定される
            // コマンドライン引数で.crecファイルが指定された場合はそこからパスを取得
            _dataFolderPath = configuration["ProjectDataPath"] ?? Environment.CurrentDirectory;
            _logger.LogInformation($"Data folder path: {_dataFolderPath}");
        }

        /// <summary>
        /// 全てのコレクションデータを取得
        /// </summary>
        public async Task<List<CollectionData>> GetAllCollectionsAsync()
        {
            // キャッシュが有効な場合はキャッシュを返す
            if (_collectionsCache.Any() && DateTime.Now - _lastCacheUpdate < _cacheExpiry)
            {
                _logger.LogInformation($"Returning {_collectionsCache.Count} collections from cache");
                return _collectionsCache;
            }

            _collectionsCache.Clear();

            try
            {
                _logger.LogInformation($"Loading collections from data folder: {_dataFolderPath}");
                _logger.LogInformation($"Data folder exists: {Directory.Exists(_dataFolderPath)}");
                
                if (!Directory.Exists(_dataFolderPath))
                {
                    _logger.LogWarning($"Data folder does not exist: {_dataFolderPath}");
                    return _collectionsCache;
                }
                
                // データフォルダ内のサブフォルダを検索
                var directories = Directory.GetDirectories(_dataFolderPath);
                _logger.LogInformation($"Found {directories.Length} subdirectories in data folder");
                
                foreach (var dir in directories)
                {
                    _logger.LogInformation($"  - {Path.GetFileName(dir)}");
                }
                
                // $SystemDataフォルダを除外
                directories = directories.Where(dir => 
                    !Path.GetFileName(dir).Equals("$SystemData", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                
                _logger.LogInformation($"After filtering: {directories.Length} collection folders");
                
                var tasks = directories.Select(async dir => await LoadCollectionFromDirectoryAsync(dir));
                var collections = await Task.WhenAll(tasks);
                
                var validCollections = collections.Where(c => c != null).Cast<CollectionData>().ToList();
                _logger.LogInformation($"Successfully loaded {validCollections.Count} collections");
                
                _collectionsCache.AddRange(validCollections);
                _lastCacheUpdate = DateTime.Now;

                _logger.LogInformation($"Total collections in cache: {_collectionsCache.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading collections");
            }

            return _collectionsCache;
        }

        /// <summary>
        /// 指定されたディレクトリからコレクションデータを読み込み
        /// </summary>
        private async Task<CollectionData?> LoadCollectionFromDirectoryAsync(string directoryPath)
        {
            try
            {
                _logger.LogDebug($"Loading collection from: {directoryPath}");
                
                // index.txtまたはIndex.txtを探す（大文字小文字を区別しない）
                var indexFilePath = Path.Combine(directoryPath, "index.txt");
                if (!File.Exists(indexFilePath))
                {
                    indexFilePath = Path.Combine(directoryPath, "Index.txt");
                }
                
                if (!File.Exists(indexFilePath))
                {
                    // index.txtが存在しない場合、フォルダ名をIDとして基本的なデータを作成
                    _logger.LogWarning($"No index file found in {directoryPath}, creating basic collection data");
                    return CreateBasicCollectionData(directoryPath);
                }
                
                _logger.LogDebug($"Found index file: {indexFilePath}");

                var collection = new CollectionData
                {
                    CollectionFolderPath = directoryPath,
                    CollectionID = Path.GetFileName(directoryPath)
                };

                // index.txtを読み込み
                var lines = await File.ReadAllLinesAsync(indexFilePath, Encoding.GetEncoding("UTF-8"));
                
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    
                    // カンマで分割して最初の要素をキーとして扱う
                    var commaIndex = line.IndexOf(',');
                    if (commaIndex < 0) continue;  // カンマがない行はスキップ
                    
                    var key = line.Substring(0, commaIndex);
                    var value = commaIndex + 1 < line.Length ? line.Substring(commaIndex + 1) : string.Empty;
                    
                    // CRECのLoadCollectionIndexDataに準拠した処理
                    switch (key)
                    {
                        case "名称": // Collection Name
                            collection.CollectionName = value;
                            break;
                        case "ID":
                            collection.CollectionID = value;
                            break;
                        case "MC":
                            collection.CollectionMC = value;
                            break;
                        case "登録日": // Registration Date
                            collection.CollectionRegistrationDate = value;
                            break;
                        case "カテゴリ": // Category
                            collection.CollectionCategory = value;
                            break;
                        case "タグ1": // Tag1
                            collection.CollectionTag1 = value;
                            break;
                        case "タグ2": // Tag2
                            collection.CollectionTag2 = value;
                            break;
                        case "タグ3": // Tag3
                            collection.CollectionTag3 = value;
                            break;
                        case "場所1(Real)": // Real Location
                            collection.CollectionRealLocation = value;
                            break;
                    }
                }

                // 在庫情報を読み込み
                LoadInventoryData(collection, directoryPath);

                // 画像ファイルとその他のファイルを検索
                LoadFileList(collection, directoryPath);
                
                _logger.LogInformation($"Successfully loaded collection: ID={collection.CollectionID}, Name={collection.CollectionName}");

                return collection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading collection from {directoryPath}");
                return null;
            }
        }

        /// <summary>
        /// 基本的なコレクションデータを作成（index.txtが存在しない場合）
        /// </summary>
        private CollectionData CreateBasicCollectionData(string directoryPath)
        {
            var collection = new CollectionData
            {
                CollectionFolderPath = directoryPath,
                CollectionID = Path.GetFileName(directoryPath),
                CollectionName = " - ",
                CollectionMC = " - ",
                CollectionRegistrationDate = " - ",
                CollectionCategory = " - ",
                CollectionTag1 = " - ",
                CollectionTag2 = " - ",
                CollectionTag3 = " - ",
                CollectionRealLocation = " - ",
                CollectionInventoryStatus = InventoryStatus.NotSet
            };

            LoadFileList(collection, directoryPath);
            return collection;
        }

        /// <summary>
        /// 在庫情報を読み込み（CREC本体のLoadCollectionInventoryDataに準拠）
        /// </summary>
        private void LoadInventoryData(CollectionData collection, string directoryPath)
        {
            try
            {
                var inventoryFilePath = Path.Combine(directoryPath, "inventory.inv");
                if (!File.Exists(inventoryFilePath))
                {
                    collection.CollectionInventoryStatus = InventoryStatus.NotSet;
                    collection.CollectionCurrentInventory = null;
                    return;
                }

                var lines = File.ReadAllLines(inventoryFilePath, Encoding.GetEncoding("UTF-8"));
                if (lines.Length == 0) return;

                int? safetyStock = null;
                int? orderPoint = null;
                int? maxStock = null;
                int totalInventory = 0;

                bool firstLine = true;
                foreach (var line in lines)
                {
                    var cols = line.Split(',');
                    if (cols.Length < 4) continue;

                    if (firstLine)
                    {
                        // 最初の行から安全在庫、発注点、最大在庫レベルを読み取る
                        if (!string.IsNullOrEmpty(cols[1]) && int.TryParse(cols[1], out int ss))
                            safetyStock = ss;
                        if (!string.IsNullOrEmpty(cols[2]) && int.TryParse(cols[2], out int op))
                            orderPoint = op;
                        if (!string.IsNullOrEmpty(cols[3]) && int.TryParse(cols[3], out int ms))
                            maxStock = ms;
                        firstLine = false;
                    }
                    else
                    {
                        // 2行目以降から在庫数を合計
                        if (cols.Length >= 3 && int.TryParse(cols[2], out int count))
                        {
                            totalInventory += count;
                        }
                    }
                }

                collection.CollectionCurrentInventory = totalInventory;
                collection.CollectionSafetyStock = safetyStock;
                collection.CollectionOrderPoint = orderPoint;
                collection.CollectionMaxStock = maxStock;

                // 在庫状況を判定（CREC本体のロジックに準拠）
                if (totalInventory == 0)
                {
                    collection.CollectionInventoryStatus = InventoryStatus.StockOut;
                }
                else if (safetyStock.HasValue && totalInventory < safetyStock.Value)
                {
                    collection.CollectionInventoryStatus = InventoryStatus.UnderStocked;
                }
                else if (maxStock.HasValue && totalInventory > maxStock.Value)
                {
                    collection.CollectionInventoryStatus = InventoryStatus.OverStocked;
                }
                else
                {
                    collection.CollectionInventoryStatus = InventoryStatus.Appropriate;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error loading inventory data from {directoryPath}");
                collection.CollectionInventoryStatus = InventoryStatus.NotSet;
                collection.CollectionCurrentInventory = null;
            }
        }

        /// <summary>
        /// ディレクトリ内のファイルリストを読み込み
        /// </summary>
        private void LoadFileList(CollectionData collection, string directoryPath)
        {
            try
            {
                // まずSystemData/Thumbnail.pngをチェック（優先）
                var systemDataThumbnail = Path.Combine(directoryPath, "SystemData", "Thumbnail.png");
                if (File.Exists(systemDataThumbnail))
                {
                    collection.ThumbnailPath = "SystemData/Thumbnail.png";
                    _logger.LogDebug($"Found thumbnail: {systemDataThumbnail}");
                }

                var files = Directory.GetFiles(directoryPath);
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

                // picturesフォルダから画像を読み込む
                // CREC構造: {dataPath}\{collectionId}\pictures\
                var collectionId = collection.CollectionID;
                var picturesPath = Path.Combine(directoryPath, "pictures");
                if (Directory.Exists(picturesPath))
                {
                    _logger.LogInformation($"Loading images from pictures folder: {picturesPath}");
                    var pictureFiles = Directory.GetFiles(picturesPath);

                    foreach (var file in pictureFiles)
                    {
                        var fileName = Path.GetFileName(file);
                        var extension = Path.GetExtension(file).ToLowerInvariant();

                        if (imageExtensions.Contains(extension))
                        {
                            // picturesフォルダの画像を追加（重複チェック）
                            if (!collection.ImageFiles.Contains(fileName))
                            {
                                collection.ImageFiles.Add(fileName);
                                _logger.LogDebug($"Added image from pictures folder: {fileName}");
                            }
                        }
                    }
                }

                // dataフォルダからデータファイルを読み込む
                // CREC構造: {dataPath}\{collectionId}\data\
                var dataPath = Path.Combine(directoryPath, "data");
                if (Directory.Exists(dataPath))
                {
                    _logger.LogInformation($"Loading data files from data folder: {dataPath}");
                    var dataFiles = Directory.GetFiles(dataPath);

                    foreach (var file in dataFiles)
                    {
                        var fileName = Path.GetFileName(file);
                        var extension = Path.GetExtension(file).ToLowerInvariant();

                        if (!collection.OtherFiles.Contains(fileName))
                        {
                            collection.OtherFiles.Add(fileName);
                            _logger.LogDebug($"Added data file from data folder: {fileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading file list from {directoryPath}");
            }
        }

        /// <summary>
        /// 検索条件に基づいてコレクションを検索
        /// </summary>
        public async Task<SearchResult> SearchCollectionsAsync(SearchCriteria criteria)
        {
            var allCollections = await GetAllCollectionsAsync();
            _logger.LogInformation($"SearchCollectionsAsync: Total collections loaded: {allCollections.Count}");
            
            var filteredCollections = allCollections.AsQueryable();

            // テキスト検索
            if (!string.IsNullOrWhiteSpace(criteria.SearchText))
            {
                _logger.LogInformation($"Filtering by search text: '{criteria.SearchText}', Field: {criteria.SearchField}, Method: {criteria.SearchMethod}");
                filteredCollections = filteredCollections.Where(c => 
                    MatchesSearchCriteria(c, criteria.SearchText, criteria.SearchField, criteria.SearchMethod));
                _logger.LogInformation($"After text search: {filteredCollections.Count()} collections match");
            }
            else
            {
                _logger.LogInformation("No search text provided, showing all collections");
            }

            // 在庫状況フィルタ
            if (criteria.InventoryStatus.HasValue)
            {
                _logger.LogInformation($"Filtering by inventory status: {criteria.InventoryStatus.Value}");
                filteredCollections = filteredCollections.Where(c => 
                    c.CollectionInventoryStatus == criteria.InventoryStatus.Value);
                _logger.LogInformation($"After inventory filter: {filteredCollections.Count()} collections match");
            }

            var totalCount = filteredCollections.Count();
            _logger.LogInformation($"Total filtered collections: {totalCount}");
            
            var pagedCollections = filteredCollections
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();

            _logger.LogInformation($"Returning {pagedCollections.Count} collections for page {criteria.Page}");

            return new SearchResult
            {
                Collections = pagedCollections,
                TotalCount = totalCount,
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };
        }

        /// <summary>
        /// コレクションが検索条件にマッチするかをチェック
        /// </summary>
        private bool MatchesSearchCriteria(CollectionData collection, string searchText, SearchField searchField, SearchMethod searchMethod)
        {
            // 検索対象フィールドの値を取得
            var fieldsToSearch = new List<string>();
            
            switch (searchField)
            {
                case SearchField.All:
                    fieldsToSearch.Add(collection.CollectionID);
                    fieldsToSearch.Add(collection.CollectionName);
                    fieldsToSearch.Add(collection.CollectionMC);
                    fieldsToSearch.Add(collection.CollectionCategory);
                    fieldsToSearch.Add(collection.CollectionTag1);
                    fieldsToSearch.Add(collection.CollectionTag2);
                    fieldsToSearch.Add(collection.CollectionTag3);
                    fieldsToSearch.Add(collection.CollectionRealLocation);
                    if (collection.Comment != null) fieldsToSearch.Add(collection.Comment);
                    break;
                case SearchField.ID:
                    fieldsToSearch.Add(collection.CollectionID);
                    break;
                case SearchField.Name:
                    fieldsToSearch.Add(collection.CollectionName);
                    break;
                case SearchField.ManagementCode:
                    fieldsToSearch.Add(collection.CollectionMC);
                    break;
                case SearchField.Category:
                    fieldsToSearch.Add(collection.CollectionCategory);
                    break;
                case SearchField.Tag:
                    fieldsToSearch.Add(collection.CollectionTag1);
                    fieldsToSearch.Add(collection.CollectionTag2);
                    fieldsToSearch.Add(collection.CollectionTag3);
                    break;
                case SearchField.Tag1:
                    fieldsToSearch.Add(collection.CollectionTag1);
                    break;
                case SearchField.Tag2:
                    fieldsToSearch.Add(collection.CollectionTag2);
                    break;
                case SearchField.Tag3:
                    fieldsToSearch.Add(collection.CollectionTag3);
                    break;
                case SearchField.Location:
                    fieldsToSearch.Add(collection.CollectionRealLocation);
                    break;
                case SearchField.Comment:
                    if (collection.Comment != null) fieldsToSearch.Add(collection.Comment);
                    break;
            }

            // 検索方式に応じてマッチングを行う
            foreach (var fieldValue in fieldsToSearch)
            {
                if (string.IsNullOrEmpty(fieldValue)) continue;
                
                bool matches = searchMethod switch
                {
                    SearchMethod.Prefix => fieldValue.StartsWith(searchText, StringComparison.OrdinalIgnoreCase),
                    SearchMethod.Suffix => fieldValue.EndsWith(searchText, StringComparison.OrdinalIgnoreCase),
                    SearchMethod.Exact => fieldValue.Equals(searchText, StringComparison.OrdinalIgnoreCase),
                    SearchMethod.Partial => fieldValue.Contains(searchText, StringComparison.OrdinalIgnoreCase),
                    _ => false
                };

                if (matches) return true;
            }

            return false;
        }

        /// <summary>
        /// IDによるコレクション取得
        /// </summary>
        public async Task<CollectionData?> GetCollectionByIdAsync(string id)
        {
            var collections = await GetAllCollectionsAsync();
            return collections.FirstOrDefault(c => c.CollectionID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 利用可能なカテゴリ一覧を取得
        /// </summary>
        public async Task<List<string>> GetCategoriesAsync()
        {
            var collections = await GetAllCollectionsAsync();
            return collections
                .Select(c => c.CollectionCategory)
                .Where(cat => !string.IsNullOrWhiteSpace(cat) && cat != " - ")
                .Distinct()
                .OrderBy(cat => cat)
                .ToList();
        }

        /// <summary>
        /// 利用可能なタグ一覧を取得
        /// </summary>
        public async Task<List<string>> GetTagsAsync()
        {
            var collections = await GetAllCollectionsAsync();
            var tags = new List<string>();
            
            tags.AddRange(collections.Select(c => c.CollectionTag1));
            tags.AddRange(collections.Select(c => c.CollectionTag2));
            tags.AddRange(collections.Select(c => c.CollectionTag3));
            
            return tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag) && tag != " - ")
                .Distinct()
                .OrderBy(tag => tag)
                .ToList();
        }
    }
}