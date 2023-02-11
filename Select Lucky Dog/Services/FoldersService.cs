using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Select_Lucky_Dog.Services
{
	internal static class FoldersService
	{
		internal static async Task<StorageFolder> GetDataSetFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
		internal static async Task<StorageFolder> GetSaveFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);
		internal static async Task<StorageFolder> GetAutoSaveFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("AutoSave", CreationCollisionOption.OpenIfExists);
		internal static async Task DeleteDataSetFolderAsync() => await (await GetDataSetFolderAsync()).DeleteAsync();
		internal static async Task DeleteSaveFolderAsync() => await (await GetSaveFolderAsync()).DeleteAsync();
		internal static async Task DeleteAutoSaveFolderAsync() => await (await GetAutoSaveFolderAsync()).DeleteAsync();
		internal static ApplicationDataContainer GetSettingsFolder() => ApplicationData.Current.LocalSettings;
		internal static async Task<IReadOnlyList<StorageFolder>> GetAllFoldersAsync() => await ApplicationData.Current.LocalFolder.GetFoldersAsync();
	}
}
