﻿using Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClusterExecutor {
    class Program {
        public class TstParam {
            public string name;
            public int Index { get; set; }
            public double Doub { get; set; }
        }

        public class TstResult {
            public double d;
        }

        public class TstClustWorker : ClustWorker<TstParam, TstResult> {
            public static Random rnd = new Random();
            public override TstResult MapAction(TstParam taskData) {
                Console.WriteLine($"ClustWorker {Id} Started  {taskData.name} at {DateTime.Now}");
                
                var dd = 2000 + (int)(4000 * rnd.NextDouble());
                Thread.Sleep(dd);
                Console.WriteLine($"ClustWorker {Id} Finished  {taskData.name} at {DateTime.Now}");
                return new TstResult() {
                    d = dd
                };
            }
        }



        static void Main(string[] args) {
            var ex = new ClusterExecutor<TstParam, TstResult, TstClustWorker>();

            var zad = Enumerable.Range(0, 10)
                .Select(i => new TstParam() {
                    name = $"pars{i}",
                    Index = i,
                    Doub = i * i
                });


            var answ = ex.Run(zad);
            ex.WaitAll(answ);
            Console.WriteLine("doneFirst 10");
            Console.ReadLine();
            answ = ex.Run(zad);
            Console.ReadLine();
            ex.Stop();
            Console.WriteLine("stoped");
            Console.ReadLine();
            ex.Run();
            Console.WriteLine("run");
            Console.ReadLine();
        }

 
    }
}
