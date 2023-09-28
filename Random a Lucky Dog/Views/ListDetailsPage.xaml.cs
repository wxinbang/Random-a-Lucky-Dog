using RLD.CPCore.Models;
using RLD.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RLD.Views
{
	public sealed partial class ListDetailsPage : Page
	{
		public ListDetailsViewModel ViewModel { get; } = new ListDetailsViewModel();
		Student stu = (Application.Current as App).stu;

		public ListDetailsPage()
		{
			InitializeComponent();
			Loaded += ListDetailsPage_Loaded;
		}

		private async void ListDetailsPage_Loaded(object sender, RoutedEventArgs e)
		{
			await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
			if ((Application.Current as App).stu != null) { TurnToStudent((Application.Current as App).stu); }
		}
		internal void TurnToStudent(Student student)
		{
			ViewModel.Selected = student;
			
		}
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			if(ViewModel.Selected != null) { (Application.Current as App).stu = ViewModel.Selected; }
		}
	}
}
