using Microsoft.Win32;
using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ProfileVM : DependencyObject
    {
        #region Fields
        private RelayCommand _imageselect;
        private RelayCommand _settlementtextchanged;
        private RelayCommand _addspec;
        private RelayCommand _deletespec;
        private RelayCommand _save;
        private RelayCommand _settlementselect;
        private RelayCommand _editspec;
        #endregion
        #region Properties
        /// <summary>
        /// Сохраняет индекс выбранной специальности для замены в <c>CommonInfo.Specialities</c>
        /// </summary>
        private int SpecialityIndex { get; set; }
        public ListCollectionView ExpertList { get; }
        public Employee Employee { get; }
        public Expert Expert { get; private set; }
        public ListCollectionView SettlementsList { get; } = new ListCollectionView(CommonInfo.Settlements);
        public IReadOnlyCollection<string> StreetTypeList { get; } = CommonInfo.StreetTypes;
        public IReadOnlyCollection<string> EmployeeStatus { get; } = CommonInfo.EmployeeStatus;
        public IReadOnlyCollection<string> Genders { get; } = CommonInfo.Genders;
        public IReadOnlyCollection<string> InnerOffice { get; } = CommonInfo.InnerOfficies;
        public ObservableCollection<Departament> Departaments { get; } = CommonInfo.Departaments;
        public IEnumerable<Speciality> SpecialitiesList { get; } = CommonInfo.Specialities;

        public object SelectedExpert
        {
            get { return (object)GetValue(SelectedExpertProperty); }
            set { SetValue(SelectedExpertProperty, value); }
        }
        public static readonly DependencyProperty SelectedExpertProperty =
            DependencyProperty.Register("SelectedExpert", typeof(object), typeof(ProfileVM), new PropertyMetadata(null, SelectedExpertChanged));

        public bool PopupVisibility
        {
            get { return (bool)GetValue(PopupVisibilityProperty); }
            set { SetValue(PopupVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(bool), typeof(ProfileVM), new PropertyMetadata(false));

        public bool SettlementPopupVisibility
        {
            get { return (bool)GetValue(SettlementPopupVisibilityProperty); }
            set { SetValue(SettlementPopupVisibilityProperty, value); }
        }
        public static readonly DependencyProperty SettlementPopupVisibilityProperty =
            DependencyProperty.Register("SettlementPopupVisibility", typeof(bool), typeof(ProfileVM), new PropertyMetadata(false));
        #endregion

        #region Commands
        public RelayCommand ImageSelect
        {
            get
            {
                return _imageselect ?? new RelayCommand(n =>
                {
                    OpenFileDialog openFile = new OpenFileDialog()
                    {
                        DefaultExt = ".jpg",
                        Filter = @"image file|*.jpg",
                        Title = "Выберите изображение"
                    };
                    if (openFile.ShowDialog() == true)
                    {
                        FileInfo fileInfo = new FileInfo(openFile.FileName);
                        FileStream fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(fs);
                        Employee.Foto = br.ReadBytes((int)fileInfo.Length);
                    }
                });
            }
        }
        public RelayCommand SettlementTextChanged
        {
            get
            {
                return _settlementtextchanged ?? new RelayCommand(n =>
                {
                    string text = n.ToString();
                    if (text.Length > 1)
                    {
                        SettlementPopupVisibility = true;
                        SettlementsList.Filter = x => (x as Settlement).Title.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else
                    {
                        SettlementPopupVisibility = false;
                        SettlementsList.Filter = null;
                    }
                });
            }
        }
        public RelayCommand SettlementSelect
        {
            get
            {
                return _settlementselect ?? new RelayCommand(n =>
                {
                    var sel = n as Settlement;
                    if (n == null) return;
                    Employee.Adress.Settlement = sel;
                    SettlementPopupVisibility = false;
                });
            }
        }
        public RelayCommand Save { get; }
        public RelayCommand AddSpeciality
        {
            get
            {
                return _addspec ?? new RelayCommand(n =>
                {
                    PopupVisibility = true;
                    SelectedExpert = new Expert()
                    {
                        Employee = this.Employee,
                        Closed = false,
                        ReceiptDate = DateTime.Now
                    };
                });
            }
        }
        public RelayCommand DeleteSpeciality
        {
            get
            {
                return _deletespec ?? new RelayCommand(n =>
                {
                    var result = MessageBox.Show($"Удалить выбранную специальность\n {(SelectedExpert as Expert).Speciality.Code}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var exp = SelectedExpert as Expert;
                        if (exp == null) return;
                        try
                        {
                            exp.DBDelete(CommonInfo.connection);
                            CommonInfo.Experts.Remove(exp);
                        }
                        catch (SqlException)
                        {
                            MessageBox.Show("Удаление невозможно. Имеются связанные записи в базе данных", "Удаление", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                },
                o =>
                {
                    return SelectedExpert != null;
                }
            );
            }
        }
        public RelayCommand EditSpeciality
        {
            get
            {
                return _editspec ?? new RelayCommand(n =>
                {
                    var exp = SelectedExpert as Expert;
                    if (exp == null) return;
                    SpecialityIndex = CommonInfo.Experts.IndexOf(exp);
                    SelectedExpert = exp.Clone();
                    PopupVisibility = true;
                },
                o =>
                {
                    return SelectedExpert != null;
                }
            );
            }
        }
        public RelayCommand SaveSpeciality
        {
            get
            {
                return _save ?? new RelayCommand(n =>
                {
                    try
                    {
                        var exp = SelectedExpert as Expert;
                        if (exp == null) return;
                        var vr = exp.Version;
                        exp.SaveChanges(CommonInfo.connection);
                        switch (vr)
                        {
                            case Model.Version.New:
                                CommonInfo.Experts.Add(exp);
                                break;
                            case Model.Version.Edited:
                                CommonInfo.Experts[SpecialityIndex] = exp;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        PopupVisibility = false;
                    }

                },
             o =>
             {
                 return (SelectedExpert as Expert)?.IsInstanceValidState ?? false;
             }
            );
            }
        }
        #endregion
        public ProfileVM()
        {
            this.Employee = (Application.Current as App).LogedEmployee.Clone();
            ExpertList = new ListCollectionView(CommonInfo.Experts);
            ExpertList.Filter = n => (n as Expert).Employee.EmployeeID == this.Employee.EmployeeID;
            Save = new RelayCommand(n =>
            {
                var wnd = n as Profile;
                if (wnd == null) return;
                wnd.DialogResult = true;
                wnd.Close();
            });
        }

        private static void SelectedExpertChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ProfileVM;
            if (d == null) return;
            obj.Expert = obj.SelectedExpert as Expert;
        }
    }
}
