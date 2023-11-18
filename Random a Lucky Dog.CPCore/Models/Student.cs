using RLD.CPCore.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RLD.CPCore.Models
{
	public class Student
	{
		public string Name { get; set; }
		public StudentStatus Status { get; set; }
		public int OrderOfGoing { get; set; }
		public int PraiseTime { get; set; }
		public int OrderInList { get; set; }
		public string InfoForSearch { get; set; }
		//public static int SumOfGoing = 0;
		public override string ToString() => Name + "\t" + Status.ToString() + "\t" + PraiseTime.ToString() + '\t' + (Status == StudentStatus.going ? OrderOfGoing.ToString() : "");
		public Student(string name, StudentStatus status, int times, int orderOfGoing, int orderInList)
		{
			PinYin namePinYin = PinYinConverter.GetTotalPinYin(name);
			StringBuilder info = new StringBuilder();
			info.Append(name + ";");
			namePinYin.TotalPinYin.ConvertAll(p => info.Append(p + ';'));
			namePinYin.FirstPinYin.ConvertAll(p => info.Append(p + ';'));

			this.Name = name;
			this.Status = status;
			this.OrderOfGoing = orderOfGoing;
			this.PraiseTime = times;
			this.OrderInList = orderInList;
			this.InfoForSearch = info.ToString();
			//if (OrderOfGoing > SumOfGoing) SumOfGoing = OrderOfGoing;
		}
		public static Collection<Student> SortGoing(Collection<Student> all)
		{
			Dictionary<int, Student> going = new Dictionary<int, Student>();
			foreach (Student student in all)
			{
				if (student.Status == StudentStatus.going) going.Add(student.OrderOfGoing, student);
			}
			var result = going.OrderBy(student => student.Key);
			int i = 1;
			Collection<Student> returnValue = new Collection<Student>();
			foreach (KeyValuePair<int, Student> student in result)
			{
				student.Value.OrderOfGoing = i++;
				returnValue.Add(student.Value);
			}
			return returnValue;
		}
		public static bool operator >(Student stu1, Student stu2) => (stu1.OrderInList > stu2.OrderInList);
		public static bool operator <(Student stu1, Student stu2) => (stu1.OrderInList < stu2.OrderInList);
	}
	public enum StudentStatus : byte
	{
		unfinished,
		going,//进行中
		finished,
		suspended,//暂停的
		skip
	}
}
