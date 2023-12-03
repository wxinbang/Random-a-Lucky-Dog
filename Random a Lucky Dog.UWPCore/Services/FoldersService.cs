using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace RLD.UWPCore.Services
{
	public static class FoldersService
	{
		public static async Task<StorageFolder> GetDataSetFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
		public static async Task<StorageFolder> GetSaveFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);
		public static async Task<StorageFolder> GetAutoSaveFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("AutoSave", CreationCollisionOption.OpenIfExists);
		public static async Task<StorageFolder> GetExcelTempFolderAsync() => await ApplicationData.Current.LocalFolder.CreateFolderAsync("ExcelTemp", CreationCollisionOption.OpenIfExists);
		public static async Task DeleteDataSetFolderAsync() => await (await GetDataSetFolderAsync()).DeleteAsync();
		public static async Task DeleteSaveFolderAsync() => await (await GetSaveFolderAsync()).DeleteAsync();
		public static async Task DeleteAutoSaveFolderAsync() => await (await GetAutoSaveFolderAsync()).DeleteAsync();
		public static async Task DeleteExcelTempFolderAsync() => await (await GetExcelTempFolderAsync()).DeleteAsync();
		public static ApplicationDataContainer GetSettingsFolder() => ApplicationData.Current.LocalSettings;
		public static async Task<IReadOnlyList<StorageFolder>> GetAllFoldersAsync() => await ApplicationData.Current.LocalFolder.GetFoldersAsync();
	}
}
