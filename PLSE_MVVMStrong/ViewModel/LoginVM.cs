using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PLSE_MVVMStrong.Properties;
using System.Runtime.InteropServices;

namespace PLSE_MVVMStrong.ViewModel
{
    class LoginVM : DependencyObject
    {
        #region Properties
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Lang
        {
            get
            {
                switch (GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero)))
                {
                    case 1049:
                        return "RU";
                    case 1033:
                        return "EN";
                    default:
                        return null;
                }
            }
        }
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
            if (Settings.Default.SaveLogin) Login = Settings.Default.InitLogin;
            else Login = String.Empty;
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
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId([In] IntPtr hWnd,[Out, Optional] IntPtr lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern ushort GetKeyboardLayout([In] int idThread);
    }
}
