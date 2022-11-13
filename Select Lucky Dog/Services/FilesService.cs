using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static Select_Lucky_Dog.Services.SettingsStorageService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using static Select_Lucky_Dog.Services.FoldersService;

namespace Select_Lucky_Dog.Services
{
    internal static class FilesService
    {
        internal static async Task<StorageFile> GetLastDataFileAsync()
        {
            if (ReadString(Saved) == "True") return await (await GetSavesFolderAsync()).GetFileAsync(ReadString(FileName));
            else return await (await GetDataSetFolderAsync()).GetFileAsync(ReadString(FileName));
        }
    }
}
