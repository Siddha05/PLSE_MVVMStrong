using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class BillAddVM
    {
        private Bill _bill = new Bill();
 #region Properties
        public Bill NewBill
        {
            get { return _bill; }
        }
        public IReadOnlyList<string> PayerList { get; } = CommonInfo.Payers.Value;
        #endregion
 #region Commands
        public RelayCommand Save
        {
            get
            {
                return new RelayCommand(n =>
                {
                    var wnd = n as Window;
                    if (wnd != null)
                    {
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                });
            }
        }
        #endregion
        public BillAddVM(Expertise exp)
        {
            NewBill.FromExpertise = exp;
            //_bill.BillDate = new DateTime(2020, 10, 3);
            //_bill.HourPrice = 660m;
            //_bill.Hours = 32;
            //_bill.Number = "233";
            //_bill.Paid = 15480m;
            //_bill.PaidDate = DateTime.Now;
            //_bill.Payer = "истца";
        }
    }
}
