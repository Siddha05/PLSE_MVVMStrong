using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertiseViewerVM
    {
        #region Fields
        Expertise _expertise;
        #endregion
        #region Properties
        public Expertise Expertise => _expertise;
        public bool DiscartChanges { get; private set; }
        public IReadOnlyList<string> ResolutionTypes => CommonInfo.ResolutionTypes;
        public IReadOnlyList<string> ResolutionStatus => CommonInfo.ResolutionStatus;
        public IReadOnlyList<string> ExpertiseTypes => CommonInfo.ExpertiseTypes;
        public IReadOnlyList<string> ExpertiseStatus => CommonInfo.ExpertiseStatus;
        public IEnumerable<KeyValuePair<string, string>> CaseTypes = CommonInfo.CaseTypes;
        public IEnumerable<Expert> Experts { get; } = CommonInfo.Experts.GroupBy(keySelector: n => n.Employee.EmployeeID)
                                                                        .First()                                                            
                                                                        .OrderBy(n => n.Employee.Sname);
        public ListCollectionView Specialities { get; }
        #endregion
        #region Commands
        public RelayCommand ExpertChanged { get; }
        public RelayCommand CustomerSelect { get; }
        #endregion

        public ExpertiseViewerVM()
        {
           
            QuestionsList questions = new QuestionsList();
            Resolution r = new Resolution
            {
                RegistrationDate = new DateTime(2018, 5, 19),
                ResolutionDate = new DateTime(2018, 5, 18),
                PrescribeType = "судебная автотехническая экспертиза",
                ResolutionType = "определение",
                Customer = CommonInfo.Customers.First(n => n.CustomerID == 7),
                ResolutionStatus = "в работе"
            };
            r.Objects.Objects.Add(new ContentWrapper("Материалы гражданского дела № 2-457/2020 на 56 л."));
            r.Objects.Objects.Add(new ContentWrapper("Руководство по эксплутации автомобиля \"Datsun on-Do\""));
            r.Questions.Questions.Add(new ContentWrapper(@"Имеются ли в предоставленном на исследование автомобиле – марка, модель  Datsun on-Do; наименование (тип ТС)  - легковой; VIN Z8NBAABD0F0018885, принадлежащем Мишину Дмитрию Андреевичу, недостатки, если да, то какие именно?"));   
            r.Questions.Questions.Add(new ContentWrapper("Если выявленные в автомобиле марки Datsun on - Do; VIN Z8NBAABD0F0018885, недостатки носят производственный характер, то являются ли они существенными(с технической точки зрения) для исследуемого автомобиля; возможно ли их устранение и каким способом, каково нормативное время на их устранение и стоимость устранения с учетом использования узлов - агрегатов завода - изготовителя ?"));
            r.Questions.Questions.Add(new ContentWrapper("Являются ли выявленные недостатки производственными или эксплуатационными ?"));
            r.Case.Annotate = "по иску Мишина Д.А. к ОАО \"Арбеково - Мотор - Плюс\" о защите прав потребителей";
            r.Case.Comment = "это фиктивная экспертиза качества автомобиля Datsun On-Do, созданная для демонстрационных целей и в качестве отладочной для проэктирования интерфейса пользователя в различных сценариях";
            r.Case.Number = "2-457/2020";
            r.Case.Plaintiff = "Мишин Д.А.";
            r.Case.Respondent = "ОАО \"Арбеково - Мотор - Плюс\"";
            r.Case.TypeCase = new KeyValuePair<string, string>("гражданское", "2");
            Expertise e = new Expertise
            {
                SpendHours = 40,
                PreviousExpertise = null,
                ExpertiseType = "первичная",
                TimeLimit = 30,
                EndDate = new DateTime(2018, 7, 10),
                StartDate = new DateTime(2018, 5, 19),
                ExpertiseStatus = "сдана",
                Number = "3410",
                Expert = CommonInfo.Experts.First(n => n.ExpertID ==1)
            };
            r.Expertisies.Add(e);
            _expertise = e;
        }
    }
}
