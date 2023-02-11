using Select_Lucky_Dog.Core.Models;
using Select_Lucky_Dog.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using static Select_Lucky_Dog.Services.StudentService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using static Select_Lucky_Dog.Helpers.KeyDictionary.StringKey;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Services.IdentityService;
using static Select_Lucky_Dog.Services.LocalizeService;
using static Select_Lucky_Dog.Services.SettingsStorageService;

namespace Select_Lucky_Dog.Services
{
	internal static class DataSetService
	{
		internal static async Task<StorageFile> SelectDataSetAsync()
		{
			StorageFile file;
			var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.Desktop
			};
			picker.FileTypeFilter.Add(".txt");//记得检查

			file = await picker.PickSingleFileAsync();

			DeleteString(Saved);

			if (file != null)
			{
				await file.CopyAsync(await GetDataSetFolderAsync(), file.Name, NameCollisionOption.ReplaceExisting);
				return file;
			}
			else return null;
		}
		public static async Task<Collection<Student>[]> ConnectDataSetAsync(StorageFile file, bool NotVerifyIdentity = false, bool saveSetting = true)
		{
			if (await VerifyIdentityAsync() || NotVerifyIdentity)
			{
				Collection<Student>[] returnCollections = new Collection<Student>[5];
				//[0]:all
				//[1]:sorted going
				//[2]:finished
				//[3]:unfinished
				//[4]:other

				returnCollections[0] = await GetStudentsAsync(file);
				var collections = ClassifyStudents(returnCollections[0]);

				returnCollections[1] = collections[0];
				returnCollections[2] = collections[1];
				returnCollections[3] = collections[2];
				returnCollections[4] = collections[3];

				if (saveSetting) { SaveString(FileName, file.Name); DeleteString(Saved); }

				return returnCollections;
			}
			else await ContentDialogs.ThrowException(Localize(NoRequiredPermissions), false);
			return null;
		}
	}
}
