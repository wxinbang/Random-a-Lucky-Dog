using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using xbb;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 抽人
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class MainPage : Page
	{
		ObservableCollection<Student> studentList = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfUnfinishedStudent = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfGoingStudent = new ObservableCollection<Student>();

		ObservableCollection<Student> listOfFinishedStudent = new ObservableCollection<Student>();

		SortedList<int, Student> lastGoingStudent = new SortedList<int, Student>();

		string version = string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

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
		Exception ex;

		string readableFilePath;
		bool whetherJoInsiderPreviewProgram;

		DispatcherTimer timer = new DispatcherTimer();

		bool importantIdentity;

		readonly long maxGCMemory = 255000000;

		public MainPage()
		{
			this.InitializeComponent();
#if (DEBUG)
			if (GCSettings.LatencyMode==GCLatencyMode.NoGCRegion&& GC.TryStartNoGCRegion(maxGCMemory))
			{
				ContentDialogs.ThrowException("已关闭GC", false);
				GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			}
			version += ".vNext";
			AppTitleTextBlock.Text += " - Developing";

			DeveloperTools.Visibility = Visibility.Visible;
			GCInfo.Visibility = Visibility.Visible;
#endif
			versionInformationBox.Text = version;

			CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
			Window.Current.SetTitleBar(AppTitleBar);
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (DealWithSettings.ReadSettings(SettingKey.DisplayMode) == null|| DealWithSettings.ReadSettings(SettingKey.DisplayMode) == "true") DisplayMode.IsOn = true;
			timer.Interval = new TimeSpan(0, 0, 0, 1);
			timer.Tick += Timer_Tick;
			timer.Start();

			dataSetFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
			saveFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);

			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Trying);
			if (DealWithSettings.ReadSettings(SettingKey.fileName) != null)
			{
				if (DealWithSettings.ReadSettings(SettingKey.saved) != "true") file = await dataSetFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.fileName));
				else file = await saveFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.fileName));
				ConnectDataSet(file);
				fileName = DealWithSettings.ReadSettings(SettingKey.fileName);
			}
			if (DealWithSettings.ReadSettings(SettingKey.joinProgram) != "true")
			{
				InfoBar.IsOpen = false;
				//layOutDataSetButton.Visibility = Visibility.Collapsed;
				layOutFlyoutButton.Visibility = Visibility.Collapsed;
				Views.Visibility = Visibility.Collapsed;
				DeleteButton.Visibility = Visibility.Collapsed;
				OperateStudent.Visibility = Visibility.Collapsed;
				StudentSuggestBox.Visibility = Visibility.Collapsed;

			}
			if (DealWithSettings.ReadSettings(SettingKey. mark) == "true") whetherMark.IsOn = true;
			if (DealWithSettings.ReadSettings(SettingKey.LastestError) != null) ;
			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Completed);
		}
		private async void Timer_Tick(object sender, object e)
		{
			if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			else GCInfo.Style = (Style)Application.Current.Resources["SuccessDotInfoBadgeStyle"];
			if (await DealWithIdentity.VerifyIdentity()) IdentifyInfo.Visibility = Visibility.Visible;
			else IdentifyInfo.Visibility = Visibility.Collapsed;
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

					var Animationlist = RandomAnimation.KeyFrames;
					foreach (var item in Animationlist) item.Value = studentList[randomStudent.Next(studentList.Count)].Name;
					Storyboard.Begin();
				}
				else
				{
					do studentNumber = randomStudent.Next(0, studentList.Count);
					while (studentList[studentNumber].StudentStatus == StudentStatus.suspended);

					var Animationlist = RandomAnimation.KeyFrames;
					foreach (var item in Animationlist) item.Value = studentList[randomStudent.Next(studentList.Count)].Name;
					Storyboard.Begin();

					resultBox.Text = studentList[studentNumber].Name;
				}
			}
			else resultBox.Text = "已经全部抽过了";//提示全部做过
		}
		private void Storyboard_Completed(object sender, object e)
		{
			if (mark)
			{
				resultBox.Text = listOfUnfinishedStudent[studentNumber].Name;
				listOfUnfinishedStudent[studentNumber].StudentStatus = StudentStatus.going;
				listOfGoingStudent.Insert(0, listOfUnfinishedStudent[studentNumber]);
				listOfGoingStudent[0].OrderOfGoing = listOfGoingStudent.Count;
				listOfUnfinishedStudent.RemoveAt(studentNumber);
				dealWithStudentDataProgressBar.Value = listOfGoingStudent.Count;
				RefreshListNumber();
			}
		}
		private void praiseButton_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogs.DisplayInvalidPraise();
		}
		private async void selectDataSetButton_Click(object sender, RoutedEventArgs e)
		{
			//unfinishedNumber = 0;
			//studentNumber = 0;

			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.List;
			picker.SuggestedStartLocation = PickerLocationId.Desktop;
			picker.FileTypeFilter.Add(".txt");//记得检查

			file = await picker.PickSingleFileAsync();

			if (file != null)
			{
				await file.CopyAsync(dataSetFolder, file.Name, NameCollisionOption.ReplaceExisting);
				resultBox.Text = "已选择：" + file.Name;
			}
			else
			{
				this.resultBox.Text = "操作已取消";
			}
			DealWithSettings.DeleteSettings(SettingKey.saved);
		}
		private void connectDataSet_Click(object sender, RoutedEventArgs e)
		{
			ConnectDataSet(file);
		}
		private async void ConnectDataSet(StorageFile file)
		{
			try
			{
				studentList.Clear();
				listOfGoingStudent.Clear();
				listOfUnfinishedStudent.Clear();
				lastGoingStudent.Clear();
				listOfFinishedStudent.Clear();

				IList<string> contents = await FileIO.ReadLinesAsync(file);
				//sumOfStudent = contents.ToArray().Length;
				dealWithStudentDataProgressBar.Maximum = sumOfStudent;
				int orderInList = 0;
				//bool[] checkId = new bool[sumOfStudent];
				foreach (string content in contents)
				{
					string[] studentData = DealWithData.DealWithStudentData(content);

					Student Somebody = new Student() { Name = studentData[0], StudentStatus = DealWithData.ConvertStatus(studentData[1]), OrderOfGoing = Convert.ToInt32(studentData[2]), OrderInList = orderInList++ };

					if (Somebody.StudentStatus == StudentStatus.unfinished) listOfUnfinishedStudent.Add(Somebody);
					else if (Somebody.StudentStatus == StudentStatus.going) lastGoingStudent.Add(Somebody.OrderOfGoing, Somebody);
					else if (Somebody.StudentStatus == StudentStatus.finished) listOfFinishedStudent.Add(Somebody);

					studentList.Add(Somebody);
					dealWithStudentDataProgressBar.Value = contents.IndexOf(content) + 1;
				}

				foreach (var someBody in lastGoingStudent) listOfGoingStudent.Insert(0, someBody.Value);

				resultBox.Text = "连接完成：" + file.Name;
				RefreshListNumber();
				DealWithSettings.WriteSettings(SettingKey.fileName, file.Name);
			}
			catch (Exception ex)
			{
				ContentDialogs.ThrowException(ex.ToString());
			}
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
				DealWithSettings.WriteSettings(SettingKey.mark, mark ? "true" : "False");
			}
		}
		private void versionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
		{
			timesOfVersionTextTapped++;
			if (timesOfVersionTextTapped == 5)
			{
				ContentDialogs.CheckJoinProgram();
				timesOfVersionTextTapped = 0;
			}

		}
		private void SendEmailButton_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogs.ComposeEmail();
		}
		private void ExitProgram_Click(object sender, RoutedEventArgs e)
		{
			DealWithSettings.WriteSettings(SettingKey.joinProgram, "False");
			InfoBar.Severity = InfoBarSeverity.Success;
			InfoBar.Message = "已退出预览模式，请尽快重启";
			MoreButton.Visibility = Visibility.Collapsed;
		}
		private async void LayoutDataSet_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				await Save_Click(sender, e, false);
				//SortedList<int, Student> updatedList = DealWithData.SumDataSets(studentList, listOfUnfinishedStudent, listOfGoingStudent, listOfFinishedStudent);
				string afterFileName = "After-" + fileName;

				var savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
				savePicker.FileTypeChoices.Add("文本文件", new List<string>() { ".txt" });
				savePicker.SuggestedFileName = afterFileName;

				StorageFile file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					CachedFileManager.DeferUpdates(file);
					await FileIO.WriteTextAsync(file, "");
					StorageFile saved = await saveFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.fileName));
					await saved.CopyAndReplaceAsync(file);
					//DealWithData.LayoutData(file, updatedList);
					FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
					if (status == FileUpdateStatus.Complete) this.resultBox.Text = "文件 " + file.Name + " 已被保存";
					else this.resultBox.Text = "文件 " + file.Name + " 未被保存";
				}
				else this.resultBox.Text = "操作已取消";
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);

		}
		private void LayoutUserData_Click(object sender, RoutedEventArgs e)
		{

		}
		private void LayoutLogs_Click(object sender, RoutedEventArgs e)
		{

		}
		private void LayoutIdentityFile_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogs.LayoutIdentityFile();
		}
		private void DeleteLogFile_Click(object sender, RoutedEventArgs e)
		{

		}
		private async void DeleteDataSet_Click(object sender, RoutedEventArgs e)
		{
			var ToDeleteItems = await dataSetFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			ToDeleteItems = await saveFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			DealWithSettings.DeleteSettings(SettingKey.fileName);
			DealWithSettings.DeleteSettings(SettingKey.saved);
			resultBox.Text = "删除完成";
		}
		private void DeleteUserData_Click(object sender, RoutedEventArgs e)
		{
			DealWithSettings.DeleteSettings();
			resultBox.Text = "删除完成";
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
			e.AcceptedOperation = DataPackageOperation.Link;


			e.DragUIOverride.Caption = "拖入以导入";
			e.DragUIOverride.IsCaptionVisible = true;
			e.DragUIOverride.IsContentVisible = true;
			e.DragUIOverride.IsGlyphVisible = true;
		}
		private async void Grid_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var items = await e.DataView.GetStorageItemsAsync();
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
		public async void Save_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				file = await saveFolder.CreateFileAsync(DealWithSettings.ReadSettings(SettingKey.fileName), CreationCollisionOption.OpenIfExists);
				SortedList<int, Student> updatedList = DealWithData.SumDataSets(studentList, listOfUnfinishedStudent, listOfGoingStudent, listOfFinishedStudent);
				await FileIO.WriteTextAsync(file, "");
				await DealWithData.LayoutData(file, updatedList);

				DealWithSettings.WriteSettings(SettingKey.saved, "true");
				DealWithSettings.WriteSettings(SettingKey.fileName, file.Name);
				resultBox.Text = "保存成功";
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
		}
		public async Task Save_Click(object sender, RoutedEventArgs e, bool showResult = true)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				file = await saveFolder.CreateFileAsync(DealWithSettings.ReadSettings(SettingKey.fileName), CreationCollisionOption.OpenIfExists);
				SortedList<int, Student> updatedList = DealWithData.SumDataSets(studentList, listOfUnfinishedStudent, listOfGoingStudent, listOfFinishedStudent);
				await FileIO.WriteTextAsync(file, "");
				await DealWithData.LayoutData(file, updatedList);

				DealWithSettings.WriteSettings(SettingKey.saved, "true");
				DealWithSettings.WriteSettings(SettingKey.fileName, file.Name);
				if (showResult) resultBox.Text = "保存成功";
			}
			else if (showResult) ContentDialogs.ThrowException("没有所需要的权限", false);
			return;

		}
		private async void MarkFinished_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				if (Views.SelectedItem == Going && GoingView.SelectedItem != null)
				{
					listOfGoingStudent[listOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.finished;
					listOfFinishedStudent.Add((Student)GoingView.SelectedItem);
					listOfGoingStudent.Remove((Student)GoingView.SelectedItem);
					DealWithData.SortStudentData(ref listOfGoingStudent);
					GoingView.ItemsSource = null;
					GoingView.ItemsSource = listOfGoingStudent;
				}
				else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
				{
					listOfUnfinishedStudent[listOfUnfinishedStudent.IndexOf((Student)UnfinishedView.SelectedItem)].StudentStatus = StudentStatus.finished;
					listOfFinishedStudent.Add((Student)UnfinishedView.SelectedItem);
					listOfUnfinishedStudent.Remove((Student)UnfinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作",false);
				//else if (Views.SelectedItem == All && AllView.SelectedItem != null)
				//{
				//	studentList[studentList.IndexOf((Student)AllView.SelectedItem)].StudentStatus = StudentStatus.finished;
				//	listOfFinishedStudent.Add((Student)AllView.SelectedItem);
				//	listOfUnfinishedStudent.Remove((Student)AllView.SelectedItem);//检查！！！
				//}
				RefreshListNumber();
			}
			else if (await DealWithIdentity.VerifyIdentity() == false) ContentDialogs.ThrowException("没有所需要的权限", false);

		}
		private async void MarkUnfinished_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				if (Views.SelectedItem == Going && GoingView.SelectedItem != null)
				{
					listOfGoingStudent[listOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					listOfUnfinishedStudent.Add((Student)GoingView.SelectedItem);
					listOfGoingStudent.Remove((Student)GoingView.SelectedItem);
					DealWithData.SortStudentData(ref listOfGoingStudent);
					GoingView.ItemsSource = null;
					GoingView.ItemsSource = listOfGoingStudent;
				}
				else if (Views.SelectedItem == Finished && FinishedView.SelectedItem != null)
				{
					listOfFinishedStudent[listOfFinishedStudent.IndexOf((Student)FinishedView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					listOfUnfinishedStudent.Add((Student)FinishedView.SelectedItem);
					listOfFinishedStudent.Remove((Student)FinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作",false);
				RefreshListNumber();
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
		}
		private async void MarkGoing_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				if (Views.SelectedItem == Finished && FinishedView.SelectedItem != null)
				{
					listOfFinishedStudent[listOfFinishedStudent.IndexOf((Student)FinishedView.SelectedItem)].StudentStatus = StudentStatus.going;
					listOfGoingStudent.Insert(0, (Student)FinishedView.SelectedItem);
					listOfGoingStudent[0].OrderOfGoing = listOfGoingStudent.Count;
					listOfFinishedStudent.Remove((Student)FinishedView.SelectedItem);
				}
				else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
				{
					listOfUnfinishedStudent[listOfUnfinishedStudent.IndexOf((Student)UnfinishedView.SelectedItem)].StudentStatus = StudentStatus.going;
					listOfGoingStudent.Insert(0, (Student)UnfinishedView.SelectedItem);
					listOfGoingStudent[0].OrderOfGoing = listOfGoingStudent.Count;
					listOfUnfinishedStudent.Remove((Student)UnfinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作",false);
				//else if (Views.SelectedItem == All && AllView.SelectedItem != null)
				//{
				//	studentList[studentList.IndexOf((Student)AllView.SelectedItem)].StudentStatus = StudentStatus.going;
				//	listOfGoingStudent.Insert(0, (Student)AllView.SelectedItem);
				//	listOfGoingStudent[0].OrderOfGoing = listOfGoingStudent.Count;
				//}
				RefreshListNumber();
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);

		}
		private void RefreshListNumber()
		{
			AllNumber.Value = studentList.Count;
			GoingNumber.Value = listOfGoingStudent.Count;
			FinishedNumber.Value = listOfFinishedStudent.Count;
			UnfinishedNumber.Value = listOfUnfinishedStudent.Count;
		}
		private void DisplayMode_Toggled(object sender, RoutedEventArgs e)
		{
			if (DisplayMode.IsOn)
			{
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, false);
				BackgroundGrid.Background = (Brush)Application.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"];
				DealWithSettings.WriteSettings(SettingKey.DisplayMode, "true");
				//ExtendAcrylicIntoTitleBar();
			}
			else
			{
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, true);
				BackgroundGrid.Background = null;
				DealWithSettings.WriteSettings(SettingKey.DisplayMode, "false");
				//ExtendAcrylicIntoTitleBar();
			}
		}
		private void ExtendAcrylicIntoTitleBar()
		{
			ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
			titleBar.ButtonBackgroundColor = Colors.Transparent;
			titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
		}
		private void OpenGC_Click(object sender, RoutedEventArgs e)
		{
			if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
			{
				GC.EndNoGCRegion();
				GCInfo.Style = (Style)Application.Current.Resources["SuccessDotInfoBadgeStyle"];
				ContentDialogs.ThrowException("已开启GC", false);
			}
		}

		private void GCNow_Click(object sender, RoutedEventArgs e)
		{
			GC.Collect();
		}

		private void CloseGC_Click(object sender, RoutedEventArgs e)
		{
			if (GC.TryStartNoGCRegion(maxGCMemory))
			{
				ContentDialogs.ThrowException("已关闭GC", false);
				GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			}
		}

		private void showAnimation_Click(object sender, RoutedEventArgs e)
		{
			if (ModalLayer.Visibility == Visibility.Visible) ModalLayer.Visibility = Visibility.Collapsed;
			else ModalLayer.Visibility = Visibility.Visible;
		}
	}
}
