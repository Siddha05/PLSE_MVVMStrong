using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.Model
{
    public enum PermissionPlural
    {
        Self,
        Group,
        All
    }
    [Flags]
    public enum PermissionAction
    {
        None = 0x0,
        View = 0x1,
        Add = 0x2,
        Edit = 0x4,
        Delete = 0x8,
        All = View | Add | Edit| Delete
    }
    enum PermissionGroup
    {
        Profile,
        Expertise,
        Bill,
        Report,
        Request,
        Speciality,
        Expert,
        Settlement,
        Equipment,
        EquipmentUsage,
        Resolution,
        Customer,
        Organization
    }

    //public class Permission<T> where T : Window
    //{
    //    public class Composite
    //    {
    //        public PermissionPlural Plurality { get; }
    //        public PermissionAction Actions { get; }
    //        private Composite()
    //        {
    //            Plurality = PermissionPlural.Self;
    //            Actions = PermissionAction.None;
    //        }
    //        public Composite(PermissionPlural plural, PermissionAction action)
    //        {
    //            Plurality = plural;
    //            Actions = action;
    //        }
    //        public static Composite Default
    //        {
    //            get => new Composite();
    //        }
    //        public override string ToString()
    //        {
    //            return $"plurality: {Plurality} actions: {Actions}";
    //        }
    //    }
    //    public Dictionary<T, Composite> EntityPermissions { get; } = new Dictionary<T, Composite>();
    //    public void Add(T entity, Composite pa)
    //    {
    //        EntityPermissions.Add(entity, pa);
    //    }
    //    public Permission()
    //    {

    //    }
    //}
    public class Permission
    {
        #region Fields
        Dictionary<string, bool> _command;
        
        #endregion
        public PermissionPlural Plurality { get; }
        public Dictionary<string,bool> Actions => _command;
           
        public static Permission Default
            {
                get => new Permission();
            }
        private Permission()
        {
            Plurality = PermissionPlural.Self;
            _command = new Dictionary<string, bool>
            {
                ["SpecialitiesView"] = false,
                ["SpecialitiesAdd"] = false,
                ["SpecialitiesEdit"] = false,
                ["SpecialitiesDelete"] = false,
                ["ExpertView"] = false,
                ["ExpertAdd"] = false,
                ["ExpertEdit"] = false,
                ["ExpertDelete"] = false,
                ["ExpertiseView"] = false,
                ["ExpertiseAdd"] = false,
                ["ExpertiseEdit"] = false,
                ["ExpertiseDelete"] = false,
                ["ResolutionView"] = false,
                ["ResolutionAdd"] = false,
                ["ResolutionEdit"] = false,
                ["ResolutionDelete"] = false,
                ["SettlementView"] = false,
                ["SettlementAdd"] = false,
                ["SettlementEdit"] = false,
                ["SettlementDelete"] = false,
                ["BillView"] = false,
                ["BillAdd"] = false,
                ["BillEdit"] = false,
                ["BillDelete"] = false,
                ["RequestAdd"] = false,
                ["RequesEdit"] = false,
                ["RequestDelete"] = false,
                ["ReportAdd"] = false,
                ["ReportEdit"] = false,
                ["ReportDelete"] = false,
                ["EquipmentView"] = false,
                ["EquipmentAdd"] = false,
                ["EquipmentEdit"] = false,
                ["EquipmentDelete"] = false,
                ["EquipmentUsageAdd"] = false,
                ["EquipmentUsageDelete"] = false,
                ["EquipmentUsageEdit"] = false,
                ["CustomerView"] = false,
                ["CustomerAdd"] = false,
                ["CustomerEdit"] = false,
                ["CustomerDelete"] = false,
                ["OrganizationView"] = false,
                ["OrganizationAdd"] = false,
                ["OrganizationEdit"] = false,
                ["OrganizationDelete"] = false,
                ["EmployeesView"] = false,
                ["EmployeesAdd"] = false,
                ["EmployeesEdit"] = false,
                ["EmployeesDelete"] = false
            };
        }
        public Permission(Employee employee) : this()
        {
            switch (employee.Profile)
            {
                case PermissionProfile.Admin:
                    _command = _command.ToDictionary(k => k.Key, v => true);
                    break;
                case PermissionProfile.Boss:
                    Plurality = PermissionPlural.All;
                    _command["SpecialitiesView"] = true;
                    _command["SpecialitiesAdd"] = true;
                    _command["SpecialitiesEdit"] = true;
                    _command["SpecialitiesDelete"] = true;
                    _command["ExpertView"] = true;
                    _command["ExpertiseView"] = true;
                    _command["ExpertiseAdd"] = true;
                    _command["ExpertiseEdit"] = true;
                    _command["ExpertiseDelete"] = true;
                    _command["ResolutionView"] = true;
                    _command["ResolutionAdd"] = true;
                    _command["ResolutionEdit"] = true;
                    _command["ResolutionDelete"] = true;
                    _command["SettlementView"] = true;
                    _command["BillView"] = true;
                    _command["BillAdd"] = true;
                    _command["BillEdit"] = true;
                    _command["BillDelete"] = true;
                    _command["RequestAdd"] = true;
                    _command["RequesEdit"] = true;
                    _command["RequestDelete"] = true;
                    _command["ReportAdd"] = true;
                    _command["ReportEdit"] = true;
                    _command["ReportDelete"] = true;
                    _command["EquipmentView"] = true;
                    _command["EquipmentAdd"] = true;
                    _command["EquipmentEdit"] = true;
                    _command["EquipmentDelete"] = true;
                    _command["EquipmentUsageAdd"] = true;
                    _command["EquipmentUsageDelete"] = true;
                    _command["EquipmentUsageEdit"] = true;
                    _command["CustomerView"] = true;
                    _command["CustomerAdd"] = true;
                    _command["CustomerEdit"] = true;
                    _command["CustomerDelete"] = true;
                    _command["OrganizationView"] = true;
                    _command["OrganizationAdd"] = true;
                    _command["OrganizationEdit"] = true;
                    _command["OrganizationDelete"] = true;
                    _command["EmployeesView"] = true;
                    _command["EmployeesAdd"] = true;
                    _command["EmployeesEdit"] = true;
                    break;
                case PermissionProfile.Subboss:
                    Plurality = PermissionPlural.Group;
                    _command["SpecialitiesView"] = true;
                    _command["ExpertView"] = true;
                    _command["ExpertiseView"] = true;
                    _command["ExpertiseAdd"] = true;
                    _command["ExpertiseEdit"] = true;
                    _command["ExpertiseDelete"] = true;
                    _command["ResolutionView"] = true;
                    _command["ResolutionAdd"] = true;
                    _command["ResolutionEdit"] = true;
                    _command["ResolutionDelete"] = true;
                    _command["SettlementView"] = true;
                    _command["BillView"] = true;
                    _command["BillAdd"] = true;
                    _command["BillEdit"] = true;
                    _command["BillDelete"] = true;
                    _command["RequestAdd"] = true;
                    _command["RequesEdit"] = true;
                    _command["RequestDelete"] = true;
                    _command["ReportAdd"] = true;
                    _command["ReportEdit"] = true;
                    _command["ReportDelete"] = true;
                    _command["EquipmentView"] = true;
                    _command["EquipmentAdd"] = true;
                    _command["EquipmentEdit"] = true;
                    _command["EquipmentDelete"] = true;
                    _command["EquipmentUsageAdd"] = true;
                    _command["EquipmentUsageDelete"] = true;
                    _command["EquipmentUsageEdit"] = true;
                    _command["CustomerView"] = true;
                    _command["CustomerAdd"] = true;
                    _command["CustomerEdit"] = true;
                    _command["CustomerDelete"] = true;
                    _command["OrganizationView"] = true;
                    _command["OrganizationAdd"] = true;
                    _command["OrganizationEdit"] = true;
                    _command["OrganizationDelete"] = true;
                    _command["EmployeesView"] = true;
                    break;
                case PermissionProfile.Accountant:
                    Plurality = PermissionPlural.All;
                    _command["SpecialitiesView"] = true;
                    _command["ExpertView"] = true;
                    _command["ExpertiseView"] = true;
                    _command["ResolutionView"] = true;
                    _command["SettlementView"] = true;
                    _command["BillView"] = true;
                    _command["BillAdd"] = true;
                    _command["BillEdit"] = true;
                    _command["BillDelete"] = true;
                    _command["EquipmentView"] = true;
                    _command["CustomerView"] = true;
                    _command["CustomerAdd"] = true;
                    _command["CustomerEdit"] = true;
                    _command["CustomerDelete"] = true;
                    _command["OrganizationView"] = true;
                    _command["OrganizationAdd"] = true;
                    _command["OrganizationEdit"] = true;
                    _command["OrganizationDelete"] = true;
                    _command["EmployeesView"] = true;
                    break;
                case PermissionProfile.Expert:
                    _command["SpecialitiesView"] = true;
                    _command["ExpertView"] = true;
                    _command["ExpertAdd"] = true;
                    _command["ExpertEdit"] = true;
                    _command["ExpertDelete"] = true;
                    _command["ExpertiseView"] = true;
                    _command["ExpertiseAdd"] = true;
                    _command["ExpertiseEdit"] = true;
                    _command["ExpertiseDelete"] = true;
                    _command["ResolutionView"] = true;
                    _command["ResolutionAdd"] = true;
                    _command["ResolutionEdit"] = true;
                    _command["ResolutionDelete"] = true;
                    _command["SettlementView"] = true;
                    _command["BillView"] = true;
                    _command["BillAdd"] = true;
                    _command["BillEdit"] = true;
                    _command["BillDelete"] = true;
                    _command["RequestAdd"] = true;
                    _command["RequesEdit"] = true;
                    _command["RequestDelete"] = true;
                    _command["ReportAdd"] = true;
                    _command["ReportEdit"] = true;
                    _command["ReportDelete"] = true;
                    _command["EquipmentView"] = true;
                    _command["EquipmentUsageAdd"] = true;
                    _command["EquipmentUsageDelete"] = true;
                    _command["EquipmentUsageEdit"] = true;
                    _command["CustomerView"] = true;
                    _command["CustomerAdd"] = true;
                    _command["CustomerEdit"] = true;
                    _command["CustomerDelete"] = true;
                    _command["OrganizationView"] = true;
                    _command["OrganizationAdd"] = true;
                    _command["OrganizationEdit"] = true;
                    _command["OrganizationDelete"] = true;
                    _command["EmployeesView"] = true;
                    break;
                case PermissionProfile.Laboratorian:
                    break;
                case PermissionProfile.Clerk:
                    break;
                case PermissionProfile.Staffinspector:
                    break;
                case PermissionProfile.Provisionboss:
                    break;
                case PermissionProfile.Rightless:
                    break;
                default:
                    break;
            }
        }
    }
}
