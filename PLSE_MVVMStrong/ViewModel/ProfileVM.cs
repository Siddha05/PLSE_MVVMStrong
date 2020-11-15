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
using System.Windows.Controls;
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
        private RelayCommand _genderchaecked;
        #endregion
        #region Properties
        /// <summary>
        /// Сохраняет индекс выбранной специальности для замены в <c>CommonInfo.Specialities</c>
        /// </summary>
        private int ExpertIndex { get; set; } = -1;
        private int EmployeeIndex { get; set; } = -1;
        public ListCollectionView ExpertList { get; }
        public Employee Employee { get; }
        public ListCollectionView SettlementsList { get; } = new ListCollectionView(CommonInfo.Settlements);
        public IReadOnlyCollection<string> StreetTypeList { get; } = CommonInfo.StreetTypes;
        public IReadOnlyCollection<string> EmployeeStatus { get; } = CommonInfo.EmployeeStatus;
        public IReadOnlyCollection<string> Genders { get; } = CommonInfo.Genders;
        public IReadOnlyCollection<string> InnerOffice { get; } = CommonInfo.InnerOfficies;
        public ObservableCollection<Departament> Departaments { get; } = CommonInfo.Departaments;
        public IEnumerable<Speciality> SpecialitiesList { get; } = CommonInfo.Specialities;
        public Expert Expert
        {
            get { return (Expert)GetValue(ExpertProperty); }
            set { SetValue(ExpertProperty, value); }
        }
        public static readonly DependencyProperty ExpertProperty =
            DependencyProperty.Register("Expert", typeof(Expert), typeof(ProfileVM), new PropertyMetadata(null));
        public object SelectedExpert
        {
            get { return (object)GetValue(SelectedExpertProperty); }
            set { SetValue(SelectedExpertProperty, value); }
        }
        public static readonly DependencyProperty SelectedExpertProperty =
            DependencyProperty.Register("SelectedExpert", typeof(object), typeof(ProfileVM), new PropertyMetadata(null));
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
                        Employee.EmployeeCore.Foto = br.ReadBytes((int)fileInfo.Length);
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
                    Employee.EmployeeCore.Adress.Settlement = sel;
                    SettlementPopupVisibility = false;
                });
            }
        }
        public RelayCommand Save
        { 
            get
            {
                return new RelayCommand(n =>
                {
                    var wnd = n as Profile;
                    if (wnd == null) return;
                    try
                    {
                        int oldid = Employee.EmployeeID;
                        Employee.SaveChanges(CommonInfo.connection);
                        if (oldid != Employee.EmployeeID)
                        {
                            CommonInfo.Employees.Add(Employee);
                            //CommonInfo.Employees.First(n => n.EmployeeID == Employee.PreviousID).Actual = false;
                        }
                        else
                        {
                            CommonInfo.Employees[EmployeeIndex] = Employee;
                        }
                        (Application.Current as App).LogedEmployee = Employee;
                    }
                        catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                    wnd.Close();
                });
            }
        }
        public RelayCommand AddSpeciality
        {
            get
            {
                return _addspec != null ? _addspec : _addspec = new RelayCommand(n =>
                {
                    PopupVisibility = true;
                    Expert = new Expert(Employee);
                });
            }
        }
        public RelayCommand DeleteSpeciality
        {
            get
            {
                return _deletespec ?? new RelayCommand(n =>
                {
                    var result = MessageBox.Show($"Удалить выбранную специальность\n {(SelectedExpert as Expert).ExpertCore.Speciality.Code}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var exp = SelectedExpert as Expert;
                        if (exp == null) return;
                        try
                        {
                            exp.DeleteFromDB(CommonInfo.connection);
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
                return _editspec != null ? _editspec : new RelayCommand(n =>
                                                            {
                                                                var exp = SelectedExpert as Expert;
                                                                if (exp == null) return;
                                                                ExpertIndex = CommonInfo.Experts.IndexOf(exp);
                                                                Expert = exp.Clone();
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
                        var v = Expert.Version;
                        Expert.SaveChanges(CommonInfo.connection);
                        switch (v)
                        {
                            case Model.Version.New:
                                CommonInfo.Experts.Add(Expert);
                                break;
                            case Model.Version.Edited:
                                CommonInfo.Experts.RemoveAt(ExpertIndex);
                                CommonInfo.Experts.Add(Expert);
                                ExpertIndex = -1;
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
                 return Expert?.IsInstanceValidState ?? false;
             }
            );
            }
        }
        public RelayCommand GenderChecked
        {
            get
            {
                return _genderchaecked != null ? _genderchaecked : _genderchaecked = new RelayCommand(n =>
                {
                    if (n != null)
                    {
                        Employee.Gender = n.ToString();
                    }
                });
            }
        }
        #endregion
        public ProfileVM()
        {
            this.Employee = (Application.Current as App).LogedEmployee.Clone();
            EmployeeIndex = CommonInfo.Employees.IndexOf((Application.Current as App).LogedEmployee);
            //this.Employee = CommonInfo.Employees.First(n => n.EmployeeID == 7);
            ExpertList = new ListCollectionView(CommonInfo.Experts);
            ExpertList.Filter = n => (n as Expert).Employee.EmployeeID == this.Employee.EmployeeID;
            ExpertList.SortDescriptions.Add(new System.ComponentModel.SortDescription("ExpertCore.Closed", System.ComponentModel.ListSortDirection.Ascending));
        }
    }
}
