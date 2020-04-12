using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class OrganizationSelectVM : DependencyObject
    {
        #region Propetries

        public ListCollectionView OrganizationList { get; } = new ListCollectionView(CommonInfo.Organizations);
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(OrganizationSelectVM), new PropertyMetadata("", SearchText_Changed));

        #endregion Propetries

        #region Commands

        public RelayCommand Cancel { get; }
        public RelayCommand Select { get; }
        public RelayCommand NewOrganization { get; }
        public RelayCommand EditOrganization { get; }

        #endregion Commands

        public OrganizationSelectVM()
        {
            Cancel = new RelayCommand(n =>
            {
                var wnd = n as OrganizationSelect;
                if (wnd != null)
                {
                    wnd.DialogResult = false;
                    wnd.Close();
                }
            });
            Select = new RelayCommand(n =>
            {
                var wnd = n as OrganizationSelect;
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            });     
            NewOrganization = new RelayCommand(n =>
            {
                var wnd = new OrganizationAdd();
                wnd.ShowDialog();
                if (wnd.DialogResult ?? false)
                {
                    try
                    {
                        var vm = wnd.DataContext as OrganizationAddVM;
                        if (vm == null) return;
                        vm.Organization.SaveChanges(CommonInfo.connection);
                        OrganizationList.AddNewItem(vm.Organization);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
            EditOrganization = new RelayCommand(n =>
            {
                var wnd = new OrganizationAdd();
                wnd.DataContext = new OrganizationAddVM(OrganizationList.CurrentItem as Organization);
                wnd.ShowDialog();
                if (wnd.DialogResult ?? false)
                {
                    try
                    {
                        var vm = wnd.DataContext as OrganizationAddVM;
                        if (vm == null) return;
                        vm.Organization.SaveChanges(CommonInfo.connection);
                        object o = OrganizationList.CurrentItem;
                        o = vm.Organization;
                        OrganizationList.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });

        }
        private static void SearchText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var w = d as OrganizationSelectVM;
            if (w == null) return;
            w.OrganizationList.Filter = n => (n as Organization).Name.ContainWithComparison(w.SearchText, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}