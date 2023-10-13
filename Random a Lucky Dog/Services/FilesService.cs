using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using static RLD.UWPCore.KeyDictionary.SettingKey;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.SettingsStorageService;

namespace RLD.Services
{
	internal static class FilesService
	{
		internal static async Task<StorageFile> GetLastDataFileAsync()
		{
			if (ReadString(Saved) == "True") return await (await GetSaveFolderAsync()).GetFileAsync(ReadString(FileName));
			//else if (ReadString(FileName) != null) return await (await GetDataSetFolderAsync()).GetFileAsync(ReadString(FileName));
			else return null;
		}
		internal static async Task<IReadOnlyList<StorageFile>> GetAllFilesAsync(this StorageFolder folder) => await folder.GetFilesAsync();
	}
}
