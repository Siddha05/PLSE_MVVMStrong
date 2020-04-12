using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertisesVM :DependencyObject
    {
        public class ResEx
        {
            public Resolution Resolution { get; set; }
            public Expertise Expertise { get; set; }
            public ResEx() { }
            public ResEx(Resolution r , Expertise e)
            {
                this.Resolution = r; this.Expertise = e;
            }
        }
        #region Fields
        private string[] all = { "все" };
        #endregion
        #region Properties
        public IEnumerable<string> ExpertiseTypes { get; }
        public IEnumerable<string> ExpertiseStatus { get; }
        public List<Resolution> Resolutions { get; set; }
        public List<ResEx> ExpertiseList
        {
            get { return (List<ResEx>)GetValue(ExpertiseListProperty); }
            set { SetValue(ExpertiseListProperty, value); }
        }
        public static readonly DependencyProperty ExpertiseListProperty =
            DependencyProperty.Register("ExpertiseList", typeof(List<ResEx>), typeof(ExpertisesVM), new PropertyMetadata(null));

        public IEnumerable<Expert> ExpertList { get; } = CommonInfo.Experts.GroupBy(n => n.Employee.EmployeeID).Select(n => n.First());
        public int ExpiredExpertise
        {
            get => ExpertiseList.Where(n => (n.Expertise.Remain2 ?? 1) < 0).Count();
        }
        public int AttentionExpertise { get; } = 2;
        public Expert QEmployee { get; set; }
        public string QExpertiseType { get; set; } = "все";
        public string QExpertiseStatus { get; set; } = "все";
        public DateTime QSStardDate { get; set; }
        public DateTime QEStartDate { get; set; }
        public DateTime QSEndDate { get; set; }
        public DateTime QEEndtDate { get; set; }
        private string DynamicQuery { get; }
        #endregion
        #region Commands
        public RelayCommand Find { get; }
        public RelayCommand Test { get; } 
        #endregion
        public ExpertisesVM()
        {
            ExpertiseTypes = CommonInfo.ExpertiseTypes.Concat(all);
            ExpertiseStatus = CommonInfo.ExpertiseStatus.Concat(all);


            Resolutions = new List<Resolution>();
            ExpertiseList = new List<ResEx>();
            ObjectsList objects = new ObjectsList();
            QuestionsList questions = new QuestionsList();
            questions.Questions.Add(new ContentWrapper("Question 1"));
            Resolution res = new Resolution(1, new DateTime(2019, 11, 21), new DateTime(2019, 11, 21),
                                            "определение",
                                            CommonInfo.Customers.First(n => n.CustomerID == 4),
                                            objects,
                                            "судебная экспертиза",
                                            questions,
                                            "в работе",
                                            Model.Version.Original, new DateTime(2019, 11, 21));
            questions.Questions.Add(new ContentWrapper("Question 2"));
            Resolution res2 = new Resolution(2, new DateTime(2020, 03, 23), new DateTime(2020, 3, 24),
                                            "определение",
                                            CommonInfo.Customers.First(n => n.CustomerID == 3),
                                            objects,
                                            "судебная экспертиза",
                                            questions,
                                            "в работе",
                                            Model.Version.Original, new DateTime(2020, 3, 24));
            res.Expertisies.Add(new Expertise(1, "324",
                                                CommonInfo.Experts.First(n => n.ExpertID == 7),
                                                "в работе",
                                                new DateTime(2019, 11, 21),
                                                null,
                                                30,
                                                1,
                                                "первичная",
                                                null,
                                                null,
                                                Model.Version.Original));
            res.Expertisies.Add(new Expertise(2, "325",
                                                CommonInfo.Experts.First(n => n.ExpertID == 59),
                                                "в работе",
                                                new DateTime(2019, 11, 21),
                                                null,
                                                30,
                                                1,
                                                "первичная",
                                                null,
                                                null,
                                                Model.Version.Original));
            res2.Expertisies.Add(new Expertise(3, "1438",
                                               CommonInfo.Experts.First(n => n.ExpertID == 6),
                                               "в работе",
                                               new DateTime(2020, 3, 24),
                                               null,
                                               20,
                                               2,
                                               "первичная",
                                               null,
                                               null,
                                               Model.Version.Original));

            Resolutions.Add(res); Resolutions.Add(res2);
            ExpertiseList = Resolutions.Join(Resolutions.SelectMany(n => n.Expertisies),
                                              kr => kr.ResolutionID,
                                              ke => ke.ResolutionID,
                                              (r, e) => new ResEx(r, e)).ToList();
            Find = new RelayCommand(x =>
            {
                Resolutions = CommonInfo.LoadResolution(QEmployee.Employee.EmployeeID);
                var lResex = Resolutions.Join(Resolutions.SelectMany(n => n.Expertisies.Where(f => f.Expert.Employee.EmployeeID == QEmployee.Employee.EmployeeID)),
                                                        kr => kr.ResolutionID,
                                                        ke => ke.ResolutionID,
                                                        (r, e) => new ResEx(r, e));
                if(QExpertiseStatus != "все")
                {
                    Debug.Print($"Status: {QExpertiseStatus}");
                    lResex = lResex.Where(n => n.Expertise.ExpertiseStatus == QExpertiseStatus);
                }
                if (QExpertiseType != "все")
                {
                    Debug.Print($"Type: {QExpertiseType}");
                    lResex = lResex.Where(n => n.Expertise.ExpertiseType == QExpertiseType);
                }
                ExpertiseList = lResex.ToList();
            });
        }
        string Query(int emp)
        {
            StringBuilder qer = new StringBuilder("select * from Activity.fResolution3(" + emp.ToString() +")", 300);

            return qer.ToString();
        }
    }
    [ValueConversion(typeof(int), typeof(int))]
    class ExpertiseLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            return val - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
