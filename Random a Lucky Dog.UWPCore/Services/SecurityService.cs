using RLD.CPCore.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static RLD.CPCore.KeyDictionary;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.Services.LocalizeService;

namespace RLD.UWPCore.Services
{
	public static class SecurityService
	{
		public static async Task PickDeviceAsync(bool checkAgain = false)
		{
			var collection = await DeviceService.GetAllPortableDeviceAsync();
			var devices = new ComboBox { Margin = new Thickness(10) };

			foreach (var device in collection) devices.Items.Add(string.Format("{0}", device.Name));
			devices.PlaceholderText = Localize(PickADevice);

			if (await VerifyIdentityAsync() && await VerifyPasswordAsync())
			{
				var passwordBox = new PasswordBox { PlaceholderText = Localize(Password), Margin = new Thickness(10) };
				var checkAgainBox = new TextBlock { Text = Localize(ExistNullValue), Margin = new Thickness(20, 0, 0, 0), Foreground = new SolidColorBrush(Colors.IndianRed) };

				if (collection.Count == 0)
				{
					devices.PlaceholderText = Localize(InsertUSBDrive);
					devices.IsEnabled = false;
				}

				var grid = new StackPanel();
				if (checkAgain) grid.Children.Add(checkAgainBox);
				grid.Children.Add(devices);
				grid.Children.Add(passwordBox);

				var dialog = new ContentDialog
				{
					Title = Localize(StringKey.ExportIdentityFile),
					Content = grid,
					PrimaryButtonText = Localize(Export),
					CloseButtonText = Localize(Cancel),
					DefaultButton = ContentDialogButton.Primary
				};
				var result = await dialog.ShowAsync();
				if (result == ContentDialogResult.Primary)
				{
					string password = passwordBox.Password;
					if (String.IsNullOrEmpty(password) || devices.SelectedIndex == -1)
					{
						await PickDeviceAsync(true);
						return;
					}

					var information = collection[devices.SelectedIndex];
					var id = information.Id.Substring(information.Id.Length - 38, 36);

					await SettingsStorageService.SaveAsync(id, password);
				}
			}
			else if (!await VerifyIdentityAsync()) await ThrowException(Localize(NoRequiredPermissions), false);

		}
		public static async Task<bool> VerifyIdentityAsync()
		{
			var result = GetSecurityOption();
			if (result == SecurityOption.None) return true;
			else if (result == SecurityOption.Normal)
			{
				var devices = await DeviceService.GetAllPortableDeviceAsync();
				foreach (var device in devices)
				{
					string _s = await SettingsStorageService.ReadAsync<string>(device.Id.Substring(device.Id.Length - 38, 36));
					if (!String.IsNullOrEmpty(_s)) return true;
				}
				return false;
			}
			else if (result == SecurityOption.Strict) return await VerifyPasswordAsync();
			else if (result == SecurityOption.WindowsHello)
			{
				if ((await WindowsHelloService.CreatePassportKeyAsync()).Status == KeyCredentialStatus.Success) return true;
				else return false;
			}
			return false;
		}
		public static async Task<bool> VerifyPasswordAsync()
		{
			var passwordBox = new PasswordBox { PlaceholderText = Localize(Password), Margin = new Thickness(10) };
			var grid = new StackPanel();
			grid.Children.Add(passwordBox);

			var dialog = new ContentDialog
			{
				Title = Localize(StringKey.VerifyPassword),
				Content = grid,
				PrimaryButtonText = Localize(Yes),
				CloseButtonText = Localize(Cancel),
				DefaultButton = ContentDialogButton.Primary
			};
			var result = await dialog.ShowAsync();
			if (result == ContentDialogResult.None) return false;
			else
			{
				string password = passwordBox.Password;
				var devices = await DeviceService.GetAllPortableDeviceAsync();
				foreach (var device in devices)
				{
					string _s = await SettingsStorageService.ReadAsync<string>(device.Id.Substring(device.Id.Length - 38, 36));
					if (await SettingsStorageService.ReadAsync<string>(_s) == password) return true;
				}
			}
			return false;
		}
		public enum SecurityOption
		{
			None,
			Normal,
			Strict,
			WindowsHello,
		}
		public static SecurityOption ConvertToSecurityOption(string s)
		{
			if (s == "WindowsHello") return SecurityOption.WindowsHello;
			else if (s == "Normal") return SecurityOption.Normal;
			else if (s == "Strict") return SecurityOption.Strict;
			else return SecurityOption.None;
		}
		private static SecurityOption GetSecurityOption() => ConvertToSecurityOption(SettingsStorageService.ReadString(SettingKey.SecurityOption));
	}
}
