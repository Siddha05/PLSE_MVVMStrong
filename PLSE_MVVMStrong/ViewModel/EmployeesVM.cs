using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class EmployeesVM : DependencyObject
    {
        #region Fields
        ListCollectionView _emloyeesList = new ListCollectionView(CommonInfo.Employees);
        #endregion
        #region Properties
        public ListCollectionView EmloyeesList
        {
            get => _emloyeesList;
        }
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(EmployeesVM), new PropertyMetadata("", SearchTextChanged));

        #endregion
        #region Commands
        public RelayCommand Close { get; }
        public RelayCommand AddEmployee { get; }
        public RelayCommand EditEmployee { get; }
        public RelayCommand DeleteEmployee { get; }
        public RelayCommand Sort { get; }
        public RelayCommand Group { get; }
        #endregion
        public EmployeesVM()
        {
            _emloyeesList.SortDescriptions.Add(new System.ComponentModel.SortDescription("Sname", System.ComponentModel.ListSortDirection.Ascending));
            Close = new RelayCommand(n =>
            {
                var wnd = n as View.Employees;
                wnd.Close();
            });
            Group = new RelayCommand(n =>
            {
                switch (n.ToString())
                {
                    case "Departament":
                        _emloyeesList.GroupDescriptions.Clear();
                        _emloyeesList.GroupDescriptions.Add(new PropertyGroupDescription("Departament"));
                        break;
                    case "Status":
                        _emloyeesList.GroupDescriptions.Clear();
                        _emloyeesList.GroupDescriptions.Add(new PropertyGroupDescription("EmployeeStatus"));
                        break;
                    default:
                        _emloyeesList.GroupDescriptions.Clear();
                        break;
                }
            });
            

        }
        private static void SearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = d as EmployeesVM;
            if (vm == null)
            {
                Debug.Print("NULL");
                return;
            }
            vm._emloyeesList.Filter = n => (n as Employee).Sname.StartsWith(vm.SearchText, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
