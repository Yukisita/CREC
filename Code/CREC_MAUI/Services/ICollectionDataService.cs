/*
Collection Data Service Interface  
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC;

namespace CREC.Services;

public interface ICollectionDataService
{
    Task<List<CollectionDataValuesClass>> LoadCollectionsAsync(string projectPath);
    Task SaveCollectionAsync(string projectPath, CollectionDataValuesClass collection);
    Task DeleteCollectionAsync(string projectPath, string collectionId);
    Task<List<CollectionDataValuesClass>> SearchCollectionsAsync(List<CollectionDataValuesClass> collections, string searchTerm);
}