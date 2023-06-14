using RLD.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using static RLD.Helpers.KeyDictionary.StringKey;
using static RLD.Services.LocalizeService;

namespace RLD.Models
{
	public class File
	{
		public StorageFile file { get; set; }
		public string name { get; set; }
		public string path { get; set; }
		public string dateCreated { get; set; }
		public string size { get; set; }
		public string dateModified { get; set; }
		public string mainContent { get; set; }
		private File(StorageFile file, BasicProperties bp, string mainContent)
		{
			this.file = file;
			this.path = file.Path;
			this.name = file.Name;
			this.dateCreated = file.DateCreated.ToString();
			this.size = bp.Size.ToString();
			this.dateModified = bp.DateModified.ToString();
			this.mainContent = mainContent;
		}
		public static async Task<File> CreateFileAsync(StorageFile file)
		{
			BasicProperties bp = await file.GetBasicPropertiesAsync();
			var collections = await DataSetService.ConnectDataSetAsync(file, saveSetting: false);
			var sb = new StringBuilder();
			sb.AppendLine(Localize(All) + collections[0].Count);
			sb.AppendLine(Localize(Going) + collections[1].Count);
			sb.AppendLine(Localize(Finished) + collections[2].Count);
			sb.AppendLine(Localize(Unfinished) + collections[3].Count);
			string mainContent = sb.ToString();
			return new File(file, bp, mainContent);
		}
	}
}
