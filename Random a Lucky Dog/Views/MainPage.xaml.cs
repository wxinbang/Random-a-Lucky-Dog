using RLD.CPCore.Models;
using RLD.Services;
using RLD.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static RLD.CPCore.Models.StudentStatus;
using static RLD.UWPCore.KeyDictionary.SettingKey;
using static RLD.UWPCore.KeyDictionary.StringKey;
using static RLD.Services.DataSetService;
using static RLD.Services.FilesService;
using static RLD.Services.FoldersService;
using static RLD.Services.IdentityService;
using static RLD.UWPCore.LocalizeService;
using static RLD.Services.SettingsStorageService;
using static RLD.Services.StudentService;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.EmailProxy;

namespace RLD.Views
{
	public sealed partial class MainPage : Page
	{
		public MainViewModel ViewModel { get; } = new MainViewModel();

		ObservableCollection<Student> AllStudentList = (Application.Current as App).AllStudentList;
		ObservableCollection<Student> SortedGoingStudentList = new ObservableCollection<Student>();
		ObservableCollection<Student> FinishedStudentList = new ObservableCollection<Student>();
		ObservableCollection<Student> UnfinishedStudentList = new ObservableCollection<Student>();
		Collection<Student> OtherStudentList = new Collection<Student>();

		Random RandomStudent = new Random();
		DispatcherTimer Timer = new DispatcherTimer();
		private int timesOfVersionTextTapped;
		private bool mark;
		private int studentNumber;

		private StorageFile file;
		private StorageFolder SaveFolder;

		App app = (Application.Current as App);

		readonly long maxGCMemory = 255000000;
		public MainPage()
		{
			InitializeComponent();
		}
		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			//if (DealWithSettings.ReadSettings(DisplayMode) == null || DealWithSettings.ReadSettings(DisplayMode) == "True") DisplayMode.IsOn = true;
			Timer.Interval = new TimeSpan(0, 0, 0, 1);
			Timer.Tick += Timer_Tick;
			Timer.Start();
			/*
			if (DealWithSettings.ReadSettings(LastestError) != null) ;
			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Completed);
			ExtendAcrylicIntoTitleBar();
			*/
			SaveFolder = await GetSaveFolderAsync();

			if (app.IsDataPrepared)
			{
				file = await GetLastDataFileAsync();
				var collections = ClassifyStudents(app.AllStudentList);
				var list = collections.ToList();
				list.Insert(0, app.AllStudentList);
				SetColletions(list.ToArray());
			}
			else if (ReadString(FileName) != null)
			{
				file = await GetLastDataFileAsync();
				var collctions = await ConnectDataSetAsync(file, true);
				SetColletions(collctions);
				app.IsDataPrepared = true;
				app.NeedSave = false;
			}

			string version = VersionManager.GetVersion();
			SaveString(JoinProgram, "False");
#if (DEBUG)
			version += ".vNext";
			SaveString(JoinProgram, "True");
			DeveloperTools.Visibility = Visibility.Visible;
			GCInfo.Visibility = Visibility.Visible;
#endif
			VersionInformationBox.Text = version;
			AllStudentList.CollectionChanged += AllStudentList_CollectionChanged;
		}

