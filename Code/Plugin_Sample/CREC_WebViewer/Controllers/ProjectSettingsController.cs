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
            projectName = _configuration["ProjectName"] ?? "CREC Project",
            uuidName = _configuration["UUIDLabel"] ?? "ID",
            managementCodeName = _configuration["ManagementCodeLabel"] ?? "MC",
            categoryName = _configuration["CategoryLabel"] ?? "Category",
            tag1Name = _configuration["FirstTagLabel"] ?? "Tag 1",
            tag2Name = _configuration["SecondTagLabel"] ?? "Tag 2",
            tag3Name = _configuration["ThirdTagLabel"] ?? "Tag 3"
        };

        _logger.LogInformation("Returning project settings: ProjectName={ProjectName}, UUIDLabel={UUIDLabel}, MCLabel={MCLabel}, CategoryLabel={CategoryLabel}, Tag1={Tag1}, Tag2={Tag2}, Tag3={Tag3}",
            settings.projectName, settings.uuidName, settings.managementCodeName, settings.categoryName, settings.tag1Name, settings.tag2Name, settings.tag3Name);
        return Ok(settings);
    }
}
