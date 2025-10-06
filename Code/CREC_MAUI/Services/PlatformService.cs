/*
Platform Service Implementation
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
namespace CREC.Services;

public class PlatformService : IPlatformService
{
    public async Task<string?> PickFileAsync(string[] fileTypes)
    {
        try
        {
            // TODO: Implement platform-specific file picker
            // On Windows: Use WinUI FilePicker
            // On Android: Use Android file picker
            // On iOS: Use iOS document picker
            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error picking file: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> PickFolderAsync()
    {
        try
        {
            // TODO: Implement platform-specific folder picker
            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error picking folder: {ex.Message}");
            return null;
        }
    }

    public async Task ShowAlertAsync(string title, string message)
    {
        try
        {
            // TODO: Implement platform-specific alert dialog
            // For now, use debug output
            System.Diagnostics.Debug.WriteLine($"Alert - {title}: {message}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing alert: {ex.Message}");
        }
    }

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        try
        {
            // TODO: Implement platform-specific confirmation dialog
            // For now, return false (no confirmation)
            System.Diagnostics.Debug.WriteLine($"Confirm - {title}: {message}");
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing confirmation: {ex.Message}");
            return false;
        }
    }

    public async Task OpenFolderAsync(string folderPath)
    {
        try
        {
            // TODO: Implement platform-specific folder opening
            System.Diagnostics.Debug.WriteLine($"Opening folder: {folderPath}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening folder: {ex.Message}");
        }
    }

    public string GetPlatformName()
    {
#if WINDOWS
        return "Windows";
#elif ANDROID
        return "Android";
#elif IOS
        return "iOS";
#elif MACCATALYST
        return "macOS";
#else
        return "Unknown";
#endif
    }
}