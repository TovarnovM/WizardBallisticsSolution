using Executor;
using MPAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClusterExecutor {
    public class ClusterExecutor<TParams, TResult, TClustWorker> : ExecutorAbstract<TParams, TResult> 
        where TClustWorker: ClustWorker<TParams, TResult> {

        Node node;
        MainWorker<TParams, TResult, TClustWorker> mainWorker;
        protected ConcurrentQueue<Res<TParams, TResult>> inputQ = new ConcurrentQueue<Res<TParams, TResult>>();

        public ClusterExecutor(IComputeTask<TParams, TResult> computeTask, string regServerAddress, int regServerPort, int port) {
            node = new Node();
            mainWorker = node.OpenDistributed<MainWorker<TParams, TResult, TClustWorker>>(regServerAddress, regServerPort, port);
            InitMainWorkerFields();
        }

        public ClusterExecutor() {
            node = new Node();
            mainWorker = node.OpenLocal<MainWorker<TParams, TResult, TClustWorker>>();
            InitMainWorkerFields();
        }

        void InitMainWorkerFields() {
            mainWorker.inputQ = inputQ;
            mainWorker.doneQueue = doneQueue;
            mainWorker.inprogressBag = inprogressBag;
            mainWorker.blackList = blackList;
            mainWorker.locker = locker;
            mainWorker.Owner = this;
            mainWorker.Send(mainWorker.MyAddr, MessageTypes.Initilised, null);
        }

        public override void OnQueueAddNew(Res<TParams, TResult> e) {
            base.OnQueueAddNew(e);
            mainWorker.Send(mainWorker.MyAddr, MessageTypes.StartNewTask, null);
        }

        public override void Run() {
            mainWorker.Send(mainWorker.MyAddr, MessageTypes.Run, null);
            
        }

        public override void Stop(bool waitExecut = true) {
            mainWorker.Send(mainWorker.MyAddr, MessageTypes.Pause, null);
        }

        public override bool WaitAll(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            var loosers = new HashSet<Res<TParams, TResult>>(tasks);
            var beg = DateTime.Now;
            while (loosers.Count > 0 || (DateTime.Now - beg > time4execMax)) {
                loosers.RemoveWhere(l => l.Status == ResStatus.finished || l.Status == ResStatus.caclError);
                Thread.Sleep(100);
            }
            return true;
        }

        public override Res<TParams, TResult> WaitAny(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            var loosers = new HashSet<Res<TParams, TResult>>(tasks);
            int count = tasks.Length;
            var beg = DateTime.Now;
            while (loosers.Count == count || (DateTime.Now - beg > time4execMax)) {
                if (loosers.RemoveWhere(l => l.Status == ResStatus.finished || l.Status == ResStatus.caclError) > 0) {
                    return tasks.FirstOrDefault(l => l.Status == ResStatus.finished || l.Status == ResStatus.caclError);

                }
                Thread.Sleep(100);
            }
            return null;
        }

        

        public override List<Res<TParams, TResult>> AddToQueue(params TParams[] tasks) {
            var lst = new List<Res<TParams, TResult>>(tasks.Length + 1);
            foreach (var pr in tasks) {
                var res = new Res<TParams, TResult>() {
                    //Executor = this,
                    Params = pr,
                    Status = ResStatus.notCalcYet
                };
                lst.Add(res);
                inputQ.Enqueue(res);
                OnQueueAddNew(res);
            }
            return lst;
        }
    }
}
