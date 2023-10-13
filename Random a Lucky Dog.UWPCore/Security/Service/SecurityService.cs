using RLD.UWPCore.Helper;
using RLD.UWPCore.Services;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.KeyDictionary;
using static RLD.UWPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.LocalizeService;


namespace RLD.UWPCore.Service
{
	public static class SecurityService
	{
		public static async Task PickDeviceAsync(bool checkAgain = false)
		{
			//DevicePicker devicePicker = new DevicePicker();
			//devicePicker.Filter.SupportedDeviceClasses.Add(DeviceClass.PortableStorageDevice);
			//var device = devicePicker.PickSingleDeviceAsync(new Windows.Foundation.Rect()).GetResults();
			var collection = await Helper.DeviceProxy.GetAllPortableDevice();
			var devices = new ComboBox { Margin = new Thickness(10) };

			foreach (var device in collection) devices.Items.Add(string.Format("{0}", device.Name));
			devices.PlaceholderText = Localize(PickADevice);

			//ContentDialog cd = new ContentDialog();
			//cd.Title = Localize(ExportIdentityFile);
			//cd.Content = grid;
			//cd.PrimaryButtonText = Localize(Done);
			//cd.CloseButtonText = Localize(Cancel);
			//await cd.ShowAsync();

			if (await VerifyIdentityAsync() && await VerifyPasswordAsync())
			{
				var userNameBox = new TextBox { PlaceholderText = Localize(UserName), Margin = new Thickness(10) };
				var passwordBox = new PasswordBox { PlaceholderText = Localize(Password), Margin = new Thickness(10) };
				var filePlace = new ComboBox { Margin = new Thickness(10) };
				var checkAgainBox = new TextBlock { Text = Localize(ExistNullValue), Margin = new Thickness(20, 0, 0, 0), Foreground = new SolidColorBrush(Colors.IndianRed) };

				var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
				foreach (var folder in Folders) filePlace.Items.Add(string.Format("{0} ({1})", folder.Path, folder.DisplayName));
				filePlace.SelectedIndex = 0;
				if (Folders.Count == 0)
				{
					filePlace.PlaceholderText = Localize(InsertUSBDrive);
					filePlace.IsEnabled = false;
				}

				//var grid = new StackPanel();
				var grid = new StackPanel();
				if (checkAgain) grid.Children.Add(checkAgainBox);
				grid.Children.Add(devices);
				grid.Children.Add(userNameBox);
				grid.Children.Add(passwordBox);
				//grid.Children.Add(filePlace);

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
					string userName = userNameBox.Text;
					string password = passwordBox.Password;
					if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password) || filePlace.SelectedIndex == -1)
					{
						await PickDeviceAsync(true);
						return;
					}

					StorageFolder folder = Folders[filePlace.SelectedIndex];
					var information = collection[devices.SelectedIndex];
					var id = information.Id;

					await SettingsStorageService.SaveAsync(SettingKey.Devices, new Device(id, password));

					//var file = await folder.CreateFileAsync("IdentityFile", CreationCollisionOption.ReplaceExisting);
					//string hash1 = GetSHA256("User:" + userName);
					//string hash2 = GetSHA256(hash1 + ".Password:" + password);
					//await FileIO.AppendTextAsync(file, userName + '\n');
					//await FileIO.AppendTextAsync(file, hash1 + '\n');
					//await FileIO.AppendTextAsync(file, hash2);
					//await ThrowException(Localize(Done));
				}
			}
			else if (!await VerifyIdentityAsync()) await ThrowException(Localize(NoRequiredPermissions), false);

		}
		public static async Task<bool> VerifyIdentityAsync()
		{
			await Task.CompletedTask;
			return true;
		}
		public static async Task<bool> VerifyPasswordAsync()
		{
			await Task.CompletedTask;
			return true;
		}
	}
}
