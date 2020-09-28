using PLSE_MVVMStrong.Model;
using PLSE_MVVMStrong.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PLSE_MVVMStrong.ViewModel
{
    class ExpertisesVM :DependencyObject
    {
        public class ExpertWrapper
        {
            private readonly string _name;
            private readonly List<int> _id;
            public string Name => _name;
            public List<int> Identify => _id;

            public ExpertWrapper(string name, IEnumerable<int> id)
            {
                _name = name;
                _id = new List<int>(id);
            }
            public override string ToString()
            {
                return _name;
            }
        }
        #region Fields
        private string[] all = { "все" };
        private List<Expertise> _expcoll = new List<Expertise>();
        private RelayCommand _editexp;
        private RelayCommand _crtconcl;
        private RelayCommand _finishexp;
        private RelayCommand _report;
        private RelayCommand _accompanying;
        private RelayCommand _request;
        private RelayCommand _addbill;
        #endregion
 #region Properties
        public IEnumerable<string> ExpertiseTypes { get; }
        public IEnumerable<string> ExpertiseStatus { get; }
        public ListCollectionView ExpertiseList { get; private set; } 
        public ListCollectionView ExpertsList { get; } = new ListCollectionView(CommonInfo.Experts.GroupBy(n => n.Employee.EmployeeID).Select(n => n.First()).ToList());
        public Visibility NotFoundVisibility
        {
            get { return (Visibility)GetValue(NotFoundVisibilityProperty); }
            set { SetValue(NotFoundVisibilityProperty, value); }
        }
        public static readonly DependencyProperty NotFoundVisibilityProperty =
            DependencyProperty.Register("NotFoundVisibility", typeof(Visibility), typeof(ExpertisesVM), new PropertyMetadata(Visibility.Hidden));
        public string QExpertiseType { get; set; } = "все";
        public string QExpertiseStatus { get; set; } = "все";
        public DateTime? QSStardDate { get; set; }
        public DateTime? QEStartDate { get; set; }
        public DateTime? QSEndDate { get; set; }
        public DateTime? QEEndDate { get; set; }
        public string ExpertiseNumber { get; set; }
        #endregion
 #region Commands
        public RelayCommand Find { get; }
        public RelayCommand ShowDetails { get; }
        public RelayCommand EditExpertise
        {
            get
            {
                return _editexp != null ? _editexp : _editexp = new RelayCommand(n =>
                                                        {
                                                            var e = n as Expertise;
                                                            if (e != null)
                                                            {
                                                                var wnd = new ExpertiseViewer();
                                                                wnd.DataContext = new ExpertiseViewerVM(e);
                                                                if (wnd.ShowDialog() ?? false)
                                                                {

                                                                }
                                                            }
                                                            
                                                        });
            }
        }
        public RelayCommand OpenWord { get; }
        public RelayCommand CreateSubscribe { get; }
        public RelayCommand CreateRequest { 
            get
            {
                return _request != null ? _request : _request = new RelayCommand(n =>
                                                                {
                                                                    MessageBox.Show("Request clicked");
                                                                },
                                                                e =>
                                                                {
                                                                    var ex = ExpertiseList.CurrentItem as Expertise;
                                                                    if (ex != null && !ex.EndDate.HasValue) return true;
                                                                    return false;
                                                                }
                                                                );
            }
                
        }
        public RelayCommand CreateConclusion
        {
            get
            {
                return _crtconcl != null ? _crtconcl : _crtconcl = new RelayCommand(n =>
                                                                                    {
                                                                                        //var dc = new DocsCreater((ExpertiseList.CurrentItem as Expertise).FromResolution);
                                                                                        //dc.CreateConclusion();
                                                                                        MessageBox.Show(n.GetType().Name);
                                                                                        
                                                                                    },
                                                                                    e => false);
            }
        }
        public RelayCommand FinishExpertise
        {
            get
            {
                return _finishexp != null ? _finishexp : _finishexp = new RelayCommand(n =>
                                                                {
                                                                    var ex = ExpertiseList.CurrentItem as Expertise;
                                                                    var wnd = new FinishExpertise();
                                                                    wnd.DataContext = new FinishExpertiseVM(ex);
                                                                    wnd.ShowDialog();
                                                                },
                                                                    e =>
                                                                    {
                                                                        var ex = ExpertiseList.CurrentItem as Expertise;
                                                                        if (ex != null && !ex.EndDate.HasValue) return true;
                                                                        return false;
                                                                    }
                                                                );
            }
        }
        public RelayCommand CreateReport
        {
            get
            {
                return _report != null ? _report : _report = new RelayCommand(n =>
                                                                {
                                                                    var exp = ExpertiseList.CurrentItem as Expertise;
                                                                    if (exp != null)
                                                                    {
                                                                        var wnd = new AddReport();
                                                                        var vm = new AddReportVM(exp);
                                                                        wnd.DataContext = vm;
                                                                        if (wnd.ShowDialog() ?? false)
                                                                        {
                                                                                exp.Reports.Add(vm.Rept);
                                                                        }
                                                                    }
                                                                    
                                                                },
                                                                e =>
                                                                    {
                                                                        var ex = ExpertiseList.CurrentItem as Expertise;
                                                                        if (ex != null && !ex.EndDate.HasValue) return true;
                                                                        return false;
                                                                    }
                                                                );
            }
        }
        public RelayCommand CreateAccompanying
        {
            get
            {
                return _accompanying != null ? _accompanying : _accompanying = new RelayCommand(n =>
                                                                {
                                                                    MessageBox.Show("Accomp. clicked");
                                                                },
                                                                e =>
                                                                {
                                                                    var ex = ExpertiseList.CurrentItem as Expertise;
                                                                    if (ex != null && ex.EndDate.HasValue) return true;
                                                                    return false;
                                                                }
                                                                );
            }
        }
        public RelayCommand AddBill
        {
            get
            {
                return _addbill != null ? _addbill : _addbill = new RelayCommand(n =>
                {
                    MessageBox.Show("Bill clicked");
                },
                e =>
                {
                    var ex = ExpertiseList.CurrentItem as Expertise;
                    if (ex != null)
                    {
                        if (ex.FromResolution.TypeCase.Equals("гражданское") || ex.FromResolution.TypeCase.Equals("административное") || ex.FromResolution.TypeCase.Equals("исследование")
                                                                    || ex.FromResolution.TypeCase.Equals("административное судопроизводство")) return true;
                    }
                    return false;
                }
                );
            }
        }
        #endregion
        public ExpertisesVM()
        {
            ExpertiseTypes = CommonInfo.ExpertiseTypes.Concat(all);
            ExpertiseStatus = CommonInfo.ExpertiseResult.Concat(all).Concat(new string[] { "в работе"});
           
            var app = Application.Current as App;
            //switch (app.Permissions.Plurality)
            //{
            //    case PermissionPlural.Self:
            //        ExpertsList = new ListCollectionView(CommonInfo.Experts.Where(n => n.Employee.EmployeeID == app.LogedEmployee.EmployeeID)
            //                                                                .GroupBy(n => n.Employee.EmployeeID).Select(n => n.First())
            //                                                                .ToList());
            //        break;
            //    case PermissionPlural.Group:
            //        ExpertsList = new ListCollectionView(CommonInfo.Experts.Where(n => n.Employee.Departament.DepartamentID == app.LogedEmployee.Departament.DepartamentID)
            //                                                                .GroupBy(n => n.Employee.EmployeeID).Select(n => n.First())
            //                                                                .ToList());
            //        ExpertsList.SortDescriptions.Add(new System.ComponentModel.SortDescription("Sname", System.ComponentModel.ListSortDirection.Ascending));
            //        break;
            //    case PermissionPlural.All:
            //        ExpertsList = new ListCollectionView(CommonInfo.Experts.GroupBy(n => n.Employee.EmployeeID).Select(n => n.First()).ToList());
            //        ExpertsList.SortDescriptions.Add(new System.ComponentModel.SortDescription("Sname", System.ComponentModel.ListSortDirection.Ascending));
            //        ExpertsList.GroupDescriptions.Add(new PropertyGroupDescription("Employee.Departament"));
            //        break;
            //    default:
            //        break;
            //}

            #region Init
            //Resolution res = new Resolution(id: 1,
            //                                registrationdate: new DateTime(2020, 08, 22),
            //                                resolutiondate: new DateTime(2020, 08, 20),
            //                                resolutiontype: "определение",
            //                                customer: CommonInfo.Customers.First(n => n.CustomerID == 4),
            //                                obj: null,
            //                                prescribe: "почерковедческая экспертиза",
            //                                quest: null,
            //                                nativenumeration: true,
            //                                status: "в работе",
            //                                casenumber: "547-1/2020",
            //                                respondent: "ОАО \"Фирма всяческих производственных направленностей\"",
            //                                plaintiff: "Карпухин А.В.",
            //                                typecase: "гражданское",
            //                                annotate: "по факту мошенничества и незаконных действий в отношении Карпухина А.В., а также возмещения причиненного ущерба",
            //                                comment: "Длинный комментарий, написанный по поводу визуального тестирования расположения и восприятия на форме отображения состояния экспертизы по требованию и не",
            //                                dispatchdate: null,
            //                                vr: Model.Version.Original,
            //                                updatedate: new DateTime(2020, 08, 22)
            //                                );
            //res.Questions.Add(new ContentWrapper("Question 2"));
            //Resolution res2 = new Resolution(id: 1,
            //                                registrationdate: new DateTime(2020, 06, 02),
            //                                resolutiondate: new DateTime(2020, 05, 28),
            //                                resolutiontype: "определение",
            //                                customer: CommonInfo.Customers.First(n => n.CustomerID == 16),
            //                                obj: null,
            //                                prescribe: "автотехническая и химическая экспертиза",
            //                                quest: null,
            //                                nativenumeration: true,
            //                                status: "выполнено",
            //                                casenumber: "112044444444440001",
            //                                respondent: null,
            //                                plaintiff: null,
            //                                typecase: "уголовное",
            //                                annotate: "по факту совершения административного происшествия имевшего место 05.11.2020 в г. Пензе",
            //                                comment: "Комментарий написанный бездумно",
            //                                dispatchdate: null,
            //                                vr: Model.Version.Original,
            //                                updatedate: new DateTime(2020, 06, 22)
            //                                );
            //Expertise e1 = new Expertise(1, "324",
            //                                    CommonInfo.Experts.First(n => n.ExpertID == 7),
            //                                    null,
            //                                    new DateTime(2019, 11, 21),
            //                                    null,
            //                                    30,
            //                                    "первичная",
            //                                    null,
            //                                    null,
            //                                    Model.Version.Original);
            //Expertise e2 = new Expertise(2, "325",
            //                                    CommonInfo.Experts.First(n => n.ExpertID == 59),
            //                                    null,
            //                                    new DateTime(2019, 11, 21),
            //                                    null,
            //                                    30,
            //                                    "первичная",
            //                                    null,
            //                                    null,
            //                                    Model.Version.Original);
            //Expertise e3 = new Expertise(3, "1438",
            //                                   CommonInfo.Experts.First(n => n.ExpertID == 6),
            //                                   "заключение эксперта",
            //                                   new DateTime(2020, 06, 01),
            //                                   new DateTime(2020, 06, 12),
            //                                   20,
            //                                   "первичная",
            //                                   null,
            //                                   null,
            //                                   Model.Version.Original);
            //Bill b = new Bill(2, "218", new DateTime(2019, 11, 12), new DateTime(2019, 11, 24), null, 12, 630m, 7000m, Model.Version.Original);
            //Bill b1 = new Bill(3, "219", new DateTime(2019, 11, 12), null, null, 24, 630m, 0m, Model.Version.Original);
            //e1.Bills.Add(b); e2.Bills.Add(b1);
            //res.Expertisies.Add(e1);
            //res.Expertisies.Add(e2);
            //res2.Expertisies.Add(e3);
            //_expcoll.Add(e1); _expcoll.Add(e2); _expcoll.Add(e3);
            #endregion
            ExpertiseList = new ListCollectionView(_expcoll);
            ExpertiseList.GroupDescriptions.Add(new PropertyGroupDescription("Focus"));
            
            Find = new RelayCommand(x =>
            {
                var lb = x as ListBox;
                IEnumerable<int> sel = null;
                if (lb.SelectedItem != null)
                {
                    sel = lb.SelectedItems.Cast<Expert>().Select(n => n.Employee.EmployeeID);
                }
                else
                {
                    sel = lb.ItemsSource.Cast<Expert>().Select(n => n.Employee.EmployeeID);
                }
                string q = Query(status: QExpertiseStatus == "все" ? null : QExpertiseStatus,
                             type: QExpertiseType == "все" ? null : QExpertiseType,
                             sdate1: QSStardDate, sdate2: QEStartDate,
                             edate1: QSEndDate, edate2: QEEndDate,
                             id: sel,
                             number: ExpertiseNumber);
                for (int i = _expcoll.Count - 1; i >= 0; i--)
                {
                    ExpertiseList.RemoveAt(i);
                }
                _expcoll = CommonInfo.LoadResolution(q).SelectMany(n => n.Expertisies)
                                                            .Join(sel, ke => ke.Expert.Employee.EmployeeID, ks => ks, (e, s) => e)
                                                            .ToList();
                if (_expcoll.Count > 0)
                {
                    for (int i = 0; i < _expcoll.Count; i++)
                    {
                        ExpertiseList.AddNewItem(_expcoll[i]);
                    }
                    ExpertiseList.CommitNew();
                    NotFoundVisibility = Visibility.Hidden;
                }
                else NotFoundVisibility = Visibility.Visible;
            });
        }
        string Query(string status = null, string type = null, DateTime? sdate1 = null, DateTime? sdate2 = null, string number = null,
                        DateTime? edate1 = null, DateTime? edate2 = null, IEnumerable<int> id = null)
        {
            bool set_where = false;
            string and = "and", where = "where";
            StringBuilder query = new StringBuilder(1428);
            query.AppendLine(@"select
		                e.ExpertiseID, e.Number,e.ExpertiseResult,e.StartDate,e.ExecutionDate,e.TypeExpertise, e.PreviousExpertise, e.SpendHours, e.Timelimit,e.ExpertID,
		                b.BillDate, b.BillID, b.BillNumber, b.HourPrice, b.NHours, b.Paid, b.PaidDate, b.PayerID,
		                r.DelayDate, r.Reason, r.ReportDate, r.ReportID,
		                rq.Comment as RequestComment, rq.DateRequest, rq.RequestID, rq.TypeRequest,
		                rl.CustomerID,rl.PrescribeType, rl.ProvidedObjects, rl.Questions,rl.RegDate, rl.ResolDate,rl.ResolutionID,rl.TypeResolID, rl.ResolutionStatusID, 
		                rl.Annotate, rl.Comment, rl.DispatchDate, rl.NumberCase, rl.Plaintiff, rl.Respondent, rl.TypeCase, rl.NativeQuestionsNumeration
		                from Activity.tblExpertises as e
		                left join Activity.tblBills as b
		                on e.ExpertiseID = b.ExpertiseID
		                left join Activity.tblReports as r
		                on e.ExpertiseID = r.ExpertiseID
		                left join Activity.tblRequest as rq
		                on e.ExpertiseID = rq.ExpertiseID
		                join Activity.tblResolutions as rl
		                on e.ResolutionID = rl.ResolutionID
		                join InnResources.tblExperts as ex
		                on ex.ExpertID = e.ExpertID");
            if (status != null)
            {
                query.AppendLine($"where e.ExpertiseResult = '{status}'");
                set_where = true;
            }
            if (type != null)
            {
                query.AppendLine($"{(set_where ? and: where)} e.TypeExpertise = '{type}'");
                set_where = true;
            }
            if (sdate1 != null)
            {
                query.AppendLine($"{(set_where ? and : where)} e.StartDate > '{sdate1.Value.ToString("yyyy-MM-dd")}'");
                set_where = true;
            }
            if (sdate2 != null)
            {
                query.AppendLine($"{(set_where ? and : where)} e.StartDate < '{sdate2.Value.ToString("yyyy-MM-dd")}'");
                set_where = true;
            }
            if (edate1 != null)
            {
                query.AppendLine($"{(set_where ? and : where)} e.ExecutionDate > '{edate1.Value.ToString("yyyy-MM-dd")}'");
                set_where = true;
            }
            if (edate2 != null)
            {
                query.AppendLine($"{(set_where ? and : where)} e.ExecutionDate < '{edate2.Value.ToString("yyyy-MM-dd")}'");
                set_where = true;
            }
            if (number != null)
            {
                query.AppendLine($"{(set_where ? and : where)} e.Number = '{number}'");
                set_where = true;
            }
            if (id != null)
            {
                StringBuilder sb = new StringBuilder(115);
                foreach (var item in id)
                {
                    sb.Append(",");
                    sb.Append(item.ToString());
                }
                if (sb.Length > 0)
                {
                    sb.Remove(0, 1);
                    query.AppendLine($"{(set_where ? and : where)} (ex.EmployeeID in ({sb})");
                    query.AppendLine(@"or e.ResolutionID in (select distinct ResolutionID
                                                                from Activity.tblExpertises as e
                                                                join InnResources.tblExperts as ex
                                                                on e.ExpertID = ex.ExpertID");
                    query.AppendLine($"where ex.EmployeeID in ({sb})))");
                }     
            }
            return query.ToString();
        }
    } 
}
