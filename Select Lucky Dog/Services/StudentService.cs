using Microsoft.VisualBasic;
using Select_Lucky_Dog.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.WebUI;

namespace Select_Lucky_Dog.Core.Services
{
    public static class StudentService
    {
        public static async Task<IEnumerable<Student>> GetStudentsAsync(IStorageFile file)
        {
            var students = new List<Student>();
            byte orderInList = 0;
            IList<string> lines = await FileIO.ReadLinesAsync(file);
            while (lines.Last() == "") lines.RemoveAt(lines.Count() - 1);

            foreach (string line in lines)
            {
                string[] studentData = SplitStudentLine(line);

                Student Somebody = new Student()
                {
                    Name = studentData[0],
                    Status = ConvertStatus(studentData[1]),
                    OrderOfGoing = Convert.ToByte(studentData[2] ?? "0"),
                    OrderInList = orderInList++
                };

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
    }
}
