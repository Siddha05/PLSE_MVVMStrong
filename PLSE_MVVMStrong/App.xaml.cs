using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.TextFormatting;

namespace PLSE_MVVMStrong
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        #region Fields
        private static string[] aphorism =
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
            "Эксперт - это человек, который сделал больше ошибок, чем вы.",
            "Ничто так не деморализует, как скромный, но постоянный доход.",
            "Брак — основная причина разводов.",
            "За одним зайцем погонишься - двух не поймаешь",
            "Настоящий специалист — это человек, который не допускает мелких ошибок, а выдает грандиозные ляпы.",
            "Все, что хорошо начинается, заканчивается плохо. Все, что плохо начинается, заканчивается еще хуже.",
            "Бисексуальность удваивает ваши шансы найти себе пару в субботу вечером.",
            "Теория — это когда все известно, но ничего не работает. Практика — это когда все работает, но никто не знает почему. Мы же объединяем теорию и практику: ничего не работает... и никто не знает почему!"
        };
        private Employee _logempl;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public Employee LogedEmployee
        {
            get => _logempl;
            set
            {
                if(value != _logempl)
                {
                    _logempl = value;
                    OnPropertyChanged();
                    SetPermission();
                }
            }
        }
        public string Aphorism
        {
            get
            {
                Random rd = new Random();
                return aphorism[rd.Next(0, aphorism.Length - 1)];
            }
        }
        public Permission Permissions { get; private set; }
        #endregion
        private void SetPermission()
        {
            Permissions = new Permission(_logempl);
        }
        private void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public App()
        {
#if DEBUG
            Random rnd = new Random();
            int i = rnd.Next(1, 50);
            System.Diagnostics.Debug.Print("Random: " + i.ToString());
            (Application.Current as App).LogedEmployee = CommonInfo.Employees.First(n => n.EmployeeID == i);
            CommonInfo.Employees.CollectionChanged += Employees_CollectionChanged;
#endif
        }

        private void Employees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (_logempl == e.OldItems[0])
                    {
                        LogedEmployee = (Employee)e.NewItems[0];
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }

    public class TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (String.IsNullOrEmpty(s))
            {
                return "Transparent";
            }
            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Конвертер перевода числа в свойство Visibility.
    /// <para>
    /// 0 - Collapsed, else Visibility
    /// </para>
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val == 0.0) return "Visible";
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}