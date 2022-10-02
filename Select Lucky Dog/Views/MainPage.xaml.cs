using System;
using System.Collections.ObjectModel;
using Select_Lucky_Dog.Core.Models;
using static Select_Lucky_Dog.Core.Services.StudentService;
using Select_Lucky_Dog.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();
        ObservableCollection<Student> listOfGoingStudent;
        public MainPage()
        {
            InitializeComponent();
            ObservableCollection<Student> studentList = GetStudentsAsync(file);
        }
    }
}
