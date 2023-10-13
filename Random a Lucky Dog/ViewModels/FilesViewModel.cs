using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RLD.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using static RLD.UWPCore.Services.FoldersService;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace RLD.ViewModels
{
	public class FilesViewModel : ObservableObject
	{
		private ICommand _itemInvokedCommand;
		private object _selectedItem;

		public object SelectedItem
		{
			get => _selectedItem;
			set => SetProperty(ref _selectedItem, value);
		}

		public ObservableCollection<Folder> SampleItems { get; } = new ObservableCollection<Folder>();

		public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.TreeViewItemInvokedEventArgs>(OnItemInvoked));

		public FilesViewModel()
		{
		}

		public async Task LoadDataAsync()
		{
			var allFolders = await GetAllFoldersAsync();

			foreach (var item in allFolders)
			{
				SampleItems.Add(await Folder.CreateFolderAsync(item));
			}
		}

		private void OnItemInvoked(WinUI.TreeViewItemInvokedEventArgs args)
			=> SelectedItem = args.InvokedItem;
	}
}
