using System;
using System.Security.Cryptography;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using static Select_Lucky_Dog.Helpers.KeyDictionary;
using static Select_Lucky_Dog.Services.SettingsStorageService;

namespace xbb.ClassLibraries
{
	public static class DealWithIdentity
	{
		public static string GetHash(HashAlgorithm hashAlgorithm, string input)
		{
			byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}
	}

	public static class ContentDialogs
	{
		public static async void CheckJoinProgram()
		{
			ContentDialog invalidPraise = new ContentDialog
			{
				Title = "体验新功能",
				Content = "这会让你体验到更多的新特性和新特性（自行体会），确定？",
				PrimaryButtonText = "来！搞！（将重启应用）",
				CloseButtonText = "不了",
				DefaultButton = ContentDialogButton.Primary
			};

			ContentDialogResult result = await invalidPraise.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				SaveString(SettingKey.JoinProgram, "True");
				await CoreApplication.RequestRestartAsync(string.Empty);
			}
			else SaveString(SettingKey.JoinProgram, "False");
		}
	}
}
