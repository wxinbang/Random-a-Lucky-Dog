using System;

using Select_Lucky_Dog.Core.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Select_Lucky_Dog.Views
{
    public sealed partial class DetailsDetailControl : UserControl
    {
        public Student ListMenuItem
        {
            get { return GetValue(ListMenuItemProperty) as Student; }
            set { SetValue(ListMenuItemProperty, value); }
        }

        public static readonly DependencyProperty ListMenuItemProperty = DependencyProperty.Register("ListMenuItem", typeof(Student), typeof(DetailsDetailControl), new PropertyMetadata(null, OnListMenuItemPropertyChanged));

        public DetailsDetailControl()
        {
            InitializeComponent();
        }

        private static void OnListMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailsDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
