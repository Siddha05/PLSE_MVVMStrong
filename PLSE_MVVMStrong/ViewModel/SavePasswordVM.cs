using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class SavePasswordVM : DependencyObject
    {
        #region Fields
        string _oldpass;
        string _newpass;
        string _newpasscomp;
        RelayCommand _oldpasschanged;
        RelayCommand _newpasschanged;
        #endregion
        #region Properties
        public string OldPass => _oldpass;
        public string NewPass
        {
            get => _newpass;
            set
            {
                _newpass = value;
            }
        }
        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            set { SetValue(HasErrorProperty, value); }
        }
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof(bool), typeof(SavePasswordVM), new PropertyMetadata(false));
        #endregion
        #region Commands
        public RelayCommand Save => new RelayCommand(n =>
        {
            if ((Application.Current as App).LogedEmployee.EmployeeCore.Password != OldPass
                    || NewPass != _newpasscomp)
            {
                HasError = true;
            }
            else
            {
                var wnd = n as Window;
                wnd.DialogResult = true;
                wnd.Close();
            }
            
        });
        public RelayCommand OldPassChanged => _oldpasschanged;
        public RelayCommand NewPassChanged => _newpasschanged;
        #endregion
        public SavePasswordVM(string pass)
        {
            _newpasscomp = pass;
            _oldpasschanged = new RelayCommand(n =>
            {
                _oldpass = (n as System.Windows.Controls.PasswordBox).Password;
                HasError = false;
            });
            _newpasschanged = new RelayCommand(n =>
            {
                _newpass = (n as System.Windows.Controls.PasswordBox).Password;
                HasError = false;
            });
        }
    }
}
