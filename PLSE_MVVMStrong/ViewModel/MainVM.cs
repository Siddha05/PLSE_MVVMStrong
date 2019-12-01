using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PLSE_MVVMStrong.ViewModel
{
    internal class MainVM : DependencyObject
    {
        private DispatcherTimer timer;
        private Progress<Message> informer;
        private MessageQuery messages = new MessageQuery();

        private string[] aphorism =
        {
            "Начинающий видит много возможностей, эксперт — лишь несколько.",
            "Авторитетность экспертного заключения обратно пропорциональна числу утверждений, понятных широкой публике.",
            "Эксперт излагает объективную точку зрения. А именно свою собственную.",
            "Эксперт – это человек, который совершил все возможные ошибки в очень узкой специальности.",
            "Эксперт даст все нужные вам ответы, если получит нужные ему вопросы.",
            "Сделайте три верные догадки подряд – и репутация эксперта вам обеспечена.",
            "Если нужно выбрать среди экспертов одного настоящего, выбирай того, кто обещает наибольший срок завершения проекта и его наибольшую стоимость.",
            "Если консультироваться с достаточно большим числом экспертов, можно подтвердить любую теорию.",
            "Начальник не всегда прав, но он всегда начальник.",
            "Не спеши выполнять приказ — его могут отменить.",
            "От трудной работы еще никто не умирал. Но зачем испытывать судьбу?",
            "Чем больше знаешь, тем больше знаешь лишнего.",
            "Вывод — то место в тексте, где вы устали думать.",
            "Если хотите остаться при своем мнении, держите его при себе.",
            "Узы брака тяжелы. Поэтому нести их приходится вдвоем. А иногда даже втроем.",
            "Если Вас уже третий рабочий день подряд клонит в сон, значит сегодня среда.",
            "Ни одно доброе дело не должно оставаться безнаказанным.",
            "Недостаточно иметь мозги, надо их иметь достаточно, чтобы воздержаться от того, чтобы иметь их слишком много.",
            "Наличие мозгов — дополнительная нагрузка на позвоночник.",
            "Работа в команде очень важна. Она позволяет свалить вину на другого.",
            "Незаменимые бывают только аминокислоты.",
            "Брак - единственная война, во время которой вы спите с врагом.",
            "Любовь — это болезнь, требующая постельного режима.",
            "Эксперт - это человек, который сделал больше ошибок, чем вы."
        };

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
                Random rd = new Random();
                return aphorism[rd.Next(0, aphorism.Length-1)];
            }
        }

        public MessageQuery Messages => messages;

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(MainVM), new PropertyMetadata(null));

        #endregion Properties

        #region Commands

        private RelayCommand _exit;
        public RelayCommand Exit => _exit;
        private RelayCommand _spec;
        public RelayCommand OpenSpeciality => _spec;

        #endregion Commands

        public MainVM()
        {
            _exit=new RelayCommand(o =>
                                      {
                                          var w = o as MainWindow;
                                          if (w!=null) w.Close();
                                      });
            _spec=new RelayCommand(o =>
                                  {
                                      Specialities sw = new Specialities()
                                      {
                                          Owner=o as Window
                                      };
                                      sw.Show();
                                  });
            timer=new DispatcherTimer()
            {
                Interval=TimeSpan.FromSeconds(1)
            };
            timer.Tick+=Timer_Tick;
            timer.Start();
            informer=new Progress<Message>(n => messages.Add(n));
            messages.Add(new Message("Welcome, "+Environment.UserName, MsgType.Temporary));
            try
            {
                if (CommonInfo.IsInitializated) ScanAsync(informer);
            }
            catch (SqlException e) when (e.Number==87)
            {
                MessageBox.Show("Ошибка при подключении к базе данных", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (AggregateException e)
            {
                MessageBox.Show("Aggregate exeption catched");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Date=DateTime.Now.ToString("F");
        }

        public void ScanAsync(IProgress<Message> progress)
        {
            Task.Run(() =>
            {
                if (CommonInfo.Employees!=null)
                {
                    DateTime today = DateTime.Now;
                    foreach (var item in CommonInfo.Employees)
                    {
                        if (item.Birthdate.HasValue&&item.Birthdate.Value.Day==today.Day&&item.Birthdate.Value.Month==today.Month)
                        {
                            int years = today.Year-item.Birthdate.Value.Year;
                            progress.Report(new Message(item.ToString()+" празднует день рождения!!! ("+years.ToString()+")", MsgType.Congratulation));
                        }
                        if (item.Hiredate.HasValue&&item.Hiredate.Value.Day==today.Day&&item.Hiredate.Value.Month==today.Month)
                        {
                            int years = today.Year-item.Hiredate.Value.Year;
                            progress.Report(new Message(item.ToString()+" выслуга лет!!! ("+years.ToString()+")", MsgType.Congratulation));
                        }
                    }
                }
                else
                {
                    progress.Report(new Message("Список сотрудников не загружен", MsgType.Error));
                }
            });
        }
    }
}