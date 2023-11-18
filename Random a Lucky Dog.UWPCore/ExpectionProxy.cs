﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.EmailProxy;
using static RLD.UWPCore.Services.LocalizeService;

namespace RLD.UWPCore
{
	public static class ExpectionProxy
	{
		public static async Task ThrowException(string message, bool sendEmail = false)
		{
			var dialog = new ContentDialog
			{
				Title = Localize(ExceptionTitle),
				Content = message,
				CloseButtonText = Localize(Close),
				DefaultButton = ContentDialogButton.Close
			};
			if (sendEmail) dialog.PrimaryButtonText = Localize(SendEmail);
			var result = await dialog.ShowAsync();

			if (sendEmail && result == ContentDialogResult.Primary) await ComposeEmail(message);
			return;
		}

	}
}
