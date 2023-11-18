using Microsoft.Toolkit.Uwp.UI.Controls;
using RLD.UWPCore.Services;
using RLD.ViewModels;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static RLD.CPCore.KeyDictionary;
using static RLD.CPCore.KeyDictionary.SettingKey;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.LocalizeService;
using static RLD.UWPCore.Services.SecurityService;
using static RLD.UWPCore.Services.SettingsStorageService;

namespace RLD.Views
{
	// TODO: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
	public sealed partial class SettingsPage : Page
	{
		public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

		public SettingsPage()
		{
			InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.InitializeAsync();

			SecurityOptionBox.SelectedIndex = (int)ConvertToSecurityOption(ReadString(SettingKey.SecurityOption));
		}
		private async void DeleteDataSet_Click(object sender, RoutedEventArgs e)
		{
			await DeleteSaveFolderAsync();
			await DeleteDataSetFolderAsync();
			DeleteString(FileName);
			DeleteString(Saved);
			await ThrowException(Localize(DeleteFinished), false);
		}
		private async void DeleteUserData_Click(object sender, RoutedEventArgs e)
		{
			DeleteAllString();
			await DeleteDataSetFolderAsync();
			await DeleteSaveFolderAsync();
			await DeleteAutoSaveFolderAsync();
			await ThrowException(Localize(DeleteFinished), false);
		}
		private async void LayoutIdentityFile_Click(object sender, RoutedEventArgs e)
		{
			await ContentDialogs.ExportIdentityFile();
		}
		private void DisplayMode_Toggled(object sender, RoutedEventArgs e)
		{/*
			if (DisplayMode.IsOn)
			{//new ResourceDictionary()
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, false);
				BackgroundGrid.Background = (Brush)Application.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"];
				SaveString(DisplayMode, "True");
				ExtendAcrylicIntoTitleBar();
			}
			else
			{
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, true);
				BackgroundGrid.Background = null;
				SaveString(DisplayMode, "false");
				ExtendAcrylicIntoTitleBar();
			}
		*/
		}

		private async void SecurityOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var key = SettingKey.SecurityOption;
			var value = SecurityService.SecurityOption.None;
			var lastSetting = ConvertToSecurityOption(ReadString(key));
			switch (SecurityOptionBox.SelectedIndex)
			{
				case 0:
					value = SecurityService.SecurityOption.None;
					break;
				case 1:
					value = SecurityService.SecurityOption.Normal;
					break;
				case 2:
					value = SecurityService.SecurityOption.Strict;
					break;
				case 3:
					value = SecurityService.SecurityOption.WindowsHello;
					break;
			}
			if(await VerifyIdentityAsync())SaveString(key, value.ToString());
			else
			{
				await ThrowException(Localize(NoRequiredPermissions), false);
				SecurityOptionBox.SelectedIndex = (int)lastSetting;
			}
		}
	}
}
