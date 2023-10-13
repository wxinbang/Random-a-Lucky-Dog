using RLD.UWPCore.Helpers;
using System.Threading.Tasks;
using static RLD.UWPCore.KeyDictionary;
using static RLD.UWPCore.Services.FoldersService;

namespace RLD.UWPCore.Services
{
	public static class SettingsStorageService
	{
		public static void SaveString(SettingKey key, string value)
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.SaveString(key.ToString(), value);
		}
		public static async Task SaveAsync<T>(SettingKey key, T value)
		{
			var settingsFolder = GetSettingsFolder();
			await settingsFolder.SaveAsync(key.ToString(), value);
		}
		public static void DeleteString(SettingKey settingKey)
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.Values.Remove(settingKey.ToString());
		}
		public static void DeleteAllString()
		{
			var settingsFolder = GetSettingsFolder();
			settingsFolder.Values.Clear();
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
