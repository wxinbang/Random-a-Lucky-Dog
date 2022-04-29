using System;
using System.Collections.Generic;
//using System.Data;
//using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Pickers;
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
        Dictionary<int, Student> studentDictionary = new Dictionary<int, Student>();
        Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
#if(DEBUG)
        string version = "Build 1.0.4.0.prealpha.220405-1012";//220413-1900 220424-2138 220427-2203
#else
        string version = "2.2.10-Beta";
#endif

        int timesOfVersionTextTapped = 0;

        int timesOfPraise = 0;
        int studentNumber;
        int sumOfStudent;
        bool finishedMark = false;
        bool mark = false;
        string DataSetPath;
        string IdString;
        string NameString;
        string StatueString;
        string fileName;
        string MarkString;

        int unfinishedNumber;
        StorageFile file;

        string readableFilePath;
        bool whetherJoInsiderPreviewProgram;

        // 加入初始化时读写配置文件

        public MainPage()
        {
            this.InitializeComponent();

            versionInformationBox.Text = version;
        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            if (unfinishedNumber != 0)
            {

                Random randomStudent = new Random();

                do studentNumber = randomStudent.Next(1, sumOfStudent + 1);
                while (studentDictionary[studentNumber].StudentStatus != Status.unfinished);

                resultBox.Text = studentDictionary[studentNumber].Name;

                if (mark) studentDictionary[studentNumber].StudentStatus = Status.going;
            }
            else resultBox.Text = "已经全部抽过了";//提示全部做过
        }

        private void praiseButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayInvalidPraise();
        }

        private static async void DisplayInvalidPraise()
        {
            ContentDialog invalidPraise = new ContentDialog
            {
                Title = "小小的提示",
                Content = "这玩意没用",
                CloseButtonText = "行"
            };

            ContentDialogResult result = await invalidPraise.ShowAsync();
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

            var readableFolderPath = ApplicationData.Current.LocalFolder;
            //readableFilePath=readableFolderPath+@"\"+file.Name;

            if (file != null)
            {
                readableFilePath = readableFolderPath + @"\" + file.Name;
                DataSetPath = file.Path;
                //if (StorageFile.)File.Delete(readableFilePath);
                await file.CopyAsync(readableFolderPath, file.Name, NameCollisionOption.ReplaceExisting);
                DealWithDictionary.WriteToDictionary(dataDictionary, "fileName", file.Name);
                // Application now has read/write access to the picked file

            }
            else
            {
                this.resultBox.Text = "Operation cancelled.";
            }
        }

        private void whetherMark_Toggled(object sender, RoutedEventArgs e)
        {
            if (whetherMark.IsOn == true) CheckWhetherMark();
            else
            {
                mark = whetherMark.IsOn;
                DealWithSettings.WriteSettings("mark",mark ? "True" : "False");
            }
            //加用户决定
            //if (studentNumber != 0) studentDictionary[studentNumber].StudentStatus = mark ? Status.going : Status.unfinished;
        }

        private async void CheckWhetherMark()
        {
            ContentDialog whetherMarkDialog = new ContentDialog
            {
                Title = "再次确认",
                Content = @"以后的人都要标记状态为“进行中”？",//记得改双引号
                CloseButtonText = "别了吧",
                PrimaryButtonText = "是的"                
            };

            ContentDialogResult result = await whetherMarkDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                mark = true;
                DealWithSettings.WriteSettings("mark", "True");
            }
            else
            {
                mark = false;
                whetherMark.IsOn = false;
                DealWithSettings.WriteSettings("mark", "False");
            }
        }

        private void connectDataSet_Click(object sender, RoutedEventArgs e)
        {
            ConnetDataSet(file.Name);
        }

        private async void ConnetDataSet(string fileName)
        {
            studentDictionary.Clear();
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync(fileName);

            string line;//定义读取行
            sumOfStudent = 0;

            //StreamReader sr = new StreamReader(readableFilePath);

            IList<string> contents = await FileIO.ReadLinesAsync(file);
            sumOfStudent = contents.ToArray().Length;
            dealWithStudentDataProgressBar.Maximum = sumOfStudent;

            bool[] checkId = new bool[sumOfStudent];
            for (int j = 0; j < sumOfStudent; j++)
            {
                //创建一个动态bool数组checkId并全部初始化为false

                string[] studentData = new string[4];
                studentData = DealWithData.DealWithStudentData(contents[j]);

                Student Somebody = new Student() { Id = Convert.ToInt32(studentData[0]), Name = studentData[1], IsMarked = true, StudentStatus = DealWithData.ConvertStatus(studentData[3]) };
                //sumOfStudent++;
                //contents.RemoveAt(0);

                if (Somebody.StudentStatus == Status.unfinished) unfinishedNumber++;
                checkId[Somebody.Id - 1] = true;
                studentDictionary.Add(Somebody.Id, Somebody);
                dealWithStudentDataProgressBar.Value = j + 1;
            }

            //sr.Close();

            int i = 0;
            while ((i < sumOfStudent) && (checkId[i] == true)) i++;
            if (i == sumOfStudent) resultBox.Text = "连接完成";
            else if (sumOfStudent == 0) resultBox.Text = "人数为0或1 无法继续操作";
            else resultBox.Text = "编号为" + i + 1.ToString() + "的人出现问题";
            //DealWithDictionary.WriteToDictionary(dataDictionary,"fileName",fileName);
            DealWithSettings.WriteSettings("fileName", fileName);
        }

        private void versionInformationBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            timesOfVersionTextTapped++;
            if (timesOfVersionTextTapped == 5)
            {
                CheckWhetherJoinInsiderPreviewProgram();
            }

        }
        private async void CheckWhetherJoinInsiderPreviewProgram()
        {
            ContentDialog invalidPraise = new ContentDialog
            {
                Title = "体验新功能",
                Content = "这会让你体验到更多的新特性和新特性（自行体会），确定？",
                PrimaryButtonText = "来！搞！",
                CloseButtonText = "不了"
            };

            ContentDialogResult result = await invalidPraise.ShowAsync();
            if (result == ContentDialogResult.Primary) DealWithSettings.WriteSettings("whetherJoinInsiderPreviewProgram", "True");
            else DealWithSettings.WriteSettings("whetherJoinInsiderPreviewProgram", "False");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            DealWithDictionary.ReadDataFileToDictionary(dataDictionary);

            string TrueOrFalse = DealWithDictionary.ReadFromDicionary(dataDictionary, "whetherJoinInsiderPreviewProgram");
            whetherJoInsiderPreviewProgram = TrueOrFalse == "-1" ? false : Convert.ToBoolean(TrueOrFalse);
            if (DealWithDictionary.ReadFromDicionary(dataDictionary, "fileName") != "-1") ConnetDataSet(DealWithDictionary.ReadFromDicionary(dataDictionary,"fileName"));
            */
            if (DealWithSettings.ReadSettings("fileName") != null)
            {
                ConnetDataSet(DealWithSettings.ReadSettings("fileName"));
                fileName=DealWithSettings.ReadSettings("fileName");

            }
            if (DealWithSettings.ReadSettings("whetherJoinInsiderPreviewProgram") != "True") layOutButton.Visibility = Visibility.Collapsed;
            if (DealWithSettings.ReadSettings("mark") == "True") whetherMark.IsOn = true;
        }

        private void layOutButton_Click(object sender, RoutedEventArgs e)
        {
            DealWithDictionary.WriteDictionaryToFile(studentDictionary, fileName);
            
        }
    }
}