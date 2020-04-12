using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class MainVM : DependencyObject
    {
        private DispatcherTimer timer;
        private Progress<Message> informer;
        private MessageQuery messages = new MessageQuery();
        //public static Employee _employee = (Application.Current as App).LogedEmployee;
        public static Employee _employee = CommonInfo.Employees.First(n => n.EmployeeID == 7);
        private int EmpIndex = CommonInfo.Employees.IndexOf(_employee);

        #region Properties
        public string Date
        {
            get => (string)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }
        public string Aphorism
        {
            get
            {
                return (Application.Current as App).Aphorism;
            }
        }
        public MessageQuery Messages => messages;
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(MainVM), new PropertyMetadata(null));
        public Employee Employee
        {
            get { return _employee; }
        }
        #endregion Properties

        #region Commands
        public RelayCommand Exit { get; }
        public RelayCommand OpenSpeciality { get; }
        public RelayCommand OpenResolutionAdd { get; }
        public RelayCommand OpenEmployees { get; }
        public RelayCommand OpenProfile { get; }
        public RelayCommand Expertises { get; }
        public RelayCommand WindowLoaded { get; }
        public RelayCommand MessageListDoubleClick { get; }
        #endregion Commands

        public MainVM()
        {
            informer = new Progress<Message>(n => messages.Add(n));
            Exit = new RelayCommand(o =>
                                        {
                                            var w = o as MainWindow;
                                            if (w != null) w.Close();
                                        });
            OpenSpeciality = new RelayCommand(o =>
                                    {
                                        Specialities sw = new Specialities()
                                        {
                                            Owner = o as MainWindow
                                        };
                                        sw.Show();
                                    });
            OpenResolutionAdd = new RelayCommand(o =>
                                        {
                                            ResolutionAdd rw = new ResolutionAdd()
                                            {
                                                Owner = o as MainWindow
                                            };
                                            rw.Show();
                                        });
            OpenEmployees = new RelayCommand(n =>
            {
                var wnd = new Employees() { Owner = n as MainWindow };
                wnd.Show();
            });
            OpenProfile = new RelayCommand(n =>
                {
                    var wnd = new Employees() { Owner = n as MainWindow, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                    if (wnd.ShowDialog() ?? false)
                    {
                        try
                        {
                            var vm = wnd.DataContext as EmployeesVM;
                            if (vm == null) return;
                            vm.Employee.SaveChanges(CommonInfo.connection);
                            //foreach (var item in vm.ExpertList)
                            //{
                            //    item.SaveChanges(CommonInfo.connection);
                            //}
                            _employee = vm.Employee;
                            CommonInfo.Employees[EmpIndex] = vm.Employee;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                });
            Expertises = new RelayCommand(n =>
            {
                var wnd = new Expertises { Owner = n as MainWindow };
                wnd.Show();
            });
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            WindowLoaded = new RelayCommand(n =>
            {
                ScanAnnualDate(informer);
                ScanExpertises(informer);
            });
            MessageListDoubleClick = new RelayCommand(n =>
            {
                Messages.Remove(n as Message);
            });
            timer.Tick += Timer_Tick;
            timer.Start();
            var curhour = DateTime.Now.Hour;
            if (curhour >= 4 && curhour <= 10) Messages.Add(new Message($"Доброе утро, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else if (curhour >= 11 && curhour <= 16) Messages.Add(new Message($"День добрый, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else if (curhour >= 17 && curhour <= 21) Messages.Add(new Message($"Добрый вечер, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else Messages.Add(new Message($"Доброй ночи, {Employee.Fname} {Employee.Mname}", MsgType.Normal, TimeSpan.FromSeconds(5)));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Date = DateTime.Now.ToString("F");
        }
        public void ScanAnnualDate(IProgress<Message> progress)
        {
            Task.Run(() =>
            {
                if (CommonInfo.Employees != null)
                {
                    DateTime today = DateTime.Now;
                    foreach (var item in CommonInfo.Employees)
                    {
                        if (item.Birthdate.HasValue && item.Birthdate.Value.Day == today.Day && item.Birthdate.Value.Month == today.Month)
                        {
                            int years = today.Year - item.Birthdate.Value.Year;
                            progress.Report(new Message(item.ToString() + " празднует день рождения!!! (" + years.ToString() + ")", MsgType.Congratulation));
                        }
                        if (item.Hiredate.HasValue && item.Hiredate.Value.Day == today.Day && item.Hiredate.Value.Month == today.Month)
                        {
                            int years = today.Year - item.Hiredate.Value.Year;
                            progress.Report(new Message(item.ToString() + " выслуга лет!!! (" + years.ToString() + ")", MsgType.Congratulation));
                        }
                    }
                }
                else
                {
                    progress.Report(new Message("Список сотрудников не загружен", MsgType.Error));
                }
            });
        }
        public void ScanExpertises (IProgress<Message> progress)
        {
            Task.Run(() =>
            {
                progress.Report(new Message($"Expertises not scanned!", MsgType.Warning));
            });
        }
    }
    [ValueConversion(typeof(MsgType), typeof(string))]
    class MessageImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MsgType msgtype = (MsgType)value;
            switch (msgtype)
            {
                case MsgType.Error:
                    return "Resources/Error.png";
                case MsgType.Congratulation:
                    return "Resources/GiftBox.png";
                case MsgType.Warning:
                    return "Resources/Warning.png";
                default:
                    return "Resources/Info.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}