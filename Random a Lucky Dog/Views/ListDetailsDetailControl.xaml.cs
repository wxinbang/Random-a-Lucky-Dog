using RLD.CPCore.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RLD.Views
{
	public sealed partial class ListDetailsDetailControl : UserControl
	{
		public Student ListMenuItem
		{
			get { return GetValue(ListMenuItemProperty) as Student; }
			set { SetValue(ListMenuItemProperty, value); }
		}

		public static readonly DependencyProperty ListMenuItemProperty = DependencyProperty.Register("ListMenuItem", typeof(Student), typeof(ListDetailsDetailControl), new PropertyMetadata(null, OnListMenuItemPropertyChanged));

		public ListDetailsDetailControl()
		{
			InitializeComponent();
		}

		private static void OnListMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as ListDetailsDetailControl;
			control.ForegroundElement.ChangeView(0, 0, 1);
		}
		private async void EditStudent_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var app = Application.Current as App;
			if (button.Name == "EditButton")
			{
				int index = app.AllStudentList.IndexOf(ListMenuItem);
				await ContentDialogs.EditStudent(ListMenuItem);
				app.stu = app.AllStudentList[index];
			}
			else
			{
				await ContentDialogs.EditStudent(null);
				app.stu = app.AllStudentList[app.AllStudentList.Count - 1];
			}
			Student.SortGoing(app.AllStudentList);
			app.IsEditing = true;
			Frame rootFrame = ((((Window.Current.Content as Page).Content as Grid).Children[2] as Microsoft.UI.Xaml.Controls.NavigationView).Content as Grid).Children[0] as Frame;
			rootFrame.Navigate(typeof(ListDetailsPage));
		}
	}
}
