using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;

using Select_Lucky_Dog.Core.Models;
using Select_Lucky_Dog.Core.Services;
using Select_Lucky_Dog.Services;

namespace Select_Lucky_Dog.ViewModels
{
    public class DetailsViewModel : ObservableObject
    {
        private Student _selected;

        public Student Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ObservableCollection<Student> SampleItems { get; private set; } = new ObservableCollection<Student>();

        public DetailsViewModel()
        {
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            SampleItems.Clear();

            var folder = await FoldersService.GetDataSetFolderAsync();

            var data = await StudentService.GetStudentsAsync(await folder.GetFileAsync(SettingsStorageService.ReadString(Helpers.KeyDictionary.SettingKey.FileName)));

            foreach (var item in data)
            {
                SampleItems.Add(item);
            }

            if (viewState == ListDetailsViewState.Both)
            {
                //Selected = SampleItems.First();
            }
        }
    }
}
