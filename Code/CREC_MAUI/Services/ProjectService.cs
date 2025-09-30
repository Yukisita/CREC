/*
Project Service Implementation
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC;

namespace CREC.Services;

public class ProjectService : IProjectService
{
    private readonly string _recentProjectsPath;

    public ProjectService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        _recentProjectsPath = Path.Combine(appDataPath, "RecentlyOpenedProjectList.log");
    }

    public async Task<ProjectSettingValuesClass> LoadProjectAsync(string projectPath)
    {
        try
        {
            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Project file not found: {projectPath}");
            }

            var content = await File.ReadAllTextAsync(projectPath);
            var project = new ProjectSettingValuesClass();
            
            // Parse the project file content
            // This would need to be implemented based on the original project file format
            
            return project;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading project: {ex.Message}");
            throw;
        }
    }

    public async Task SaveProjectAsync(string projectPath, ProjectSettingValuesClass project)
    {
        try
        {
            // Serialize project to file
            // This would need to be implemented based on the original project file format
            
            await Task.CompletedTask; // Placeholder
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving project: {ex.Message}");
            throw;
        }
    }

    public async Task<List<string>> GetRecentProjectsAsync()
    {
        try
        {
            if (!File.Exists(_recentProjectsPath))
            {
                return new List<string>();
            }

            var lines = await File.ReadAllLinesAsync(_recentProjectsPath);
            return lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recent projects: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task AddRecentProjectAsync(string projectPath)
    {
        try
        {
            var recentProjects = await GetRecentProjectsAsync();
            
            // Remove if already exists
            recentProjects.Remove(projectPath);
            
            // Add to beginning
            recentProjects.Insert(0, projectPath);
            
            // Keep only last 10 projects
            if (recentProjects.Count > 10)
            {
                recentProjects = recentProjects.Take(10).ToList();
            }

            await File.WriteAllLinesAsync(_recentProjectsPath, recentProjects);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding recent project: {ex.Message}");
        }
    }
}