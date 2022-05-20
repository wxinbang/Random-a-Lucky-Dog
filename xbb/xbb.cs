﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace xbb
{
	public class Student
	{
		public string Name { get; set; }
		public StudentStatus StudentStatus { get; set; }
		public int OrderOfGoing { get; set; }
	}

	public enum StudentStatus //状态
	{
		unfinished,
		going,//进行中
		finished,
		suspended,//暂停的
		error
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
		public static async Task CreateLog(string content,TaskStatus status)
		{
			StorageFolder logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("logs", CreationCollisionOption.OpenIfExists);
			StorageFile logFile=await logFolder.CreateFileAsync(DateTime.Now.ToString("yyyy-MM-dd")+".txt", CreationCollisionOption.OpenIfExists);
			await FileIO.AppendTextAsync(logFile, DateTime.Now.TimeOfDay.ToString()+"    "+status.ToString()+"    "+content+"\n");
		}
		
		public static async void LayoutLogs(StorageFile file,string resultBoxText)
		{

		}
	}
	
	
	//public static class DealWithDictionary
	//{
	//	public static void WriteToDictionary(Dictionary<string, string> dataDictionary, string key, string value)
	//	{
	//		StorageFolder localFolder = ApplicationData.Current.LocalFolder;
	//		if (dataDictionary.ContainsKey(key)) dataDictionary[key] = value;
	//		else dataDictionary.Add(key, value);
	//	}

	//	public static string ReadFromDicionary(Dictionary<string, string> dictonary, string key)
	//	{
	//		if (dictonary.ContainsKey(key)) return dictonary[key];
	//		else return "-1";
	//	}

	//	public static async void ReadDataFileToDictionary(Dictionary<string, string> dictionary)
	//	{
	//		/*
	//		StorageFolder folder = ApplicationData.Current.LocalFolder;
	//		StorageFile file = await folder.CreateFileAsync("data.txt", CreationCollisionOption.OpenIfExists);

	//		IList<string> data = await FileIO.ReadLinesAsync(file);

	//		int positionOfSpace;
	//		string resultKey;
	//		string resultValue;

	//		foreach (string dataLine in data)
	//		{
	//			positionOfSpace = dataLine.IndexOf(' ');
	//			resultKey = dataLine.Substring(0, positionOfSpace);
	//			resultValue = dataLine.Substring(positionOfSpace + 1);

	//			dictionary.Add(resultKey, resultValue);
	//		}
	//		*/
	//	}

	//	public static async void WriteDictionaryToDataFile(Dictionary<string, string> dictionary)
	//	{
	//		StorageFolder localFolder = ApplicationData.Current.LocalFolder;
	//		StorageFile storageFile = await localFolder.CreateFileAsync("data.txt", CreationCollisionOption.ReplaceExisting);

	//		string output;
	//		foreach (string key in dictionary.Keys)
	//		{
	//			output = key + " " + dictionary[key] + "\n";
	//			await FileIO.WriteTextAsync(storageFile, output);
	//		}

	//	}

	//	public static async void WriteDictionaryToFile(Dictionary<int ,Student>dictionary,string fileName)
	//	{
	//		StorageFolder folder = ApplicationData.Current.LocalFolder;
	//		StorageFile file=await folder.CreateFileAsync("After-"+fileName, CreationCollisionOption.ReplaceExisting);

	//		foreach (Student student in dictionary.Values)
	//		{
	//			await FileIO.AppendTextAsync(file, student.Name + " " +  DealWithData.ConvertStatus(student.StudentStatus)+student.OrderOfGoing+"\n");
	//		}
	//	}
	//}


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

		public static async void LayoutData(StorageFile file,List<Student> students)
		{
			//StorageFolder folder = ApplicationData.Current.LocalFolder;
			//StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			foreach (Student student in students) await FileIO.AppendTextAsync(file, student.Name + "\t" + ConvertStatus(student.StudentStatus) + "\t" + (student.StudentStatus==StudentStatus.going ? student.OrderOfGoing.ToString() + "\n":"\n"));
		}
	}

	public static class DealWithSettings
	{
		public static string ReadSettings(string settingKey)
		{
			ApplicationDataContainer setting = ApplicationData.Current.LocalSettings;
			
			string settingValue = setting.Values[settingKey] as string;
			return settingValue;
		}

		public static void WriteSettings(string settingKey,string settingValue)
		{
			ApplicationDataContainer setting =ApplicationData.Current.LocalSettings;
			setting.Values[settingKey]=settingValue;
		}
	}

	public static class ContentDialogs
	{

	}
}