using Select_Lucky_Dog.Helpers;
using System.Threading.Tasks;
using static Select_Lucky_Dog.Helpers.KeyDictionary;
using static Select_Lucky_Dog.Services.FoldersService;

namespace Select_Lucky_Dog.Services
{
	internal static class SettingsStorageService
	{
		internal static void SaveString(SettingKey key, string value)
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.SaveString(key.ToString(), value);
		}
		internal static async Task SaveAsync<T>(SettingKey key, T value)
		{
			var settingsFolder = GetSettingsFolder();
			await settingsFolder.SaveAsync(key.ToString(), value);
		}
		internal static void DeleteString(SettingKey settingKey)
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.Values.Remove(settingKey.ToString());
		}
		internal static void DeleteAllString()
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.Values.Clear();
		}
		internal static string ReadString(SettingKey key)
		{
			var settingsFolder = GetSettingsFolder();
			string settingValue = settingsFolder.Values[key.ToString()] as string;
			return settingValue;
		}
		internal static async Task<T> ReadAsync<T>(SettingKey key)
		{
			var settingsFolder = GetSettingsFolder();
			return await settingsFolder.ReadAsync<T>(key.ToString());
		}

	}
}
