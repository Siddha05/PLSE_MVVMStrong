using Microsoft.Win32;
using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class EmployeesVM : DependencyObject
    {
        #region Properties
        public ObservableCollection<Expert> ExpertList { get; }
        public Employee Employee { get; }
        public ListCollectionView SettlementsList { get; } = new ListCollectionView(CommonInfo.Settlements);
        public IReadOnlyCollection<string> StreetTypeList { get; } = CommonInfo.StreetTypes;
        public IReadOnlyCollection<string> EmployeeStatus { get; } = CommonInfo.EmployeeStatus;
        public IReadOnlyCollection<string> Genders { get; } = CommonInfo.Genders;
        public IReadOnlyCollection<string> InnerOffice { get; } = CommonInfo.InnerOfficies;
        public ObservableCollection<Departament> Departaments { get; } = CommonInfo.Departaments;
        public IEnumerable<Speciality> SpecialitiesList { get; }
        public bool PopupVisibility
        {
            get { return (bool)GetValue(PopupVisibilityProperty); }
            set { SetValue(PopupVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(bool), typeof(EmployeesVM), new PropertyMetadata(false));

        //public Employee Employee
        //{
        //    get { return (Employee)GetValue(EmployeeProperty); }
        //    set { SetValue(EmployeeProperty, value); }
        //}
        //public static readonly DependencyProperty EmployeeProperty =
        //    DependencyProperty.Register("Employee", typeof(Employee), typeof(EmployeesVM), new PropertyMetadata(null));

        #endregion

        #region Commands
        public RelayCommand Exit { get; }
        public RelayCommand ImageSelect { get; }
        public RelayCommand SettlementTextChanged { get; }
        public RelayCommand SettlementSelect { get; }
        public RelayCommand Save { get; }
        public RelayCommand AddSpeciality { get; }
        public RelayCommand DeleteSpeciality { get; }
        #endregion
        public EmployeesVM()
        {
            this.Employee = MainVM._employee.Clone();
            ExpertList = new ObservableCollection<Expert>(CommonInfo.Experts.Where(n => n.Employee.EmployeeID == Employee.EmployeeID));
            Exit = new RelayCommand(n =>
            {
                var wnd = n as Employees;
                if (wnd == null) return;
                wnd.DialogResult = false;
                wnd.Close();
            });
            Save = new RelayCommand(n =>
            {
                var wnd = n as Employees;
                if (wnd == null) return;
                wnd.DialogResult = true;
                wnd.Close();
            });
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
