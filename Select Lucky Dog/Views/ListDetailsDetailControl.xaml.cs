using Select_Lucky_Dog.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
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
			await ContentDialogs.EditStudent(button.Content as string == "Edit" ? ListMenuItem : ListMenuItem);
		}
	}
}
