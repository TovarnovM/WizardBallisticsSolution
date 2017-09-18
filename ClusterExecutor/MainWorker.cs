using Executor;
using MPAPI;
using Newtonsoft.Json;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterExecutor {
    class MainWorker<TParams, TResult, TClustWorker> : Worker where TClustWorker : ClustWorker<TParams, TResult> {
        public ClusterExecutor<TParams, TResult, TClustWorker> Owner;
        public BlockingCollection<Res<TParams, TResult>> inputCollection;
        public ConcurrentQueue<Res<TParams, TResult>> doneQueue;
        public ConcurrentDictionary<int, Res<TParams, TResult>> inprogressBag;
        public HashSet<TParams> blackList;
        public object locker;
        public enum States { justCreated, paused, running };
        public enum Triggers { initialized, run, pause };
        public StateMachine<States,Triggers> sm;
       

        public override void Main() {
            Log.Info("Main worker creating");
            readyOverseers = new Queue<WorkerAddress>();
            allOverseers = new List<WorkerAddress>();
            buisyOverseers = new Dictionary<WorkerAddress, Res<TParams, TResult>>();
            sm = new StateMachine<States, Triggers>(States.justCreated);
            sm.Configure(States.justCreated)
                .Permit(Triggers.initialized, States.paused);
            sm.Configure(States.paused)
                .Permit(Triggers.run, States.running);
            sm.Configure(States.running)
                .Permit(Triggers.pause, States.paused);
            Log.Info("Main worker sm created");
            InputEnumenator = inputCollection.GetConsumingEnumerable().GetEnumerator();
            SpawnOverseers();
            StartOverseers();
            Message msg;
            do {
                msg = Receive();
                switch (msg.MessageType) {
                    case MessageTypes.ReadyAgain:
                        readyOverseers.Enqueue(msg.SenderAddress);
                        break;
                    case MessageTypes.Result:
                        DecodeAndSendToDoneQueue(msg);
                        break;
                    case MessageTypes.ResultError:
                        DecodeAndSendToDoneQueue(msg, true);
                        break;
                    default:
                        break;
                }
                switch (sm.State) {
                    case States.paused:
                        OnJustPaused(msg);
                        break;
                    case States.running:
                        OnRunningActions(msg);
                        break;
                    case States.justCreated:
                        OnJustJustCreated(msg);
                        break;
                    default:
                        break;
                }

            }
            while (msg.MessageType != MessageTypes.Terminate);
        }

        private void OnJustJustCreated(Message msg) {
            switch (msg.MessageType) {
                case MessageTypes.Initilised:
                    sm.Fire(Triggers.initialized);
                    break;
                default:
                    break;
            }
        }

        private void DecodeAndSendToDoneQueue(Message msg, bool error = false) {
            var js_tsk_pars = (string)msg.Content;
            var tskResult = JsonConvert.DeserializeObject<Res<TParams, TResult>>(js_tsk_pars, JsonSett.DefSSetings);
            var tsk = buisyOverseers[msg.SenderAddress];
            buisyOverseers.Remove(msg.SenderAddress);
            tsk.StopExecuting = DateTime.Now;
            tsk.Result = tskResult.Result;
            tsk.Status = error ? ResStatus.caclError : tskResult.Status;
            doneQueue.Enqueue(tsk);
            Owner.OnExecutDoneNew(tsk);
        }

        private void OnRunningActions(Message msg) {
            switch (msg.MessageType) {
                case MessageTypes.Pause:
                    sm.Fire(Triggers.pause);
                    break;
                case MessageTypes.StartNewTask:
                    StartNewTask();
                    break;
                case MessageTypes.ReadyAgain:
                    Send(MyAddr, MessageTypes.StartNewTask, null);
                    break;

                default:
                    break;
            }
        }

        private void OnJustPaused(Message msg) {
            switch (msg.MessageType) {
                case MessageTypes.Run:
                    sm.Fire(Triggers.run);
                    Send(MyAddr, MessageTypes.StartNewTask, null);
                    break;

                default:
                    break;
            }
        }

        public WorkerAddress MyAddr {
            get {
                return new WorkerAddress(Node.GetId(), Id);
            }
        }

        public IEnumerator<Res<TParams, TResult>> InputEnumenator { get; private set; }
        public Queue<WorkerAddress> readyOverseers;
        public List<WorkerAddress> allOverseers;
        public Dictionary<WorkerAddress, Res<TParams,TResult>> buisyOverseers;
        public ushort MyFreeProcessors { get; set; } = 1;
        public ushort FreeProcessorsInSlaves { get; set; } = 0;
        public void StartOverseers() {
            foreach (var os in allOverseers) {
                Send(os, MessageTypes.Start, null);
            }
        }
        public void SpawnOverseers() {
            var vacNodes = GetVacantNodes();
            foreach (var nid in vacNodes) {
                var wOverseer = Spawn<ClustWorkerOverseer<TParams, TResult, TClustWorker>>(nid);
                allOverseers.Add(wOverseer);
                Monitor(wOverseer);
            }
        }
        public List<ushort> GetVacantNodes() {
            return Node.GetRemoteNodeIds()
                .Concat(new ushort[] { Node.GetId() })
                .SelectMany(node_id => {
                    var freeProc = node_id == Node.GetId() ? MyFreeProcessors : FreeProcessorsInSlaves;
                    return Enumerable.Repeat(node_id, Node.GetProcessorCount(node_id) - freeProc - Node.GetWorkerCount(node_id)/2);
                })
                .ToList();
        }
 
        private void StartNewTask() {
            if (readyOverseers.Count == 0)
                return;

            if (InputEnumenator.MoveNext()) {
                var wOverseer = readyOverseers.Dequeue();
                var tsk = InputEnumenator.Current;
                tsk.StartExecuting = DateTime.Now;
                tsk.Status = ResStatus.calculating;
                var js_str_res = JsonConvert.SerializeObject(tsk, JsonSett.DefSSetings);
                if (!buisyOverseers.ContainsKey(wOverseer))
                    buisyOverseers.Add(wOverseer, tsk);
                else
                    buisyOverseers[wOverseer] = tsk;
                Send(wOverseer, MessageTypes.ReplyTask, js_str_res);
                Owner.OnExecutAddNew(tsk);
                if (readyOverseers.Count > 0)
                    StartNewTask();
            } else {
                return;
            }

        }

    }
}
