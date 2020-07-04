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
        SpecialitiesView,
        SpecialitiesAdd,
        SpecialitiesEdit,
        SpecialitiesDelete,
        ExpertView,
        ExpertAdd,
        ExpertEdit,
        ExpertDelete,
        ExpertiseView,
        ExpertiseAdd,
        ExpertiseEdit,
        ExpertiseDelete,
        ResolutionView,
        ResolutionAdd,
        ResolutionEdit,
        ResolutionDelete,
        SettlementView,
        SettlementAdd,
        SettlementEdit,
        SettlementDelete,
        BillView,
        BillAdd,
        BillEdit,
        BillDelete,
        RequestAdd,
        RequesEdit,
        RequestDelete,
        ReportAdd,
        ReportEdit,
        ReportDelete,
        EquipmentView,
        EquipmentAdd,
        EquipmentEdit,
        EquipmentDelete,
        EquipmentUsageAdd,
        EquipmentUsageDelete,
        EquipmentUsageEdit,
        CustomerView,
        CustomerAdd,
        CustomerEdit,
        CustomerDelete,
        OrganizationView,
        OrganizationAdd,
        OrganizationEdit,
        OrganizationDelete,
        EmployeesView,
        EmployeesAdd,
        EmployeesEdit,
        EmployeesDelete
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
        Dictionary<PermissionAction, bool> _command;
        
        #endregion
        public PermissionPlural Plurality { get; }
        public Dictionary<PermissionAction,bool> Actions => _command;
           
        public static Permission Default
            {
                get => new Permission();
            }
        private Permission()
        {
            Plurality = PermissionPlural.Self;
            _command = new Dictionary<PermissionAction, bool>
            {
                [PermissionAction.SpecialitiesView] = false,
                [PermissionAction.SpecialitiesAdd] = false,
                [PermissionAction.SpecialitiesEdit] = false,
                [PermissionAction.SpecialitiesDelete] = false,
                [PermissionAction.ExpertView] = false,
                [PermissionAction.ExpertAdd] = false,
                [PermissionAction.ExpertEdit] = false,
                [PermissionAction.ExpertDelete] = false,
                [PermissionAction.ExpertiseView] = false,
                [PermissionAction.ExpertiseAdd] = false,
                [PermissionAction.ExpertiseEdit] = false,
                [PermissionAction.ExpertiseDelete] = false,
                [PermissionAction.ResolutionView] = false,
                [PermissionAction.ResolutionAdd] = false,
                [PermissionAction.ResolutionEdit] = false,
                [PermissionAction.ResolutionDelete] = false,
                [PermissionAction.SettlementView] = false,
                [PermissionAction.SettlementAdd] = false,
                [PermissionAction.SettlementEdit] = false,
                [PermissionAction.SettlementDelete] = false,
                [PermissionAction.BillView] = false,
                [PermissionAction.BillAdd] = false,
                [PermissionAction.BillEdit] = false,
                [PermissionAction.BillDelete] = false,
                [PermissionAction.RequestAdd] = false,
                [PermissionAction.RequesEdit] = false,
                [PermissionAction.RequestDelete] = false,
                [PermissionAction.ReportAdd] = false,
                [PermissionAction.ReportEdit] = false,
                [PermissionAction.ReportDelete] = false,
                [PermissionAction.EquipmentView] = false,
                [PermissionAction.EquipmentAdd] = false,
                [PermissionAction.EquipmentEdit] = false,
                [PermissionAction.EquipmentDelete] = false,
                [PermissionAction.EquipmentUsageAdd] = false,
                [PermissionAction.EquipmentUsageDelete] = false,
                [PermissionAction.EquipmentUsageEdit] = false,
                [PermissionAction.CustomerView] = false,
                [PermissionAction.CustomerAdd] = false,
                [PermissionAction.CustomerEdit] = false,
                [PermissionAction.CustomerDelete] = false,
                [PermissionAction.OrganizationView] = false,
                [PermissionAction.OrganizationAdd] = false,
                [PermissionAction.OrganizationEdit] = false,
                [PermissionAction.OrganizationDelete] = false,
                [PermissionAction.EmployeesView] = false,
                [PermissionAction.EmployeesAdd] = false,
                [PermissionAction.EmployeesEdit] = false,
                [PermissionAction.EmployeesDelete] = false
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
                    _command[PermissionAction.SpecialitiesView] = true;
                    _command[PermissionAction.SpecialitiesAdd] = true;
                    _command[PermissionAction.SpecialitiesEdit] = true;
                    _command[PermissionAction.SpecialitiesDelete] = true;
                    _command[PermissionAction.ExpertView] = true;
                    _command[PermissionAction.ExpertiseView] = true;
                    _command[PermissionAction.ExpertiseAdd] = true;
                    _command[PermissionAction.ExpertiseEdit] = true;
                    _command[PermissionAction.ExpertiseDelete] = true;
                    _command[PermissionAction.ResolutionView] = true;
                    _command[PermissionAction.ResolutionAdd] = true;
                    _command[PermissionAction.ResolutionEdit] = true;
                    _command[PermissionAction.ResolutionDelete] = true;
                    _command[PermissionAction.SettlementView] = true;
                    _command[PermissionAction.BillView] = true;
                    _command[PermissionAction.BillAdd] = true;
                    _command[PermissionAction.BillEdit] = true;
                    _command[PermissionAction.BillDelete] = true;
                    _command[PermissionAction.RequestAdd] = true;
                    _command[PermissionAction.RequesEdit] = true;
                    _command[PermissionAction.RequestDelete] = true;
                    _command[PermissionAction.ReportAdd] = true;
                    _command[PermissionAction.ReportEdit] = true;
                    _command[PermissionAction.ReportDelete] = true;
                    _command[PermissionAction.EquipmentView] = true;
                    _command[PermissionAction.EquipmentAdd] = true;
                    _command[PermissionAction.EquipmentEdit] = true;
                    _command[PermissionAction.EquipmentDelete] = true;
                    _command[PermissionAction.EquipmentUsageAdd] = true;
                    _command[PermissionAction.EquipmentUsageDelete] = true;
                    _command[PermissionAction.EquipmentUsageEdit] = true;
                    _command[PermissionAction.CustomerView] = true;
                    _command[PermissionAction.CustomerAdd] = true;
                    _command[PermissionAction.CustomerEdit] = true;
                    _command[PermissionAction.CustomerDelete] = true;
                    _command[PermissionAction.OrganizationView] = true;
                    _command[PermissionAction.OrganizationAdd] = true;
                    _command[PermissionAction.OrganizationEdit] = true;
                    _command[PermissionAction.OrganizationDelete] = true;
                    _command[PermissionAction.EmployeesView] = true;
                    _command[PermissionAction.EmployeesAdd] = true;
                    _command[PermissionAction.EmployeesEdit] = true;
                    break;
                case PermissionProfile.Subboss:
                    Plurality = PermissionPlural.Group;
                    _command[PermissionAction.SpecialitiesView] = true;
                    _command[PermissionAction.ExpertView] = true;
                    _command[PermissionAction.ExpertiseView] = true;
                    _command[PermissionAction.ExpertiseAdd] = true;
                    _command[PermissionAction.ExpertiseEdit] = true;
                    _command[PermissionAction.ExpertiseDelete] = true;
                    _command[PermissionAction.ResolutionView] = true;
                    _command[PermissionAction.ResolutionAdd] = true;
                    _command[PermissionAction.ResolutionEdit] = true;
                    _command[PermissionAction.ResolutionDelete] = true;
                    _command[PermissionAction.SettlementView] = true;
                    _command[PermissionAction.BillView] = true;
                    _command[PermissionAction.BillAdd] = true;
                    _command[PermissionAction.BillEdit] = true;
                    _command[PermissionAction.BillDelete] = true;
                    _command[PermissionAction.RequestAdd] = true;
                    _command[PermissionAction.RequesEdit] = true;
                    _command[PermissionAction.RequestDelete] = true;
                    _command[PermissionAction.ReportAdd] = true;
                    _command[PermissionAction.ReportEdit] = true;
                    _command[PermissionAction.ReportDelete] = true;
                    _command[PermissionAction.EquipmentView] = true;
                    _command[PermissionAction.EquipmentAdd] = true;
                    _command[PermissionAction.EquipmentEdit] = true;
                    _command[PermissionAction.EquipmentDelete] = true;
                    _command[PermissionAction.EquipmentUsageAdd] = true;
                    _command[PermissionAction.EquipmentUsageDelete] = true;
                    _command[PermissionAction.EquipmentUsageEdit] = true;
                    _command[PermissionAction.CustomerView] = true;
                    _command[PermissionAction.CustomerAdd] = true;
                    _command[PermissionAction.CustomerEdit] = true;
                    _command[PermissionAction.CustomerDelete] = true;
                    _command[PermissionAction.OrganizationView] = true;
                    _command[PermissionAction.OrganizationAdd] = true;
                    _command[PermissionAction.OrganizationEdit] = true;
                    _command[PermissionAction.OrganizationDelete] = true;
                    _command[PermissionAction.EmployeesView] = true;
                    break;
                case PermissionProfile.Accountant:
                    Plurality = PermissionPlural.All;
                    _command[PermissionAction.SpecialitiesView] = true;
                    _command[PermissionAction.ExpertView] = true;
                    _command[PermissionAction.ExpertiseView] = true;
                    _command[PermissionAction.ResolutionView] = true;
                    _command[PermissionAction.SettlementView] = true;
                    _command[PermissionAction.BillView] = true;
                    _command[PermissionAction.BillAdd] = true;
                    _command[PermissionAction.BillEdit] = true;
                    _command[PermissionAction.BillDelete] = true;
                    _command[PermissionAction.EquipmentView] = true;
                    _command[PermissionAction.CustomerView] = true;
                    _command[PermissionAction.CustomerAdd] = true;
                    _command[PermissionAction.CustomerEdit] = true;
                    _command[PermissionAction.CustomerDelete] = true;
                    _command[PermissionAction.OrganizationView] = true;
                    _command[PermissionAction.OrganizationAdd] = true;
                    _command[PermissionAction.OrganizationEdit] = true;
                    _command[PermissionAction.OrganizationDelete] = true;
                    _command[PermissionAction.EmployeesView] = true;
                    break;
                case PermissionProfile.Expert:
                    _command[PermissionAction.SpecialitiesView] = true;
                    _command[PermissionAction.ExpertView] = true;
                    _command[PermissionAction.ExpertAdd] = true;
                    _command[PermissionAction.ExpertEdit] = true;
                    _command[PermissionAction.ExpertDelete] = true;
                    _command[PermissionAction.ExpertiseView] = true;
                    _command[PermissionAction.ExpertiseAdd] = true;
                    _command[PermissionAction.ExpertiseEdit] = true;
                    _command[PermissionAction.ExpertiseDelete] = true;
                    _command[PermissionAction.ResolutionView] = true;
                    _command[PermissionAction.ResolutionAdd] = true;
                    _command[PermissionAction.ResolutionEdit] = true;
                    _command[PermissionAction.ResolutionDelete] = true;
                    _command[PermissionAction.SettlementView] = true;
                    _command[PermissionAction.BillView] = true;
                    _command[PermissionAction.BillAdd] = true;
                    _command[PermissionAction.BillEdit] = true;
                    _command[PermissionAction.BillDelete] = true;
                    _command[PermissionAction.RequestAdd] = true;
                    _command[PermissionAction.RequesEdit] = true;
                    _command[PermissionAction.RequestDelete] = true;
                    _command[PermissionAction.ReportAdd] = true;
                    _command[PermissionAction.ReportEdit] = true;
                    _command[PermissionAction.ReportDelete] = true;
                    _command[PermissionAction.EquipmentView] = true;
                    _command[PermissionAction.EquipmentUsageAdd] = true;
                    _command[PermissionAction.EquipmentUsageDelete] = true;
                    _command[PermissionAction.EquipmentUsageEdit] = true;
                    _command[PermissionAction.CustomerView] = true;
                    _command[PermissionAction.CustomerAdd] = true;
                    _command[PermissionAction.CustomerEdit] = true;
                    _command[PermissionAction.CustomerDelete] = true;
                    _command[PermissionAction.OrganizationView] = true;
                    _command[PermissionAction.OrganizationAdd] = true;
                    _command[PermissionAction.OrganizationEdit] = true;
                    _command[PermissionAction.OrganizationDelete] = true;
                    _command[PermissionAction.EmployeesView] = true;
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
