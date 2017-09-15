using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor {
    public enum ResStatus  { notCalcYet, calculating, finished, caclError };
    public enum ExecStatus { notReady, ready, executing, error };

    public class Res<TParams, TResult>  {
        public TResult Result { get; set; }
        public TParams Params { get; set; }
        public DateTime StartExecuting { get; set; }
        public DateTime StopExecuting { get; set; }
        public ResStatus Status { get; set; }
        public string Info { get; set; }
        public ExecutorAbstract<TParams, TResult> Executor { get; set; }
    }

    public interface IComputeTask<TParams, TResult> {
        TResult MapAction(TParams taskData);
    }

    public class ExecutorAbstract<TParams, TResult> {

        ExecStatus status;
        public ExecStatus Status { get { return status; } }

        public List<Res<TParams, TResult>> AddToQueue(IEnumerable<TParams> tasks, bool startExecuting = false) {
            throw new NotImplementedException();
        }

        public List<Res<TParams, TResult>> Run(IEnumerable<TParams> tasks) {
            throw new NotImplementedException();
        }

        public void Run() {
            throw new NotImplementedException();
        }

        public void Stop(bool killExecution = true) {
            throw new NotImplementedException();
        }

        public bool WaitAll(IEnumerable<Res<TParams, TResult>> tasks) {
            return WaitAll(tasks.ToArray());
        }
        public bool WaitAll(params Res<TParams, TResult>[] tasks) {
            var ts = TimeSpan.MaxValue;
            return WaitAll(ts,tasks);
        }
        public bool WaitAll(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            throw new NotImplementedException();
        }
        public bool WaitAll(IEnumerable<Res<TParams, TResult>> tasks, TimeSpan time4execMax) {
            return WaitAll(time4execMax, tasks.ToArray());
        }

        public Res<TParams, TResult> WaitAny(params Res<TParams, TResult>[] tasks) {
            var ts = TimeSpan.MaxValue;
            return WaitAny(ts, tasks);
        }
        public Res<TParams, TResult> WaitAny(TimeSpan time4execMax, params Res<TParams, TResult>[] tasks) {
            throw new NotImplementedException();
        }
        public Res<TParams, TResult> WaitAny(IEnumerable<Res<TParams, TResult>> tasks) {
            var ts = TimeSpan.MaxValue;
            return WaitAny(tasks.ToArray());
        }
        public Res<TParams, TResult> WaitAny(IEnumerable<Res<TParams, TResult>> tasks, TimeSpan time4execMax) {
            return WaitAny(tasks.ToArray(), time4execMax);
        }

        public List<Res<TParams, TResult>> DoneList {
            get {
                throw new NotImplementedException();
            }
        }

        public List<Res<TParams, TResult>> InProgress {
            get {
                throw new NotImplementedException();
            }
        }

        public List<Res<TParams, TResult>> InQueue {
            get {
                throw new NotImplementedException();
            }
        }

        public bool RemoveFromDoneList(params TParams[] who) {
            throw new NotImplementedException();
        }

        public bool RemoveFromDoneList(params Res<TParams, TResult>[] who) {
            throw new NotImplementedException();
        }

        public bool RemoveFromQueue(params TParams[] who) {
            throw new NotImplementedException();
        }

        public bool RemoveFromQueue(params Res<TParams, TResult>[] who) {
            throw new NotImplementedException();
        }

        public List<Res<TParams, TResult>> ClearDoneList() {
            throw new NotImplementedException();
        }
        public List<Res<TParams, TResult>> ClearQueue() {
            throw new NotImplementedException();
        }

        public event EventHandler<Res<TParams, TResult>> QueueAddNew;
        protected virtual void OnQueueAddNew(Res<TParams, TResult> e) {
            QueueAddNew?.Invoke(this, e);
        }
        public event EventHandler<Res<TParams, TResult>> ExecutAddNew;
        protected virtual void OnExecutAddNew(Res<TParams, TResult> e) {
            ExecutAddNew?.Invoke(this, e);
        }
        public event EventHandler<Res<TParams, TResult>> ExecutDoneNew;
        protected virtual void OnExecutDoneNew(Res<TParams, TResult> e) {
            ExecutDoneNew?.Invoke(this, e);
        }
    }

}
