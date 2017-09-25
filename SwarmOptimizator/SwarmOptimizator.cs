using Executor;
using Microsoft.Research.Oslo;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {
    public class Stepper {

    }
    public class SwarmOptimizator {
        public ExecutorAbstract<ParConst,Fitness> Executor { get; set; }
        public BeeColony colony;
    }
}
