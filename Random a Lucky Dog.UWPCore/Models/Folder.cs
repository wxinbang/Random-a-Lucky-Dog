using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using static RLD.UWPCore.Services.FilesService;

namespace RLD.UWPCore.Models
{
	public class Folder
	{
		public StorageFolder folder { get; set; }
		public string name { get; set; }
		public string path { get; set; }
		public string dateCreated { get; set; }
		public string size { get; set; }
		public string dateModified { get; set; }
		public IList<File> files { get; set; }
		private Folder(StorageFolder folder, IList<File> files, BasicProperties bp)
		{
			this.folder = folder;
			this.name = folder.Name;
			this.path = folder.Path;
			this.files = files;
			this.dateCreated = folder.DateCreated.ToString();
			this.size = bp.Size.ToString();
			this.dateModified = bp.DateModified.ToString();
		}
		public static async Task<Folder> CreateFolderAsync(StorageFolder folder)
		{
			var files = await folder.GetAllFilesAsync();
			var list = new List<File>();
			foreach (var item in files) list.Add(await File.CreateFileAsync(item));
			var bp = await folder.GetBasicPropertiesAsync();
			return new Folder(folder, list, bp);
		}
	}
}
