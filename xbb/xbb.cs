using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace xbb
{
	public class Student
	{
		public string Name { get; set; }
		public StudentStatus StudentStatus { get; set; }
		public int OrderOfGoing { get; set; }
		public int OrderInList { get; set; }

		public override string ToString() => Name + "\t" + DealWithData.ConvertStatus(StudentStatus) + "\t" + (StudentStatus == StudentStatus.going ? OrderOfGoing.ToString() + "\n" : "\n");
	}

	public enum StudentStatus //状态
	{
		unfinished,
		going,//进行中
		finished,
		suspended,//暂停的
		error
	}

	public enum SettingKey
	{
		mark, joinProgram, DisplayMode, fileName, saved, LastestError
	}


	public enum TaskStatus
	{
		Trying,
		Completed,
		Failed,
		Unknown
	}

	public static class DealWithLogs
	{
		public static async Task CreateLog(string content, TaskStatus status)
		{
			StorageFolder logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("logs", CreationCollisionOption.OpenIfExists);
			StorageFile logFile = await logFolder.CreateFileAsync(DateTime.Now.ToString("yyyy-MM-dd") + ".txt", CreationCollisionOption.OpenIfExists);
			await FileIO.AppendTextAsync(logFile, DateTime.Now.TimeOfDay.ToString() + "    " + status.ToString() + "    " + content + "\n");
		}

		public static async void LayoutLogs(StorageFile file, string resultBoxText)
		{

		}
	}

	public static class DealWithData
	{
		public static string[] DealWithStudentData(string dataLine)
		{
			string[] studentData = new string[3];

			dataLine.Trim();

			int dataLineLenth = dataLine.Length, i = 0;
			char[] DataArray = dataLine.ToCharArray();

			while (i < dataLineLenth && (DataArray[i] == ' ' || DataArray[i] == '\t')) i++;
			while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
			{
				studentData[0] += DataArray[i];
				i++;
			}
			while (i < dataLineLenth && (DataArray[i] == ' ' || DataArray[i] == '\t')) i++;

			while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
			{
				studentData[1] += DataArray[i];
				i++;
			}
			while (i < dataLineLenth && (DataArray[i] == ' ' || DataArray[i] == '\t')) i++;

			while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
			{
				studentData[2] += DataArray[i];
				i++;
			}
			while (i < dataLineLenth && (DataArray[i] == ' ' || DataArray[i] == '\t')) i++;

			/*
			while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
			{
				studentData[3] += DataArray[i];
				i++;
			}
			*/
			return studentData;//记得简化
		}

		public static StudentStatus ConvertStatus(string status)
		{
			if ((status == "unfinished") || (status == "")) return StudentStatus.unfinished;
			else if (status == "going") return StudentStatus.going;
			else if (status == "finished") return StudentStatus.finished;
			else if (status == "suspended") return StudentStatus.suspended;
			else if (status == "error") return StudentStatus.error;
			else return StudentStatus.unfinished;
		}

		public static string ConvertStatus(StudentStatus status)
		{
			if (status == StudentStatus.unfinished) return "unfinished";
			else if (status == StudentStatus.going) return "going";
			else if (status == StudentStatus.finished) return "finished";
			else if (status == StudentStatus.suspended) return "suspended";
			else return "error";
		}


		public static void SortStudentData(ref ObservableCollection<Student> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				data[i].OrderOfGoing = data.Count - i;
			}
		}

		public static async void LayoutData(StorageFile file, SortedList<int, Student> students)
		{
			//StorageFolder folder = ApplicationData.Current.LocalFolder;
			//StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			foreach (Student student in students.Values) await FileIO.AppendTextAsync(file, student.ToString());
		}

		public static SortedList<int, Student> SumDataSets(ObservableCollection<Student> students, ObservableCollection<Student> unfinished, ObservableCollection<Student> going, ObservableCollection<Student> finished)
		{
			SortedList<int, Student> returnList = new SortedList<int, Student>();
			foreach (Student student in going) returnList.Add(student.OrderInList, student);
			foreach (Student student in unfinished) returnList.Add(student.OrderInList, student);
			foreach (Student student in finished) returnList.Add(student.OrderInList, student);
			for (int i = 0; i < students.Count(); i++) if (!returnList.ContainsKey(i)) returnList.Add(i, students[i]);
			return returnList;
		}

	}

	public static class DealWithSettings
	{
		public static string ReadSettings(SettingKey settingKey)
		{
			ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;

			string settingValue = setting.Values[settingKey.ToString()] as string;
			return settingValue;
		}

		public static void WriteSettings(SettingKey settingKey, string settingValue)
		{
			ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;
			setting.Values[settingKey.ToString()] = settingValue;
		}

		public static void DeleteSettings(SettingKey settingKey)
		{
			ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;
			setting.Values.Remove(settingKey.ToString());
		}
		public static void DeleteSettings()
		{
			ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;
			setting.Values.Clear();
		}
	}

	public static class DealWithIdentity
	{
		public static async Task<bool> VerifyIdentity()
		{
#if DEBUG
			return true;
#else
			var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
			foreach (var folder in Folders)
			{
				try
				{
					var file = await folder.GetFileAsync("IdentityFile");
					IList<string> contents = await FileIO.ReadLinesAsync(file);
					using (SHA256 sha256Hash = SHA256.Create())
					{
						string hash = GetHash(sha256Hash, "User:" + contents[0]);
						Debug.WriteLine(hash);
						if (hash == contents[1]) return true;
					}
				}
				catch {; }
			}
			return false;
#endif
		}
		public static async Task<bool> VerifyIdentity(string password)
		{
			var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
			foreach (var folder in Folders)
			{
				try
				{
					var file = await folder.GetFileAsync("IdentityFile");
					IList<string> contents = await FileIO.ReadLinesAsync(file);
					using (SHA256 sha256Hash = SHA256.Create())
					{
						string hash = GetHash(sha256Hash, contents[1] + ".Password:" + password);
						Debug.WriteLine(hash);
						if (hash == contents[2]) return true;
					}
				}
				catch {; }
			}
			return false;
		}

		public static string GetHash(HashAlgorithm hashAlgorithm, string input)
		{
			byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}
	}

	public static class ContentDialogs
	{
		public static async void DisplayInvalidPraise()
		{
			ContentDialog invalidPraise = new ContentDialog
			{
				Title = "小小的提示",
				Content = "这玩意没用",
				CloseButtonText = "行"
			};

			ContentDialogResult result = await invalidPraise.ShowAsync();
		}
		public static async Task<bool> CheckWhetherMark()
		{
			ContentDialog whetherMarkDialog = new ContentDialog
			{
				Title = "再次确认",
				Content = @"以后的人都要标记状态为“进行中”？",
				CloseButtonText = "别了吧",
				PrimaryButtonText = "是的",
				DefaultButton = ContentDialogButton.Primary
			};

			ContentDialogResult result = await whetherMarkDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				DealWithSettings.WriteSettings(SettingKey.mark, "true");
				return true;
			}
			else
			{
				DealWithSettings.WriteSettings(SettingKey.mark, "false");
				return false;
			}
		}

		public static async void ThrowException(string exception, bool sendEmail = true)
		{
			ContentDialog ErrorDialog = new ContentDialog
			{
				Title = "Oops!",
				Content = (sendEmail ? "发生了问题：" : "") + exception,
				CloseButtonText = "好吧",
				DefaultButton = ContentDialogButton.Primary
			};

			if (sendEmail) ErrorDialog.PrimaryButtonText = "去反馈";

			ContentDialogResult result = await ErrorDialog.ShowAsync();
			if (sendEmail && result == ContentDialogResult.Primary) ComposeEmail(exception.ToString());
		}

		public static async void CheckJoinProgram()
		{
			ContentDialog invalidPraise = new ContentDialog
			{
				Title = "体验新功能",
				Content = "这会让你体验到更多的新特性和新特性（自行体会），确定？",
				PrimaryButtonText = "来！搞！（将重启应用）",
				CloseButtonText = "不了",
				DefaultButton = ContentDialogButton.Primary
			};

			ContentDialogResult result = await invalidPraise.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				DealWithSettings.WriteSettings(SettingKey.joinProgram, "true");
				await CoreApplication.RequestRestartAsync(string.Empty);
			}
			else DealWithSettings.WriteSettings(SettingKey.joinProgram, "false");
		}

		public static async void ComposeEmail()
		{
			var emailMessage = new EmailMessage();
			emailMessage.Body = "";

			var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
			emailMessage.To.Add(emailRecipient);
			emailMessage.Subject = "软件反馈";

			await EmailManager.ShowComposeNewEmailAsync(emailMessage);
		}
		public static async void ComposeEmail(string exception)
		{
			var emailMessage = new EmailMessage();
			emailMessage.Body = "于" + DateTime.Now.ToString() + "出现问题：" + exception;

			var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
			emailMessage.To.Add(emailRecipient);
			emailMessage.Subject = "软件崩溃";

			await EmailManager.ShowComposeNewEmailAsync(emailMessage);
		}

		public static async void LayoutIdentityFile(bool checkAgain=false)
		{
			if (await ContentDialogs.VerifyImportantIdentity())
			{
				var userNameBox = new TextBox { PlaceholderText = "随便输个用户名", Margin = new Thickness(10) };
				var passwordBox = new PasswordBox { PlaceholderText = "密码,要记住的", Margin = new Thickness(10) };
				var filePlace = new ComboBox { Margin = new Thickness(10) };
				var checkAgainBox = new TextBlock { Text = "刚才有值是空的！！！", Foreground = new SolidColorBrush(Colors.Red) };

				var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
				foreach (var folder in Folders) filePlace.Items.Add(string.Format("{0} ({1})", folder.Path, folder.DisplayName));
				filePlace.SelectedIndex = 0;
				if (Folders.Count == 0)
				{
					filePlace.PlaceholderText = "插个U盘再来试试吧";
					filePlace.IsEnabled = false;
				}

				var grid = new StackPanel();
				if(checkAgain)grid.Children.Add(checkAgainBox);
				grid.Children.Add(userNameBox);
				grid.Children.Add(passwordBox);
				grid.Children.Add(filePlace);

				var dialog = new ContentDialog
				{
					Title = "导出新的身份文件",
					Content = grid,
					PrimaryButtonText = "导出",
					CloseButtonText = "取消",
					DefaultButton = ContentDialogButton.Primary
				};
				var result = await dialog.ShowAsync();
				if (result == ContentDialogResult.Primary)
				{
					string userName = userNameBox.Text;
					string password = passwordBox.Password;
					StorageFolder folder = filePlace.SelectedItem as StorageFolder;
					if (userName == null || password == null || folder == null)
					{
						LayoutIdentityFile(true);
						return;
					}
					SHA256 sha256 = SHA256.Create();
					var file = await folder.CreateFileAsync ("IdentityFile",CreationCollisionOption.ReplaceExisting);
					string hash1 = DealWithIdentity.GetHash(sha256, "User:" + userName);
					string hash2 = DealWithIdentity.GetHash(sha256, hash1 + ".Password:" + password);
					await FileIO.AppendTextAsync(file,userName+'\n');
					await FileIO.AppendTextAsync(file, hash1 + '\n');
					await FileIO.AppendTextAsync(file, hash2);
				}
			}
			else ContentDialogs.ThrowException("没有所需的权限", false);
		}

		public static async Task<bool> VerifyImportantIdentity(bool checkAgain = false)
		{
#if DEBUG
			return true;
#else
			var passwordBox = new PasswordBox { PlaceholderText = checkAgain ? "刚才好像输错了" : "请输入密码" };
			ContentDialog contentDialog = new ContentDialog
			{
				Title = "再次确认身份",
				Content = passwordBox,
				PrimaryButtonText = "验证",
				CloseButtonText = "取消",
				DefaultButton = ContentDialogButton.Primary
			};
			var result = await contentDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				if (await DealWithIdentity.VerifyIdentity(passwordBox.Password)) return true;
				else await VerifyImportantIdentity(true);
			}
			return false;
#endif
		}
	}
}
