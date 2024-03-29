﻿using RLD.CPCore.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using static RLD.CPCore.KeyDictionary.SettingKey;
using static RLD.CPCore.KeyDictionary.StringKey;
using static RLD.UWPCore.ExpectionProxy;
using static RLD.UWPCore.Services.FoldersService;
using static RLD.UWPCore.Services.IdentityService;
using static RLD.UWPCore.Services.LocalizeService;
using static RLD.UWPCore.Services.SettingsStorageService;
using static RLD.UWPCore.Services.StudentService;

namespace RLD.UWPCore.Services
{
	public static class DataSetService
	{
		public static async Task<StorageFile> SelectDataSetAsync()
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

				if (saveSetting)
				{
					SaveString(FileName, file.Name);
					SaveString(Saved, "True");
					//DeleteString(Saved);
				}

				return returnCollections;
			}
			else await ThrowException(Localize(NoRequiredPermissions), false);
			return null;
		}
	}
}
