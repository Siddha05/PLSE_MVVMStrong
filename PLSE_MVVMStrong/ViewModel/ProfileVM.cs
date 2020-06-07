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
        #region Properties
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

        private static void SelectedExpertChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ProfileVM;
            if (d == null) return;
            obj.Expert = obj.SelectedExpert as Expert;
        }

        public bool PopupVisibility
        {
            get { return (bool)GetValue(PopupVisibilityProperty); }
            set { SetValue(PopupVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(bool), typeof(ProfileVM), new PropertyMetadata(false));
        #endregion

        #region Commands
        public RelayCommand Exit { get; }
        public RelayCommand ImageSelect { get; }
        public RelayCommand SettlementTextChanged { get; }
        public RelayCommand SettlementSelect { get; }
        public RelayCommand Save { get; }
        public RelayCommand AddSpeciality { get; }
        public RelayCommand DeleteSpeciality { get; }
        public RelayCommand EditSpeciality { get; }
        public RelayCommand SaveSpeciality { get; }
        #endregion
        public ProfileVM()
        {
            //this.Employee = (Application.Current as App).LogedEmployee.Clone();
            this.Employee = CommonInfo.Employees.Single(n => n.EmployeeID == 7);
            ExpertList = new ListCollectionView(CommonInfo.Experts);
            ExpertList.Filter = n => (n as Expert).Employee.EmployeeID == this.Employee.EmployeeID;
            Save = new RelayCommand(n =>
            {
                var wnd = n as Profile;
                if (wnd == null) return;
                wnd.DialogResult = true;
                wnd.Close();
            });
            AddSpeciality = new RelayCommand(n =>
            {
                PopupVisibility = true;
                SelectedExpert = new Expert()
                {
                    Employee = this.Employee,
                    IsValid = true,
                    ReceiptDate = DateTime.Now
                };
            });
            EditSpeciality = new RelayCommand(n =>
            {
                SelectedExpert = (SelectedExpert as Expert).Clone();
                PopupVisibility = true;
            },
                o =>
                {
                    return SelectedExpert != null;
                }
            );
            DeleteSpeciality = new RelayCommand(n =>
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
            SaveSpeciality = new RelayCommand(n =>
            {
                var type = n.ToString();
                MessageBox.Show(type);
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
                            CommonInfo.Experts[] = exp;
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
            ImageSelect = new RelayCommand(n =>
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
}
