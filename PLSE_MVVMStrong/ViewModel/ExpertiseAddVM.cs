using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public RelayCommand TypeChanged { get; }
        public RelayCommand Select { get; }
        public RelayCommand ExpertChanged { get; }
        #endregion
        public ExpertiseAddVM()
        {         
            Experts = CommonInfo.Experts.Where(n => n.Employee.Actual)
                                                .GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                .Select(n => n.First().Employee)
                                                .OrderBy(n => n.Sname);
           
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
            TypeChanged = new RelayCommand(n =>
            {
                var b = n as RadioButton;
                Expertise.ExpertiseType = b.Content.ToString();
            });
            Specialities.Filter = n => false;
            ExpertChanged = new RelayCommand(n =>
            {
                Specialities.Filter = x => (x as Expert).Employee.EmployeeID == (n as Employee).EmployeeID && !(x as Expert).ExpertCore.Closed;  
            });
        }
    }
}
