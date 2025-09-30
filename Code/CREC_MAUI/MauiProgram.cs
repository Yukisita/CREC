/*
MauiProgram
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using Microsoft.Extensions.Logging;

namespace CREC;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<Services.IConfigurationService, Services.ConfigurationService>();
        builder.Services.AddSingleton<Services.IProjectService, Services.ProjectService>();
        builder.Services.AddSingleton<Services.ICollectionDataService, Services.CollectionDataService>();
        builder.Services.AddSingleton<Services.IPlatformService, Services.PlatformService>();
        
        // Register ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.ProjectSettingsViewModel>();
        
        // Register Views
        builder.Services.AddTransient<Views.MainPage>();
        builder.Services.AddTransient<Views.ProjectSettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}