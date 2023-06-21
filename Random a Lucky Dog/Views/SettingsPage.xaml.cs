using RLD.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static RLD.UWPCore.KeyDictionary.SettingKey;
using static RLD.UWPCore.KeyDictionary.StringKey;
using static RLD.Services.FoldersService;
using static RLD.UWPCore.LocalizeService;
using static RLD.Services.SettingsStorageService;
using static RLD.UWPCore.ExpectionProxy;

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
	}
}
