using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Services.SettingsStorageService;

namespace Select_Lucky_Dog.Services
{
	internal static class FilesService
	{
		internal static async Task<StorageFile> GetLastDataFileAsync()
		{
			if (ReadString(Saved) == "True") return await (await GetSaveFolderAsync()).GetFileAsync(ReadString(FileName));
			else return await (await GetDataSetFolderAsync()).GetFileAsync(ReadString(FileName));
		}
		internal static async Task<IReadOnlyList<StorageFile>> GetAllFilesAsync(this StorageFolder folder) => await folder.GetFilesAsync();
	}
}
