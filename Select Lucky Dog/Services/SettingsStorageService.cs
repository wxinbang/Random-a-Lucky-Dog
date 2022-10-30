using Select_Lucky_Dog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Helpers.KeyDictionary;

namespace Select_Lucky_Dog.Services
{
    internal static class SettingsStorageService
    {
        public static void SaveString(SettingKey key,string value)
        {
            var settingsFolder = GetSettingsFolder();
            settingsFolder.SaveString(key.ToString(),value);
        }
        public static async Task SaveAsync<T>(SettingKey key,T value)
        {
            var settingsFolder = GetSettingsFolder();
            await settingsFolder.SaveAsync(key.ToString(),value);
        }
        public static string ReadString(SettingKey key)
        {
            var settingsFolder = GetSettingsFolder();
            string settingValue = settingsFolder.Values[key.ToString()] as string;
            return settingValue;
        }
        public static async Task<T> ReadAsync<T>(SettingKey key)
        {
            var settingsFolder = GetSettingsFolder();
            return await settingsFolder.ReadAsync<T>(key.ToString());
        }

    }
}
