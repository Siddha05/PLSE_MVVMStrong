using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSE_MVVMStrong.Model
{
    static class ApplicationSet
    {
        public static Employee Employee { get; set; } = CommonInfo.Employees.First(n => n.EmployeeID == 7);
    }
}
