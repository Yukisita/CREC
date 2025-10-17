/*
CREC Web Viewer - Main Program
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using CREC_WebViewer.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Parse command line arguments for .crec project file
ProjectSettings? projectSettings = null;
if (args.Length > 0 && args[0].EndsWith(".crec", StringComparison.OrdinalIgnoreCase))
{
    var crecFilePath = args[0];
    Console.WriteLine($"Loading project settings from: {crecFilePath}");
    projectSettings = ParseCrecFile(crecFilePath);
    if (projectSettings != null)
    {
        Console.WriteLine($"Project name: {projectSettings.ProjectName}");
        Console.WriteLine($"Project data folder: {projectSettings.ProjectDataPath}");
        Console.WriteLine($"Custom field labels loaded from .crec file");
    }
    else
    {
        Console.WriteLine("Warning: Failed to parse .crec file or extract project settings");
    }
}

// Store project settings in configuration
if (projectSettings != null)
{
    builder.Configuration["ProjectDataPath"] = projectSettings.ProjectDataPath;
    builder.Configuration["ProjectName"] = projectSettings.ProjectName;
    builder.Configuration["UUIDLabel"] = projectSettings.UUIDLabel;
    builder.Configuration["ManagementCodeLabel"] = projectSettings.ManagementCodeLabel;
    builder.Configuration["CategoryLabel"] = projectSettings.CategoryLabel;
    builder.Configuration["FirstTagLabel"] = projectSettings.FirstTagLabel;
    builder.Configuration["SecondTagLabel"] = projectSettings.SecondTagLabel;
    builder.Configuration["ThirdTagLabel"] = projectSettings.ThirdTagLabel;
}

// Configure web root to be relative to executable location, not current directory
var executablePath = AppContext.BaseDirectory;
var webRootPath = Path.Combine(executablePath, "wwwroot");
builder.Environment.WebRootPath = webRootPath;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CREC Web Viewer API", Version = "v1" });
});

// Add CREC data service
builder.Services.AddSingleton<CrecDataService>();

// Add CORS for browser access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Log the paths for debugging
Console.WriteLine($"Executable directory: {executablePath}");
Console.WriteLine($"Web root path: {webRootPath}");
Console.WriteLine($"wwwroot exists: {Directory.Exists(webRootPath)}");

// Configure URLs to listen on all interfaces
builder.WebHost.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CREC Web Viewer API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();

// Configure static files middleware
if (Directory.Exists(webRootPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(webRootPath)
    });
}

app.UseRouting();
app.MapControllers();

// Serve the main web interface - map fallback to serve index.html for SPA
app.MapFallback(async context =>
{
    var indexPath = Path.Combine(webRootPath, "index.html");
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("index.html not found");
    }
});

// Display startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("CREC Web Viewer starting...");
if (projectSettings != null)
{
    logger.LogInformation($"Project: {projectSettings.ProjectName}");
    logger.LogInformation($"Data folder (from .crec file): {projectSettings.ProjectDataPath}");
}
else
{
    logger.LogInformation($"Data folder (current directory): {Environment.CurrentDirectory}");
}
logger.LogInformation($"Web root path: {webRootPath}");
logger.LogInformation($"wwwroot exists: {Directory.Exists(webRootPath)}");
logger.LogInformation("Web interface will be available at:");
logger.LogInformation("  - http://localhost:5000");
logger.LogInformation("  - http://[your-ip]:5000");
logger.LogInformation("API documentation available at: http://localhost:5000/swagger");

// Helper method to parse .crec file and extract project settings
static ProjectSettings? ParseCrecFile(string crecFilePath)
{
    try
    {
        if (!File.Exists(crecFilePath))
        {
            Console.WriteLine($"Error: .crec file not found: {crecFilePath}");
            return null;
        }

        var settings = new ProjectSettings();
        var lines = File.ReadAllLines(crecFilePath, System.Text.Encoding.GetEncoding("UTF-8"));
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var cols = line.Split(',');
            if (cols.Length < 2) continue;
            
            switch (cols[0])
            {
                case "projectname":
                    settings.ProjectName = cols[1];
                    break;
                case "projectlocation":
                    settings.ProjectDataPath = cols[1];
                    break;
                case "UUIDName":
                    if (cols[1].Length > 0)
                        settings.UUIDLabel = cols[1];
                    break;
                case "ManagementCodeName":
                    if (cols[1].Length > 0)
                        settings.ManagementCodeLabel = cols[1];
                    break;
                case "CategoryName":
                    if (cols[1].Length > 0)
                        settings.CategoryLabel = cols[1];
                    break;
                case "Tag1Name":
                    if (cols[1].Length > 0)
                        settings.FirstTagLabel = cols[1];
                    break;
                case "Tag2Name":
                    if (cols[1].Length > 0)
                        settings.SecondTagLabel = cols[1];
                    break;
                case "Tag3Name":
                    if (cols[1].Length > 0)
                        settings.ThirdTagLabel = cols[1];
                    break;
            }
        }
        
        // Validate that we at least have a data path
        if (string.IsNullOrEmpty(settings.ProjectDataPath))
        {
            Console.WriteLine("Error: 'projectlocation' not found in .crec file");
            return null;
        }
        
        if (!Directory.Exists(settings.ProjectDataPath))
        {
            Console.WriteLine($"Warning: Project data folder does not exist: {settings.ProjectDataPath}");
            // Return anyway so user can see the configured path
        }
        
        return settings;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing .crec file: {ex.Message}");
        return null;
    }
}

app.Run();

// Helper class to hold project settings
public class ProjectSettings
{
    public string ProjectName { get; set; } = "CREC Project";
    public string ProjectDataPath { get; set; } = "";
    public string UUIDLabel { get; set; } = "UUID";
    public string ManagementCodeLabel { get; set; } = "MC";
    public string CategoryLabel { get; set; } = "Category";
    public string FirstTagLabel { get; set; } = "Tag 1";
    public string SecondTagLabel { get; set; } = "Tag 2";
    public string ThirdTagLabel { get; set; } = "Tag 3";
}
