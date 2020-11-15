using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PLSE_MVVMStrong.Properties;

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
        public RelayCommand Inter
        { 
            get
            {
                return new RelayCommand(n =>
                                        {
                                            var em = CommonInfo.Employees.FirstOrDefault(e => e.Actual == true && e.Sname == Login && e.EmployeeCore.Password == Pass);
                                            if (em != null)
                                            {
                                                (Application.Current as App).LogedEmployee = em;
                                                var wnd = new MainWindow();
                                                Settings.Default.InitLogin = Login;
                                                Settings.Default.Save();
                                                (n as Window).Close();
                                                
                                                wnd.Show();
                                            }
                                            else Error = true;
                                        });
            }
        }
        public RelayCommand Exit
        {
            get
            {
                return new RelayCommand(n =>
                                    {
                                        
                                        var wnd = n as View.Login;
                                        wnd.Close();
                                    });
            }
        }
        public RelayCommand PassChanged { get; }
        public RelayCommand TextChanged { get; }
        public RelayCommand WindowLoaded
        {
            get
            {
                return new RelayCommand(n =>
                {
                    if (!CommonInfo.IsInitializated)
                    {
                       MessageBox.Show("Ошибка при подключении к базе данных. Приложение будет закрыто");
                       (n as Window).Close();
                    }
                });
            }
        }
        #endregion
        public LoginVM()
        {
#if DEBUG
            //Login = "Кожаева";
            //Pass = "Кожаева";
#endif
            Login = Settings.Default.InitLogin;
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
