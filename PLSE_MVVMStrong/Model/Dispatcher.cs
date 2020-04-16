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
        public PermissionPlural Plurality { get; }
        public PermissionAction Actions { get; }
           
        public static Permission Default
            {
                get => new Permission();
            }
        public override string ToString()
            {
                return $"for {Plurality} actions: {Actions}";
            }
        private Permission()
        {
            Plurality = PermissionPlural.Self;
            Actions = PermissionAction.None;
        }
        public Permission(PermissionPlural plural, PermissionAction action)
        {
            Plurality = plural;
            Actions = action;
        }
    }
    public class WindowManager<T> where T: Window, new()
    {
        #region Fields
        private T _target;
        private Permission _permissions;
        #endregion
        #region Properties
        public T Target => _target;
        public Permission Permissions
        {
            get { return _permissions; }
            set { _permissions = value; }
        }
        public RelayCommand Open { get; }
        public RelayCommand Close { get; }
        #endregion

        
        public void Show()
        {
            if (_target == null)
            {
                T wnd = new T();
                if (wnd != null)
                {
                    _target = wnd;
                    wnd.Show();
                }
            }
            else _target.Focus();
        }
        public void ShowDialog()
        {
            if (_target == null)
            {
                T wnd = new T();
                if (wnd != null)
                {
                    _target = wnd;
                    wnd.ShowDialog();
                }
            }
        }
        //public void Close()
        //{
        //    if (_target != null) _target.Close();
        //    _target = null;
        //}
        public WindowManager(Permission perm)
        {
            _permissions = perm;
            Open = new RelayCommand(n =>
            {
                if (_target == null)
                {
                    T wnd = new T();
                    if (wnd != null)
                    {
                        _target = wnd;
                        wnd.Show();
                    }
                }
                else _target.Focus();
            },
            x => (_permissions.Actions & PermissionAction.View) != 0);
            Close = new RelayCommand(n =>
            {
                _target.Close();
                _target = null;
            });
        }
        public WindowManager()
        {
            _permissions = Permission.Default;
        }
    }
    public class WindowDispatcher
    {
        #region Fields
        Dictionary<string, WindowManager<Window>> _dispatcher = new Dictionary<string, WindowManager<Window>>();
        #endregion
        #region Properties
        public WindowManager<Window> this[string id]
        {
            get
            {
                return _dispatcher[id];
            }
        }
        #endregion
        public void Register(string id, WindowManager<Window> wm)
        {
            _dispatcher.Add(id, wm);
        }
        public void SetPermissions (string id, Permission perm)
        {
            _dispatcher[id].Permissions = perm;
        }

    }
}
