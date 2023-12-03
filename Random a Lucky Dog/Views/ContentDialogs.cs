using Microsoft.UI.Xaml.Controls;
using RLD.CPCore.Models;
using RLD.UWPCore.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static RLD.CPCore.Helpers.Security;
using static RLD.CPCore.KeyDictionary;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.Services.IdentityService;
using static RLD.UWPCore.Services.LocalizeService;
using static RLD.UWPCore.Services.StudentService;

namespace RLD.Views
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
		internal static async Task CheckJoinProgram()
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
		internal static async Task ExportIdentityFile(bool checkAgain = false)
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
						await ExportIdentityFile(true);
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
		internal static async Task<bool> VerifyPassword(bool checkAgain = false)
		{
#if DEBUG
			await Task.CompletedTask;
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
		internal static async Task<bool?> CheckSave()
		{
			var dialog = new ContentDialog
			{
				Title = Localize(WhetherSaveTitle),
				Content = Localize(WhetherSaveContent),
				PrimaryButtonText = Localize(Yes),
				SecondaryButtonText = Localize(No),
				CloseButtonText = Localize(Cancel),
				DefaultButton = ContentDialogButton.Primary
			};
			var result = await dialog.ShowAsync();
			switch (result)
			{
				case ContentDialogResult.None: return null;
				case ContentDialogResult.Primary: return true;
				case ContentDialogResult.Secondary: return false;
			}
			return null;
		}
		internal static async Task EditStudent(Student student, bool checkAgain = false)
		{
			if (await VerifyIdentityAsync() && await VerifyPassword())
			{

				var nameBox = new TextBox { PlaceholderText = Localize(EnterName), Margin = new Thickness(10) };
				var statusBox = new ComboBox { Margin = new Thickness(10), ItemsSource = new List<string> { Localize(Unfinished), Localize(Going), Localize(Finished), Localize(Suspended) } };
				var praiseTimeBox = new NumberBox { Value = 0, Header = (Localize(PraiseTime)), Margin = new Thickness(10), SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact, SmallChange = 1, LargeChange = 5 };
				var checkAgainBox = new TextBlock { Text = Localize(ExistNullValue), Margin = new Thickness(20, 0, 0, 0), Foreground = new SolidColorBrush(Colors.IndianRed) };

				if (student != null)
				{
					nameBox.Text = student.Name;
					praiseTimeBox.Text = student.PraiseTime.ToString();
					statusBox.SelectedIndex = (int)student.Status;
				}

				var grid = new StackPanel();
				if (checkAgain) grid.Children.Add(checkAgainBox);
				grid.Children.Add(nameBox);
				grid.Children.Add(statusBox);
				grid.Children.Add(praiseTimeBox);

				var app = Application.Current as App;

				var dialog = new ContentDialog
				{
					Title = Localize(EditStudentTitle),
					Content = grid,
					PrimaryButtonText = Localize(Done),
					CloseButtonText = Localize(Cancel),
					DefaultButton = ContentDialogButton.Primary
				};
				var result = await dialog.ShowAsync();
				if (result == ContentDialogResult.Primary)
				{
					string name = nameBox.Text;
					int praiseTime = (int)praiseTimeBox.Value;
					StudentStatus status = ConvertStatus(statusBox.SelectedItem.ToString());
					if (String.IsNullOrEmpty(name))
					{
						await EditStudent(student, true);
						return;
					}
					//int nowOrderOfGoing = 0;
					Student newStudent = new Student(name, status, praiseTime, status == StudentStatus.going ? (ClassifyStudents(app.AllStudentList)[0].Count + 1) : 0, student == null ? app.AllStudentList.Count : student.OrderInList);//检查
					if (student == null)
					{
						app.AllStudentList.Add(newStudent);
					}
					else
					{
						app.AllStudentList.Insert(student.OrderInList, newStudent);
						app.AllStudentList.Remove(student);
					}
					if (status == StudentStatus.going)//原来的状态
					{
						//if (student.Status != StudentStatus.going) nowOrderOfGoing =
						//nowOrderOfGoing = student.OrderOfGoing;
					}

					//student = new Student(name,
					//	ConvertStatus((string)statusBox.SelectedItem),
					//	praiseTime,
					//	nowOrderOfGoing,
					//	app.AllStudentList.Count);
				}
			}
		}
		internal async static Task<bool?> CheckWhetherSave()
		{
			var messageDialog = new ContentDialog
			{
				Title = Localize(StringKey.WhetherSaveTitle),
				Content = Localize(WhetherSaveContent),
				PrimaryButtonText = Localize(Save),
				SecondaryButtonText = Localize(NotSave),
				CloseButtonText = Localize(Cancel)
			};

			messageDialog.DefaultButton = ContentDialogButton.Primary;
			var result = await messageDialog.ShowAsync();
			if (result == ContentDialogResult.Primary) return true;
			else if (result == ContentDialogResult.Secondary) return false;
			else return null;
		}
	}
}
