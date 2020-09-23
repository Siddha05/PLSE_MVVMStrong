using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLSE_MVVMStrong.ViewModel
{
    class FinishExpertiseVM : INotifyPropertyChanged
    {
 #region Fields
        private static SolidBrush _transp = new SolidBrush(Color.Transparent);
        private static SolidBrush _red = new SolidBrush(Color.Red);
        private RelayCommand _starclick;
        #endregion
 #region Propertise
        public Expertise Expertise { get; set; }
        public ExpertiseDetail Detail { get; }
        public IReadOnlyList<string> ExpertiseResultList { get; }= CommonInfo.ExpertiseResult;
        public Brush[] StarsArray { get; } = new Brush[] { _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp, _transp };
        #endregion
#region Commands
        public RelayCommand StarClick
        {
            get
            {
                return _starclick != null ? _starclick : new RelayCommand(n =>
                                                                {
                                                                    MessageBox.Show("Stars!");
                                                                });
            }
        }
        #endregion
        public FinishExpertiseVM(Expertise expertise)
        {
            Expertise = expertise;
        }
        public FinishExpertiseVM()
        {
            Resolution res = new Resolution(id: 1,
                                            registrationdate: new DateTime(2020, 08, 22),
                                            resolutiondate: new DateTime(2020, 08, 20),
                                            resolutiontype: "определение",
                                            customer: CommonInfo.Customers.First(n => n.CustomerID == 4),
                                            obj: null,
                                            prescribe: "почерковедческая экспертиза",
                                            quest: null,
                                            nativenumeration: true,
                                            status: "в работе",
                                            casenumber: "547-1/2020",
                                            respondent: "ОАО \"Фирма всяческих производственных направленностей\"",
                                            plaintiff: "Карпухин А.В.",
                                            typecase: "гражданское",
                                            annotate: "по факту мошенничества и незаконных действий в отношении Карпухина А.В., а также возмещения причиненного ущерба",
                                            comment: "Длинный комментарий, написанный по поводу визуального тестирования расположения и восприятия на форме отображения состояния экспертизы по требованию и не",
                                            dispatchdate: null,
                                            vr: Model.Version.Original,
                                            updatedate: new DateTime(2020, 08, 22)
                                            );
            Expertise e1 = new Expertise(1, "324",
                                                CommonInfo.Experts.First(n => n.ExpertID == 7),
                                                null,
                                                new DateTime(2020, 08, 21),
                                                null,
                                                30,
                                                "первичная",
                                                null,
                                                null,
                                                Model.Version.Original);
            res.Expertisies.Add(e1);
            Expertise = e1;
            Detail = new ExpertiseDetail();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetEvaluation (uint eval)
        {
            if (eval < 11)
            {
                for (int i = 0; i < StarsArray.Length; i++)
                {
                    if (i <= eval) StarsArray[i] = _red;
                    else StarsArray[i] = _transp;
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StarsArray)));
        }
    }
}
