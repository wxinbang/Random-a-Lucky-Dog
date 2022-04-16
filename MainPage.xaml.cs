using System;
using System.Collections.Generic;
//using System.Data;
//using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 抽人
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<int,Student> studentDictionary=new Dictionary<int, Student>();
#if(DEBUG)
        string version = "Build 1.0.4.0.prealpha.220405-1012";//220413-1900
        //bool whetherDeveloping = true;
#else
        string version = "1.0.4-Beta";
        //bool whetherDeveloping = false;
#endif
       
        int timesOfPraise =0;
        int studentNumber;
        int sumOfStudent;
        bool finishedMark=false;
        bool mark = false;

        string DataSetPath;
        string IdString;
        string NameString;
        string StatueString;
        string MarkString;

        int unfinishedNumber;
        StorageFile file;

        string readableFilePath;

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
                DataSetPath= file.Path;
                //if (StorageFile.)File.Delete(readableFilePath);
                await file.CopyAsync(readableFolderPath,file.Name,NameCollisionOption.ReplaceExisting);

                // Application now has read/write access to the picked file
                
            }
            else
            {
                this.resultBox.Text = "Operation cancelled.";
            }
        }

        private void whetherMark_Toggled(object sender, RoutedEventArgs e)
        {
            mark=whetherMark.IsOn;
            //加用户决定
            if (studentNumber != 0) studentDictionary[studentNumber].StudentStatus = mark ? Status.going : Status.unfinished;
        }

        private static async void CheckWhetherMark()
        {
            ContentDialog whetherMarkDialog = new ContentDialog
            {
                Title = "再次确认",
                Content = @"以后的人都要标记状态为“进行中”？",//记得改双引号
                CloseButtonText = "别了吧"
            };

            ContentDialogResult result = await whetherMarkDialog.ShowAsync();
        }


        private async void connectDataSet_Click(object sender, RoutedEventArgs e)
        {
            string line;//定义读取行
            sumOfStudent = 0;

            //StreamReader sr = new StreamReader(readableFilePath);

            IList<string> contents = await FileIO.ReadLinesAsync(file);
            sumOfStudent=contents.ToArray().Length;

            bool[] checkId=new bool[sumOfStudent];
            for(int j=0;j<sumOfStudent;j++)
            {
//创建一个动态bool数组checkId并全部初始化为false

                string[] studentData = new string[4];
                studentData = DealWithStudentData(contents[j]);

                Student Somebody=new Student() { Id=Convert.ToInt32(studentData[0]),Name=studentData[1],IsMarked=true,StudentStatus=ConvertStatus(studentData[3])};
                //sumOfStudent++;
                //contents.RemoveAt(0);

                if (Somebody.StudentStatus == Status.unfinished) unfinishedNumber++;
                checkId[Somebody.Id-1] = true;
                studentDictionary.Add(Somebody.Id, Somebody);
            }

            //sr.Close();

            int i = 0;
            while ((i < sumOfStudent)&&(checkId[i] == true)) i++;
            if (i == sumOfStudent) resultBox.Text = "连接完成";
            else if (sumOfStudent == 0) resultBox.Text = "人数为0或1 无法继续操作";
            else resultBox.Text = "编号为" + i+1.ToString ()+ "的人出现问题";
        }

        public string[] DealWithStudentData (string dataLine)
        {
            string[] studentData = new string[4];

            dataLine.Trim();

            /*
            studentData[0].Trim();
            studentData[1].Trim();
            studentData[2].Trim();
            studentData[3].Trim();//简化
            */

            int dataLineLenth = dataLine.Length, i = 0;
            char[] DataArray = dataLine.ToCharArray();

            while (i<dataLineLenth&&DataArray[i] != ' ' && DataArray[i] != '\t')
            {
                studentData[0] += DataArray[i];
                i++;
            }
            while (i < dataLineLenth && (DataArray[i] == ' '||DataArray[i] == '\t')) i++;

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

            while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
            {
                studentData[3] += DataArray[i];
                i++;
            }
            return studentData;//记得简化
        }

        public Status ConvertStatus(string status)
        {
            if ((status == "unfinished") || (status == "")) return Status.unfinished;
            else if (status == "going") return Status.going;
            else if (status == "finished") return Status.finished;
            else if (status == "suspended") return Status.suspended;
            else if (status == "error") return Status.error;
            else return Status.unfinished;
        }

        public string ConvertStatus(Status status)
        {
            if (status == Status.unfinished) return "unfinished";
            else if (status == Status.going) return "going";
            else if (status == Status.finished) return "finished";
            else if (status == Status.suspended) return "suspended";
            else return "error";
}


    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMarked { get; set; }
        public Status StudentStatus { get; set; }
    }

     public enum Status //状态
    {
        unfinished,
        going,//进行中
        finished,
        suspended,//暂停的
        error
    }
}
