using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class CustomerAddVM : DependencyObject
    {
#region Properties

        public IReadOnlyCollection<string> Genders => CommonInfo.Genders;
        public IReadOnlyCollection<string> Ranks { get; } = CommonInfo.Ranks;
        public ListCollectionView OrganizationsList { get; } = new ListCollectionView(CommonInfo.Organizations);
        public Customer Customer { get; set; } = Customer.New;
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
        private RelayCommand _addneworg;
        private RelayCommand _selectorg;
        private RelayCommand _editorg;
        private RelayCommand _searchtext;
        public RelayCommand Save
        {
            get
            {
                return new RelayCommand(o =>
                                                    {
                                                        var w = o as View.CustomerAdd;
                                                        try
                                                        {
                                                            Customer.SaveChanges(CommonInfo.connection);
                                                            CommonInfo.Customers.Add(Customer);
                                                            MessageBox.Show("Сохраненение в базу данных успешно", "",  MessageBoxButton.OK,  MessageBoxImage.Information);
                                                            w.DialogResult = true;
                                                        }
                                                        catch (System.Exception)
                                                        {
                                                            MessageBox.Show("Ошибка при сохраненении в базу данных", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                                            w.DialogResult = false;
                                                        }
                                                        w.Close();
                                                    },
                                                        x => Customer.IsInstanceValidState
                                                    );
            }
        }
        public RelayCommand AddNewOrganization
        {
            get
            {
                return _addneworg != null ? _addneworg : _addneworg = new RelayCommand(n =>
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
                    var o = n as Organization;
                    Customer.Organization = o;
                    OrganizationListOpen = false;
                });
            }
        }
        public RelayCommand SearchTextChanged
        {
            get
            {
                return _searchtext != null ? _searchtext : _searchtext = new RelayCommand(n =>
                {
                    var tbox = n as TextBox;
                    if (n == null) return;
                    if (tbox.Text.Length > 3)
                    {
                        OrganizationsList.Filter = k => (k as Organization).Name.ContainWithComparison(tbox.Text, System.StringComparison.CurrentCultureIgnoreCase);
                        OrganizationListOpen = true;
                    }
                    else
                    {
                        OrganizationsList.Filter = null;
                        OrganizationListOpen = false;
                    }
                });
            }
        }
#endregion Commands
        public CustomerAddVM() { }
        public CustomerAddVM(Customer obj) : this()
        {
            Customer = obj.Clone();
        }
    }
}