using System;
using System.Collections.ObjectModel;
using Select_Lucky_Dog.Core.Models;
using static Select_Lucky_Dog.Core.Services.StudentService;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Services.SettingsStorageService;
using static Select_Lucky_Dog.Services.DataSetService;
using static Select_Lucky_Dog.Services.LocalizeService;
using Select_Lucky_Dog.ViewModels;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.UI.Xaml;
using System.Runtime;
using xbb.ClassLibraries;
using Windows.UI.ViewManagement;
using Windows.UI;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Provider;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Windows.Storage.Pickers;
using Select_Lucky_Dog.Services;
using Windows.ApplicationModel.Resources;
using static Select_Lucky_Dog.Helpers.KeyDictionary.StringKey;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using static Select_Lucky_Dog.Core.Models.StudentStatus;
using Select_Lucky_Dog.Helpers;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        ObservableCollection<Student> AllStudentList = new ObservableCollection<Student>();
        Collection<Student> goingStudentList = new Collection<Student>();
        ObservableCollection<Student> FinishedStudentList = new ObservableCollection<Student>();
        ObservableCollection<Student> UnfinishedStudentList = new ObservableCollection<Student>();
        Collection<Student> OtherStudentList = new Collection<Student>();
        ObservableCollection<Student> SortedGoingStudentList = new ObservableCollection<Student>();

        Random RandomStudent = new Random();
        DispatcherTimer Timer = new DispatcherTimer();
        private bool isChoose;
        private int timesOfVersionTextTapped;
        private bool mark;
        private int studentNumber;

        private StorageFile file;
        private StorageFolder DataSetFolder;
        private StorageFolder SaveFolder;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {/*
			if (DealWithSettings.ReadSettings(DisplayMode) == null || DealWithSettings.ReadSettings(DisplayMode) == "true") DisplayMode.IsOn = true;
			Timer.Interval = new TimeSpan(0, 0, 0, 1);
			Timer.Tick += Timer_Tick;
			Timer.Start();

			if (DealWithSettings.ReadSettings(joinProgram) != "true")
			{
				InfoBar.IsOpen = false;
				//layOutDataSetButton.Visibility = Visibility.Collapsed;
				//layOutFlyoutButton.Visibility = Visibility.Collapsed;
				//Views.Visibility = Visibility.Collapsed;
				DeleteButton.Visibility = Visibility.Collapsed;
				//OperateStudent.Visibility = Visibility.Collapsed;
				//StudentSuggestBox.Visibility = Visibility.Collapsed;

			}

			if (DealWithSettings.ReadSettings(LastestError) != null) ;
			//DealWithLogs.CreateLog("ReadSettings", xbb.TaskStatus.Completed);
			ExtendAcrylicIntoTitleBar();
			*/
            DataSetFolder = await GetDataSetFolderAsync();
            SaveFolder = await GetSavesFolderAsync();

            if (ReadString(FileName) != null)
            {
                if (ReadString(Saved) != "True") file = await DataSetFolder.GetFileAsync(ReadString(FileName));
                else file = await SaveFolder.GetFileAsync(ReadString(FileName));

                var collctions = await ConnectDataSetAsync(file, true);
                SetColletions(collctions);
            }

            string version = VersionManager.GetVersion();
#if (DEBUG)

            version += ".vNext";

            SaveString(JoinProgram, "true");
#endif
            VersionInformationBox.Text = version;
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
            GoingView.ItemsSource = SortedGoingStudentList;
            DealWithStudentDataProgressBar.Maximum = AllStudentList.Count();

            if (UnfinishedStudentList.Count != 0)
            {
                if (mark)
                {
                    do studentNumber = RandomStudent.Next(0, UnfinishedStudentList.Count);
                    while (UnfinishedStudentList[studentNumber].Status == StudentStatus.suspended);

                    var Animationlist = RandomAnimation.KeyFrames;
                    foreach (var item in Animationlist) item.Value = AllStudentList[RandomStudent.Next(AllStudentList.Count)].Name;
                    AnimationBoard.Begin();
                }
                else
                {
                    do studentNumber = RandomStudent.Next(0, AllStudentList.Count);
                    while (AllStudentList[studentNumber].Status == StudentStatus.suspended);

                    var Animationlist = RandomAnimation.KeyFrames;
                    foreach (var item in Animationlist) item.Value = AllStudentList[RandomStudent.Next(AllStudentList.Count)].Name;
                    AnimationBoard.Begin();

                    ResultBox.Text = AllStudentList[studentNumber].Name;
                }
            }

            else ResultBox.Text = Localize(AllAlreadyFinished);//提示全部做过
        }
        private void Storyboard_Completed(object sender, object e)
        {
            if (mark)
            {
                ResultBox.Text = UnfinishedStudentList[studentNumber].Name;
                MoveStudentToTopOfCollection(UnfinishedStudentList[studentNumber], UnfinishedStudentList, SortedGoingStudentList, going);
                //UnfinishedStudentList[studentNumber].Status = StudentStatus.going;
                ////ListOfGoingStudent.Insert(0, ListOfUnfinishedStudent[studentNumber]);
                //SortedGoingStudentList[0].OrderOfGoing = SortedGoingStudentList.Count;
                //ListOfUnfinishedStudent.RemoveAt(studentNumber);
                //MoveToTopOfCollection<Student>(UnfinishedStudentList[studentNumber], UnfinishedStudentList, GoingStudentList);
                DealWithStudentDataProgressBar.Value = SortedGoingStudentList.Count;
                RefreshListNumber();
            }
        }
        private async void PraiseButton_Click(object sender, RoutedEventArgs e)
        {
            await ContentDialogs.Praise();
        }
        private async void ConnectDataSet_Click(object sender, RoutedEventArgs e)
        {
            await ImportDataSetAsync();
        }
        private async void WhetherMark_Toggled(object sender, RoutedEventArgs e)
        {
            if (WhetherMark.IsOn == true) WhetherMark.IsOn = await ContentDialogs.CheckMark();
            //else SaveString(Mark, WhetherMark.IsOn ? "true" : "False");
            mark = WhetherMark.IsOn;
        }
        private async void VersionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            timesOfVersionTextTapped++;
            if (timesOfVersionTextTapped == 5)
            {
                await ContentDialogs.CheckJoinProgram();
                timesOfVersionTextTapped = 0;
            }

        }
        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            //ContentDialogs.ComposeEmail();
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
        {/*
			if (await DealWithIdentity.VerifyIdentity())
			{
				//SortedList<int, Student> updatedList = DealWithData.SumDataSets(ListOfAllStudent, ListOfUnfinishedStudent, ListOfGoingStudent, FinishedStudentList);
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
					StorageFile saved = await SaveFolder.GetFileAsync(DealWithSettings.ReadSettings(FileName));
					await saved.CopyAndReplaceAsync(saveFile);
					//DealWithData.LayoutData(file, updatedList);
					FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(saveFile);
					if (status == FileUpdateStatus.Complete) this.ResultBox.Text = "文件 " + saveFile.Name + " 已被保存";
					else this.ResultBox.Text = "文件 " + saveFile.Name + " 未被保存";
				}
				else this.ResultBox.Text = "操作已取消";
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
*/
        }
        private void LayoutUserData_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LayoutLogs_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LayoutIdentityFile_Click(object sender, RoutedEventArgs e)
        {
            //ContentDialogs.LayoutIdentityFile();
        }
        private void DeleteLogFile_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void DeleteDataSet_Click(object sender, RoutedEventArgs e)
        {/*
			var ToDeleteItems = await DataSetFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			ToDeleteItems = await SaveFolder.GetItemsAsync();
			foreach (var item in ToDeleteItems) await item.DeleteAsync();
			DealWithSettings.DeleteSettings(FileName);
			DealWithSettings.DeleteSettings(saved);
			ResultBox.Text = "删除完成";
		*/
        }
        private void DeleteUserData_Click(object sender, RoutedEventArgs e)
        {
            DealWithSettings.DeleteSettings();
            ResultBox.Text = "删除完成";
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
        private void StudentSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Student student = AllStudentList.Where(p => p.Name == args.SelectedItem.ToString()).Select(p => p).ToList()[0];
            isChoose = true;
            if (SortedGoingStudentList.Contains(student))
            {
                Views.SelectedItem = Going;
                GoingView.SelectedItem = student;
                GoingView.ScrollIntoView(student);
            }
            else if (FinishedStudentList.Contains(student))
            {
                Views.SelectedItem = Finished;
                FinishedView.SelectedItem = student;
                FinishedView.ScrollIntoView(student);
            }
            else if (UnfinishedStudentList.Contains(student))
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
        private void GoTOStudent()
        {
            ;
        }
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;


            e.DragUIOverride.Caption = "拖放以导入";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }
        private async void Grid_Drop(object sender, DragEventArgs e)
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
        {/*
			if (await DealWithIdentity.VerifyIdentity())
			{
				SortedList<int, Student> updatedList = DealWithData.SumDataSets(ListOfAllStudent, ListOfUnfinishedStudent, ListOfGoingStudent, FinishedStudentList);
				await FileIO.WriteTextAsync(file, "");
				await DealWithData.LayoutData(file, updatedList);

				ResultBox.Text = "保存成功";
			}
			else ContentDialogs.ThrowException("没有所需要的权限", false);
		*/
            var collection = CollectionService.MergeCollections(UnfinishedStudentList, FinishedStudentList, goingStudentList, OtherStudentList);
            await SaveStudentsAsync(await SaveFolder.CreateFileAsync(ReadString(FileName), CreationCollisionOption.OpenIfExists), collection);
            SaveString(Saved, "True");
            SaveString(FileName, file.Name);
            ResultBox.Text = Localize(FileSaved);


        }
        private async void MarkFinished_Click(object sender, RoutedEventArgs e)
        {
            if (await DealWithIdentity.VerifyIdentity())
            {
                if (Views.SelectedItem == Going && GoingView.SelectedItem != null)
                {
                    SortedGoingStudentList[SortedGoingStudentList.IndexOf((Student)GoingView.SelectedItem)].Status = StudentStatus.finished;
                    FinishedStudentList.Add((Student)GoingView.SelectedItem);
                    SortedGoingStudentList.Remove((Student)GoingView.SelectedItem);
                    UpgradeGoingStudentData(ref SortedGoingStudentList);
                    GoingView.ItemsSource = null;
                    GoingView.ItemsSource = SortedGoingStudentList;
                }
                else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
                {
                    UnfinishedStudentList[UnfinishedStudentList.IndexOf((Student)UnfinishedView.SelectedItem)].Status = StudentStatus.finished;
                    FinishedStudentList.Add((Student)UnfinishedView.SelectedItem);
                    UnfinishedStudentList.Remove((Student)UnfinishedView.SelectedItem);
                }
                else ContentDialogs.ThrowException("暂时进行不了这样的操作", false);
                //else if (Views.SelectedItem == All && AllView.SelectedItem != null)
                //{
                //	ListOfAllStudent[ListOfAllStudent.IndexOf((Student)AllView.SelectedItem)].StudentStatus = StudentStatus.finished;
                //	FinishedStudentList.Add((Student)AllView.SelectedItem);
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
                    SortedGoingStudentList[SortedGoingStudentList.IndexOf((Student)GoingView.SelectedItem)].Status = StudentStatus.unfinished;
                    UnfinishedStudentList.Add((Student)GoingView.SelectedItem);
                    SortedGoingStudentList.Remove((Student)GoingView.SelectedItem);
                    UpgradeGoingStudentData(ref SortedGoingStudentList);
                    GoingView.ItemsSource = null;
                    GoingView.ItemsSource = SortedGoingStudentList;
                }
                else if (Views.SelectedItem == Finished && FinishedView.SelectedItem != null)
                {
                    FinishedStudentList[FinishedStudentList.IndexOf((Student)FinishedView.SelectedItem)].Status = StudentStatus.unfinished;
                    UnfinishedStudentList.Add((Student)FinishedView.SelectedItem);
                    FinishedStudentList.Remove((Student)FinishedView.SelectedItem);
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
                    FinishedStudentList[FinishedStudentList.IndexOf((Student)FinishedView.SelectedItem)].Status = StudentStatus.going;
                    SortedGoingStudentList.Insert(0, (Student)FinishedView.SelectedItem);
                    SortedGoingStudentList[0].OrderOfGoing = SortedGoingStudentList.Count;
                    FinishedStudentList.Remove((Student)FinishedView.SelectedItem);
                }
                else if (Views.SelectedItem == Unfinished && UnfinishedView.SelectedItem != null)
                {
                    UnfinishedStudentList[UnfinishedStudentList.IndexOf((Student)UnfinishedView.SelectedItem)].Status = StudentStatus.going;
                    SortedGoingStudentList.Insert(0, (Student)UnfinishedView.SelectedItem);
                    SortedGoingStudentList[0].OrderOfGoing = SortedGoingStudentList.Count;
                    UnfinishedStudentList.Remove((Student)UnfinishedView.SelectedItem);
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
            AllNumber.Value = AllStudentList.Count;
            GoingNumber.Value = SortedGoingStudentList.Count;
            FinishedNumber.Value = FinishedStudentList.Count;
            UnfinishedNumber.Value = UnfinishedStudentList.Count;
        }
        private void DisplayMode_Toggled(object sender, RoutedEventArgs e)
        {/*
			if (DisplayMode.IsOn)
			{//new ResourceDictionary()
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, false);
				BackgroundGrid.Background = (Brush)Application.Current.Resources["AcrylicBackgroundFillColorDefaultBrush"];
				SaveString(DisplayMode, "true");
				ExtendAcrylicIntoTitleBar();
			}
			else
			{
				BackdropMaterial.SetApplyToRootOrPageBackground(mainPage, true);
				BackgroundGrid.Background = null;
				SaveString(DisplayMode, "false");
				ExtendAcrylicIntoTitleBar();
			}
		*/
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
        {/*
			if (GC.TryStartNoGCRegion(maxGCMemory))
			{
				ContentDialogs.ThrowException("已关闭GC", false);
				GCInfo.Style = (Style)Application.Current.Resources["CriticalDotInfoBadgeStyle"];
			}
		*/
        }

        private void ShowAnimation_Click(object sender, RoutedEventArgs e)
        {
            //ModalLayer.Visibility = Visibility.Visible;
        }

        private void CloseAnimationButton_Click(object sender, RoutedEventArgs e)
        {
            //ModalLayer.Visibility = Visibility.Collapsed;
        }
        private void SetColletions(Collection<Student>[] collections)
        {
            foreach (var sb in collections[0]) AllStudentList.Add(sb);
            foreach (var sb in collections[1]) SortedGoingStudentList.Add(sb);
            foreach (var sb in collections[2]) FinishedStudentList.Add(sb);
            foreach (var sb in collections[3]) UnfinishedStudentList.Add(sb);
            foreach (var sb in collections[4]) OtherStudentList.Add(sb);
            RefreshListNumber();
        }
    }
}
