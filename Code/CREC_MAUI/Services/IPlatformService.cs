/*
Platform Service Interface
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
namespace CREC.Services;

public interface IPlatformService
{
    Task<string?> PickFileAsync(string[] fileTypes);
    Task<string?> PickFolderAsync();
    Task ShowAlertAsync(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message);
    Task OpenFolderAsync(string folderPath);
    string GetPlatformName();
}