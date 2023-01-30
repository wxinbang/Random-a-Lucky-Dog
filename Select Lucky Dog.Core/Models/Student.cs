using Select_Lucky_Dog.Core.Helpers;
using System.Text;

namespace Select_Lucky_Dog.Core.Models
{
	public class Student
	{
		public string Name { get; set; }
		public StudentStatus Status { get; set; }
		public int OrderOfGoing { get; set; }
		public int PraiseTime { get; set; }
		public int OrderInList { get; set; }
		public string InfoForSearch { get; set; }
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
		}
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
