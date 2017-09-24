using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {
    class Fitness {
        public FitInfo Info { get; set; }
        public double? Value { get; set; }
        public Fitness(FitInfo info, double? value = null) {
            Info = info;
            Value = value;
        }
        public Fitness(Fitness cloneMe) : this(cloneMe.Info, cloneMe.Value) { }
        public Fitness Clone() {
            return new Fitness(this);
        }
        
    }
}
