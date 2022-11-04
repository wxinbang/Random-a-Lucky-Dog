using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using xbb.ClassLibraries;
using static Select_Lucky_Dog.Services.FoldersService;
using static Select_Lucky_Dog.Services.SettingsStorageService;
using static Select_Lucky_Dog.Helpers.KeyDictionary.SettingKey;
using Select_Lucky_Dog.Core.Models;
using System.Collections.ObjectModel;
using static Select_Lucky_Dog.Core.Services.StudentService;

namespace Select_Lucky_Dog.Services
{
    internal static class DataSetService
    {
        public static async Task ImportDataSetAsync()
        {
            var file = await SelectDataSetAsync();
            await ConnectDataSetAsync(file);

        }
        private static async Task<StorageFile> SelectDataSetAsync()
        {
            StorageFile file;
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".txt");//记得检查

            file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                await file.CopyAsync(await GetDataSetFolderAsync(), file.Name, NameCollisionOption.ReplaceExisting);
                SaveString(FileName, file.Name);
                //DealWithSettings.DeleteSettings(SettingKey.saved);
                return file;
            }
            else
            {
                DealWithSettings.DeleteSettings(SettingKey.saved);
                return null;
            }
        }
        public static async Task<Collection<Student>[]> ConnectDataSetAsync(StorageFile file, bool NotVerifyIdentity = false)
        {
            if (await DealWithIdentity.VerifyIdentity() || NotVerifyIdentity)
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
            else ContentDialogs.ThrowException("没有所需要的权限", false);
            return null;
        }
    }
}
