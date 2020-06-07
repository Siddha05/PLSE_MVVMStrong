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
        public IEnumerable<Speciality> SpecialitiesList { get; } = CommonInfo.Specialities;
        #endregion
        #region Commands
        public RelayCommand SaveExpert { get; }
        #endregion
        public ExpertAddVM()
        {
            SaveExpert = new RelayCommand(n =>
            {
                
            });
        }
    }
}
