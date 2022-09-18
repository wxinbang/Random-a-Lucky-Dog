using System;

using Select_Lucky_Dog.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class DetailsPage : Page
    {
        public DetailsViewModel ViewModel { get; } = new DetailsViewModel();

        public DetailsPage()
        {
            InitializeComponent();
            Loaded += DetailsPage_Loaded;
        }

        private async void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}
