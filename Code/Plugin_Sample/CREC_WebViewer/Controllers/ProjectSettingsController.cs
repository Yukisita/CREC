/*
CREC Web Viewer - Project Settings Controller
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using Microsoft.AspNetCore.Mvc;

namespace CREC_WebViewer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectSettingsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProjectSettingsController> _logger;

    public ProjectSettingsController(IConfiguration configuration, ILogger<ProjectSettingsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetProjectSettings()
    {
        var settings = new
        {
            ProjectName = _configuration["ProjectName"] ?? "CREC Project",
            UUIDLabel = _configuration["UUIDLabel"] ?? "UUID",
            ManagementCodeLabel = _configuration["ManagementCodeLabel"] ?? "MC",
            CategoryLabel = _configuration["CategoryLabel"] ?? "Category",
            FirstTagLabel = _configuration["FirstTagLabel"] ?? "Tag 1",
            SecondTagLabel = _configuration["SecondTagLabel"] ?? "Tag 2",
            ThirdTagLabel = _configuration["ThirdTagLabel"] ?? "Tag 3"
        };

        _logger.LogInformation("Project settings requested");
        return Ok(settings);
    }
}
