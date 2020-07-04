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
        private RelayCommand _addnewcust;
        private RelayCommand _selectorg;
        private RelayCommand _editorg;
        private RelayCommand _save;
        public RelayCommand Save
        {
            get
            {
                return _save != null ? _save : _save = new RelayCommand(o =>
                                                    {
                                                        var w = o as View.CustomerAdd;
                                                        w.DialogResult = true;
                                                        w.Close();
                                                    },
                                                        x => Customer.IsInstanceValidState ? true : false);
            }
        }
        public RelayCommand AddNewOrganization
        {
            get
            {
                return _addnewcust != null ? _addnewcust : _addnewcust = new RelayCommand(n =>
                {
                                                                var wnd = new OrganizationAdd();
                                                                OrganizationListOpen = false;
                                                                if (wnd.ShowDialog() == true)
                                                                {
                                                                    Customer.Organization = (wnd.DataContext as OrganizationAddVM)?.Organization;
                                                                }
                });
            }
        }
        public RelayCommand EditOrganization
        {
            get
            {
                 return _editorg != null ? _editorg : _editorg = new RelayCommand(n =>
                                                     {
                                                         var wnd = new OrganizationAdd();
                                                         wnd.DataContext = new OrganizationAddVM(SelectedOrganization as Organization);
                                                         OrganizationListOpen = false;
                                                         if (wnd.ShowDialog() == true)
                                                         {
                                                             Customer.Organization = (wnd.DataContext as OrganizationAddVM)?.Organization;
                                                         }
                                                     },
                                                        o =>
                                                     {
                                                            if (SelectedOrganization != null) return true;
                                                            return false;
                                                     }
            );}
        }
        public RelayCommand SelectOrganization
        {
            get
            {
                return _selectorg != null ? _selectorg : _selectorg = new RelayCommand(n =>
                {
                                                                Customer.Organization = SelectedOrganization as Organization;
                                                                OrganizationListOpen = false;
                });
            }
        }
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

        public CustomerAddVM() { }
        public CustomerAddVM(Customer obj) : this()
        {
            Customer = obj.Clone();
        }
    }
}