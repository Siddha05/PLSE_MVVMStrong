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
        #endregion
        #region Properties
        public string SavePath
        {
            get { return (string)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("SavePath", typeof(string), typeof(SettingsVM), new PropertyMetadata(String.Empty));


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
        public RelayCommand Save
        {
            get => new RelayCommand(n =>
            {
                Settings.Default.SavePath = SavePath;
                Settings.Default.Save();
                var w = n as Window;
                if (w != null) w.Close();
            });
        }
        #endregion
        public SettingsVM()
        {
            SavePath = Settings.Default["SavePath"].ToString();
        }
    }
}
