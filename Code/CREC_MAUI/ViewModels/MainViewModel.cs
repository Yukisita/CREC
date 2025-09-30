/*
Main ViewModel
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CREC.Services;

namespace CREC.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IConfigurationService _configService;
    private readonly IProjectService _projectService;
    private readonly ICollectionDataService _collectionDataService;

    [ObservableProperty]
    private ObservableCollection<CollectionDataValuesClass> _collections = new();

    [ObservableProperty]
    private ObservableCollection<CollectionDataValuesClass> _filteredCollections = new();

    [ObservableProperty]
    private CollectionDataValuesClass? _selectedCollection;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _currentProjectPath = string.Empty;

    [ObservableProperty]
    private bool _isProjectLoaded;

    public MainViewModel(
        IConfigurationService configService,
        IProjectService projectService,
        ICollectionDataService collectionDataService)
    {
        _configService = configService;
        _projectService = projectService;
        _collectionDataService = collectionDataService;
        Title = "CREC - Inventory Management System";
    }

    [RelayCommand]
    private async Task OpenProjectAsync()
    {
        try
        {
            IsBusy = true;
            
            // This would open a file picker to select project file
            // For now, just a placeholder
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Handle error
            System.Diagnostics.Debug.WriteLine($"Error opening project: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateNewProjectAsync()
    {
        try
        {
            IsBusy = true;
            
            // Navigate to project creation page
            await Shell.Current.GoToAsync("//ProjectSettings");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating project: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchCollectionsAsync()
    {
        try
        {
            if (Collections.Count == 0)
                return;

            var filtered = await _collectionDataService.SearchCollectionsAsync(Collections.ToList(), SearchText);
            FilteredCollections.Clear();
            
            foreach (var collection in filtered)
            {
                FilteredCollections.Add(collection);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error searching collections: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadProjectAsync(string projectPath)
    {
        try
        {
            IsBusy = true;
            
            var project = await _projectService.LoadProjectAsync(projectPath);
            CurrentProjectPath = projectPath;
            
            var collections = await _collectionDataService.LoadCollectionsAsync(projectPath);
            Collections.Clear();
            FilteredCollections.Clear();
            
            foreach (var collection in collections)
            {
                Collections.Add(collection);
                FilteredCollections.Add(collection);
            }
            
            IsProjectLoaded = true;
            await _projectService.AddRecentProjectAsync(projectPath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading project: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchCollectionsAsync();
    }
}