using PLSE_MVVMStrong.Model;
using System;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class SpecialityVM : DependencyObject
    {
        #region Properties

        public ListCollectionView Specialities { get; } = new ListCollectionView(CommonInfo.Specialities);

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SpecialityVM), new PropertyMetadata(""));

        #endregion Properties

        #region Commands

        private RelayCommand _exit = new RelayCommand(o =>
                                                          {
                                                              var w = o as View.Specialities;
                                                              if (w!=null) w.Close();
                                                          });

        private RelayCommand _delete;
        private RelayCommand _find;
        private RelayCommand _addnew;

        public RelayCommand Find => _find;
        public RelayCommand Exit => _exit;
        public RelayCommand AddNew => _addnew;
        public RelayCommand Delete => _delete;

        #endregion Commands

        public SpecialityVM()
        {
            _find=new RelayCommand(o => Specialities.Filter=n => (n as Speciality).Code.ContainWithComparison(SearchText, StringComparison.CurrentCultureIgnoreCase));
            _delete=new RelayCommand(o => { if (Specialities.CurrentItem!=null) Specialities.Remove(Specialities.CurrentItem); });
        }
    }
}