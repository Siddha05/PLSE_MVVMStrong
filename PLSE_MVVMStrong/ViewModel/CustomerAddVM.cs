using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class CustomerAddVM : DependencyObject
    {
        #region Properties

        public IReadOnlyCollection<string> Genders => CommonInfo.Genders;
        public IReadOnlyCollection<string> Ranks { get; } = CommonInfo.Ranks;
        public ListCollectionView OrganizationsList { get; } = new ListCollectionView(CommonInfo.Organizations);

        public Customer Customer { get; set; } = new Customer
        {
            IsValid = true,
            Declinated = true,
        };
        public string OrganizationSearchText
        {
            get => (string)GetValue(OrganizationSearchProperty);
            set => SetValue(OrganizationSearchProperty, value);
        }
        public static readonly DependencyProperty OrganizationSearchProperty =
            DependencyProperty.Register("OrganizationSearchText", typeof(string), typeof(CustomerAddVM), new PropertyMetadata("", OrganizationSearch_Changed));

        public bool OrganizationListOpen
        {
            get { return (bool)GetValue(OrganizationListOpenProperty); }
            set { SetValue(OrganizationListOpenProperty, value); }
        }
        public static readonly DependencyProperty OrganizationListOpenProperty =
            DependencyProperty.Register("OrganizationListOpen", typeof(bool), typeof(CustomerAddVM), new PropertyMetadata(false));
        public object SelectedOrganization { get; set; }

        #endregion Properties

        #region Commands
        public RelayCommand AddCustomer { get; }
        public RelayCommand AddNewOrganization { get; }
        public RelayCommand EditOrganization { get; }
        public RelayCommand SelectOrganization { get; }
        #endregion Commands

        private static void OrganizationSearch_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as CustomerAddVM;
            if (view == null) return;
            if (view.OrganizationSearchText.Length > 1)
            {
                view.OrganizationListOpen = true;
                view.OrganizationsList.Filter = n => (n as Organization).Name.ContainWithComparison(view.OrganizationSearchText, System.StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                view.OrganizationListOpen = false;
                view.OrganizationsList.Filter = null;
            }
        }

        public CustomerAddVM()
        {
            AddCustomer = new RelayCommand(o =>
                {
                    var w = o as View.CustomerAdd;
                    w.DialogResult = true;
                    w.Close();
                },
                    x => Customer.InstanceValidState ? true : false);
            AddNewOrganization = new RelayCommand(n =>
            {
                var wnd = new OrganizationAdd();
                OrganizationListOpen = false;
                if(wnd.ShowDialog() == true)
                {
                    MessageBox.Show("Save new organization and set  to this cusnomer");
                }
            });
            SelectOrganization = new RelayCommand(n =>
            {
                Customer.Organization = SelectedOrganization as Organization;
                OrganizationListOpen = false;
            });
            EditOrganization = new RelayCommand(n =>
            {
                var wnd = new OrganizationAdd();
                wnd.DataContext = new OrganizationAddVM(SelectedOrganization as Organization);
                OrganizationListOpen = false;
                if (wnd.ShowDialog() == true)
                {
                    MessageBox.Show("Save new organization and set to this cusnomer");
                }
            },
                o =>
                {
                    if (SelectedOrganization != null) return true;
                    return false;
                }
            );
        }
        public CustomerAddVM(Customer obj) : this()
        {
            Customer = obj.Clone();
        }
    }
}