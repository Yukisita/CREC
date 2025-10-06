/*
Configuration Service Implementation
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC;

namespace CREC.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly string _configPath;

    public ConfigurationService()
    {
        // Use platform-specific app data directory
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _configPath = Path.Combine(appDataPath, "config.sys");
    }

    public async Task<ConfigValuesClass> LoadConfigurationAsync()
    {
        var config = new ConfigValuesClass();
        
        if (File.Exists(_configPath))
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(_configPath);
                foreach (var line in lines)
                {
                    if (line.Contains("="))
                    {
                        var parts = line.Split('=', 2);
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        // Parse configuration values
                        switch (key)
                        {
                            case "AllowEdit":
                                config.AllowEdit = bool.Parse(value);
                                break;
                            case "AllowEditID":
                                config.AllowEditID = bool.Parse(value);
                                break;
                            case "ShowConfidentialData":
                                config.ShowConfidentialData = bool.Parse(value);
                                break;
                            case "ShowUserAssistToolTips":
                                config.ShowUserAssistToolTips = bool.Parse(value);
                                break;
                            case "AutoSearch":
                                config.AutoSearch = bool.Parse(value);
                                break;
                            case "RecentShownContents":
                                config.RecentShownContents = bool.Parse(value);
                                break;
                            case "BootUpdateCheck":
                                config.BootUpdateCheck = bool.Parse(value);
                                break;
                            case "AutoLoadProjectPath":
                                config.AutoLoadProjectPath = value;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and return default config
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
            }
        }

        return config;
    }

    public async Task SaveConfigurationAsync(ConfigValuesClass config)
    {
        try
        {
            var lines = new List<string>
            {
                $"AllowEdit={config.AllowEdit}",
                $"AllowEditID={config.AllowEditID}",
                $"ShowConfidentialData={config.ShowConfidentialData}",
                $"ShowUserAssistToolTips={config.ShowUserAssistToolTips}",
                $"AutoSearch={config.AutoSearch}",
                $"RecentShownContents={config.RecentShownContents}",
                $"BootUpdateCheck={config.BootUpdateCheck}",
                $"AutoLoadProjectPath={config.AutoLoadProjectPath}"
            };

            await File.WriteAllLinesAsync(_configPath, lines);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
            throw;
        }
    }

    public string GetConfigurationPath()
    {
        return _configPath;
    }
}