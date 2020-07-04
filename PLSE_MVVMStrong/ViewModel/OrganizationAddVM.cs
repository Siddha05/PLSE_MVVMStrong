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
        private RelayCommand _save;
        private RelayCommand _settlementselect;
        private RelayCommand _settlementsearch;
        public RelayCommand Save
        {
            get
            {
                return _save != null ? _save : _save = new RelayCommand(
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
                                                       if (Organization.IsInstanceValidState) return true;
                                                       else return false;
                                                   });
            }
        }
        public RelayCommand SettlementSelect
        {
            get
            {
                return _settlementselect != null ? _settlementselect : _settlementselect = new RelayCommand(n =>
                                                                    {
                                                                        var obj = n as Settlement;
                                                                        if (obj != null)
                                                                        {
                                                                            Organization.Adress.Settlement = obj;
                                                                            PopupVisibility = false;
                                                                        }
                                                                    });
            }
        }
        public RelayCommand SettlementSearch
        {
            get
            {
                return _settlementsearch != null ? _settlementsearch : _settlementsearch = new RelayCommand(n =>
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
        }

        #endregion Commands

        public OrganizationAddVM() { }
        public OrganizationAddVM(Organization obj) : this()
        {
            this.Organization = obj;
        }
    }
}