/*
Configuration Service Interface
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC;

namespace CREC.Services;

public interface IConfigurationService
{
    Task<ConfigValuesClass> LoadConfigurationAsync();
    Task SaveConfigurationAsync(ConfigValuesClass config);
    string GetConfigurationPath();
}