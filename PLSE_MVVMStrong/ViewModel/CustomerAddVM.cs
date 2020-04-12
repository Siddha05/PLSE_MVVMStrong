using PLSE_MVVMStrong.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class CustomerAddVM : DependencyObject
    {
        #region Properties

        public IReadOnlyCollection<string> Genders => CommonInfo.Genders;
        public IReadOnlyCollection<string> Ranks { get; } = CommonInfo.Ranks;
        public CollectionView Organizations { get; } = new CollectionView(CommonInfo.Organizations);

        public Customer Customer { get; set; } = new Customer
        {
            IsValid = true,
            Declinated = true,
        };

        public string OrganizationSearch
        {
            get => (string)GetValue(OrganizationSearchProperty);
            set => SetValue(OrganizationSearchProperty, value);
        }

        public static readonly DependencyProperty OrganizationSearchProperty =
            DependencyProperty.Register("MyProperty", typeof(string), typeof(CustomerAddVM), new PropertyMetadata("", OrganizationSearch_Changed));

        #endregion Properties

        #region Commands

        public RelayCommand Exit { get; }
        public RelayCommand AddCustomer { get; }
        public RelayCommand SelectOrganization { get; }

        #endregion Commands

        private static void OrganizationSearch_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as CustomerAddVM;
            if (view == null) return;
            if (view.OrganizationSearch.Length > 1)
            {
            }
        }

        public CustomerAddVM()
        {
            Exit = new RelayCommand(o =>
            {
                var w = o as View.CustomerAdd;
                w.DialogResult = false;
                w.Close();
            });
            AddCustomer = new RelayCommand(o =>
                {
                    var w = o as View.CustomerAdd;
                    w.DialogResult = true;
                    w.Close();
                },
                    x => Customer.InstanceValidState ? true : false
                );
            SelectOrganization = new RelayCommand(o =>
            {
                var w = o as View.CustomerAdd;
                var wnd = new View.OrganizationSelect { Owner = w };
                wnd.ShowDialog();
                if (wnd.DialogResult ?? false)
                {
                    Customer.Organization = wnd.lbOrganization.SelectedItem as Organization;
                }
            });
        }
        public CustomerAddVM(Customer obj) : this()
        {
            Customer = obj.Clone();
        }
    }
}