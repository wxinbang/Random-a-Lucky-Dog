using System;

using Select_Lucky_Dog.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
