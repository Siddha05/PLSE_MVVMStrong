﻿using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class ResolutionAddVM : DependencyObject
    {
#region Fields
        static private string[] CaseTypeContract = new string[] { "исследование"};
        static private IEnumerable<string> CaseTypesFull = CommonInfo.CaseTypes.Keys.Except(CaseTypeContract);
        private RelayCommand _delexpertise;
        private RelayCommand _addnewcustomer;
        private RelayCommand _editcustomer;
        private RelayCommand _addexpertise;
        #endregion Fields

        #region Properties
        public IReadOnlyList<string> ResolutionTypes => CommonInfo.ResolutionTypes;
        public IReadOnlyList<string> ResolutionStatus => CommonInfo.ResolutionStatus;
        public ListCollectionView CustomersList { get; }
        public object SelectedCustomer { get; set; }
        public Resolution Resolution { get; set; }
        public bool CustomersListOpened
        {
            get { return (bool)GetValue(CustomersListOpenedProperty); }
            set { SetValue(CustomersListOpenedProperty, value); }
        }
        public static readonly DependencyProperty CustomersListOpenedProperty =
            DependencyProperty.Register("CustomersListOpened", typeof(bool), typeof(ResolutionAddVM), new PropertyMetadata(false));

        public string CustomerSearchText
        {
            get { return (string)GetValue(CustomerSearchTextProperty); }
            set { SetValue(CustomerSearchTextProperty, value); }
        }
        public static readonly DependencyProperty CustomerSearchTextProperty =
            DependencyProperty.Register("CustomerSearchText", typeof(string), typeof(ResolutionAddVM), new PropertyMetadata(String.Empty, CustomerSearchTextChanged));

        [Obsolete]
        public string Info
        {
            get => (string)GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }
        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(string), typeof(ResolutionAddVM), new PropertyMetadata(string.Empty));
        public IEnumerable<string> CaseTypesList
        {
            get => (IEnumerable<string>)GetValue(CaseTypesListProperty);
            set => SetValue(CaseTypesListProperty, value);
        }
        public static readonly DependencyProperty CaseTypesListProperty =
            DependencyProperty.Register("CaseTypesList", typeof(IEnumerable<string>), typeof(ResolutionAddVM), new PropertyMetadata(CommonInfo.CaseTypes.Keys));

        public Visibility NumberVisible
        {
            get => (Visibility)GetValue(NumberVisibleProperty);
            set => SetValue(NumberVisibleProperty, value);
        }
        public static readonly DependencyProperty NumberVisibleProperty =
            DependencyProperty.Register("NumberVisible", typeof(Visibility), typeof(ResolutionAddVM), new PropertyMetadata(Visibility.Visible));

        public Visibility RespondentVisible
        {
            get => (Visibility)GetValue(RespondentVisiblProperty);
            set => SetValue(RespondentVisiblProperty, value);
        }
        public static readonly DependencyProperty RespondentVisiblProperty =
            DependencyProperty.Register("RespondentVisible", typeof(Visibility), typeof(ResolutionAddVM), new PropertyMetadata(Visibility.Collapsed));
        public object SelectedExpertise { get; set; }
        #endregion Properties

