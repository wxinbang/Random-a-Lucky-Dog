using Select_Lucky_Dog.Services;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using xbb.ClassLibraries;
using static Select_Lucky_Dog.Helpers.KeyDictionary;
using static Select_Lucky_Dog.Helpers.KeyDictionary.StringKey;
using static Select_Lucky_Dog.Services.LocalizeService;
using static Select_Lucky_Dog.Services.IdentityService;

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
        internal static async Task ComposeEmail()
        {
            var emailMessage = new EmailMessage();
            emailMessage.Body = "";

            var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
            emailMessage.To.Add(emailRecipient);
            emailMessage.Subject = Localize(Feedback);

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
        internal static async Task ComposeEmail(string exception)
        {
            var emailMessage = new EmailMessage();
            emailMessage.Body = DateTime.Now.ToString() + Localize(ExceptionAt) + exception;

            var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
            emailMessage.To.Add(emailRecipient);
            emailMessage.Subject = Localize(SoftwareCrashes);

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
        internal static async Task ThrowException(string message, bool sendEmail = true)
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
        public static async Task ExportIdentityFile(bool checkAgain = false)
        {
            if (await VerifyIdentityAsync() && await VerifyPassword())
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

                var grid = new StackPanel();
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
                        ExportIdentityFile(true);
                        return;
                    }
                    SHA256 sha256 = SHA256.Create();
                    var file = await folder.CreateFileAsync("IdentityFile", CreationCollisionOption.ReplaceExisting);
                    string hash1 = DealWithIdentity.GetHash(sha256, "User:" + userName);
                    string hash2 = DealWithIdentity.GetHash(sha256, hash1 + ".Password:" + password);
                    await FileIO.AppendTextAsync(file, userName + '\n');
                    await FileIO.AppendTextAsync(file, hash1 + '\n');
                    await FileIO.AppendTextAsync(file, hash2);
                }
            }
            else ThrowException(Localize(NoRequiredPermissions), false);
        }
        internal static async Task<bool> VerifyPassword(bool checkAgain = false)
        {
#if DEBUG
            return true;
#else
			var passwordBox = new PasswordBox { PlaceholderText = checkAgain ? "刚才好像输错了" : "请输入密码" };
			ContentDialog contentDialog = new ContentDialog
			{
				Title = Localize(StringKey.VerifyPassword),
				Content = passwordBox,
				PrimaryButtonText = Localize(Verify),
				CloseButtonText = Localize(Cancel),
				DefaultButton = ContentDialogButton.Primary
			};
			var result = await contentDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				if (await VerifyIdentityAsync(passwordBox.Password)) return true;
				else return await VerifyPassword(true);
			}
			return false;
#endif
        }
    }
}
