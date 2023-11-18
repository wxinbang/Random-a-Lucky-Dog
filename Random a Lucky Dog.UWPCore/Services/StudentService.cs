using RLD.CPCore.Helpers;
using RLD.CPCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using static RLD.CPCore.KeyDictionary.SettingKey;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.CPCore.Models.StudentStatus;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.LocalizeService;
using static RLD.UWPCore.Services.SettingsStorageService;

namespace RLD.UWPCore.Services
{
	public static class StudentService
	{
		public static async Task<Collection<Student>> GetStudentsAsync(IStorageFile file)
		{
			IList<string> lines = new List<string>();
			if (file.FileType.ToLower() == ".txt") lines = await FileIO.ReadLinesAsync(file);
			else if (file.FileType.ToLower() == ".xlsx") lines = xlsx.GetAllLines(file.Path);
			for (int i = 0; i < lines.Count; i++)
			{
				if (String.IsNullOrWhiteSpace(lines[i]))
				{
					lines.Remove(lines[i]);
					i--;
				}
			}
			//while (lines.Last() == "") lines.RemoveAt(lines.Count() - 1);

			return GetStudents(lines);
		}
		public static Collection<Student> GetStudents(IList<string> lines)
		{
			var students = new Collection<Student>();
			int orderInList = 0;
			foreach (string line in lines)
			{
				string[] studentData = SplitStudentLine(line);

				Student Somebody = new Student(studentData[0],
					ConvertStatus(studentData[1]),
					Convert.ToInt32(studentData[2] ?? "0"),
					Convert.ToInt32(studentData[3] ?? "0"),
					orderInList++);
				//2:times
				//3:orderOfGoing

				students.Add(Somebody);
			}
			return students;
		}
		public static StudentStatus ConvertStatus(string status)
		{
			if (status == "unfinished" || status == Localize(Unfinished)) return unfinished;
			else if (status == "going" || status == Localize(Going)) return going;
			else if (status == "finished" || status == Localize(Finished)) return finished;
			else if (status == "suspended" || status == Localize(Suspended)) return suspended;
			else return StudentStatus.unfinished;
		}
		private static string[] SplitStudentLine(string line)
		{
			var lines = line.Split('\t', 4, StringSplitOptions.RemoveEmptyEntries);
			var returnLines = new string[4];
			for (byte i = 0; i < lines.Count(); i++) returnLines[i] = lines[i];
			return returnLines;
		}
		public static Collection<Student>[] ClassifyStudents(ICollection<Student> students)
		{
			Collection<Student>[] collections = new Collection<Student>[4];
			collections[0] = new Collection<Student>();
			collections[1] = new Collection<Student>();
			collections[2] = new Collection<Student>();
			collections[3] = new Collection<Student>();
			//collection[0]:going
			//[1]:finished
			//[2]:unfinished
			//[3]:others
			foreach (Student student in students)
			{
				if (student.Status == going) collections[0].Add(student);
				else if (student.Status == finished) collections[1].Add(student);
				else if (student.Status == unfinished) collections[2].Add(student);
				else collections[3].Add(student);
			}

			SortedDictionary<int, Student> sortedGoing = new SortedDictionary<int, Student>();
			foreach (Student student in collections[0]) sortedGoing.Add(student.OrderOfGoing, student);

			collections[0].Clear();

			foreach (var student in sortedGoing.Values) collections[0].Insert(0, student);

			return collections;
		}
		public static void UpgradeGoingStudentData(ref ObservableCollection<Student> data)
		{
			for (int i = 0; i < data.Count; i++) data[i].OrderOfGoing = data.Count - i;
		}
		public static void MoveStudentToTopOfCollection(Student sb, Collection<Student> FromCollection, Collection<Student> ToCollection, StudentStatus Status)
		{
			ToCollection.Insert(0, sb);
			ToCollection[0].Status = Status;
			FromCollection.RemoveAt(FromCollection.IndexOf(sb));
			if (Status == going) ToCollection[0].OrderOfGoing = ToCollection.Count;
		}
		private static IList<string> GetStudentStringList(List<Student> collection)
		{
			IList<string> list = new List<string>();
			foreach (var student in collection) list.Add(student.ToString());
			return list;
		}
		public static async Task SaveStudentsAsync(string fileName, Collection<Student> collection)
		{
			var file = await (await GetSaveFolderAsync()).CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			await SaveToFile(collection, file);
		}


		public static async Task SaveStudentsAsync(StorageFile file, Collection<Student> collection)
		{
			await SaveToFile(collection, file);
		}
		private static async Task SaveToFile(Collection<Student> collection, StorageFile file)
		{
			IList<string> list = GetStudentStringList(collection.ToList());
			if (file.FileType.ToLower() == ".txt") await FileIO.WriteLinesAsync(file, list);
			else if (file.FileType.ToLower() == ".xlsx")
			{
				xlsx.WriteLines((await GetExcelTempFolderAsync()).Path + "\\" + file.Name, list);
				await (await (await GetExcelTempFolderAsync()).GetFileAsync(file.Name)).CopyAndReplaceAsync(file);
			}
			SaveString(Saved, "True");
			SaveString(FileName, file.Name);

		}
		internal static List<StudentStatus> GetStudentStatuses() => Enum.GetValues(typeof(StudentStatus)).Cast<StudentStatus>().ToList();
	}
}
