using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static RLD.UWPCore.KeyDictionary;
using static RLD.UWPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.LocalizeService;
using static RLD.CPCore.Helpers.Security;
using static RLD.UWPCore.ExpectionProxy;


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
			devices.PlaceholderText = "eihei";

			var grid = new StackPanel();
			grid.Children.Add(devices);

			ContentDialog cd = new ContentDialog();
			cd.Title = "title";
			cd.Content = grid;
			cd.PrimaryButtonText = "p";
			cd.CloseButtonText = "c";
			await cd.ShowAsync();

			var information = collection[devices.SelectedIndex];
			var id = information.Id;




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
				if (checkAgain) grid.Children.Add(checkAgainBox);
				grid.Children.Add(userNameBox);
				grid.Children.Add(passwordBox);
				grid.Children.Add(filePlace);

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
					StorageFolder folder = Folders[filePlace.SelectedIndex];
					if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password) || folder == null)
					{
						//await ExportIdentityFile(true);
						return;
					}
					var file = await folder.CreateFileAsync("IdentityFile", CreationCollisionOption.ReplaceExisting);
					string hash1 = GetSHA256("User:" + userName);
					string hash2 = GetSHA256(hash1 + ".Password:" + password);
					await FileIO.AppendTextAsync(file, userName + '\n');
					await FileIO.AppendTextAsync(file, hash1 + '\n');
					await FileIO.AppendTextAsync(file, hash2);
					await ThrowException(Localize(Done));
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
