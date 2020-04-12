using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.ViewModel;
using System.Windows;

namespace PLSE_MVVMStrong.View
{
    /// <summary>
    /// Логика взаимодействия для SpecialityAdd.xaml
    /// </summary>
    public partial class SpecialityAdd : Window
    {
        public SpecialityAdd()
        {
            InitializeComponent();
            DataContext = new SpecialityAddVM();
        }

        public SpecialityAdd(Speciality s)
        {
            InitializeComponent();
            DataContext = new SpecialityAddVM(s);
        }
    }
}