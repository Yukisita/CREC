/*
CREC Web Viewer - File Controller
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using Microsoft.AspNetCore.Mvc;

namespace CREC_WebViewer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileController> _logger;

        public FileController(IConfiguration configuration, ILogger<FileController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("{collectionId}/{fileName}")]
        public IActionResult GetFile(string collectionId, string fileName)
        {
            try
            {
                // Security: Validate collection ID (alphanumeric, hyphens, underscores only)
                if (string.IsNullOrWhiteSpace(collectionId) || 
                    !System.Text.RegularExpressions.Regex.IsMatch(collectionId, @"^[a-zA-Z0-9_-]+$") ||
                    collectionId.Length > 255)
                {
                    _logger.LogWarning($"Invalid collection ID: {collectionId}");
                    return BadRequest("Invalid collection ID");
                }

                // Security: Validate file name (no path traversal characters)
                if (string.IsNullOrWhiteSpace(fileName) || 
                    fileName.Contains("..") || 
                    fileName.Contains("/") || 
                    fileName.Contains("\\") ||
                    fileName.Length > 255)
                {
                    _logger.LogWarning($"Invalid file name: {fileName}");
                    return BadRequest("Invalid file name");
                }

                // Get data path from configuration or use current directory
                var dataPath = _configuration["ProjectDataPath"] ?? Directory.GetCurrentDirectory();
                
                // Build path to pictures folder: dataPath\collectionId\pictures\fileName
                var filePath = Path.Combine(dataPath, collectionId, "pictures", fileName);

                // Security: Ensure the resolved path stays within the pictures directory
                var fullPath = Path.GetFullPath(filePath);
                var allowedPath = Path.GetFullPath(Path.Combine(dataPath, collectionId, "pictures"));
                
                if (!fullPath.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Path traversal attempt detected: {fullPath}");
                    return BadRequest("Invalid file path");
                }

                _logger.LogInformation($"Attempting to serve file: {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found: {filePath}");
                    return NotFound();
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    ".svg" => "image/svg+xml",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation($"Serving file with content type: {contentType}");

                // Security headers
                Response.Headers["Access-Control-Allow-Origin"] = "*";
                Response.Headers["Cache-Control"] = "public, max-age=3600";
                Response.Headers["X-Content-Type-Options"] = "nosniff";
                
                // Images are view-only (inline), not downloadable
                return PhysicalFile(fullPath, contentType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error serving file {collectionId}/{fileName}");
                return StatusCode(500, "Error retrieving file");
            }
        }

        [HttpGet("data/{collectionId}/{fileName}")]
        public IActionResult GetDataFile(string collectionId, string fileName)
        {
            try
            {
                // Get data path from configuration or use current directory
                var dataPath = _configuration["ProjectDataPath"] ?? Directory.GetCurrentDirectory();
                
                // Build path to data folder: dataPath\collectionId\data\fileName
                var filePath = Path.Combine(dataPath, collectionId, "data", fileName);

                _logger.LogInformation($"Attempting to serve data file: {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"Data file not found: {filePath}");
                    return NotFound();
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".txt" => "text/plain",
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".csv" => "text/csv",
                    ".xml" => "application/xml",
                    ".json" => "application/json",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation($"Serving data file with content type: {contentType}");

                // Add CORS and cache headers
                Response.Headers["Access-Control-Allow-Origin"] = "*";
                Response.Headers["Cache-Control"] = "public, max-age=3600";
                
                // Use PhysicalFile for direct file serving
                return PhysicalFile(filePath, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error serving data file {collectionId}/{fileName}");
                return StatusCode(500, "Error retrieving data file");
            }
        }
    }
}
