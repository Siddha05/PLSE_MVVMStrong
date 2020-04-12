using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.View
{
    /// <summary>
    /// Логика взаимодействия для ResolutionAdd.xaml
    /// </summary>
    public partial class ResolutionAdd : Window
    {
        public ResolutionAdd()
        {
            InitializeComponent();
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    internal class LenghtToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s.Length > 1) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    internal class ResTypeToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as string)
            {
                case "договор":
                    return false;

                default:
                    return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    internal class CaseTypeToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ct = value as string;
            switch (ct)
            {
                case "исследование":
                    return Visibility.Collapsed;

                case "административное правонарушение":
                case "проверка КУСП":
                case "уголовное":
                    if (parameter.ToString() == "v") return Visibility.Visible;
                    else return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}