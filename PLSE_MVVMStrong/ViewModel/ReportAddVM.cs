using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class ReportAddVM
    {
#region Fields
        Report _report = new Report { ReportDate = DateTime.Now};
        DateTime? _delay;
        #endregion
        #region Properties
        public Report NewReport => _report;
        public DateTime? Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                if (_delay.HasValue) _report.DelayDate = _delay.Value;
            }
        }
        #endregion
#region Commands
        public RelayCommand Save
        {
            get
            {
                return new RelayCommand(n =>
                {
                    var wnd = n as Window;
                    if (n != null)
                    {
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                },
                e =>
                {
                    return _report.ReportValidState;
                });
            }
        }
        #endregion
        public ReportAddVM(Expertise exp)
        {
            NewReport.FromExpertise = exp;
        }
    }
}
