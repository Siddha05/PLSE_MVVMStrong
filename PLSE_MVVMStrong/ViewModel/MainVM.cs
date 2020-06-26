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
        private App app = Application.Current as App;
        private MessageQuery messages = new MessageQuery();
        private int empIndex;

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
                return app.Aphorism;
            }
        }
        public MessageQuery Messages => messages;
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(MainVM), new PropertyMetadata(null));

        public Employee Employee
        {
            get { return (Employee)GetValue(EmployeeProperty); }
            set { SetValue(EmployeeProperty, value); }
        }
        public static readonly DependencyProperty EmployeeProperty =
            DependencyProperty.Register("Employee", typeof(Employee), typeof(MainVM), new PropertyMetadata((Application.Current as App).LogedEmployee));

        #endregion Properties

        #region Commands
        public RelayCommand Exit { get; }
        public RelayCommand OpenSpeciality { get; }
        public RelayCommand OpenResolutionAdd { get; }
        public RelayCommand OpenEmployees { get; }
        public RelayCommand OpenProfile { get; }
        public RelayCommand OpenExpertises { get; }
        public RelayCommand WindowLoaded { get; }
        public RelayCommand MessageListDoubleClick { get; }
        public RelayCommand OpenAbout { get; }
#endregion Commands

        public MainVM()
        {
            empIndex = CommonInfo.Employees.IndexOf(Employee);
            informer = new Progress<Message>(n => messages.Add(n));
            app.PropertyChanged += App_PropertyChanged;
            Exit = new RelayCommand(o =>
                                        {
                                            var w = o as MainWindow;
                                            if (w != null) w.Close();
                                        });
            OpenSpeciality = new RelayCommand(exec: o =>
                                     {
                                         Specialities sw = new Specialities()
                                         {
                                             Left = 0,
                                             Top = 0,
                                             Width = SystemParameters.WorkArea.Width,
                                             Height = SystemParameters.WorkArea.Height,
                                             Owner = o as MainWindow
                                         };
                                         sw.Show();
                                     },
                                     canexec: o => app.Permissions.Actions["SpecialitiesView"]
                                     );
            OpenResolutionAdd = new RelayCommand(o =>
                                        {
                                            ResolutionAdd rw = new ResolutionAdd()
                                            {
                                                Owner = o as MainWindow
                                            };
                                            rw.Show();
                                        },
                                        o => app.Permissions.Actions["ResolutionAdd"]
                                        );
            OpenEmployees = new RelayCommand(n =>
            {
                Employees ew = new Employees
                {
                    Owner = n as MainWindow
                };
                ew.Show();
            },
                o => app.Permissions.Actions["EmployeesView"]    
            );
            OpenProfile = new RelayCommand(n =>
                {
                    var wnd = new Profile() { Owner = n as MainWindow, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                    if (wnd.ShowDialog() ?? false)
                    {
                        try
                        {
                            var vm = wnd.DataContext as ProfileVM;
                            if (vm == null) return;
                            vm.Employee.SaveChanges(CommonInfo.connection);
                            app.LogedEmployee = vm.Employee;
                            CommonInfo.Employees[empIndex] = vm.Employee;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                });
            OpenExpertises = new RelayCommand(n =>
            {
                var wnd = new Expertises { Owner = n as MainWindow };
                wnd.Show();
            },
                o => app.Permissions.Actions["ExpertiseView"]
                );
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            WindowLoaded = new RelayCommand(n =>
            {
                ScanAnnualDate(informer);
                ScanExpertises(informer);
            });
            OpenAbout = new RelayCommand(n =>
           {
               About wnd = new About();
               wnd.Show();
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

        private void App_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LogedEmployee":
                    Employee = app.LogedEmployee;
                    break;
                default:
                    break;
            }
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