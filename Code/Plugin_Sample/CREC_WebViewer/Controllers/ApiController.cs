/*
CREC Web Viewer - API Controller
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using Microsoft.AspNetCore.Mvc;
using CREC_WebViewer.Models;
using CREC_WebViewer.Services;

namespace CREC_WebViewer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly CrecDataService _crecDataService;
        private readonly ILogger<CollectionsController> _logger;

        public CollectionsController(CrecDataService crecDataService, ILogger<CollectionsController> logger)
        {
            _crecDataService = crecDataService;
            _logger = logger;
        }

        /// <summary>
        /// コレクション検索
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<SearchResult>> Search([FromQuery] SearchCriteria criteria)
        {
            try
            {
                _logger.LogInformation($"Search request: Text={criteria.SearchText}, Field={criteria.SearchField}, Method={criteria.SearchMethod}");
                var result = await _crecDataService.SearchCollectionsAsync(criteria);
                _logger.LogInformation($"Search returned {result.Collections.Count} collections out of {result.TotalCount} total");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching collections");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 全コレクション取得
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CollectionData>>> GetAll()
        {
            try
            {
                _logger.LogInformation("GetAll collections request");
                var collections = await _crecDataService.GetAllCollectionsAsync();
                _logger.LogInformation($"Returning {collections.Count} collections");
                return Ok(collections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all collections");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// IDによるコレクション取得
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CollectionData>> GetById(string id)
        {
            try
            {
                var collection = await _crecDataService.GetCollectionByIdAsync(id);
                if (collection == null)
                {
                    return NotFound($"Collection with ID '{id}' not found");
                }
                return Ok(collection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting collection with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 利用可能なカテゴリ一覧取得
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            try
            {
                var categories = await _crecDataService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 利用可能なタグ一覧取得
        /// </summary>
        [HttpGet("tags")]
        public async Task<ActionResult<List<string>>> GetTags()
        {
            try
            {
                var tags = await _crecDataService.GetTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// サムネイル画像取得
        /// </summary>
        [HttpGet("thumbnail/{collectionId}")]
        public IActionResult GetThumbnail(string collectionId)
        {
            try
            {
                // セキュリティ：パストラバーサル攻撃を防ぐ
                if (collectionId.Contains("..") || collectionId.Contains("/") || collectionId.Contains("\\"))
                {
                    return BadRequest("Invalid collection ID");
                }

                var dataFolder = Environment.CurrentDirectory;
                var collectionFolder = Path.Combine(dataFolder, collectionId);
                var thumbnailPath = Path.Combine(collectionFolder, "SystemData", "Thumbnail.png");

                if (!System.IO.File.Exists(thumbnailPath))
                {
                    // サムネイルが見つからない場合は404を返す（フロントエンドでデフォルト画像表示）
                    return NotFound($"Thumbnail not found for collection '{collectionId}'");
                }

                // ファイルが指定されたコレクションフォルダ内にあることを確認
                if (!thumbnailPath.StartsWith(collectionFolder))
                {
                    return BadRequest("Access denied");
                }

                var fileBytes = System.IO.File.ReadAllBytes(thumbnailPath);
                return File(fileBytes, "image/png", "Thumbnail.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting thumbnail for collection {collectionId}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// ファイル取得（画像やその他ファイル）
        /// </summary>
        [HttpGet("{collectionId}/{fileName}")]
        public IActionResult GetFile(string collectionId, string fileName)
        {
            try
            {
                // セキュリティ：パストラバーサル攻撃を防ぐ
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest("Invalid file name");
                }

                var dataFolder = Environment.CurrentDirectory;
                var collectionFolder = Path.Combine(dataFolder, collectionId);
                var filePath = Path.Combine(collectionFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"File '{fileName}' not found in collection '{collectionId}'");
                }

                // ファイルが指定されたコレクションフォルダ内にあることを確認
                if (!filePath.StartsWith(collectionFolder))
                {
                    return BadRequest("Access denied");
                }

                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = GetContentType(fileExtension);

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting file {fileName} from collection {collectionId}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// ファイル拡張子からContent-Typeを取得
        /// </summary>
        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" or ".tif" => "image/tiff",
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
    }
}