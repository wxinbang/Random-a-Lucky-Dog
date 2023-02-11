using System.Collections.Generic;

using Windows.ApplicationModel.DataTransfer;

namespace Select_Lucky_Dog.Models
{
	public class DragDropCompletedData
	{
		public DataPackageOperation DropResult { get; set; }

		public IReadOnlyList<object> Items { get; set; }
	}
}
