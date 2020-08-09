using Microsoft.Office.Interop.Word;
using PLSE_MVVMStrong.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PLSE_MVVMStrong.ViewModel
{
    class ResolutionAddInfoVM
    {
 #region Fields
        RelayCommand _exit;
        RelayCommand _continue;
        private Resolution _resol;
        private ObservableCollection<RuningTask> _tasks = new ObservableCollection<RuningTask>();


        #endregion
        #region Properties
        public bool Completed { get; set; } = false;
        public Resolution Resolution
        {
            get => _resol;
        }
        public ObservableCollection<RuningTask> Tasks
        {
            get => _tasks;
        }
        #endregion
        #region Commands
        public RelayCommand Exit
        {
            get => _exit;
        }
        public RelayCommand Continue
        {
            get => _continue;
        }
#endregion
        public ResolutionAddInfoVM(Resolution resolution)
        {
            _resol = resolution;
            _exit = new RelayCommand(n =>
                                    {

                                        var w = (n as View.ResolutionAddInfo);
                                        w.DialogResult = false;
                                        w.Close();
                                    },
                                    e => Completed);
            _continue = new RelayCommand(n =>
                                        {

                                            var w = (n as View.ResolutionAddInfo);
                                            w.DialogResult = true;
                                            w.Close();
                                        },
                                        e => Completed);
        }
        public async void Proceed()
        {
            var cr = new DocsCreater(_resol);
            var bd = new RuningTask("Сохранение в базу данных");
            Tasks.Add(bd);
            try
            {
                //Resolution.SaveChanges(CommonInfo.connection);
                bd.Status = RuningTaskStatus.Completed;
            }
            catch (Exception)
            {
                bd.Status = RuningTaskStatus.Error;
                goto End;
            }
            var pod = new RuningTask("");
            Tasks.Add(pod);
            var t = Worker(pod);
            var pod1 = new RuningTask("");
            Tasks.Add(pod1);
            var t1 = Worker1(pod1);
            await System.Threading.Tasks.Task.WhenAll(new System.Threading.Tasks.Task[] { t, t1});
        End:
            Completed = true;
        }
        async System.Threading.Tasks.Task Worker(RuningTask task)
        {
            task.RuningAction = "Worker started";
            await System.Threading.Tasks.Task.Delay(2000);
            var tk = new RuningTask("Worker subtask started");
            task.AddSubTask(tk);
            await System.Threading.Tasks.Task.Delay(1000);
            tk.Status = RuningTaskStatus.Completed;
            task.RuningAction = "Worker compleated";
            task.Status = RuningTaskStatus.Completed;
        }
        async System.Threading.Tasks.Task Worker1(RuningTask task)
        {
            task.RuningAction = "Worker1 started";
            await System.Threading.Tasks.Task.Delay(4000);
            task.RuningAction = "Worker error";
            task.Status = RuningTaskStatus.Error;
        }
    }
}
