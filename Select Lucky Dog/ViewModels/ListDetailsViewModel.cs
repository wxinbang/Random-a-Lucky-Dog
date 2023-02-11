using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Select_Lucky_Dog.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Select_Lucky_Dog.ViewModels
{
	public class ListDetailsViewModel : ObservableObject
	{
		private Student _selected;
		App app = (Application.Current as App);

		public Student Selected
		{
			get { return _selected; }
			set { SetProperty(ref _selected, value); }
		}

		public ObservableCollection<Student> SampleItems { get; private set; } = new ObservableCollection<Student>();

		public ListDetailsViewModel()
		{
		}

		public async Task LoadDataAsync(ListDetailsViewState viewState)
		{
			SampleItems.Clear();

			//var data = await StudentService.GetStudentsAsync(await FilesService.GetLastDataFileAsync());
			var data = app.AllStudentList;

			foreach (var item in data)
			{
				SampleItems.Add(item);
			}

			if (viewState == ListDetailsViewState.Both)
			{
				//Selected = SampleItems.First();
			}
			await Task.CompletedTask;
		}
	}
}
