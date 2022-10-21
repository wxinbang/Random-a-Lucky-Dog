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

namespace Select_Lucky_Dog.Services
{
    internal static class DataSetService
    {
        public static async Task<StorageFile> SelectDataSetAsync()
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
                SaveString("FileName", file.Name);
                //DealWithSettings.DeleteSettings(SettingKey.saved);
                return file;
            }
            else
            {
                DealWithSettings.DeleteSettings(SettingKey.saved);
                return null;
            }
        }
    }
}
