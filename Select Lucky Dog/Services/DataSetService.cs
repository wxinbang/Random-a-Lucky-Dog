using Select_Lucky_Dog.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using static Select_Lucky_Dog.Core.Services.StudentService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Services.SettingsStorageService;
using static Select_Lucky_Dog.Services.IdentityService;
using Select_Lucky_Dog.Views;
using static Select_Lucky_Dog.Services.LocalizeService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.StringKey;

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
                SaveString(FileName, file.Name);
                //DealWithSettings.DeleteSettings(SettingKey.saved);
                return file;
            }
            else return null;
        }
        public static async Task<Collection<Student>[]> ConnectDataSetAsync(StorageFile file, bool NotVerifyIdentity = false)
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

                Collection<Student> going = collections[0];
                returnCollections[2] = collections[1];
                returnCollections[3] = collections[2];
                returnCollections[4] = collections[3];

                SortedDictionary<int ,Student> sortedGoing = new SortedDictionary<int ,Student>();
                foreach (Student student in going) sortedGoing.Add(student.OrderOfGoing, student);

                going.Clear();

                foreach (var student in sortedGoing.Values) going.Insert(0, student);
                returnCollections[1] = going;

                SaveString(FileName, file.Name);

                return returnCollections;
            }
            else ContentDialogs.ThrowException(Localize(NoRequiredPermissions), false);
            return null;
        }
    }
}
