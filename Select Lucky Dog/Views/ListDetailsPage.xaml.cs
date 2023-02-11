﻿using Select_Lucky_Dog.Core.Models;
using Select_Lucky_Dog.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
	public sealed partial class ListDetailsPage : Page
	{
		public ListDetailsViewModel ViewModel { get; } = new ListDetailsViewModel();

		public ListDetailsPage()
		{
			InitializeComponent();
			Loaded += ListDetailsPage_Loaded;
		}

		private async void ListDetailsPage_Loaded(object sender, RoutedEventArgs e)
		{
			await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
		}
		internal void TurnToStudent(Student student)
		{
			ViewModel.Selected = student;
		}
	}
}