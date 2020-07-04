using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;


namespace PLSE_MVVMStrong.ViewModel
{
    internal class SpecialityVM : DependencyObject
    {
        #region Fields
        App app = Application.Current as App;
        #endregion
        #region Properties
        public ListCollectionView Specialities { get; } = new ListCollectionView(CommonInfo.Specialities);

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SpecialityVM), new PropertyMetadata("", SearchTextChanged));

        #endregion Properties

        #region Commands

        private RelayCommand _addnewspec;
        private RelayCommand _editspec;
        private RelayCommand _deletespec;
        public RelayCommand AddNew
        {
            get
            {
                return _addnewspec != null ? _addnewspec : _addnewspec = new RelayCommand(n =>
                                                                                {
                                                                                    var w = new SpecialityAdd();
                                                                                    if (w.ShowDialog() == true)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            Speciality sp = (w.DataContext as SpecialityAddVM).Speciality;
                                                                                            sp.SaveChanges(CommonInfo.connection);
                                                                                            CommonInfo.Specialities.Add(sp);
                                                                                        }
                                                                                        catch (Exception e)
                                                                                        {
                                                                                            MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                                                                        }
                                                                                    }
                                                                                },
                                                                                    o =>
                                                                                    {
                                                                                        return app.Permissions.Actions[PermissionAction.SpecialitiesAdd];
                                                                                    }
                                                                                    );
            }
        }
        public RelayCommand Delete
        {
            get
            {
                return _deletespec != null ? _deletespec : _deletespec = new RelayCommand(n =>
                                {
                                    var sp = Specialities.CurrentItem as Speciality;
                                    if (sp == null) return;
                                    if (MessageBox.Show("Вы уверены, что хотите удалить выбранную специальность?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    {
                                        try
                                        {
                                            sp.DeleteFromDB(CommonInfo.connection);
                                            Specialities.Remove(Specialities.CurrentItem);
                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                },
                                    o =>
                                    {
                                        return app.Permissions.Actions[PermissionAction.SpecialitiesDelete];
                                    }
                                    );
            }
        }
        public RelayCommand Edit
        {
            get
            {
                return _editspec != null ? _editspec : _editspec = new RelayCommand(n =>
                                                            {
                                                                Speciality sp = Specialities.CurrentItem as Speciality;
                                                                var w = new SpecialityAdd(sp);
                                                                if (w.ShowDialog() == true)
                                                                {
                                                                    try
                                                                    {
                                                                        var vm = w.DataContext as SpecialityAddVM;
                                                                        if (vm == null) return;
                                                                        vm.Speciality.SaveChanges(CommonInfo.connection);
                                                                        sp.Copy(vm.Speciality);
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                                                    }
                                                                }
                                                            },
                                                                o =>
                                                                {
                                                                    return app.Permissions.Actions[PermissionAction.SpecialitiesEdit];
                                                                });
            }
        }

        #endregion Commands

        public SpecialityVM() { }
        private static void SearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = d as SpecialityVM;
            if (vm == null) return;
            if (vm.SearchText.Length > 1)
            {
                vm.Specialities.Filter = n => (n as Speciality).Code.ContainWithComparison(vm.SearchText, StringComparison.CurrentCultureIgnoreCase);
            }
            else vm.Specialities.Filter = null;
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (val) return "действует";
            else return "не действует";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
