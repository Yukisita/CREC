/*
CREC Web Viewer - Main Program
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using CREC_WebViewer.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

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
logger.LogInformation($"Data folder: {Environment.CurrentDirectory}");
logger.LogInformation($"Web root path: {webRootPath}");
logger.LogInformation($"wwwroot exists: {Directory.Exists(webRootPath)}");
logger.LogInformation("Web interface will be available at:");
logger.LogInformation("  - http://localhost:5000");
logger.LogInformation("  - http://[your-ip]:5000");
logger.LogInformation("API documentation available at: http://localhost:5000/swagger");

app.Run();
