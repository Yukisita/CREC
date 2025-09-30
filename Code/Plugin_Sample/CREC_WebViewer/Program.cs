/*
CREC Web Viewer - Main Program
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

using CREC_WebViewer.Services;

var builder = WebApplication.CreateBuilder(args);

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
app.UseStaticFiles(); // For serving HTML/CSS/JS files
app.UseRouting();
app.MapControllers();

// Serve the main web interface
app.MapFallbackToFile("index.html");

// Display startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("CREC Web Viewer starting...");
logger.LogInformation($"Data folder: {Environment.CurrentDirectory}");
logger.LogInformation("Web interface will be available at:");
logger.LogInformation("  - http://localhost:5000");
logger.LogInformation("  - http://[your-ip]:5000");
logger.LogInformation("API documentation available at: http://localhost:5000/swagger");

app.Run();
