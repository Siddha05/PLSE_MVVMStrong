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
    /// Логика взаимодействия для Expertises.xaml
    /// </summary>
    public partial class Expertises : Window
    {
        public Expertises()
        {
            InitializeComponent();
            Top = 0; Left = 0;
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var sp = sender as StackPanel;
            //if (sp != null)
            //{
            //    popMenu.IsOpen = true;
            //    popMenu.PlacementTarget = sp;
                
            //}
            //else MessageBox.Show("Eror");
        }
    }
}
