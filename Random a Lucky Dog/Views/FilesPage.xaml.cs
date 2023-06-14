using RLD.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RLD.Views
{
	// For more info about the TreeView Control see
	// https://docs.microsoft.com/windows/uwp/design/controls-and-patterns/tree-view
	// For other samples, get the XAML Controls Gallery app http://aka.ms/XamlControlsGallery
	public sealed partial class FilesPage : Page
	{
		public FilesViewModel ViewModel { get; } = new FilesViewModel();

		public FilesPage()
		{
			InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			await ViewModel.LoadDataAsync();
		}
	}
}
