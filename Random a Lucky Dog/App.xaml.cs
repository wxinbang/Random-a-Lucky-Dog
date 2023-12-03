using RLD.CPCore.Helpers;
using RLD.CPCore.Models;
using RLD.Services;
using RLD.UWPCore.Services;
using RLD.Views;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using static RLD.CPCore.KeyDictionary.SettingKey;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.LocalizeService;
using static RLD.UWPCore.Services.SecurityService;

namespace RLD
{
	public sealed partial class App : Application
	{
		private Lazy<ActivationService> _activationService;
		public ObservableCollection<Student> AllStudentList = new ObservableCollection<Student>();
		public bool IsDataPrepared;
		public bool NeedSave;
		public StorageFile file;
		public Student stu;
		public SecurityOption SecurityOption = SecurityOption.None;
		public bool IsEditing;

		private ActivationService ActivationService
		{
			get { return _activationService.Value; }
		}

		public App()
		{
			InitializeComponent();

			EnteredBackground += App_EnteredBackground;
			Resuming += App_Resuming;
			UnhandledException += OnAppUnhandledException;

			// Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
			_activationService = new Lazy<ActivationService>(CreateActivationService);
		}

		protected override async void OnLaunched(LaunchActivatedEventArgs e)
		{
			SecurityOption = ConvertToSecurityOption(SettingsStorageService.ReadString(CPCore.KeyDictionary.SettingKey.SecurityOption));
			if (!e.PrelaunchActivated)
			{
				await ActivationService.ActivateAsync(e);
				Window.Current.Activate();
				SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, args) =>
				{
					var deferral = args.GetDeferral();

					var result = await ContentDialogs.CheckWhetherSave();
					switch (result)
					{
						case null:
							args.Handled = true;
							break;
						case true:
							if (!await IdentityService.VerifyIdentityAsync()) { args.Handled = true; await ThrowException(Localize(NoRequiredPermissions)); }
							else
							{
								if (SettingsStorageService.ReadString(FileName) != null)
								{
									StorageFile saveFile = await (await GetSaveFolderAsync()).CreateFileAsync(SettingsStorageService.ReadString(FileName), CreationCollisionOption.ReplaceExisting);
									await StudentService.SaveStudentsAsync(saveFile, AllStudentList);
								}
							}
							break;
						default:
							break;
					}
					deferral.Complete();
				};
			}
			IsDataPrepared = false;
			IsEditing = false;
		}

		protected override async void OnActivated(IActivatedEventArgs args)
		{
			await ActivationService.ActivateAsync(args);
		}

		private async void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			await ThrowException(e.Message, true);
			// TODO: Please log and handle the exception as appropriate to your scenario
			// For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
		}

		private ActivationService CreateActivationService()
		{
			return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
		}

		private UIElement CreateShell()
		{
			return new Views.ShellPage();
		}

		private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
		{
			var deferral = e.GetDeferral();
			await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
			deferral.Complete();
		}

		private void App_Resuming(object sender, object e)
		{
			Singleton<SuspendAndResumeService>.Instance.ResumeApp();
		}
	}
}
