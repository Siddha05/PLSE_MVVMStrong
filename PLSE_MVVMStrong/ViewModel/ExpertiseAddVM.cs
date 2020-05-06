using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertiseAddVM
    {
        public class GroupExperts
        {
            public int Key { get; set; }
            public string FIO { get; set; }
        }
        #region Properties
        public Expertise Expertise { get; } = Expertise.New;
        public IEnumerable<GroupExperts> Experts { get; } = CommonInfo.Experts.GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                                              .Select(n => new GroupExperts { Key = n.Key, FIO = n.First().Employee.ToString()})
                                                                              .OrderBy(n => n.FIO);
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
                Specialities.Filter = x => (x as Expert).Employee.EmployeeID == (n as GroupExperts).Key;  
            });
        }
    }
}
