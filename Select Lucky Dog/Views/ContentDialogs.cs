using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using static Select_Lucky_Dog.Services.LocalizeService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.StringKey;
using static Select_Lucky_Dog.Helpers.KeyDictionary;
using System.Reflection;
using System.ServiceModel.Channels;
using Select_Lucky_Dog.Services;

namespace Select_Lucky_Dog.Views
{
    internal static class ContentDialogs
    {
        internal static async Task<bool> CheckMark()
        {
            var dialog = new ContentDialog
            {
                CloseButtonText = Localize(Close),
                PrimaryButtonText = Localize(CheckMarkConfirmText),
                DefaultButton = ContentDialogButton.Primary,
                Title = Localize(CheckMarkTitle),
                Content = Localize(CheckMarkContent),
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary) return true;
            else return false;
        }
        internal static async Task Praise()
        {
            var dialog = new ContentDialog
            {
                Title = Localize(PraiseTitle),
                Content = Localize(PraiseContent),
                CloseButtonText = Localize(PraiseClose),
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
            return;
        }
        public static async Task CheckJoinProgram()
        {
            var dialog = new ContentDialog
            {
                Title = Localize(JoinProgramTitle),
                Content = Localize(JoinProgramContent),
                CloseButtonText = Localize(JoinProgramClose),
                PrimaryButtonText = Localize(JoinProgramPrimary),
                DefaultButton = ContentDialogButton.Primary
            };
            SettingsStorageService.SaveString(SettingKey.JoinProgram, await dialog.ShowAsync() == ContentDialogResult.Primary ? "True" : "False");
            return;
        }
        internal static async void ThrowException(string message, bool isSendEmail = true)
        {
            var dialog = new ContentDialog
            {
                Title = Localize(ExceptionTitle),
                Content = message,
                //Content = Localize(message) == String.IsNullOrEmpty ? 1 : 2,
                CloseButtonText = Localize(Close),
                PrimaryButtonText = isSendEmail ? Localize(SendEmail) : null,
                DefaultButton = ContentDialogButton.Close
            };
            await dialog.ShowAsync();
            return;
        }
        internal static async Task FirstRunDialog()
        {
            var dialog = new ContentDialog
            {
                Title = Localize(FirstRunDialogTitle),
                Content = Localize(FirstRun_BodyText),
                PrimaryButtonText = Localize(FirstRunDialogPrimaryButtonText),
                DefaultButton = ContentDialogButton.Primary
            };
            await dialog.ShowAsync();
            return;
        }
    }
}
