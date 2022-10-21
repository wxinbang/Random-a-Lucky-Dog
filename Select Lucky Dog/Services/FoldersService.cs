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
        public static async Task<StorageFolder> GetDataSetFolderAsync()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
            return folder;
        }
        public static async Task<StorageFolder> GetSavesFolderAsync()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);
            return folder;
        }
        public static ApplicationDataContainer GetSettingsFolder()
        {
            ApplicationDataContainer SettingsFolder = ApplicationData.Current.LocalSettings;
            return SettingsFolder;
        }
    }
}
