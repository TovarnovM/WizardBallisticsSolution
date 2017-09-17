using Executor;
using MPAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterExecutor {

    public abstract class ClustWorker<TParams, TResult> : Worker, IComputeTask<TParams, TResult> {
        public static JsonSerializerSettings DefSSetings { get; set; } = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public override void Main() {
            Log.Info("Prime worker {0} online", Id);
            Message msg;
            do {
                msg = Receive(); //block and wait for incoming messages
                switch (msg.MessageType) {
                    case MessageTypes.Start:
                        //request the first batch to process
                        Send(msg.SenderAddress, MessageTypes.RequestBatch, null);
                        break;

                    case MessageTypes.ReplyBatch:
                        //We have receives a batch from the main worker. Process it
                        var js_str_pars = (string)msg.Content;
                        var pars = JsonConvert.DeserializeObject<TParams>(js_str_pars, DefSSetings);
                        var res = MapAction(pars);
                        var js_str_res = JsonConvert.SerializeObject(res, DefSSetings);
                        Send(msg.SenderAddress, MessageTypes.Result, js_str_res);
                        //request the next batch to process
                        Send(msg.SenderAddress, MessageTypes.RequestBatch, null);
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
