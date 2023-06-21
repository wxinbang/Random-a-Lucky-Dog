using System.Collections.Generic;

namespace RLD.CPCore.Models
{
	public class PinYin
	{
		public PinYin()
		{
			TotalPinYin = new List<string>();
			FirstPinYin = new List<string>();
		}
		public List<string> TotalPinYin { get; set; }
		public List<string> FirstPinYin { get; set; }
	}
}
