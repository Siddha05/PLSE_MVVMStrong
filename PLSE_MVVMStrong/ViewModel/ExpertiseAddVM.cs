using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertiseAddVM
    {
        #region Properties
        public Expertise Expertise { get; } = Expertise.New;
        public IEnumerable<Employee> Experts { get; } = Enumerable.Empty<Employee>();
        public ListCollectionView Specialities { get; } = new ListCollectionView(CommonInfo.Experts);
        public IReadOnlyCollection<string> ExpertiseType { get; } = CommonInfo.ExpertiseTypes;
        #endregion

        #region Commands
        public RelayCommand Cancel { get; }
        public RelayCommand Select { get; }
        public RelayCommand ExpertChanged { get; }
        #endregion
        public ExpertiseAddVM()
        {
            var app = Application.Current as App;
            switch (app.Permissions.Plurality)
            {
                case PermissionPlural.Self:
                    Experts = Enumerable.Repeat(app.LogedEmployee, 1);
                    break;
                case PermissionPlural.Group:
                    Experts = CommonInfo.Experts.Where(n => n.Employee.EmployeeStatus != "не работает" && 
                                                            n.Employee.Departament.DepartamentID == app.LogedEmployee.Departament.DepartamentID)
                                                .GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                .Select(n => n.First().Employee)
                                                .OrderBy(n => n.Sname);
                    break;
                case PermissionPlural.All:
                    Experts = CommonInfo.Experts.Where(n => n.Employee.EmployeeStatus != "не работает")
                                                .GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                .Select(n => n.First().Employee)
                                                .OrderBy(n => n.Sname);
                    break;
                default:
                    break;
            }
            Cancel = new RelayCommand(n =>
            {
                var wnd = n as ExpertiseAdd;
                if (wnd == null) return;
                wnd.DialogResult = false;
                wnd.Close();
            });
            Select = new RelayCommand(n =>
            {
                var wnd = n as ExpertiseAdd;
                if (wnd == null) return;
                wnd.DialogResult = true;
                wnd.Close();
            },
                o => 
                {
                    if (Expertise.InstanceValidState()) return true;
                    else return false;
                }
            );
            Specialities.Filter = n => false;
            ExpertChanged = new RelayCommand(n =>
            {
                Specialities.Filter = x => (x as Expert).Employee.EmployeeID == (n as Employee).EmployeeID;  
            });
        }
    }
}
