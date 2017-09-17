using Executor;
using MPAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterExecutor {
    public class ClusterExecutor<TParams, TResult, TWorker> : ExecutorAbstract<TParams, TResult> 
        where TWorker: ClustWorker<TParams, TResult> {

        Node node;
        MainWorker<TParams, TResult> mainWorker;
        public ClusterExecutor(IComputeTask<TParams, TResult> computeTask, string regServerAddress, int regServerPort, int port) {
            node = new Node();
            mainWorker = node.OpenDistributed<MainWorker<TParams, TResult>>(regServerAddress, regServerPort, port);
        }

        public ClusterExecutor() {
            node = new Node();
            mainWorker = node.OpenLocal<MainWorker<TParams, TResult>>();
        }

        void initMainWorkerFields() {
            mainWorker.inputCollection = inputCollection;
            mainWorker.doneQueue = doneQueue;
            mainWorker.inprogressBag = inprogressBag;
            mainWorker.blackList = blackList;
            mainWorker.locker = locker;

            
    }

        public override void Run() {
            //mainWorker.Send(new WorkerAddress(node.GetId(),mainWorker.Id),)
            throw new NotImplementedException();
        }

        public override void Stop(bool waitExecut = true) {
            throw new NotImplementedException();
        }

        public override bool WaitAll(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            throw new NotImplementedException();
        }

        public override Res<TParams, TResult> WaitAny(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            throw new NotImplementedException();
        }
    }
}
