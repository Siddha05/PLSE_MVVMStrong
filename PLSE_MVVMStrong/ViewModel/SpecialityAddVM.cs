using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class SpecialityAddVM : DependencyObject
    {
        #region Properties

        public Speciality Speciality { get; private set; }
        public int Index { get; private set; }
        #endregion Properties

        #region Commands

        public RelayCommand Ok { get; }
        public RelayCommand Cancel { get; }

        #endregion Commands

        public SpecialityAddVM()
        {
            Ok = new RelayCommand(o =>
                                    {
                                        var w = o as SpecialityAdd;
                                        if (w != null)
                                        {
                                            w.DialogResult = true;
                                            w.Close();
                                        }
                                    },
                                    n =>
                                    {
                                        if (Speciality.IsValidInstanceState()) return true;
                                        else return false;
                                    });
            Cancel = new RelayCommand(o =>
                                         {
                                             var w = o as SpecialityAdd;
                                             if (w != null)
                                             {
                                                 w.DialogResult = false;
                                                 w.Close();
                                             }
                                         });
            Speciality = new Speciality() { IsValid = true};
        }
        public SpecialityAddVM(Speciality speciality) : this()
        {
            Speciality = speciality.Clone();
            Index = CommonInfo.Specialities.IndexOf(speciality);
        }
    }
}