using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.View
{
    /// <summary>
    /// Логика взаимодействия для CustomerSelect.xaml
    /// </summary>
    public partial class CustomerSelect : Window
    {
        public CustomerSelect()
        {
            InitializeComponent();
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class NullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}