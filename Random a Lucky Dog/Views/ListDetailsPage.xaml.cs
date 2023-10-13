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
		App app = Application.Current as App;

		public ListDetailsPage()
		{
			InitializeComponent();
			Loaded += ListDetailsPage_Loaded;
		}

		private void ListDetailsPage_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
			if (stu != null) { TurnToStudent(stu); }
			app.IsEditing = false;
		}
		internal void TurnToStudent(Student student)
		{
			ViewModel.Selected = student;
		}
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			if ((ViewModel.Selected != null) && (!app.IsEditing)) { stu = ViewModel.Selected; }
		}
	}
}
