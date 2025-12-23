/*
Project Service Interface
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC;

namespace CREC.Services;

public interface IProjectService
{
    Task<ProjectSettingValuesClass> LoadProjectAsync(string projectPath);
    Task SaveProjectAsync(string projectPath, ProjectSettingValuesClass project);
    Task<List<string>> GetRecentProjectsAsync();
    Task AddRecentProjectAsync(string projectPath);
}