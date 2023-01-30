﻿using Select_Lucky_Dog.Core.Helpers;
using Select_Lucky_Dog.Core.Models;
using Select_Lucky_Dog.Services;
using Select_Lucky_Dog.Views;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Select_Lucky_Dog
{
	public sealed partial class App : Application
	{
		private Lazy<ActivationService> _activationService;
		public ObservableCollection<Student> AllStudentList = new ObservableCollection<Student>();
		public bool IsDataPrepared;

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

		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (!args.PrelaunchActivated)
			{
				await ActivationService.ActivateAsync(args);
			}
			IsDataPrepared = false;
		}

		protected override async void OnActivated(IActivatedEventArgs args)
		{
			await ActivationService.ActivateAsync(args);
		}

		private async void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			await ContentDialogs.ThrowException(e.Message);
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
