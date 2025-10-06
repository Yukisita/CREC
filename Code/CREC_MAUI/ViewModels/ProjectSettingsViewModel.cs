/*
Project Settings ViewModel
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CREC.Services;

namespace CREC.ViewModels;

public partial class ProjectSettingsViewModel : BaseViewModel
{
    private readonly IProjectService _projectService;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _projectPath = string.Empty;

    [ObservableProperty]
    private string _backupPath = string.Empty;

    [ObservableProperty]
    private bool _uuidVisible = true;

    [ObservableProperty]
    private bool _managementCodeVisible = true;

    [ObservableProperty]
    private bool _categoryVisible = true;

    public ProjectSettingsViewModel(IProjectService projectService)
    {
        _projectService = projectService;
        Title = "Project Settings";
    }

    [RelayCommand]
    private async Task SaveProjectAsync()
    {
        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                // Show validation error
                return;
            }

            var project = new ProjectSettingValuesClass
            {
                // Map properties from ViewModel to model
                UUIDVisible = UuidVisible,
                ManagementCodeVisible = ManagementCodeVisible,
                CategoryVisible = CategoryVisible
            };

            await _projectService.SaveProjectAsync(ProjectPath, project);
            
            // Navigate back to main page
            await Shell.Current.GoToAsync("//Main");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving project: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectProjectPathAsync()
    {
        try
        {
            // This would open a folder picker
            // Platform-specific implementation needed
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selecting project path: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SelectBackupPathAsync()
    {
        try
        {
            // This would open a folder picker  
            // Platform-specific implementation needed
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selecting backup path: {ex.Message}");
        }
    }
}