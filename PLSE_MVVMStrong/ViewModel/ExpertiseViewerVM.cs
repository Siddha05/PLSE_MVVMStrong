using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertiseViewerVM : INotifyPropertyChanged
    {
        #region Fields
        Expertise _expertise;
        private static SolidColorBrush _transp = new SolidColorBrush(Colors.Transparent);
        private static SolidColorBrush _red = new SolidColorBrush(Colors.Red);
        RelayCommand _starclick;
        RelayCommand _expchanged;
        #endregion
        #region Properties
        public Expertise Expertise => _expertise;
        public IReadOnlyList<string> ResolutionTypes => CommonInfo.ResolutionTypes;
        public IReadOnlyList<string> ResolutionStatus => CommonInfo.ResolutionStatus;
        public IReadOnlyList<string> ExpertiseTypes => CommonInfo.ExpertiseTypes;
        public IReadOnlyList<string> ExpertiseResult => CommonInfo.ExpertiseResult;
        public IEnumerable<KeyValuePair<string, string>> CaseTypes = CommonInfo.CaseTypes;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Expert> Experts { get; } = CommonInfo.Experts.Where(n => n.Employee.EmployeeStatus != "не работает")
                                                                                    .GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                                                    .Select(n => n.First())
                                                                                    .OrderBy(n => n.Employee.Sname);
        //public IReadOnlyList<string> RequestTypes => CommonInfo.RequestTypes;
        public ListCollectionView Specialities { get; } = new ListCollectionView(CommonInfo.Experts);
        public SolidColorBrush[] StarsArray { get; } = new SolidColorBrush[] { _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp };
        #endregion
        #region Commands
        public RelayCommand CustomerSelect { get; }
        public RelayCommand StarClick
        {
            get
            {
                return _starclick != null ? _starclick : _starclick = new RelayCommand(n =>
                {
                    if (Int32.TryParse(n.ToString(), out int r))
                    {
                        if (StarsArray[r] == _transp)
                        {
                            SetEvaluation(r);
                        }
                        else
                        {
                            SetEvaluation(r - 1);
                        }
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StarsArray)));
                });
            }
        }
        public RelayCommand ExpertChanged
        {
            get
            {
                return _expchanged != null ? _expchanged : _expchanged = new RelayCommand(n =>
                {
                    Specialities.Filter = x => (x as Expert).Employee.EmployeeID == (n as Employee).EmployeeID;
                });
            }
        }
        #endregion

        public ExpertiseViewerVM()
        {
            Resolution r = Resolution.New;
            r.RegistrationDate = new DateTime(2018, 5, 19);
            r.ResolutionDate = new DateTime(2018, 5, 18);
            r.PrescribeType = "судебная автотехническая экспертиза";
            r.ResolutionType = "определение";
            r.Customer = CommonInfo.Customers.First(n => n.CustomerID == 7);
            r.ResolutionStatus = "в работе";
            r.Objects.Add(new ContentWrapper("Материалы гражданского дела № 2-457/2020 на 56 л."));
            r.Objects.Add(new ContentWrapper("Руководство по эксплутации автомобиля \"Datsun on-Do\""));
            r.Questions.Add(new ContentWrapper(@"Имеются ли в предоставленном на исследование автомобиле – марка, модель  Datsun on-Do; наименование (тип ТС)  - легковой; VIN Z8NBAABD0F0018885, принадлежащем Мишину Дмитрию Андреевичу, недостатки, если да, то какие именно?"));   
            r.Questions.Add(new ContentWrapper("Если выявленные в автомобиле марки Datsun on - Do; VIN Z8NBAABD0F0018885, недостатки носят производственный характер, то являются ли они существенными(с технической точки зрения) для исследуемого автомобиля; возможно ли их устранение и каким способом, каково нормативное время на их устранение и стоимость устранения с учетом использования узлов - агрегатов завода - изготовителя ?"));
            r.Questions.Add(new ContentWrapper("Являются ли выявленные недостатки производственными или эксплуатационными ?"));
            r.CaseAnnotate = "по иску Мишина Д.А. к ОАО \"Арбеково - Мотор - Плюс\" о защите прав потребителей";
            r.Comment = "это фиктивная экспертиза качества автомобиля Datsun On-Do, созданная для демонстрационных целей и в качестве отладочной для проэктирования интерфейса пользователя в различных сценариях";
            r.CaseNumber = "2-457/2020";
            r.Plaintiff = "Мишин Д.А.";
            r.Respondent = "ОАО \"Арбеково - Мотор - Плюс\"";
            r.TypeCase = "гражданское";
            Expertise e = Expertise.New;
            e.SpendHours = 40;
            e.PreviousExpertise = null;
            e.ExpertiseType = "первичная";
            e.TimeLimit = 30;
            e.StartDate = new DateTime(2018, 5, 19);
            e.EndDate = new DateTime(2018, 5, 29);
            e.ObjectsCount = 2;
            e.ExpertiseResult = "заключение эксперта";
            e.Number = "3410";
            e.Expert = CommonInfo.Experts.First(n => n.ExpertID == 1);
            Bill b = new Bill
            {
                Number = "233",
                BillDate = new DateTime(2018,5,20),
                PaidDate = new DateTime(2018, 5, 24),
                Payer = "истца",
                Hours = 24,
                HourPrice = 600,
                Paid = 24 * 600
            };
            Bill b1 = new Bill
            {
                Number = "234",
                BillDate = new DateTime(2018, 5, 20),
                PaidDate = new DateTime(2018, 5, 27),
                Payer = "ответчика",
                Hours = 32,
                HourPrice = 600,
                Paid = 32 * 600 + 23
            };
            e.Bills.Add(b); e.Bills.Add(b1);
            Request rq = new Request
            {
                RequestDate = new DateTime(2018, 5, 20),
                //RequestType = CommonInfo.RequestTypes[1],
                Comment = "запрос на осмотр объекта исследования"
            };
            e.Requests.Add(rq);
            r.Expertisies.Add(e);
            _expertise = e;
            Specialities = new ListCollectionView(CommonInfo.Specialities);
        }
        public ExpertiseViewerVM(Expertise expertise)
        {
            _expertise = expertise;
        }
        private void SetEvaluation(int eval)
        {
            for (int i = 0; i < StarsArray.Length; i++)
            {
                if (i <= eval) StarsArray[i] = _red;
                else StarsArray[i] = _transp;
            }
            Expertise.Evaluation = (short)(eval + 1);
        }
    }
}
