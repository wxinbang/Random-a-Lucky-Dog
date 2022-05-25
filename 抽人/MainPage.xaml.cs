using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using xbb;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 抽人
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class MainPage : Page
	{
		//Dictionary<int, Student> studentDictionary = new Dictionary<int, Student>();

		ObservableCollection<Student> studentList = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfUnfinishedStudent = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfGoingStudent = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfFinishedStudent = new ObservableCollection<Student>();

		SortedList<int, Student> lastGoingStudent = new SortedList<int, Student>();

#if (DEBUG)
		string version = "Build 3.3.10.0.prealpha.220405-1012";//220413-1900 220424-2138 220427-2203 220430-2200
#else
		string version = "2.2.10-Beta";
#endif

		Random randomStudent = new Random();

		int timesOfVersionTextTapped = 0;

		int studentNumber;
		int sumOfStudent;
		bool mark = false;
		string DataSetPath;

		string fileName;


		int unfinishedNumber;
		StorageFile file;
		StorageFolder dataSetFolder;
		StorageFolder saveFolder;

		string readableFilePath;
		bool whetherJoInsiderPreviewProgram;

		// 加入初始化时读写配置文件

		public MainPage()
		{
			this.InitializeComponent();
			versionInformationBox.Text = version;
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			dataSetFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
			saveFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);

			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Trying);
			if (DealWithSettings.ReadSettings("fileName") != null)
			{
				if (DealWithSettings.ReadSettings("saved") != "true") file = await dataSetFolder.GetFileAsync(DealWithSettings.ReadSettings("fileName"));
				else file = await saveFolder.GetFileAsync(DealWithSettings.ReadSettings("fileName"));
				ConnectDataSet(file);
				fileName = DealWithSettings.ReadSettings("fileName");
			}
			if (DealWithSettings.ReadSettings("joinProgram") != "True")
			{
				InfoBar.IsOpen = false;
				//layOutDataSetButton.Visibility = Visibility.Collapsed;
				layOutFlyoutButton.Visibility = Visibility.Collapsed;
				Views.Visibility = Visibility.Collapsed;
				DeleteButton.Visibility = Visibility.Collapsed;
				OperateStudent.Visibility = Visibility.Collapsed;
				StudentSuggestBox.Visibility = Visibility.Collapsed;

			}
			if (DealWithSettings.ReadSettings("mark") == "True") whetherMark.IsOn = true;
			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Completed);
		}

		private void randomButton_Click(object sender, RoutedEventArgs e)
		{
			GoingView.ItemsSource = listOfGoingStudent;
			dealWithStudentDataProgressBar.Maximum = listOfGoingStudent.Count + listOfUnfinishedStudent.Count;

			if (listOfUnfinishedStudent.Count != 0)
			{
				if (mark)
				{
					do studentNumber = randomStudent.Next(0, listOfUnfinishedStudent.Count);
					while (listOfUnfinishedStudent[studentNumber].StudentStatus == StudentStatus.suspended);

					resultBox.Text = listOfUnfinishedStudent[studentNumber].Name;
					listOfUnfinishedStudent[studentNumber].StudentStatus = StudentStatus.going;
					listOfGoingStudent.Insert(0, listOfUnfinishedStudent[studentNumber]);
					listOfGoingStudent[0].OrderOfGoing = listOfGoingStudent.Count;
					listOfUnfinishedStudent.RemoveAt(studentNumber);
					dealWithStudentDataProgressBar.Value = listOfGoingStudent.Count;
				}
				else
				{
					do studentNumber = randomStudent.Next(0, studentList.Count);
					while (studentList[studentNumber].StudentStatus == StudentStatus.suspended);

					resultBox.Text = studentList[studentNumber].Name;
				}
			}
			else resultBox.Text = "已经全部抽过了";//提示全部做过
		}

		private void praiseButton_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogs.DisplayInvalidPraise();
		}


		private async void selectDataSetButton_Click(object sender, RoutedEventArgs e)
		{
			unfinishedNumber = 0;
			studentNumber = 0;

			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.List;
			picker.SuggestedStartLocation = PickerLocationId.Desktop;
			picker.FileTypeFilter.Add(".txt");//记得检查

			file = await picker.PickSingleFileAsync();

			//var readableFolderPath = ApplicationData.Current.LocalFolder;
			//readableFilePath=readableFolderPath+@"\"+file.Name;
			if (file != null)
			{
				//readableFilePath = readableFolderPath + @"\" + file.Name;
				//DataSetPath = file.Path;
				await file.CopyAsync(dataSetFolder, file.Name, NameCollisionOption.ReplaceExisting);
				resultBox.Text = "已选择：" + file.Name;
				// Application now has read/write access to the picked file

			}
			else
			{
				this.resultBox.Text = "操作已取消";
			}
			DealWithSettings.WriteSettings("saved", "");
		}

		private async void whetherMark_Toggled(object sender, RoutedEventArgs e)
		{
			if (whetherMark.IsOn == true)
			{
				mark = await ContentDialogs.CheckWhetherMark();
				whetherMark.IsOn = mark;
			}
			else
			{
				mark = whetherMark.IsOn;
				DealWithSettings.WriteSettings("mark", mark ? "True" : "False");
			}
		}


		private void connectDataSet_Click(object sender, RoutedEventArgs e)
		{
			ConnectDataSet(file);
		}

		private async void ConnectDataSet(StorageFile file)
		{
			studentList.Clear();
			listOfGoingStudent.Clear();
			listOfUnfinishedStudent.Clear();
			lastGoingStudent.Clear();
			listOfFinishedStudent.Clear();

			GoingView.ItemsSource = listOfGoingStudent;

			//StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			//StorageFile file = await dataSetFolder.GetFileAsync(fileName);

			try
			{
				IList<string> contents = await FileIO.ReadLinesAsync(file);
				sumOfStudent = contents.ToArray().Length;
				dealWithStudentDataProgressBar.Maximum = sumOfStudent;

				bool[] checkId = new bool[sumOfStudent];
				for (int j = 0; j < sumOfStudent; j++)
				{
					//创建一个动态bool数组checkId并全部初始化为false

					string[] studentData = new string[3];
					studentData = DealWithData.DealWithStudentData(contents[j]);

					Student Somebody = new Student() { Name = studentData[0], StudentStatus = DealWithData.ConvertStatus(studentData[1]), OrderOfGoing = Convert.ToInt32(studentData[2]), OrderInList = j };

					if (Somebody.StudentStatus == StudentStatus.unfinished)
					{
						listOfUnfinishedStudent.Add(Somebody);
					}
					else if (Somebody.StudentStatus == StudentStatus.going)
					{
						lastGoingStudent.Add(Somebody.OrderOfGoing, Somebody);
						//listOfGoingStudent.Add(Somebody);
					}
					else if (Somebody.StudentStatus == StudentStatus.finished)
					{
						listOfFinishedStudent.Add(Somebody);
					}

					studentList.Add(Somebody);
					dealWithStudentDataProgressBar.Value = j + 1;
				}

				foreach (var someBody in lastGoingStudent)
				{
					listOfGoingStudent.Insert(0, someBody.Value);
				}

				resultBox.Text = "连接完成：" + file.Name;
				DealWithSettings.WriteSettings("fileName", file.Name);
			}
			catch(Exception ex)
			{
				await ContentDialogs.ThrowException(ex);
			}
		}


		private void versionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
		{
			timesOfVersionTextTapped++;
			if (timesOfVersionTextTapped == 5)
			{
				CheckJoinProgram();
				timesOfVersionTextTapped = 0;
			}

		}
		private async void CheckJoinProgram()
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
				DealWithSettings.WriteSettings("joinProgram", "True");
				AppRestartFailureReason restartFailureReason = await CoreApplication.RequestRestartAsync(string.Empty);
				resultBox.Text = Convert.ToString(restartFailureReason);
			}
			else DealWithSettings.WriteSettings("joinProgram", "False");
		}

		private async Task ComposeEmail()
		{
			var emailMessage = new EmailMessage();
			emailMessage.Body = "于" + DateTime.Now.ToString() + "发现问题：";

			var emailRecipient = new EmailRecipient("wxinbang@outlook.com");
			emailMessage.To.Add(emailRecipient);
			emailMessage.Subject = "软件反馈";

			await EmailManager.ShowComposeNewEmailAsync(emailMessage);
		}

		private async void SendEmailButton_Click(object sender, RoutedEventArgs e)
		{
			await ComposeEmail();
		}

		private void ExitProgram_Click(object sender, RoutedEventArgs e)
		{
			DealWithSettings.WriteSettings("joinProgram", "False");
			InfoBar.Severity = InfoBarSeverity.Success;
			InfoBar.Message = "已退出预览模式，请尽快重启";
			MoreButton.Visibility = Visibility.Collapsed;
		}

		private async void LayoutDataSet_Click(object sender, RoutedEventArgs e)
		{
			SortedList<int, Student> updatedList = SumDataSets(studentList, listOfUnfinishedStudent, listOfGoingStudent, listOfFinishedStudent);
			string afterFileName = "After-" + fileName;
			//DealWithData.LayoutData(afterFileName, updatedList);

			var savePicker = new FileSavePicker();
			savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
			// Dropdown of file types the user can save the file as
			savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
			// Default file name if the user does not type one in or select a file to replace
			savePicker.SuggestedFileName = afterFileName;

			StorageFile file = await savePicker.PickSaveFileAsync();
			//StorageFile updatedFile = await ApplicationData.Current.LocalFolder.GetFileAsync(afterFileName);
			if (file != null)
			{
				// Prevent updates to the remote version of the file until
				// we finish making changes and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);
				// write to file
				await FileIO.WriteTextAsync(file, "");
				DealWithData.LayoutData(file, updatedList);
				// Let Windows know that we're finished changing the file so
				// the other app can update the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
				if (status == FileUpdateStatus.Complete)
				{
					this.resultBox.Text = "File " + file.Name + " was saved.";
				}
				else
				{
					this.resultBox.Text = "File " + file.Name + " couldn't be saved.";
				}
			}
			else
			{
				this.resultBox.Text = "Operation cancelled.";
			}
		}

		public static SortedList<int, Student> SumDataSets(ObservableCollection<Student> students, ObservableCollection<Student> unfinished, ObservableCollection<Student> going, ObservableCollection<Student> finished)
		{
			SortedList<int, Student> returnList = new SortedList<int, Student>();
			/*
						foreach (Student student in students)
						{
							if (unfinished.Contains(student)) returnList.Add(unfinished[unfinished.IndexOf(student)]);
							else if (going.Contains(student)) returnList.Add(going[going.IndexOf(student)]);
							else if (finished.Contains(student)) returnList.Add(finished[finished.IndexOf(student)]);
							else returnList.Add(students[students.IndexOf(student)]);
						}
			*/
			foreach (Student student in going) returnList.Add(student.OrderInList, student);
			foreach (Student student in unfinished) returnList.Add(student.OrderInList, student);
			foreach (Student student in finished) returnList.Add(student.OrderInList, student);
			for (int i = 0; i < students.Count(); i++) if (!returnList.ContainsKey(i)) returnList.Add(i, students[i]);
			return returnList;
		}

		private void LayoutUserData_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteLogFile_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteDataSet_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteUserData_Click(object sender, RoutedEventArgs e)
		{

		}

		private void LayoutLogs_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MarkFinished_Click(object sender, RoutedEventArgs e)
		{
			if (GoingView.SelectedItem != null)
			{
				listOfGoingStudent[listOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.finished;
				listOfFinishedStudent.Add((Student)GoingView.SelectedItem);
				listOfGoingStudent.Remove((Student)GoingView.SelectedItem);
				DealWithData.SortStudentData(ref listOfGoingStudent);
				GoingView.ItemsSource = null;
				GoingView.ItemsSource = listOfGoingStudent;
			}
		}

		private void StudentSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (sender.Text != "") StudentSuggestBox.ItemsSource = studentList.Where(p => p.Name.Contains(sender.Text)).Select(p => p.Name).ToList();
			else StudentSuggestBox.ItemsSource = null;
		}

		private void StudentSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			Student student = studentList.Where(p => p.Name == args.SelectedItem.ToString()).Select(p => p).ToList()[0];
			if (listOfGoingStudent.Contains(student))
			{
				Views.SelectedItem = Going;
				GoingView.SelectedItem = student;
				GoingView.ScrollIntoView(student);
			}
			else if (listOfFinishedStudent.Contains(student))
			{
				Views.SelectedItem = Finished;
				FinishedView.SelectedItem = student;
				FinishedView.ScrollIntoView(student);
			}
			else if (listOfUnfinishedStudent.Contains(student))
			{
				Views.SelectedItem = Unfinished;
				UnfinishedView.SelectedItem = student;
				UnfinishedView.ScrollIntoView(student);
			}
			else
			{
				Views.SelectedItem = All;
				AllView.SelectedItem = student;
				AllView.ScrollIntoView(student);
			}
		}

		private void Grid_DragOver(object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Copy;
			

			e.DragUIOverride.Caption="拖入以导入";
			e.DragUIOverride.IsCaptionVisible = true;
			e.DragUIOverride.IsContentVisible = true;
			e.DragUIOverride.IsGlyphVisible = true;
		}

		private async void Grid_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items=await e.DataView.GetStorageItemsAsync();
				if (items.Any())
				{
					//if( (items[0]as StorageFile).ContentType == "text/txt")
					{
						StorageFile file = items[0] as StorageFile;
						await file.CopyAsync(dataSetFolder, file.Name, NameCollisionOption.ReplaceExisting);

						ConnectDataSet(items[0] as StorageFile);
					}
				}
			}
		}

		private async void Save_Click(object sender, RoutedEventArgs e)
		{
			file = await saveFolder.CreateFileAsync(DealWithSettings.ReadSettings("fileName"), CreationCollisionOption.OpenIfExists);
			SortedList<int, Student> updatedList = SumDataSets(studentList, listOfUnfinishedStudent, listOfGoingStudent, listOfFinishedStudent);
			await FileIO.WriteTextAsync(file, "");
			DealWithData.LayoutData(file, updatedList);

			DealWithSettings.WriteSettings("saved", "true");
			DealWithSettings.WriteSettings("fileName", file.Name);
			resultBox.Text = "保存成功";
		}

		private async void MarkUnfinished_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				if (GoingView.SelectedItem != null)
				{
					listOfGoingStudent[listOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					listOfUnfinishedStudent.Add((Student)GoingView.SelectedItem);
					listOfGoingStudent.Remove((Student)GoingView.SelectedItem);
					DealWithData.SortStudentData(ref listOfGoingStudent);
					GoingView.ItemsSource = null;
					GoingView.ItemsSource = listOfGoingStudent;
				}
				else if (FinishedView.SelectedItem != null)
				{
					listOfFinishedStudent[listOfFinishedStudent.IndexOf((Student)FinishedView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					listOfUnfinishedStudent.Add((Student)FinishedView.SelectedItem);
					listOfFinishedStudent.Remove((Student)FinishedView.SelectedItem);
					//DealWithData.SortStudentData(ref listOfGoingStudent);
					//GoingView.ItemsSource = null;
					//GoingView.ItemsSource = listOfGoingStudent;
				}

			}
			else resultBox.Text = "没有所需要的权限";
		}
	}
}
 