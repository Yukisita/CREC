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
        Console.WriteLine($"Custom field labels loaded from .crec file:");
        Console.WriteLine($"  Name Label: {projectSettings.CollectionNameLabel}");
        Console.WriteLine($"  UUID Label: {projectSettings.UUIDLabel}");
        Console.WriteLine($"  MC Label: {projectSettings.ManagementCodeLabel}");
        Console.WriteLine($"  Category Label: {projectSettings.CategoryLabel}");
        Console.WriteLine($"  Tag 1 Label: {projectSettings.FirstTagLabel}");
        Console.WriteLine($"  Tag 2 Label: {projectSettings.SecondTagLabel}");
        Console.WriteLine($"  Tag 3 Label: {projectSettings.ThirdTagLabel}");
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
        
        Console.WriteLine($"Parsing .crec file with {lines.Length} lines...");
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var cols = line.Split(',');
            if (cols.Length < 2) continue;
            
            // Extract the value (second column, index 1)
            var key = cols[0].Trim();
            var value = cols[1].Trim();
            
            switch (key)
            {
                case "projectname":
                    settings.ProjectName = value;
                    Console.WriteLine($"  - Found projectname: {value}");
                    break;
                case "projectlocation":
                    settings.ProjectDataPath = value;
                    Console.WriteLine($"  - Found projectlocation: {value}");
                    break;
                case "ShowObjectNameLabel":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.CollectionNameLabel = value;
                        Console.WriteLine($"  - Found ShowObjectNameLabel: {value}");
                    }
                    break;
                case "ShowIDLabel":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.UUIDLabel = value;
                        Console.WriteLine($"  - Found ShowIDLabel: {value}");
                    }
                    break;
                case "ShowMCLabel":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.ManagementCodeLabel = value;
                        Console.WriteLine($"  - Found ShowMCLabel: {value}");
                    }
                    break;
                case "ShowCategoryLabel":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.CategoryLabel = value;
                        Console.WriteLine($"  - Found ShowCategoryLabel: {value}");
                    }
                    break;
                case "Tag1Name":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.FirstTagLabel = value;
                        Console.WriteLine($"  - Found Tag1Name: {value}");
                    }
                    break;
                case "Tag2Name":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.SecondTagLabel = value;
                        Console.WriteLine($"  - Found Tag2Name: {value}");
                    }
                    break;
                case "Tag3Name":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        settings.ThirdTagLabel = value;
                        Console.WriteLine($"  - Found Tag3Name: {value}");
                    }
                    break;
            }
        }
        
        Console.WriteLine($"Finished parsing .crec file");
        
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
    public string CollectionNameLabel { get; set; } = "Name";
    public string UUIDLabel { get; set; } = "UUID";
    public string ManagementCodeLabel { get; set; } = "MC";
    public string CategoryLabel { get; set; } = "Category";
    public string FirstTagLabel { get; set; } = "Tag 1";
    public string SecondTagLabel { get; set; } = "Tag 2";
    public string ThirdTagLabel { get; set; } = "Tag 3";
}
