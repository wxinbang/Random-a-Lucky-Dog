using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Microsoft.VisualBasic;
using Select_Lucky_Dog.Core.Models;
using Select_Lucky_Dog.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.WebUI;

using static Select_Lucky_Dog.Core.Models.StudentStatus;

namespace Select_Lucky_Dog.Core.Services
{
    public static class StudentService
    {
        public static async Task<Collection<Student>> GetStudentsAsync(IStorageFile file)
        {
            var students = new Collection<Student>();
            int orderInList = 0;
            IList<string> lines = await FileIO.ReadLinesAsync(file);
            while (lines.Last() == "") lines.RemoveAt(lines.Count() - 1);

            foreach (string line in lines)
            {
                string[] studentData = SplitStudentLine(line);

                Student Somebody = new Student(studentData[0],
                    ConvertStatus(studentData[1]),
                    Convert.ToInt32(studentData[2] ?? "0"),
                    Convert.ToInt32(studentData[3] ?? "0"),
                    orderInList++);

                students.Add(Somebody);
            }
            return students;
        }
        private static StudentStatus ConvertStatus(string status)
        {
            if (status == "unfinished") return StudentStatus.unfinished;
            else if (status == "going") return StudentStatus.going;
            else if (status == "finished") return StudentStatus.finished;
            else if (status == "suspended") return StudentStatus.suspended;
            else return StudentStatus.unfinished;
        }
        private static string[] SplitStudentLine(string line)
        {
            line.Trim();
            string[] returnLines = new string[4];
            var lines = line.Split(new char[2] { ' ', '\t' },StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length && i < returnLines.Length; i++) returnLines[i] = lines[i];

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

            return collections;
        }
        public static void UpgradeGoingStudentData(ref ObservableCollection<Student> data)
        {
            for (int i = 0; i < data.Count; i++) data[i].OrderOfGoing = data.Count - i;
        }
        public static void MoveStudentToTopOfCollection(Student sb,Collection<Student> FromCollection,Collection<Student> ToCollection,StudentStatus Status)
        {
            ToCollection.Insert(0, sb);
            ToCollection[0].Status = Status;
            FromCollection.RemoveAt(FromCollection.IndexOf(sb));
            if (Status == going) ToCollection[0].OrderOfGoing = ToCollection.Count;
        }
        private static IList<string> GetStudentStringList(List<Student> collection)
        {
            IList<string> list = new List<string>();
            foreach (var student in collection)list.Add(student.ToString());
            return list;
        }
        public static async Task SaveStudentsAsync(StorageFile file,Collection<Student> collection)
        {
            IList<string> list = GetStudentStringList(SortStudents(collection));
            await FileIO.WriteLinesAsync(file, list);
        }
        private static List<Student> SortStudents(Collection<Student> inCollection)
        {
            SortedDictionary<int,Student> dictionary = new SortedDictionary<int,Student>();
            foreach (var item in inCollection) dictionary.Add(item.OrderInList, item);
            return dictionary.Values.ToList();
        }
    }
}
