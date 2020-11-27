using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PLSE_MVVMStrong.ViewModel
{
    class SettingsVM : DependencyObject
    {
        #region Fields
        RelayCommand _folderselect;
        RelayCommand _passchange;
        RelayCommand _savepass;
        #endregion
        #region Properties
        public string SavePath
        {
            get { return (string)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("SavePath", typeof(string), typeof(SettingsVM), new PropertyMetadata(String.Empty));
        public bool IsCheckExpertise
        {
            get { return (bool)GetValue(CheckExpertiseProperty); }
            set { SetValue(CheckExpertiseProperty, value); }
        }
        public static readonly DependencyProperty CheckExpertiseProperty =
            DependencyProperty.Register("CheckExpertise", typeof(bool), typeof(SettingsVM), new PropertyMetadata(false));
        public bool IsSaveLogin
        {
            get { return (bool)GetValue(IsSaveLoginProperty); }
            set { SetValue(IsSaveLoginProperty, value); }
        }
        public static readonly DependencyProperty IsSaveLoginProperty =
            DependencyProperty.Register("IsSaveLogin", typeof(bool), typeof(SettingsVM), new PropertyMetadata(false));
        public bool GroupBySpeciality
        {
            get { return (bool)GetValue(GroupBySpecialityProperty); }
            set { SetValue(GroupBySpecialityProperty, value); }
        }
        public static readonly DependencyProperty GroupBySpecialityProperty =
            DependencyProperty.Register("GroupBySpeciality", typeof(bool), typeof(SettingsVM), new PropertyMetadata(false));
        public string Pass
        {
            get { return (string)GetValue(PassProperty); }
            set { SetValue(PassProperty, value); }
        }
        public static readonly DependencyProperty PassProperty =
            DependencyProperty.Register("Pass", typeof(string), typeof(SettingsVM), new PropertyMetadata(string.Empty));
        #endregion
        #region Commands
        public RelayCommand FolderSelectClick
        {
            get
            {
                return _folderselect != null ? _folderselect : _folderselect = new RelayCommand(n =>
                {
                    FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        SavePath = folderBrowser.SelectedPath;
                    }
                });
            }
        }
        public RelayCommand PassChange => _passchange;
        public RelayCommand SavePassword
        {
            get
            {
                return _savepass != null ? _savepass : _savepass = new RelayCommand(n =>
                {
                    var wnd = new View.SavePassword();
                    var date = new SavePasswordVM(Pass);
                    wnd.DataContext = date;
                    if (wnd.ShowDialog() ?? false == true)
                    {
                        var e = (System.Windows.Application.Current as App).LogedEmployee;
                        string p = e.EmployeeCore.Password;
                        e.EmployeeCore.Password = Pass;
                        try
                        {
                            e.EmployeeCore.UpdatePassword(CommonInfo.connection);
                            System.Windows.MessageBox.Show("Пароль успешно изменен");
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                            e.EmployeeCore.Password = p;
                        }
                    }
                });
            }
        }
        public RelayCommand Save
        {
            get => new RelayCommand(n =>
            {
                Settings.Default.SavePath = SavePath;
                Settings.Default.SaveLogin = IsSaveLogin;
                Settings.Default.CheckExpertise = IsCheckExpertise;
                Settings.Default.GroupBySpec = GroupBySpeciality;
                Settings.Default.Save();
                var w = n as Window;
                if (w != null) w.Close();
            });
        }
        #endregion
        public SettingsVM()
        {
            SavePath = Settings.Default.SavePath;
            IsCheckExpertise = Settings.Default.CheckExpertise;
            IsSaveLogin = Settings.Default.SaveLogin;
            GroupBySpeciality = Settings.Default.GroupBySpec;
            _passchange = new RelayCommand(n =>
                                {
                                    Pass = (n as System.Windows.Controls.PasswordBox).Password;
                                });
        }
    }
}
