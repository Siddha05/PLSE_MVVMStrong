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

        private RelayCommand _exit = new RelayCommand(o =>
                                                          {
                                                              var w = o as View.Specialities;
                                                              if (w != null) w.Close();
                                                          });

        public RelayCommand Exit => _exit;
        public RelayCommand AddNew { get; }
        public RelayCommand Delete { get; }
        public RelayCommand Edit { get; }

        #endregion Commands


        public SpecialityVM()
        {

            Delete = new RelayCommand(RemoveSpec, n => app.Permissions.Actions["SpecialitiesDelete"]);
            AddNew = new RelayCommand(AddSpeciality, n => app.Permissions.Actions["SpecialitiesAdd"]);
            Edit = new RelayCommand(EditSpeciality, n => app.Permissions.Actions["SpecialitiesEdit"]);
        }
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

        private void EditSpeciality(object obj)
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
                    CommonInfo.Specialities[vm.Index] = vm.Speciality;
                    Specialities.Refresh();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void RemoveSpec(object o)
        {
            var sp = Specialities.CurrentItem as Speciality;
            if (sp == null) return;
            if(MessageBox.Show("Вы уверены, что хотите удалить выбранную специальность?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    sp.DeleteFromDB(CommonInfo.connection);
                    Specialities.Remove(Specialities.CurrentItem);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
           
        }
        private void AddSpeciality(object o)
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
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
