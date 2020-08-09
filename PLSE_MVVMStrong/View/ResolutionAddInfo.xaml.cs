using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.ViewModel;
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
    /// Interaction logic for ResolutionAddInfo.xaml
    /// </summary>
    public partial class ResolutionAddInfo : Window
    {
        public ResolutionAddInfo(Resolution resolution)
        {
            var dc = new ResolutionAddInfoVM(resolution);
            DataContext = dc;
            InitializeComponent();
            
            
        }

        private void wnd_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void wnd_ContentRendered(object sender, EventArgs e)
        {
            (this.DataContext as ResolutionAddInfoVM).Proceed();
        }
    }
}
