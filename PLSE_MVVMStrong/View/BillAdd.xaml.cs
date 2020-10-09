using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PLSE_MVVMStrong.View
{
    /// <summary>
    /// Interaction logic for BillAdd.xaml
    /// </summary>
    public partial class BillAdd : Window
    {
        public BillAdd()
        {
            InitializeComponent();
        }
    }
    public class PriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            byte h = (byte)values[0];
            decimal pr = (decimal)values[1];
            return h * pr;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
