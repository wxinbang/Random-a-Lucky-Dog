﻿using Microsoft.Toolkit.Uwp.Helpers;
using Select_Lucky_Dog.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Select_Lucky_Dog.Services
{
	public static class FirstRunDisplayService
	{
		private static bool shown = false;

		internal static async Task ShowIfAppropriateAsync()
		{
			await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
				CoreDispatcherPriority.Normal, async () =>
				{
					if (SystemInformation.Instance.IsFirstRun && !shown)
					{
						shown = true;
						await ContentDialogs.FirstRunDialog();
					}
				});
		}
	}
}