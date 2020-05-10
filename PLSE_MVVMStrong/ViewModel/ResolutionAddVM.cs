using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class ResolutionAddVM : DependencyObject
    {
        #region Fields
        static private KeyValuePair<string, string>[] CaseTypeContract = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("исследование", "6") };
        static private IEnumerable<KeyValuePair<string, string>> CaseTypesFull = CommonInfo.CaseTypes.Except(CaseTypeContract);
        #endregion Fields

        #region Properties

        public IReadOnlyList<string> ResolutionTypes => CommonInfo.ResolutionTypes;
        public IReadOnlyList<string> ResolutionStatus => CommonInfo.ResolutionStatus;
        public Resolution Resolution { get; set; } = new Resolution()
        {
            RegistrationDate = DateTime.Now,
            ResolutionType = "постановление",
            ResolutionStatus = "рассмотрение"
        };

        [Obsolete]
        public string Info
        {
            get => (string)GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }
        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(string), typeof(ResolutionAddVM), new PropertyMetadata(string.Empty));

        public KeyValuePair<string,string> CaseType
        {
            get => (KeyValuePair<string, string>)GetValue(CaseTypeProperty);
            set => SetValue(CaseTypeProperty, value);
        }
        public static readonly DependencyProperty CaseTypeProperty =
            DependencyProperty.Register("CaseType", typeof(KeyValuePair<string, string>), typeof(ResolutionAddVM), new PropertyMetadata(new KeyValuePair<string,string>(), CaseType_Changed));

        public IEnumerable<KeyValuePair<string, string>> CaseTypesList
        {
            get => (IEnumerable<KeyValuePair<string, string>>)GetValue(CaseTypesListProperty);
            set => SetValue(CaseTypesListProperty, value);
        }
        public static readonly DependencyProperty CaseTypesListProperty =
            DependencyProperty.Register("CaseTypesList", typeof(IEnumerable<KeyValuePair<string, string>>), typeof(ResolutionAddVM), new PropertyMetadata(CommonInfo.CaseTypes.Except(CaseTypeContract)));

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
            DependencyProperty.Register("RespondentVisible", typeof(Visibility), typeof(ResolutionAddVM), new PropertyMetadata(Visibility.Visible));

        #endregion Properties

        #region Commands

        public RelayCommand Exit { get; } = new RelayCommand(o =>
                                                                {
                                                                    var w = o as View.ResolutionAdd;
                                                                    if (w != null) w.Close();
                                                                });
        public RelayCommand AddCustomer { get; }
        public RelayCommand ObjectsClick { get; }
        public RelayCommand QuestionsClick { get; }
        public RelayCommand Save { get; }
        public RelayCommand AddExpertise { get; }
        public RelayCommand DeleteExpertise { get; }
        #endregion Commands

        public ResolutionAddVM()
        {
            Resolution.PropertyChanged += Resolution_PropertyChanged;
            Resolution.Expertisies.Add(new Expertise(id: 0,
                                                      number: "12",
                                                      expert: CommonInfo.Experts.Single(n => n.ExpertID == 6),
                                                      status: "в работе",
                                                      start: DateTime.Now,
                                                      end: null,
                                                      timelimit: (byte)20,
                                                      type: "первичная",
                                                      previous: null,
                                                      spendhours: null,
                                                      vr: Model.Version.New));
            Resolution.Expertisies.Add(new Expertise(id: 0,
                                                      number: "2056",
                                                      expert: CommonInfo.Experts.Single(n => n.ExpertID == 8),
                                                      status: "выполнена",
                                                      start: DateTime.Now.AddDays(-34),
                                                      end: DateTime.Now.AddDays(-11),
                                                      timelimit: (byte)30,
                                                      type: "первичная",
                                                      previous: null,
                                                      spendhours: 48,
                                                      vr: Model.Version.New));
            AddCustomer = new RelayCommand(o =>
                                            {
                                                var wnd = new View.CustomerSelect() { Owner = o as View.ResolutionAdd };
                                                wnd.ShowDialog();
                                                if (wnd.DialogResult ?? false)
                                                {
                                                    Resolution.Customer = wnd.lbCustomers.SelectedItem as Customer;
                                                }
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            },
             o=>
             {
                 if (Resolution.InstanceValidState) return true;
                 else return false;
             });
            DeleteExpertise = new RelayCommand(n =>
            {
                Resolution.Expertisies.Clear();
            },
                o =>
                {
                    if (Resolution.Expertisies.Count > 0) return true;
                    else return false;
                });
            AddExpertise = new RelayCommand(n =>
            {
                var wnd = new ExpertiseAdd { Owner = n as ResolutionAdd};
                wnd.ShowDialog();
                if (wnd.DialogResult ?? false)
                {
                    Resolution.Expertisies.Add((wnd.DataContext as ExpertiseAddVM).Expertise);
                }
            });
            Info = Resolution.ToString();
        }

        private static void CaseType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ResolutionAddVM;
            if (d == null) return;
            o.Resolution.Case.TypeCase = (KeyValuePair<string,string>)e.NewValue;
            //switch (o.Resolution.Case.TypeCase.Key)
            //{
            //    case "исследование":
            //        o.NumberVisible = Visibility.Collapsed;
            //        o.RespondentVisible = Visibility.Collapsed;
            //        break;
            //    case "административное правонарушение":
            //    case "проверка КУСП":
            //    case "уголовное":
            //        o.NumberVisible = Visibility.Visible;
            //        o.RespondentVisible = Visibility.Collapsed;
            //        break;
            //    default:
            //        o.NumberVisible = Visibility.Visible;
            //        o.RespondentVisible = Visibility.Visible;
            //        break;
            //}
        }
        private void Resolution_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Info = Resolution.ToString();
            if (e.PropertyName == "ResolutionType")
            {
                if (Resolution.ResolutionType == "договор")
                {
                    CaseTypesList = CaseTypeContract;
                    CaseType = CaseTypeContract.Single();
                }
                else
                {
                    CaseTypesList = CaseTypesFull;
                    CaseType = CaseTypesFull.First();
                }
            }
            if (e.PropertyName == "TypeCase")
            {
                switch (Resolution.Case.TypeCase.Key)
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
    }
}