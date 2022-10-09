using System;
using System.Collections.Generic;
using System.Text;

namespace Select_Lucky_Dog.Core.Models
{
    public class Student
    {
        public string Name { get; set; }
        public StudentStatus Status { get; set; }
        public byte OrderOfGoing { get; set; }
        public byte OrderInList { get; set; }
        public override string ToString() => Name + "\t" + Status.ToString() + "\t" + (Status == StudentStatus.going ? OrderOfGoing.ToString() : "");
        public Student(string name,StudentStatus status,byte orderOfGoing,byte orderInList)
        {
            this.Name=name;
            this.Status=status;
            this.OrderOfGoing=orderOfGoing;
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