#region Commands
        public RelayCommand ObjectsClick { get; }
        public RelayCommand QuestionsClick { get; }
        public RelayCommand Save { get; }
        public RelayCommand AddExpertise
        {
            get
            {
                return _addexpertise != null ? _addexpertise : new RelayCommand(n =>
                                                                {
                                                                    var wnd = new ExpertiseAdd { Owner = n as ResolutionAdd };
                                                                    wnd.ShowDialog();
                                                                    if (wnd.DialogResult ?? false)
                                                                    {
                                                                        Resolution.Expertisies.Add((wnd.DataContext as ExpertiseAddVM).Expertise);
                                                                    }
                                                                });
            }
        }
        public RelayCommand DeleteExpertise
        {
            get
            {
                return _delexpertise != null ? _delexpertise : new RelayCommand(n =>
                                                    {
                                                        Resolution.Expertisies.Remove(SelectedExpertise as Expertise);
                                                    },
                                                        o =>
                                                    {
                                                        if (SelectedExpertise != null) return true;
                                                        else return false;
                                                    });
            }
        }
        public RelayCommand AddNewCustomer
        {
            get
            {
                return _addnewcustomer != null ? _addnewcustomer : new RelayCommand(n =>
                                                    {
                                                        var wnd = new CustomerAdd();
                                                        CustomersListOpened = false;
                                                        wnd.ShowDialog();
                                                        if (wnd.DialogResult == true)
                                                        {
                                                            Resolution.Customer = (wnd.DataContext as CustomerAddVM)?.Customer;
                                                        }
                                                    });
            }
        }
        public RelayCommand EditCustomer
        {
            get
            {
                return _editcustomer != null ? _editcustomer : new RelayCommand(n =>
                                                        {
                                                            var wnd = new CustomerAdd();
                                                            wnd.DataContext = new CustomerAddVM(SelectedCustomer as Customer);
                                                            CustomersListOpened = false;
                                                            wnd.ShowDialog();
                                                            if (wnd.DialogResult == true)
                                                            {
                                                                Resolution.Customer = (wnd.DataContext as CustomerAddVM)?.Customer;
                                                            }
                                                        },
                                                            o =>
                                                        {
                                                            if (SelectedCustomer != null) return true;
                                                            return false;
                                                        });
            }
        }
        public RelayCommand SelectCustomer { get; }
        #endregion Commands
        
        public ResolutionAddVM()
        {
            Resolution = InicialState();
            Resolution.PropertyChanged += Resolution_PropertyChanged;
            CustomersList = new ListCollectionView(CommonInfo.Customers);
            //Resolution.Expertisies.Add(new Expertise(id: 0,
            //                                          number: "12",
            //                                          expert: CommonInfo.Experts.Single(n => n.ExpertID == 6),
            //                                          status: "в работе",
            //                                          start: DateTime.Now,
            //                                          end: null,
            //                                          timelimit: (byte)20,
            //                                          type: "первичная",
            //                                          previous: null,
            //                                          spendhours: null,
            //                                          vr: Model.Version.New));
            //Resolution.Expertisies.Add(new Expertise(id: 0,
            //                                          number: "2056",
            //                                          expert: CommonInfo.Experts.Single(n => n.ExpertID == 8),
            //                                          status: "выполнена",
            //                                          start: DateTime.Now.AddDays(-34),
            //                                          end: DateTime.Now.AddDays(-11),
            //                                          timelimit: (byte)30,
            //                                          type: "первичная",
            //                                          previous: null,
            //                                          spendhours: 48,
            //                                          vr: Model.Version.New));

            SelectCustomer = new RelayCommand(n =>
            {
                Resolution.Customer = SelectedCustomer as Customer;
                CustomerSearchText = "";
            },
                o => // Advisably ?? 
            {
                if (SelectedCustomer != null) return true;
                return false;
            });
            QuestionsClick = new RelayCommand(n =>
            {
                var o = n as Popup;
                if (o.IsOpen) o.IsOpen = false;
                else o.IsOpen = true;
            });
            ObjectsClick = new RelayCommand(n =>
            {
                var o = n as Popup;
                if (o.IsOpen) o.IsOpen = false;
                else o.IsOpen = true;
            });
            Save = new RelayCommand(n =>
            {
                try
                {
                    Resolution.SaveChanges(CommonInfo.connection);
                    var pr = new DocsCreater(Resolution);
                    var t = pr.OnExpertiseCreateAsync();
                    if (MessageBox.Show("Сохранение успешно. Продолжить?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Resolution = InicialState();
                    }
                    else (n as Window).Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            },
             o=>
             {
                 if (Resolution.IsInstanceValidState) return true;
                 else return false;
             });
        }
        private void Resolution_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResolutionType")
            {
                if (Resolution.ResolutionType == "договор")
                {
                    CaseTypesList = CaseTypeContract;
                    
                }
                else
                {
                    CaseTypesList = CaseTypesFull;
                    Resolution.Case.TypeCase = "уголовное";
                }
                return;
            }
            if (e.PropertyName == "TypeCase")
            {
                switch (Resolution.Case.TypeCase)
                {
                    case "исследование":
                        NumberVisible = Visibility.Collapsed;
                        RespondentVisible = Visibility.Collapsed;
                        break;
                    case "административное правонарушение":
                    case "проверка КУCП":
                    case "уголовное":
                        NumberVisible = Visibility.Visible;
                        RespondentVisible = Visibility.Collapsed;
                        break;
                    default:
                        NumberVisible = Visibility.Visible;
                        RespondentVisible = Visibility.Visible;
                        break;
                }
            }
        }
        private static void CustomerSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ResolutionAddVM;
            if (instance.CustomerSearchText.Length > 1)
            {
                instance.CustomersListOpened = true;
                instance.CustomersList.Filter = n => (n as Customer).Sname.StartsWith(instance.CustomerSearchText, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                instance.CustomersList.Filter = null;
                instance.CustomersListOpened = false;
            }
        }
        private Resolution InicialState()
        {
            var r = new Resolution()
            {
                RegistrationDate = DateTime.Now,
                ResolutionType = "постановление",
                ResolutionStatus = "рассмотрение",
            };
            r.Case.TypeCase = "уголовное";
            return r;
        }

    }
}