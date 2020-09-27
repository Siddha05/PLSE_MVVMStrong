using System;
using System.Collections.Generic;
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
    /// Interaction logic for FinishExpertise.xaml
    /// </summary>
    public partial class FinishExpertise : Window
    {
        public FinishExpertise()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1 || e.Key == Key.D0 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6
                || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9)
            {
            }
            else e.Handled = true;
        }
        private void Comment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
