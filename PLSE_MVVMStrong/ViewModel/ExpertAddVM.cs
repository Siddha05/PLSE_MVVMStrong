using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertAddVM
    {
        #region Properties
        public Expert Expert { get; }
        private Employee Empl {get;}
        public IEnumerable<Speciality> SpecialitiesList { get; }
        #endregion
        #region Commands
        public RelayCommand Save { get; }
        public RelayCommand Cancel { get; }
        #endregion
        public ExpertAddVM()
        {
            
        }
    }
}
