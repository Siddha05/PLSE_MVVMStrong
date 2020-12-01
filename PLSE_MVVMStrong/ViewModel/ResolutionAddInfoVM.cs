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
            RuningTask task = new RuningTask("Testing task visualizator");

            task.AddSubTask(new RuningTask("Sub task fo testing task"));
            Tasks.Add(task);
        }
        public async void Proceed()
        {
            var cr = new DocsCreater();
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
            Application word = new Application();
            var not = new RuningTask("");
            Tasks.Add(not);
            var t = cr.CreateNotifyAsync(_resol,not, word);
            await t;
            if (t.IsFaulted) not.Status = RuningTaskStatus.Error;
            var pod = new RuningTask("");
            Tasks.Add(pod);
            var t1 = cr.CreateSubscribeAsync(_resol,pod);
            await t1;
            word.Quit(SaveChanges: WdSaveOptions.wdDoNotSaveChanges);
            if (t1.IsFaulted) pod.Status = RuningTaskStatus.Error;
        End:
            Completed = true;
        }
    }
}
