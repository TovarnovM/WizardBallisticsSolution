using Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutorConsoleTest {
    class Program {

        class tstClass : IComputeTask<int, int> {
            public int MapAction(int taskData) {
                var rnd = new Random();
               // Console.WriteLine($"Start  Computing    {taskData}");
                Thread.Sleep((int)(2000+3000 * rnd.NextDouble()));
              //  Console.WriteLine($"Finish Computing    {taskData}");
                return taskData;
            }
        }
        static void Main(string[] args) {
            var tstC = new tstClass();
            var tid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"id { tid}");
            Console.ReadLine();
            var exc = new ThreadExecutor<int, int>(tstC) { WorkerCountMax = 50 };
            var tsks = Enumerable.Range(0, 100);
            //exc.QueueAddNew += Exc_QueueAddNew;
            exc.ExecutAddNew += Exc_ExecutAddNew;
            exc.ExecutDoneNew += Exc_ExecutDoneNew;
            var res = exc.Run(tsks);
           // exc.WaitAll(res);
            Console.ReadLine();
            exc.Stop(true);
            res = exc.AddToQueue(tsks);
            
            Console.WriteLine("stoped");
            Console.ReadLine();
            Console.WriteLine("run");
            exc.Run();
            Console.ReadLine();
        }

        private static void Exc_ExecutDoneNew(object sender, Res<int, int> e) {
            var tid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Done {e.Params}  id {tid}");
        }

        private static void Exc_ExecutAddNew(object sender, Res<int, int> e) {
            var tid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Exec {e.Params}   id {tid}");
        }

        private static void Exc_QueueAddNew(object sender, Res<int, int> e) {
            var tid = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Adding {e.Params}   id {tid}");
        }
    }
}
