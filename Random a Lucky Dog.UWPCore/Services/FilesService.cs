using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using static RLD.CPCore.KeyDictionary.SettingKey;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.SettingsStorageService;

namespace RLD.UWPCore.Services
{
	public static class FilesService
	{
		public static async Task<StorageFile> GetLastDataFileAsync()
		{
			if (ReadString(Saved) == "True") return await (await GetSaveFolderAsync()).GetFileAsync(ReadString(FileName));
			//else if (ReadString(FileName) != null) return await (await GetDataSetFolderAsync()).GetFileAsync(ReadString(FileName));
			else return null;
		}
		public static async Task<IReadOnlyList<StorageFile>> GetAllFilesAsync(this StorageFolder folder) => await folder.GetFilesAsync();
	}
}
