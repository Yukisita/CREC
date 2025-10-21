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
    [Route("api/files")]
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
                // Get data path from configuration or use current directory
                var dataPath = _configuration["ProjectDataPath"] ?? Directory.GetCurrentDirectory();
                
                // Build path to pictures folder: dataPath\pictures\collectionId\fileName
                var filePath = Path.Combine(dataPath, "pictures", collectionId, fileName);

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

                // Use FileStream to return the file directly without loading into memory
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(fileStream, contentType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error serving file {collectionId}/{fileName}");
                return StatusCode(500, "Error retrieving file");
            }
        }
    }
}
