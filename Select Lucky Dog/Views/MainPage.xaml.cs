using System;
using System.Collections.ObjectModel;
using Select_Lucky_Dog.Core.Models;
using static Select_Lucky_Dog.Core.Services.StudentService;
using Select_Lucky_Dog.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();
        ObservableCollection<Student> ListOfAllStudent = new ObservableCollection<Student>();
        ObservableCollection<Student> ListOfGoingStudent = new ObservableCollection<Student>();
        ObservableCollection<Student> ListOfFinishedStudnet = new ObservableCollection<Student>();
        ObservableCollection<Student> ListOfUnfinishedStudent = new ObservableCollection<Student>();
        List<Student> ListOfOtherStudent = new List<Student>();
        Random RandomStudent = new Random();
        DispatcherTimer Timer = new DispatcherTimer();
        public MainPage()
        {
            InitializeComponent();
            //ObservableCollection<Student> ListOfAllStudent = GetStudentsAsync(file);
#if (DEBUG)
			if (GCSettings.LatencyMode==GCLatencyMode.NoGCRegion&& GC.TryStartNoGCRegion(maxGCMemory))
			{
				ContentDialogs.ThrowException("GCClosedAlready", false);
				GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			}
			version += ".vNext";
			AppTitleTextBlock.Text += " - Developing";

			DealWithSettings.WriteSettings(SettingKey.joinProgram, "true");

			DeveloperTools.Visibility = Visibility.Visible;
			GCInfo.Visibility = Visibility.Visible;
#endif
			VersionInformationBox.Text = version;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (DealWithSettings.ReadSettings(SettingKey.DisplayMode) == null || DealWithSettings.ReadSettings(SettingKey.DisplayMode) == "true") DisplayMode.IsOn = true;
			Timer.Interval = new TimeSpan(0, 0, 0, 1);
			Timer.Tick += Timer_Tick;
			Timer.Start();

			DataSetFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataSets", CreationCollisionOption.OpenIfExists);
			SaveFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Saves", CreationCollisionOption.OpenIfExists);

			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Trying);
			if (DealWithSettings.ReadSettings(SettingKey.FileName) != null)
			{
				if (DealWithSettings.ReadSettings(SettingKey.saved) != "true") file = await DataSetFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.FileName));
				else file = await SaveFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.FileName));
				ConnectDataSet(file,true);
				FileName = DealWithSettings.ReadSettings(SettingKey.FileName);
			}
			if (DealWithSettings.ReadSettings(SettingKey.joinProgram) != "true")
			{
				InfoBar.IsOpen = false;
				//layOutDataSetButton.Visibility = Visibility.Collapsed;
				//layOutFlyoutButton.Visibility = Visibility.Collapsed;
				//Views.Visibility = Visibility.Collapsed;
				DeleteButton.Visibility = Visibility.Collapsed;
				//OperateStudent.Visibility = Visibility.Collapsed;
				//StudentSuggestBox.Visibility = Visibility.Collapsed;

			}
			if (DealWithSettings.ReadSettings(SettingKey.mark) == "true") WhetherMark.IsOn = true;
			if (DealWithSettings.ReadSettings(SettingKey.LastestError) != null) ;
			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Completed);
			ExtendAcrylicIntoTitleBar();
		}
		private async void Timer_Tick(object sender, object e)
		{
			if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			else GCInfo.Style = (Style)Application.Current.Resources["SuccessDotInfoBadgeStyle"];
			if (await DealWithIdentity.VerifyIdentity()) IdentifyInfo.Visibility = Visibility.Visible;
			else IdentifyInfo.Visibility = Visibility.Collapsed;
		}
		private void RandomButton_Click(object sender, RoutedEventArgs e)
		{
			GoingView.ItemsSource = ListOfGoingStudent;
			DealWithStudentDataProgressBar.Maximum = ListOfAllStudent.Count();

			if (ListOfUnfinishedStudent.Count != 0)
			{
				if (mark)
				{
					do studentNumber = RandomStudent.Next(0, ListOfUnfinishedStudent.Count);
					while (ListOfUnfinishedStudent[studentNumber].StudentStatus == StudentStatus.suspended);

					var Animationlist = RandomAnimation.KeyFrames;
					foreach (var item in Animationlist) item.Value = ListOfAllStudent[RandomStudent.Next(ListOfAllStudent.Count)].Name;
					Storyboard.Begin();
				}
				else
				{
					do studentNumber = RandomStudent.Next(0, ListOfAllStudent.Count);
					while (ListOfAllStudent[studentNumber].StudentStatus == StudentStatus.suspended);

					var Animationlist = RandomAnimation.KeyFrames;
					foreach (var item in Animationlist) item.Value = ListOfAllStudent[RandomStudent.Next(ListOfAllStudent.Count)].Name;
					Storyboard.Begin();

					ResultBox.Text = ListOfAllStudent[studentNumber].Name;
				}
			}
			else ResultBox.Text = ResourceLoader.GetForCurrentView().GetString("AllAlreadyFinished");//提示全部做过
		}
		private void Storyboard_Completed(object sender, object e)
		{
			if (mark)
			{
				ResultBox.Text = ListOfUnfinishedStudent[studentNumber].Name;
				ListOfUnfinishedStudent[studentNumber].StudentStatus = StudentStatus.going;
				//ListOfGoingStudent.Insert(0, ListOfUnfinishedStudent[studentNumber]);
				ListOfGoingStudent[0].OrderOfGoing = ListOfGoingStudent.Count;
				//ListOfUnfinishedStudent.RemoveAt(studentNumber);
				MoveToTopOfCollection(ListOfUnfinishedStudent[studentNumber],ListOfUnfinishedStudent,ListOfGoingStudent)
				dealWithStudentDataProgressBar.Value = ListOfGoingStudent.Count;
				RefreshListNumber();
			}
		}
        private void MoveToTopOfCollection(T sth,Collection<T> FromCollection,Collection<T> ToCollection)
        {
            ToCollection.Insert(0,T);
            FromCollection.RemoveAt(FromCollection.IndexOf(T));
        }
		private void PraiseButton_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogs.DisplayInvalidPraise();
		}
		private async void SelectDataSetButton_Click(object sender, RoutedEventArgs e)
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
				await file.CopyAsync(DataSetFolder, file.Name, NameCollisionOption.ReplaceExisting);
				ResultBox.Text = "已选择：" + file.Name;
			}
			else
			{
				this.ResultBox.Text = "操作已取消";
			}
			DealWithSettings.DeleteSettings(SettingKey.saved);
		}
		private void ConnectDataSet_Click(object sender, RoutedEventArgs e)
		{
			ConnectDataSet(file);
		}
		private async void ConnectDataSet(StorageFile file,bool NotVerifyIdentity=false)
		{
			if (await DealWithIdentity.VerifyIdentity()||NotVerifyIdentity)
			{
				ListOfAllStudent.Clear();
				ListOfGoingStudent.Clear();
				ListOfUnfinishedStudent.Clear();
				lastGoingStudent.Clear();
				ListOfFinishedStudent.Clear();
				ListOfOtherStudent.Clear();
				IList<string> contents = await FileIO.ReadLinesAsync(file);
				//sumOfStudent = contents.ToArray().Length;
				while(contents.Last()=="")contents.RemoveAt(contents.Count()-1);
				dealWithStudentDataProgressBar.Maximum = contents.Count();
				int orderInList = 0;
				//bool[] checkId = new bool[sumOfStudent];
				foreach (string content in contents)
				{
					string[] studentData = DealWithData.DealWithStudentData(content);
					Student Somebody = new Student()
					{ 	studentData[0], 
						DealWithData.ConvertStatus(studentData[1]),
					 	Convert.ToByte(studentData[2]), 
						orderInList++ 
					};
					SortStudent(Somebody);
					ListOfAllStudent.Add(Somebody);
					dealWithStudentDataProgressBar.Value = contents.IndexOf(content) + 1;
				}
				foreach (var someBody in lastGoingStudent) ListOfGoingStudent.Insert(0, someBody.Value);
				ResultBox.Text = "连接完成：" + file.Name;
				RefreshListNumber();
				DealWithSettings.WriteSettings(SettingKey.FileName, file.Name);
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
		}
		private void SortStudent(Student Somebody)
		{
			if (Somebody.StudentStatus == StudentStatus.unfinished) ListOfUnfinishedStudent.Add(Somebody);
			else if (Somebody.StudentStatus == StudentStatus.going) lastGoingStudent.Add(Somebody.OrderOfGoing, Somebody);
			else if (Somebody.StudentStatus == StudentStatus.finished) ListOfFinishedStudent.Add(Somebody);
			else ListOfOtherStudent.Add(Somebody);
		}
		private async void WhetherMark_Toggled(object sender, RoutedEventArgs e)
		{
			if (WhetherMark.IsOn == true)WhetherMark.IsOn = await ContentDialogs.CheckWhetherMark();
			else DealWithSettings.WriteSettings(SettingKey.mark, WhetherMark.IsOn ? "true" : "False");
		}
		private void VersionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
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
				//SortedList<int, Student> updatedList = DealWithData.SumDataSets(ListOfAllStudent, ListOfUnfinishedStudent, ListOfGoingStudent, ListOfFinishedStudent);
				string afterFileName = "After-" + file.Name;

				var savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
				savePicker.FileTypeChoices.Add("文本文件", new List<string>() { ".txt" });
				savePicker.SuggestedFileName = afterFileName;

				StorageFile saveFile = await savePicker.PickSaveFileAsync();
				if (saveFile != null)
				{
					await Save_Click(sender, e, false);
					CachedFileManager.DeferUpdates(saveFile);
					await FileIO.WriteTextAsync(saveFile, "");
					StorageFile saved = await SaveFolder.GetFileAsync(DealWithSettings.ReadSettings(SettingKey.FileName));
					await saved.CopyAndReplaceAsync(saveFile);
					//DealWithData.LayoutData(file, updatedList);
					FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(saveFile);
					if (status == FileUpdateStatus.Complete) this.ResultBox.Text = "文件 " + saveFile.Name + " 已被保存";
					else this.ResultBox.Text = "文件 " + saveFile.Name + " 未被保存";
				}
				else this.ResultBox.Text = "操作已取消";
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
			var ToDeleteItems = await DataSetFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			ToDeleteItems = await SaveFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			DealWithSettings.DeleteSettings(SettingKey.FileName);
			DealWithSettings.DeleteSettings(SettingKey.saved);
			ResultBox.Text = "删除完成";
		}
		private void DeleteUserData_Click(object sender, RoutedEventArgs e)
		{
			DealWithSettings.DeleteSettings();
			ResultBox.Text = "删除完成";
		}
		private void StudentSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (sender.Text == "") StudentSuggestBox.ItemsSource = null;
			else if (sender.Text != ""&& !isChoose)
			{
				//sender.Text = suggestBoxLastString;
				StudentSuggestBox.ItemsSource = ListOfAllStudent.Where(p => p.Name.Contains(sender.Text)).Select(p => p.Name).ToList();
			}
			isChoose = false;
		}
		private void StudentSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			Student student = ListOfAllStudent.Where(p => p.Name == args.SelectedItem.ToString()).Select(p => p).ToList()[0];
			isChoose = true;
			if (ListOfGoingStudent.Contains(student))
			{
				Views.SelectedItem = Going;
				GoingView.SelectedItem = student;
				GoingView.ScrollIntoView(student);
			}
			else if (ListOfFinishedStudent.Contains(student))
			{
				Views.SelectedItem = Finished;
				FinishedView.SelectedItem = student;
				FinishedView.ScrollIntoView(student);
			}
			else if (ListOfUnfinishedStudent.Contains(student))
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
		private void GoTOStudent(Pivot pivot,)
		private void Grid_DragOver(object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Link;


			e.DragUIOverride.Caption = "拖放以导入";
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
						file = items[0] as StorageFile;
						await file.CopyAsync(DataSetFolder, file.Name, NameCollisionOption.ReplaceExisting);

						ConnectDataSet(items[0] as StorageFile);
					}
				}
			}
		}
		public async void Save_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				file = await SaveFolder.CreateFileAsync(DealWithSettings.ReadSettings(SettingKey.FileName), CreationCollisionOption.OpenIfExists);
				SortedList<int, Student> updatedList = DealWithData.SumDataSets(ListOfAllStudent, ListOfUnfinishedStudent, ListOfGoingStudent, ListOfFinishedStudent);
				await FileIO.WriteTextAsync(file, "");
				await DealWithData.LayoutData(file, updatedList);

				DealWithSettings.WriteSettings(SettingKey.saved, "true");
				DealWithSettings.WriteSettings(SettingKey.FileName, file.Name);
				ResultBox.Text = "保存成功";
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
		}
		public async Task<bool> Save_Click(object sender, RoutedEventArgs e, bool showResult = true)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				file = await SaveFolder.CreateFileAsync(DealWithSettings.ReadSettings(SettingKey.FileName), CreationCollisionOption.OpenIfExists);
				SortedList<int, Student> updatedList = DealWithData.SumDataSets(ListOfAllStudent, ListOfUnfinishedStudent, ListOfGoingStudent, ListOfFinishedStudent);
				await FileIO.WriteTextAsync(file, "");
				await DealWithData.LayoutData(file, updatedList);

				DealWithSettings.WriteSettings(SettingKey.saved, "true");
				DealWithSettings.WriteSettings(SettingKey.FileName, file.Name);
				if (showResult) ResultBox.Text = "保存成功";
				return true;
			}
			else if (showResult) ContentDialogs.ThrowException("没有所需要的权限", false);
			return false;

		}
		private async void MarkFinished_Click(object sender, RoutedEventArgs e)
		{
			if (await DealWithIdentity.VerifyIdentity())
			{
				if (Views.SelectedItem == Going && GoingView.SelectedItem != null)
				{
					ListOfGoingStudent[ListOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.finished;
					ListOfFinishedStudent.Add((Student)GoingView.SelectedItem);
					ListOfGoingStudent.Remove((Student)GoingView.SelectedItem);
					DealWithData.SortStudentData(ref ListOfGoingStudent);
					GoingView.ItemsSource = null;
					GoingView.ItemsSource = ListOfGoingStudent;
				}
				else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
				{
					ListOfUnfinishedStudent[ListOfUnfinishedStudent.IndexOf((Student)UnfinishedView.SelectedItem)].StudentStatus = StudentStatus.finished;
					ListOfFinishedStudent.Add((Student)UnfinishedView.SelectedItem);
					ListOfUnfinishedStudent.Remove((Student)UnfinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作", false);
				//else if (Views.SelectedItem == All && AllView.SelectedItem != null)
				//{
				//	ListOfAllStudent[ListOfAllStudent.IndexOf((Student)AllView.SelectedItem)].StudentStatus = StudentStatus.finished;
				//	ListOfFinishedStudent.Add((Student)AllView.SelectedItem);
				//	ListOfUnfinishedStudent.Remove((Student)AllView.SelectedItem);//检查！！！
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
					ListOfGoingStudent[ListOfGoingStudent.IndexOf((Student)GoingView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					ListOfUnfinishedStudent.Add((Student)GoingView.SelectedItem);
					ListOfGoingStudent.Remove((Student)GoingView.SelectedItem);
					DealWithData.SortStudentData(ref ListOfGoingStudent);
					GoingView.ItemsSource = null;
					GoingView.ItemsSource = ListOfGoingStudent;
				}
				else if (Views.SelectedItem == Finished && FinishedView.SelectedItem != null)
				{
					ListOfFinishedStudent[ListOfFinishedStudent.IndexOf((Student)FinishedView.SelectedItem)].StudentStatus = StudentStatus.unfinished;
					ListOfUnfinishedStudent.Add((Student)FinishedView.SelectedItem);
					ListOfFinishedStudent.Remove((Student)FinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作", false);
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
					ListOfFinishedStudent[ListOfFinishedStudent.IndexOf((Student)FinishedView.SelectedItem)].StudentStatus = StudentStatus.going;
					ListOfGoingStudent.Insert(0, (Student)FinishedView.SelectedItem);
					ListOfGoingStudent[0].OrderOfGoing = ListOfGoingStudent.Count;
					ListOfFinishedStudent.Remove((Student)FinishedView.SelectedItem);
				}
				else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
				{
					ListOfUnfinishedStudent[ListOfUnfinishedStudent.IndexOf((Student)UnfinishedView.SelectedItem)].StudentStatus = StudentStatus.going;
					ListOfGoingStudent.Insert(0, (Student)UnfinishedView.SelectedItem);
					ListOfGoingStudent[0].OrderOfGoing = ListOfGoingStudent.Count;
					ListOfUnfinishedStudent.Remove((Student)UnfinishedView.SelectedItem);
				}
				else ContentDialogs.ThrowException("暂时进行不了这样的操作", false);
				//else if (Views.SelectedItem == All && AllView.SelectedItem != null)
				//{
				//	ListOfAllStudent[ListOfAllStudent.IndexOf((Student)AllView.SelectedItem)].StudentStatus = StudentStatus.going;
				//	ListOfGoingStudent.Insert(0, (Student)AllView.SelectedItem);
				//	ListOfGoingStudent[0].OrderOfGoing = ListOfGoingStudent.Count;
				//}
				RefreshListNumber();
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);

		}
		private void RefreshListNumber()
		{
			AllNumber.Value = ListOfAllStudent.Count;
			GoingNumber.Value = ListOfGoingStudent.Count;
			FinishedNumber.Value = ListOfFinishedStudent.Count;
			UnfinishedNumber.Value = ListOfUnfinishedStudent.Count;
		}
		private void DisplayMode_Toggled(object sender, RoutedEventArgs e)
		{
			if (DisplayMode.IsOn)
			{//new ResourceDictionary()
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, false);
				BackgroundGrid.Background = (Brush)Application.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"];
				DealWithSettings.WriteSettings(SettingKey.DisplayMode, "true");
				ExtendAcrylicIntoTitleBar();
			}
			else
			{
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, true);
				BackgroundGrid.Background = null;
				DealWithSettings.WriteSettings(SettingKey.DisplayMode, "false");
				ExtendAcrylicIntoTitleBar();
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
			ModalLayer.Visibility = Visibility.Visible;
		}

		private void CloseAnimationButton_Click(object sender, RoutedEventArgs e)
		{
			ModalLayer.Visibility = Visibility.Collapsed;
		}
	}
}

    }
}
