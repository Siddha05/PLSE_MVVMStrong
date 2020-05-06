using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class LoginVM : DependencyObject
    {
        #region Properties
        public string Login { get; set; }
        public string Pass { get; set; }
        public bool Error
        {
            get { return (bool)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }
        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(bool), typeof(LoginVM), new PropertyMetadata(false));


        #endregion
        #region Commands
        public RelayCommand Inter { get; }
        public RelayCommand Exit { get; }
        public RelayCommand PassChanged { get; }
        public RelayCommand TextChanged { get; }
        #endregion
        public LoginVM()
        {
#if DEBUG
            Login = "Кожаева";
            Pass = "Кожаева";
#endif
            Exit = new RelayCommand(n =>
            {
                var wnd = n as View.Login;
                wnd.Close();
            });
            Inter = new RelayCommand(n =>
            {
                var em = CommonInfo.Employees.FirstOrDefault(e => e.Sname == Login && e.Password == Pass);
                if (em != null)
                {
                    (n as Window).Close(); 
                }
                else Error = true;
            });
            PassChanged = new RelayCommand(n =>
            {
                Pass = (n as System.Windows.Controls.PasswordBox).Password;
                Error = false;
            });
            TextChanged = new RelayCommand(n =>
            {
                Error = false;
            });
        }
    }
}
