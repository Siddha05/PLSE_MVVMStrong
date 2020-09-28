using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class AddReportVM
    {
#region Fields
        #endregion
#region Properties
        public Report Rept { get; }
#endregion
        #region Commands
        public RelayCommand Save
        {
            get
            {
                return new RelayCommand(n =>
                {
                    var wnd = n as Window;
                    try
                    {
                        Rept.FromExpertise.SaveChanges(CommonInfo.connection);
                        wnd.DialogResult = true;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка при сохранении в базу данных");
                        wnd.DialogResult = false;
                    }
                    wnd.Close();
                },
                e =>
                {
                    return Rept.ReportValidState;
                });
            }
        }
        #endregion
        public AddReportVM(Expertise expertise)
        {
            Rept = new Report() { ReportDate = DateTime.Now, FromExpertise = expertise};
        }
    }
}