		private void AllStudentList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			app.NeedSave = true;
		}

		private async void Timer_Tick(object sender, object e)
		{
			if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion) GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			else GCInfo.Style = (Style)Application.Current.Resources["SuccessDotInfoBadgeStyle"];
			if (await VerifyIdentityAsync()) IdentifyInfo.Visibility = Visibility.Visible;
			else IdentifyInfo.Visibility = Visibility.Collapsed;
		}
		private void RandomButton_Click(object sender, RoutedEventArgs e)
		{
			GoingView.ItemsSource = SortedGoingStudentList;
			DealWithStudentDataProgressBar.Maximum = AllStudentList.Count();

			UnfinishedView.SelectedItem = null;
			GoingView.SelectedItem = null;
			FinishedView.SelectedItem = null;
			AllView.SelectedItem = null;

			if (UnfinishedStudentList.Count != 0)
			{
				if (mark)
				{
					do studentNumber = RandomStudent.Next(0, UnfinishedStudentList.Count);
					while (UnfinishedStudentList[studentNumber].Status == StudentStatus.suspended);
				}
				else
				{
					do studentNumber = RandomStudent.Next(0, AllStudentList.Count);
					while (AllStudentList[studentNumber].Status == StudentStatus.suspended);
				}
				var Animationlist = RandomAnimation.KeyFrames;
				foreach (var item in Animationlist) item.Value = AllStudentList[RandomStudent.Next(AllStudentList.Count)].Name;
				AnimationBoard.Begin();
			}
			else ResultBox.Text = Localize(AllAlreadyFinished);//提示全部做过
		}
		private void Storyboard_Completed(object sender, object e)
		{
			if (mark)
			{
				ResultBox.Text = UnfinishedStudentList[studentNumber].Name;
				MoveStudentToTopOfCollection(UnfinishedStudentList[studentNumber], UnfinishedStudentList, SortedGoingStudentList, going);
				DealWithStudentDataProgressBar.Value = SortedGoingStudentList.Count;
				RefreshListNumber();

				Views.SelectedItem = Going;
				GoingView.SelectedIndex = 0;
				GoingView.ScrollIntoView(SortedGoingStudentList[0]);
			}
			else
			{
				ResultBox.Text = AllStudentList[studentNumber].Name;

				Views.SelectedItem = All;
				AllView.SelectedItem = AllStudentList[studentNumber];
				AllView.ScrollIntoView(AllStudentList[studentNumber]);
			}
		}
		private void PraiseButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			var students = ((ListView)((PivotItem)Views.SelectedItem).Content).SelectedItems;
			if (students != null && button.Name == "Praise") foreach (var student in students) AllStudentList[AllStudentList.IndexOf(student as Student)].PraiseTime++;
			else if (students != null && button.Name == "UndoPraise") foreach (var student in students) AllStudentList[AllStudentList.IndexOf(student as Student)].PraiseTime--;
		}
		private async void ConnectDataSet_Click(object sender, RoutedEventArgs e)
		{
			file = await SelectDataSetAsync();
			if (file != null && await VerifyIdentityAsync())
			{
				app.IsDataPrepared = false;
				var collections = await ConnectDataSetAsync(file);
				SetColletions(collections);
				app.IsDataPrepared = true;
			}
			else if (file == null) { ResultBox.Text = Localize(OperationCanceled); file = await GetLastDataFileAsync(); }
			else await ThrowException(Localize(NoRequiredPermissions));
			app.NeedSave = false;
		}
		private async void QuickCreate_Click(object sender, RoutedEventArgs e)
		{
			var number = ((int)QuickCreateBox.Value);
			if (number <= 0) await ThrowException(Localize(InputNotValid));
			else
			{
				app.IsDataPrepared = false;
				ClearAllList();
				for (int i = 1; i <= number; i++)
				{
					Student newStu = new Student(i.ToString(), unfinished, 0, 0, i - 1);
					AllStudentList.Add(newStu);
					UnfinishedStudentList.Add(newStu);
				}
				RefreshListNumber();
				app.IsDataPrepared = true;
			}
		}
		private async void WhetherMark_Toggled(object sender, RoutedEventArgs e)
		{
			if (WhetherMark.IsOn == true) WhetherMark.IsOn = await ContentDialogs.CheckMark();
			mark = WhetherMark.IsOn;
		}
		private async void VersionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
		{
			timesOfVersionTextTapped++;
			if (timesOfVersionTextTapped == 5) { await ContentDialogs.CheckJoinProgram(); timesOfVersionTextTapped = 0; }
		}
		private async void SendEmailButton_Click(object sender, RoutedEventArgs e)
		{
			await ComposeEmail();
		}
		private void ExitProgram_Click(object sender, RoutedEventArgs e)
		{/*
			SaveString(joinProgram, "False");
			InfoBar.Severity = InfoBarSeverity.Success;
			InfoBar.Message = "已退出预览模式，请尽快重启";
			MoreButton.Visibility = Visibility.Collapsed;
		*/
		}
		private async void LayoutDataSet_Click(object sender, RoutedEventArgs e)
		{
			if (await VerifyIdentityAsync())
			{
				var savePicker = new FileSavePicker
				{
					SuggestedStartLocation = PickerLocationId.ComputerFolder,
					SuggestedFileName = "After-" + file.Name
				};
				savePicker.FileTypeChoices.Add("文本文件", new List<string>() { ".txt" });
				StorageFile saveFile = await savePicker.PickSaveFileAsync();

				if (saveFile != null)
				{
					CachedFileManager.DeferUpdates(saveFile);
					await SaveStudentsAsync(saveFile, AllStudentList);
					FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(saveFile);
					if (status == FileUpdateStatus.Complete) this.ResultBox.Text = Localize(FileSaved) + saveFile.Name;
					else this.ResultBox.Text = Localize(FileNotSaved) + saveFile.Name;
				}
				else this.ResultBox.Text = Localize(OperationCanceled);
			}
			else await ThrowException(Localize(NoRequiredPermissions), false);
		}
		private void StudentSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{/*
			if (sender.Text == "") StudentSuggestBox.ItemsSource = null;
			else if (sender.Text != ""&& !isChoose)
			{
				//sender.Text = suggestBoxLastString;
				StudentSuggestBox.ItemsSource = ListOfAllStudent.Where(p => p.Name.Contains(sender.Text)).Select(p => p.Name).ToList();
			}
			isChoose = false;
		*/
		}
		private void Grid_DragOver(object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Link;


			e.DragUIOverride.Caption = "拖放以导入";
			e.DragUIOverride.IsCaptionVisible = true;
			e.DragUIOverride.IsContentVisible = true;
			e.DragUIOverride.IsGlyphVisible = true;
		}
		private void Grid_Drop(object sender, DragEventArgs e)
		{/*
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
		*/
		}
		public async void Save_Click(object sender, RoutedEventArgs e)
		{
			if (ReadString(FileName) != null && await VerifyIdentityAsync())
			{
				await SaveStudentsAsync(file.Name, AllStudentList);
				ResultBox.Text = Localize(FileSaved) + file.Name;
			}
			else if (!await VerifyIdentityAsync()) await ThrowException(Localize(NoRequiredPermissions));
		}
		private async void Mark_Click(object sender, RoutedEventArgs e)
		{
			if (await VerifyIdentityAsync())
			{
				ListView listView = (Views.SelectedItem as PivotItem).Content as ListView;
				IList<object> selectedStudents = listView.SelectedItems;
				string menuName = (sender as MenuFlyoutItem).Name;
				Collection<Student> toCollection = null;
				Collection<Student> fromCollection = null;
				StudentStatus status = unfinished;
				Collection<object> students = new Collection<object>();
				foreach (var item in selectedStudents) students.Add(item);

				if (menuName == "MarkFinished") { toCollection = FinishedStudentList; status = finished; }
				else if (menuName == "MarkGoing") { toCollection = SortedGoingStudentList; status = going; }
				else if (menuName == "MarkUnfinished") { toCollection = UnfinishedStudentList; status = unfinished; }

				if (students != null)
				{
					foreach (object item in students)
					{
						var student = item as Student;
						if (listView.Name == "GoingView" || (listView.Name == "AllView" && SortedGoingStudentList.Contains(student))) fromCollection = SortedGoingStudentList;
						else if (listView.Name == "FinishedView" || (listView.Name == "AllView" && FinishedStudentList.Contains(student))) fromCollection = FinishedStudentList;
						else if (listView.Name == "UnfinishedView" || (listView.Name == "AllView" && UnfinishedStudentList.Contains(student))) fromCollection = UnfinishedStudentList;
						else if (listView.Name == "AllView" && OtherStudentList.Contains(student)) fromCollection = OtherStudentList;
						MoveStudentToTopOfCollection((Student)item, fromCollection, toCollection, status);
					}
					RefreshListNumber();
					RefreshGoingOrder();
					if (menuName == "MarkFinished") Views.SelectedItem = Finished;
					else if (menuName == "MarkGoing") Views.SelectedItem = Going;
					else if (menuName == "MarkUnfinished") Views.SelectedItem = Unfinished;
					((Views.SelectedItem as PivotItem).Content as ListView).ScrollIntoView(toCollection[0]);
				}
			}
			else await ThrowException(Localize(NoRequiredPermissions));
		}
		private void RefreshListNumber()
		{
			AllNumber.Value = AllStudentList.Count;
			GoingNumber.Value = SortedGoingStudentList.Count;
			FinishedNumber.Value = FinishedStudentList.Count;
			UnfinishedNumber.Value = UnfinishedStudentList.Count;
		}
		private async void OpenGC_Click(object sender, RoutedEventArgs e)
		{
			if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
			{
				GC.EndNoGCRegion();
				GCInfo.Style = (Style)Application.Current.Resources["SuccessDotInfoBadgeStyle"];
				await ThrowException("已开启GC", false);
			}
		}
		private void GCNow_Click(object sender, RoutedEventArgs e)
		{
			GC.Collect();
		}
		private async void CloseGC_Click(object sender, RoutedEventArgs e)
		{
			if (GC.TryStartNoGCRegion(maxGCMemory))
			{
				await ThrowException("已关闭GC", false);
				GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			}
		}
		private void SetColletions(Collection<Student>[] collections)
		{
			ClearAllList();
			if (!app.IsDataPrepared) foreach (var sb in collections[0]) AllStudentList.Add(sb);
			foreach (var sb in collections[1]) SortedGoingStudentList.Add(sb);
			foreach (var sb in collections[2]) FinishedStudentList.Add(sb);
			foreach (var sb in collections[3]) UnfinishedStudentList.Add(sb);
			foreach (var sb in collections[4]) OtherStudentList.Add(sb);
			RefreshListNumber();
		}
		private void RefreshGoingOrder()
		{
			foreach (var item in SortedGoingStudentList) item.OrderOfGoing = SortedGoingStudentList.Count() - SortedGoingStudentList.IndexOf(item);
			GoingView.ItemsSource = null;
			GoingView.ItemsSource = SortedGoingStudentList;
		}
		private void ClearAllList()
		{
			if (!app.IsDataPrepared) AllStudentList.Clear();
			SortedGoingStudentList.Clear();
			FinishedStudentList.Clear();
			UnfinishedStudentList.Clear();
			OtherStudentList.Clear();
			RefreshListNumber();
		}
		internal void TurnToStudent(Student student)
		{
			ListView listView;
			if (SortedGoingStudentList.Contains(student)) { Views.SelectedItem = Going; listView = GoingView; }
			else if (FinishedStudentList.Contains(student)) { Views.SelectedItem = Finished; listView = FinishedView; }
			else if (UnfinishedStudentList.Contains(student)) { Views.SelectedItem = Unfinished; listView = UnfinishedView; }
			else { Views.SelectedItem = All; listView = AllView; }
			listView.SelectedItem = student;
			listView.ScrollIntoView(listView.SelectedItem);
		}

		private async void Lab_Click(object sender, RoutedEventArgs e)
		{
			await UWPCore.Service.SecurityService.PickDeviceAsync();
        }
    }
}
