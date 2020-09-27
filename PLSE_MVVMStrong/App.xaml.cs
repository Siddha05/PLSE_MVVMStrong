using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
        private int _empindex;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public Employee LogedEmployee
        {
            get => _logempl;
            set
            {
                _logempl = value;
                _empindex = CommonInfo.Employees.IndexOf(_logempl);
                value.PropertyChanged += E_PropertyChanged;
                OnPropertyChanged();
                SetPermission();
            }
        }
        public int LogedEmployeeIndex => _empindex;
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
            Debug.Print("Random: " + i.ToString());
            Debug.Print(CommonInfo.IsInitializated.ToString());
            var e = CommonInfo.Employees.First(n => n.EmployeeID == i);
            LogedEmployee = e; 
#endif      
        }
        private void E_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LogedEmployee));
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
    /// Конвертер перевода длинны строки в целое
    /// <list type="bullet">
    ///     <item>
    ///         <term>Null</term>
    ///         <description>0</description>
    ///     </item>
    ///     <item>
    ///         <term>Lenght = 0</term>
    ///         <description>1</description>
    ///     </item>
    ///     <item>
    ///         <term>Lenght > 0</term>
    ///         <description>0</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class TextLenghtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s != null)
            {
                 return s.Length == 0 ? 1 : 0;
            }
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Конвертер перевода длинны строки в целое, указанное параметром.
    /// <para>Если параметр не задан возвращает 'auto'</para>
    /// </summary>
    public class TextLenghtToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (String.IsNullOrEmpty(s))
            {
                return "0";
            }
            else return parameter == null ? "auto" : parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер перевода числа в свойство Visibility.
    /// <para>
    /// 0 - Visibility, else Collapsed
    /// </para>
    /// <para>Если <paramref name="parameter"/> не null, то инвертирует поведение</para>
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (parameter == null)
            {
                if (val == 0) return "Visible";
                else return "Collapsed";
            }
            else
            {
                if (val == 0) return "Collapsed";
                else return "Visible";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Конвертер перевода перечисления RuningTaskStatus в свойство Color
    /// <list type="bullet">
    ///     <item>
    ///         <term>None</term>
    ///         <description>Transparent</description>
    ///     </item>
    ///     <item>
    ///         <term>Completed</term>
    ///         <description>Green</description>
    ///     </item>
    ///     <item>
    ///         <term>Running</term>
    ///         <description>Grey</description>
    ///     </item>
    ///     <item>
    ///         <term>Error</term>
    ///         <description>Red</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class TaskStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RuningTaskStatus)
            {
                var status = (RuningTaskStatus)value;
                switch (status)
                {
                    case RuningTaskStatus.None:
                        return "Transparent";
                    case RuningTaskStatus.Completed:
                        return "#FF92D825";
                    case RuningTaskStatus.Running:
                        return "Gray";
                    case RuningTaskStatus.Error:
                        return "Red";
                    default:
                        return "Transparent";
                }
            }
            else return "Transparent";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Конвертер превода состояния экспертизы (сдана, просрочена и т.д.) в цвет
    /// </summary>
    public class FocusColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exp = value as String;
            switch (exp)
            {
                case "Просроченные":
                    return "Red";
                case "Требующие внимания":
                    return "#FFE0E005";
                default:
                    return "#FF82C51C";
            }
            //if (exp.Remain2.HasValue)
            //{
            //    if (exp.Remain2.Value > 3) return "Green";
            //    if (exp.Remain2.Value < 1) return "Red";
            //    return "Yellow";
            //}
            //else return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibleConverter : IValueConverter
    {
        Visibility _truestate = Visibility.Visible;
        Visibility _falsestate = Visibility.Collapsed;
        public BoolToVisibleConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = parameter as string;
            if (s != null)
            {
                switch (s)
                {
                    case "vc":
                        _truestate = Visibility.Visible;
                        _falsestate = Visibility.Collapsed;
                        break;
                    case "vh":
                        _truestate = Visibility.Visible;
                        _falsestate = Visibility.Hidden;
                        break;
                    case "cv":
                        _truestate = Visibility.Collapsed;
                        _falsestate = Visibility.Visible;
                        break;
                    case "ch":
                        _truestate = Visibility.Collapsed;
                        _falsestate = Visibility.Hidden;
                        break;
                    case "hc":
                        _truestate = Visibility.Hidden;
                        _falsestate = Visibility.Collapsed;
                        break;
                    case "hv":
                        _truestate = Visibility.Hidden;
                        _falsestate = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
            bool r = (bool)value;
            if (r) return _truestate;
            return _falsestate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ParamToHideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = parameter as string;
            if (s != null)
            {
                string v = value as string;
                if ( String.Equals(v,s, StringComparison.OrdinalIgnoreCase))
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}