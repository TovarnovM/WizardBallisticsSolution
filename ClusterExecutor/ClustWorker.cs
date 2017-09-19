using Executor;
using MPAPI;
using Newtonsoft.Json;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterExecutor {

    public class ClustWorkerStatus {
        
        public const int ready = 1;
        public const int buisy = 2;
        public const int notReady = 3;
    }

    public class ClustWorkerOverseer<TParams, TResult, TClustWorker> : Worker where TClustWorker: ClustWorker<TParams,TResult> {
        enum Triggers { ownerInit, startNewTask, taskFinished };
        StateMachine<int, Triggers> sm;
        WorkerAddress daddyAddress;
        WorkerAddress mySlave;
        public override void Main() {
            Log.Info("Clust Overseer {0} online (NodeId {1})", Id, Node.GetId());
            sm = new StateMachine<int, Triggers>(ClustWorkerStatus.notReady);
            sm.Configure(ClustWorkerStatus.notReady)
                .Permit(Triggers.ownerInit, ClustWorkerStatus.ready);
            sm.Configure(ClustWorkerStatus.ready)
                .Permit(Triggers.startNewTask, ClustWorkerStatus.buisy);
            sm.Configure(ClustWorkerStatus.buisy)
                .Permit(Triggers.taskFinished, ClustWorkerStatus.ready);
            
            Message msg;
            do {
                msg = Receive();
                switch (sm.State) {
                    case ClustWorkerStatus.ready:
                        OnReadyAction(msg);
                        break;
                    case ClustWorkerStatus.buisy:
                        OnBuisyAct(msg);
                        break;
                    case ClustWorkerStatus.notReady:
                        OnNotReadyAct(msg);
                        break;
                    default:
                        break;
                }
                switch (msg.MessageType) {
                    case MessageTypes.Result:
                        Send(daddyAddress, MessageTypes.Result, msg.Content);
                        sm.Fire(Triggers.taskFinished);
                        Send(daddyAddress, MessageTypes.ReadyAgain, null);
                        break;
                    case MessageTypes.ResultError:
                        Send(daddyAddress, MessageTypes.ResultError, msg.Content);
                        sm.Fire(Triggers.taskFinished);
                        Send(daddyAddress, MessageTypes.ReadyAgain, null);
                        break;
                    case MessageTypes.RequestStatus:
                        Send(msg.SenderAddress, MessageTypes.ReplyStatus, sm.State);
                        break;
                    case MessageTypes.Terminate:
                        Send(mySlave, MessageTypes.Terminate, null);
                        break;
                    default:
                        break;
                }

            }
            while (msg.MessageType != MessageTypes.Terminate);
            Log.Info("Clust Overseer {0} terminating (NodeId {1})", Id, Node.GetId());
        }

        private void OnNotReadyAct(Message msg) {
            switch (msg.MessageType) {
                case MessageTypes.Start:
                    daddyAddress = msg.SenderAddress;
                    mySlave = Spawn<TClustWorker>(Node.GetId());
                    Monitor(mySlave);
                    sm.Fire(Triggers.ownerInit);
                    Send(daddyAddress, MessageTypes.ReadyAgain, null);
                    break;
                default:
                    break;
            }
        }

        private void OnBuisyAct(Message msg) {
            switch (msg.MessageType) {

                default:
                    break;
            }
        }

        private void OnReadyAction(Message msg) {
            switch (msg.MessageType) {
                case MessageTypes.ReplyTask:
                    Send(mySlave, MessageTypes.ReplyTask, msg.Content);
                    sm.Fire(Triggers.startNewTask);
                    Send(daddyAddress, MessageTypes.Buisy, null);
                    break;
                default:
                    break;
            }
        }
    }

    public static class JsonSett {
        public static JsonSerializerSettings DefSSetings { get; set; } = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            
        };
    }

    public abstract class ClustWorker<TParams, TResult> : Worker, IComputeTask<TParams, TResult> {
        public override void Main() {
            Log.Info("Clust worker {0} online (NodeId {1})", Id, Node.GetId());
            Message msg;
            do {
                msg = Receive(); //block and wait for incoming messages
                switch (msg.MessageType) {
                    case MessageTypes.ReplyTask:
                        try {
                            var js_tsk_pars = (string)msg.Content;
                            var tsk = JsonConvert.DeserializeObject<Res<TParams, TResult>>(js_tsk_pars, JsonSett.DefSSetings);
                            tsk.Result = MapAction(tsk.Params);
                            tsk.Status = ResStatus.finished;
                            var js_str_res = JsonConvert.SerializeObject(tsk, JsonSett.DefSSetings);
                            Send(msg.SenderAddress, MessageTypes.Result, js_str_res);
                            //request the next batch to process
                            //Send(msg.SenderAddress, MessageTypes.RequestTask, null);
                        } catch (Exception) {
                            Send(msg.SenderAddress, MessageTypes.ResultError, msg.Content);
                        }
                        //We have receives a batch from the overseer. Process it

                        break;

                    default:
                        //we do not care about all other messages
                        break;
                }
            }
            while (msg.MessageType != MessageTypes.Terminate);

            Log.Info("Prime worker {0} terminating", Id);
        }

        public abstract TResult MapAction(TParams taskData);
    }
}
