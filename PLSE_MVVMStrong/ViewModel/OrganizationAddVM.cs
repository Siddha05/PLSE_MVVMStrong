using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class OrganizationAddVM : DependencyObject
    {
        #region Properties

        public ListCollectionView SettlementsList { get; } = new ListCollectionView(CommonInfo.Settlements);
        public IReadOnlyCollection<String> StreetTypeList { get; } = CommonInfo.StreetTypes;
        public Organization Organization { get; } = new Organization { IsValid = true };

        public bool PopupVisibility
        {
            get => (bool)GetValue(PopupVisibilityProperty);
            set => SetValue(PopupVisibilityProperty, value);
        }

        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(bool), typeof(OrganizationAddVM), new PropertyMetadata(false));

        #endregion Properties

        #region Commands

        public RelayCommand Cancel { get; }
        public RelayCommand Add { get; }
        public RelayCommand SettlementSelect { get; }
        public RelayCommand SettlementSearch { get; }

        #endregion Commands

        public OrganizationAddVM()
        {
            Cancel = new RelayCommand(n =>
            {
                var wnd = n as OrganizationAdd;
                if (wnd != null)
                {
                    wnd.DialogResult = false;
                    wnd.Close();
                }
            });
            Add = new RelayCommand(
                n =>
            {
                var wnd = n as OrganizationAdd;
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            },

               n =>
            {
                if (Organization.InstanceValidState) return true;
                else return false;
            });
            SettlementSelect = new RelayCommand(n =>
            {
                var obj = n as Settlement;
                if (obj != null)
                {
                    Organization.Adress.Settlement = obj;
                    PopupVisibility = false;
                }
            });
            SettlementSearch = new RelayCommand(n =>
            {
                var tb = n as TextBox;
                if (tb == null) return;
                if (tb.Text.Length > 1)
                {
                    SettlementsList.Filter = k => (k as Settlement).Title.StartsWith(tb.Text, StringComparison.CurrentCultureIgnoreCase);
                    PopupVisibility = true;
                }
                else PopupVisibility = false;
            });
        }
        public OrganizationAddVM(Organization obj) : this()
        {
            this.Organization = obj;
        }
    }
}