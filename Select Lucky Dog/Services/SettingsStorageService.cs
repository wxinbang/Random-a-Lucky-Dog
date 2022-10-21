using Select_Lucky_Dog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static Select_Lucky_Dog.Services.FoldersService;

namespace Select_Lucky_Dog.Services
{
    internal static class SettingsStorageService
    {
        public static void SaveString(string key,string value)
        {
            var settingsFolder = GetSettingsFolder();
            settingsFolder.SaveString(key,value);
        }
        public static async Task SaveAsync<T>(string key,T value)
        {
            var settingsFolder = GetSettingsFolder();
            await settingsFolder.SaveAsync(key,value);
        }
        public static string ReadString(string key)
        {
            var settingsFolder = GetSettingsFolder();
            string settingValue = settingsFolder.Values[key] as string;
            return settingValue;
        }
        public static async Task<T> ReadAsync<T>(string key)
        {
            var settingsFolder = GetSettingsFolder();
            return await settingsFolder.ReadAsync<T>(key);
        }

    }
}
