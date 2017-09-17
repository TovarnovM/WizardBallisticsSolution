using Executor;
using MPAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterExecutor {
    class MainWorker<TParams, TResult> : Worker {
        public BlockingCollection<Res<TParams, TResult>> inputCollection;
        public ConcurrentQueue<Res<TParams, TResult>> doneQueue;
        public ConcurrentDictionary<int, Res<TParams, TResult>> inprogressBag;
        public HashSet<TParams> blackList;
        public object locker;

        public override void Main() {
            throw new NotImplementedException();
        }
    }
}
