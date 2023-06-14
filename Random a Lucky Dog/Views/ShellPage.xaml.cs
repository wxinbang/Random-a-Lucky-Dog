using Microsoft.Toolkit.Uwp.UI;
using RLD.Core.Models;
using RLD.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RLD.Views
{
	// TODO: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
	public sealed partial class ShellPage : Page
	{
		private bool isChoose;
		ObservableCollection<Student> AllStudentList;

		public ShellViewModel ViewModel { get; } = new ShellViewModel();

		public ShellPage()
		{
			InitializeComponent();
			DataContext = ViewModel;
			ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
			CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
			Window.Current.SetTitleBar(AppTitleBar);

			ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
			titleBar.ButtonBackgroundColor = Colors.Transparent;
			titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

			AllStudentList = (Application.Current as App).AllStudentList;

#if (DEBUG)
			AppTitleTextBlock.Text += " - Developing";
#endif
		}

		private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (sender.Text == "") SearchBox.ItemsSource = null;
			else if (sender.Text != "" && !isChoose)
			{
				//sender.Text = suggestBoxLastString;
				SearchBox.ItemsSource = AllStudentList.Where(p => p.InfoForSearch.Contains(sender.Text)).Select(p => p.Name).ToList();
			}
			isChoose = false;
		}

		private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			isChoose = true;
		}

		private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (args.ChosenSuggestion != null)
			{
				Student student = AllStudentList.Where(p => p.Name == args.ChosenSuggestion.ToString()).Select(p => p).ToList()[0];
				var page = this.FindChildren().ToList()[0].FindChildren().ToList()[3].FindChildren().ToList()[1].FindChildren().ToList()[0];
				if (page is MainPage)
				{
					var mainPage = page as MainPage;
					mainPage.TurnToStudent(student);
				}
				else if (page is ListDetailsPage)
				{
					var detailsPage = page as ListDetailsPage;
					detailsPage.TurnToStudent(student);
				}

			}
		}
	}
}
