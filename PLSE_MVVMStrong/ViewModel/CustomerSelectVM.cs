using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Windows;
using System.Windows.Data;
//using System.Linq;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class CustomerSelectVM : DependencyObject
    {
        #region Properties

        public ListCollectionView CustomersList { get; } = new ListCollectionView(CommonInfo.Customers);

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(CustomerSelectVM), new PropertyMetadata(String.Empty, SearchText_Changed));

        #endregion Properties

        #region Commands

        public RelayCommand Cancel { get; }
        public RelayCommand Select { get; }
        public RelayCommand NewCustomer { get; }
        public RelayCommand Edit { get; }
        public RelayCommand Delete { get; }

        #endregion Commands

        public CustomerSelectVM()
        {
            Cancel = new RelayCommand(n =>
                                        {
                                            var wnd = n as CustomerSelect;
                                            if (wnd != null)
                                            {
                                                wnd.DialogResult = false;
                                                wnd.Close();
                                            }
                                        });
            Select = new RelayCommand(n =>
            {
                var wnd = n as CustomerSelect;
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            });
            NewCustomer = new RelayCommand(n =>
            {
                var wnd = n as CustomerSelect;
                if (wnd == null) return;
                var w = new CustomerAdd { Owner = wnd };
                w.ShowDialog();
                if (w.DialogResult ?? false)
                {
                    try
                    {
                        var vm = w.DataContext as CustomerAddVM;
                        if (vm == null) return;
                        vm.Customer.SaveChanges(CommonInfo.connection);
                        CustomersList.AddNewItem(vm.Customer);
                        CustomersList.MoveCurrentTo(vm.Customer);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });
            Edit = new RelayCommand(n =>
            {
                var w = new CustomerAdd();
                w.DataContext = new CustomerAddVM(CustomersList.CurrentItem as Customer);
                w.ShowDialog();
                if (w.DialogResult ?? false)
                {
                    try
                    {
                        var vm = w.DataContext as CustomerAddVM;
                        if (vm == null) return;
                        vm.Customer.SaveChanges(CommonInfo.connection);
                        object o = CustomersList.CurrentItem;
                        o = vm.Customer;
                        CustomersList.Refresh();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });
        }

        private static void SearchText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as CustomerSelectVM;
            if (o == null) return;
            o.CustomersList.Filter = n => (n as Customer).Sname.StartsWith(o.SearchText, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}