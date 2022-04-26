using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace xbb
{
    public class Student
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
    public static class DealWithDictionary
    {
        public static void WriteToDictionary(Dictionary<string, string> dataDictionary, string key, string value)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            if (dataDictionary.ContainsKey(key)) dataDictionary[key] = value;
            else dataDictionary.Add(key, value);
        }

        public static string ReadFromDicionary(Dictionary<string,string>dictonary,string key)
        {
            if (dictonary.ContainsKey(key)) return dictonary[key];
            else return "-1";
        }

        public static async void ReadDataFileToDictionary(Dictionary<string, string> dictionary)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("data.txt", CreationCollisionOption.OpenIfExists);

            IList<string> data = await FileIO.ReadLinesAsync(file);

            int positionOfSpace;
            string resultKey;
            string resultValue;

            foreach (string dataLine in data)
            {
                positionOfSpace = dataLine.IndexOf(' ');
                resultKey = dataLine.Substring(0, positionOfSpace);
                resultValue = dataLine.Substring(positionOfSpace + 1);

                dictionary.Add(resultKey, resultValue);
            }
        }

        public static async void WriteDictionaryToDataFile(Dictionary<string, string> dictionary)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile storageFile = await localFolder.CreateFileAsync("data.txt",CreationCollisionOption.ReplaceExisting);

            string output;
            foreach (string key in dictionary.Keys)
            {
                output = key + " " + dictionary[key] + "\n";
                await FileIO.WriteTextAsync(storageFile, output);
            }

        }

        
    }

    public static class DealWithData
    {
        public static string[] DealWithStudentData(string dataLine)
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

            while (i < dataLineLenth && DataArray[i] != ' ' && DataArray[i] != '\t')
            {
                studentData[3] += DataArray[i];
                i++;
            }
            return studentData;//记得简化
        }

        public static Status ConvertStatus(string status)
        {
            if ((status == "unfinished") || (status == "")) return Status.unfinished;
            else if (status == "going") return Status.going;
            else if (status == "finished") return Status.finished;
            else if (status == "suspended") return Status.suspended;
            else if (status == "error") return Status.error;
            else return Status.unfinished;
        }
        
        public static string ConvertStatus(Status status)
        {
            if (status == Status.unfinished) return "unfinished";
            else if (status == Status.going) return "going";
            else if (status == Status.finished) return "finished";
            else if (status == Status.suspended) return "suspended";
            else return "error";
        }

        public static void ConnectDataSet()
        {

        }
    }
}