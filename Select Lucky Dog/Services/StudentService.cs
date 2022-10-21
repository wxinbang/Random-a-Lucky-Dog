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
        private static async Task<IEnumerable<Student>> GetStudentsAsync(IStorageFile file)
        {
            var students = new List<Student>();
            byte orderInList = 0;
            IList<string> lines = await FileIO.ReadLinesAsync(file);
            while (lines.Last() == "") lines.RemoveAt(lines.Count() - 1);

            foreach (string line in lines)
            {
                string[] studentData = SplitStudentLine(line);

                Student Somebody = new Student(studentData[0],
                    ConvertStatus(studentData[1]),
                    Convert.ToByte(studentData[2] ?? "0"),
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
            string[] lines = new string[3];
            lines = line.Split(new char[2] { ' ', '\t' });
            
            return lines;
        }
        public static ICollection<Student>[] SortStudents(ICollection<Student> students)
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
        
    }
}
