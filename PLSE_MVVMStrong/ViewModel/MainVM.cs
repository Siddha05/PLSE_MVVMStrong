﻿using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class MainVM : DependencyObject
    {
#region Fields
        private DispatcherTimer timer;
        private Progress<Message> informer;
        private App app = Application.Current as App;
        private MessageQuery messages = new MessageQuery();
        private RelayCommand _openprofile;
        private RelayCommand _openspeciality;
        private RelayCommand _openemployees;
        private RelayCommand _openabout;
        private RelayCommand _openaddresol;
        private RelayCommand _openexpertise;
        private RelayCommand _message2click;
        private RelayCommand _opensettings;
        #endregion
        
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
        public RelayCommand Exit
        {
            get => new RelayCommand(o =>
                                    {
                                        var w = o as MainWindow;
                                        if (w != null) w.Close();
                                    });
        }
        public RelayCommand OpenSpeciality
        {
            get
            {
                return _openspeciality != null ? _openspeciality : _openspeciality = new RelayCommand(exec: o =>
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
                                                                canexec: o => app.Permissions.Actions[PermissionAction.SpecialitiesView]
                                                                );
            }
        }
        public RelayCommand OpenResolutionAdd
        {
            get
            {
                return _openaddresol != null ? _openaddresol : _openaddresol = new RelayCommand(o =>
                {
                                                                ResolutionAdd rw = new ResolutionAdd()
                                                                {
                                                                    Owner = o as MainWindow
                                                                };
                                                                rw.Show();
                                                            },
                                                                o => app.Permissions.Actions[PermissionAction.ResolutionAdd]
                                                            );
            }
        }
        public RelayCommand OpenEmployees
        {
            get
            {
                return _openemployees != null ? _openemployees : _openemployees = new RelayCommand(n =>
                                                                {
                                                                    Employees ew = new Employees
                                                                    {
                                                                        Owner = n as MainWindow
                                                                    };
                                                                    ew.Show();
                                                                });
            }
        }
        public RelayCommand OpenProfile
        {
            get
            {
                return _openprofile != null ? _openprofile : _openprofile = new RelayCommand(n =>
                                            {
                                                var wnd = new Profile() { Owner = n as MainWindow, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                                                if (wnd.ShowDialog() ?? false)
                                                {
                                                    try
                                                    {
                                                        var vm = wnd.DataContext as ProfileVM;
                                                        if (vm == null) return;
                                                        vm.Employee.SaveChanges(CommonInfo.connection);
                                                        //CommonInfo.Employees[app.LogedEmployeeIndex].Copy(vm.Employee);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.Message);
                                                    }
                                                }
                                            });
            }
        }
        public RelayCommand OpenExpertises
        {
            get
            {
                return _openexpertise != null ? _openexpertise : _openexpertise = new RelayCommand(n =>
                                                                            {
                                                                                var wnd = new Expertises { Owner = n as MainWindow };
                                                                                wnd.Show();
                                                                            },
                                                                                o => app.Permissions.Actions[PermissionAction.ExpertiseView]
                                                                            );
            }
        }
        public RelayCommand WindowLoaded { get; }
        public RelayCommand MessageListDoubleClick 
        { 
            get
            {
                return _message2click != null ? _message2click : _message2click = new RelayCommand(n =>
                                                                                    {
                                                                                        Messages.Remove(n as Message);
                                                                                    });
            }
        }
        public RelayCommand OpenAbout
        {
            get
            {
                return _openabout != null ? _openabout : _openabout = new RelayCommand(n =>
                                                        {
                                                            About wnd = new About();
                                                            wnd.Show();
                                                        });
            }
        }
        public RelayCommand OpenSettings
        {
            get
            {
                return _opensettings != null ? _opensettings : _opensettings = new RelayCommand(n =>
                {
                    var wnd = new View.Settings();
                    wnd.ShowDialog();
                });
            }
        }
#endregion Commands

        public MainVM()
        {
            informer = new Progress<Message>(n => messages.Add(n));
            app.PropertyChanged += App_PropertyChanged;
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            WindowLoaded = new RelayCommand(n =>
            {
                ScanAnnualDate(informer);
                ScanExpertises(informer);
                //TestInfo(informer);
            });
            timer.Tick += Timer_Tick;
            timer.Start();
            var curhour = DateTime.Now.Hour;
            if (curhour >= 4 && curhour <= 10) Messages.Add(new Message($"Доброе утро, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else if (curhour >= 11 && curhour <= 16) Messages.Add(new Message($"День добрый, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else if (curhour >= 17 && curhour <= 21) Messages.Add(new Message($"Добрый вечер, {Employee.Fname} {Employee.Mname}", MsgType.Temporary, TimeSpan.FromSeconds(5)));
            else Messages.Add(new Message($"Доброй ночи, {Employee.Fname} {Employee.Mname}", MsgType.Normal, TimeSpan.FromSeconds(5)));
            messages.Add(new Message($"Connettion: {System.Configuration.ConfigurationManager.ConnectionStrings[0]}", MsgType.Temporary, TimeSpan.FromSeconds(2)));
            messages.Add(new Message($"Connettion: {System.Configuration.ConfigurationManager.ConnectionStrings["PLSE"]}", MsgType.Temporary, TimeSpan.FromSeconds(4)));
            //app.PropertyChanged += (o, e) => Employee = app.LogedEmployee;
            //foreach (var item in CommonInfo.Employees)
            //{
            //    string text = null;
            //    try
            //    {
            //        text += item.ToString("D") + "\t";
            //        text += item.ToString("G");
            //    }
            //    catch (Exception e)
            //    {

            //        text += e.Message;
            //    }
            //    Messages.Add(new Message(text, MsgType.Warning));
            //}


        }

        private void App_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Employee = app.LogedEmployee;
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
                        if (item.EmployeeCore.Birthdate.HasValue && item.EmployeeCore.Birthdate.Value.Day == today.Day && item.EmployeeCore.Birthdate.Value.Month == today.Month)
                        {
                            int years = today.Year - item.EmployeeCore.Birthdate.Value.Year;
                            progress.Report(new Message(item.ToString() + " празднует день рождения!!! (" + years.ToString() + ")", MsgType.Congratulation));
                        }
                        if (item.EmployeeCore.Hiredate.HasValue && item.EmployeeCore.Hiredate.Value.Day == today.Day && item.EmployeeCore.Hiredate.Value.Month == today.Month)
                        {
                            int years = today.Year - item.EmployeeCore.Hiredate.Value.Year;
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
                foreach (var item in CommonInfo.Experts.Where(n => n.Employee.EmployeeID == app.LogedEmployee.EmployeeID))
                {
                    if (!item.ExpertCore.ValidAttestation)
                    {
                        progress.Report(new Message($"Аттестация по специальности {item.ExpertCore.Speciality.Code} просрочена", MsgType.Warning));
                    }
                }
            });
        }
        public void TestInfo (IProgress<Message> progress)
        {
            Task.Run(() =>
            {
                foreach (var item in CommonInfo.Specialities)
                {
                    progress.Report(new Message(item.DeclineSpeciality(LingvoNET.Case.Genitive), MsgType.Warning));  
                    Thread.Sleep(500);
                }
                
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