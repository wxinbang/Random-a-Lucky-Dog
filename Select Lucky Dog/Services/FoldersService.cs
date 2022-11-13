using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Select_Lucky_Dog.Services
{
    internal static class FoldersService
    {
        internal static async Task<StorageFolder> GetDataSetFolderAsync()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
            return folder;
        }
        internal static async Task<StorageFolder> GetSavesFolderAsync()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);
            return folder;
        }
        internal static async Task DeleteDataSetFolderAsync()=>await(await GetDataSetFolderAsync()).DeleteAsync();
        internal static async Task DeleteSavesFolderAsync() => await (await GetSavesFolderAsync()).DeleteAsync();
        internal static ApplicationDataContainer GetSettingsFolder()
        {
            ApplicationDataContainer SettingsFolder = ApplicationData.Current.LocalSettings;
            return SettingsFolder;
        }
    }
}
