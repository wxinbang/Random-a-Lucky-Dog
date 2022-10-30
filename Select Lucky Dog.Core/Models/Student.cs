using System;
using System.Collections.Generic;
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
  
		public override string ToString() => Name + "\t" +  Status.ToString() + "\t" + (Status == StudentStatus.going ? OrderOfGoing.ToString() : "" + PraiseTime.ToString());
        public Student(string name,StudentStatus status,int orderOfGoing, int times, int orderInList)
        {
            this.Name=name;
            this.Status=status;
            this.OrderOfGoing=orderOfGoing;
			this.PraiseTime = times;
            this.OrderInList=orderInList;
        }
    }
    public enum StudentStatus:byte
    {
        unfinished,
        going,//进行中
        finished,
        suspended,//暂停的
        skip
    }
}
