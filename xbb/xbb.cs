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
    class DealWithDictionary
    {
        public void WriteToDictionary(Dictionary<string, string> dataDictionary, string key, string value)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            if (dataDictionary.ContainsKey(key)) dataDictionary[key] = value;
            else dataDictionary.Add(key, value);
        }

        public async void s(Dictionary<string, string> dictionary)
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

        public async void WriteDictionaryToDataFile(Dictionary<string, string> dictionary)
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
}