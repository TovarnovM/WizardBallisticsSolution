using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {
    public enum FitExtremum { minimize, maximize }
    public class FitInfo {
        public FitExtremum Extremum { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public bool Matters { get; set; } = true;
        public string Name { get; set; }
        public FitInfo(string name, FitExtremum extr = FitExtremum.maximize, double? min = null, double? max = null) {
            Name = name;
            Extremum = extr;
            Min = min;
            Max = max;
        }
        public bool ValidValue(double? value) {
            if (value == null)
                return false;
            if (Min != null && value < Min)
                return false;
            if (Max != null && value > Max)
                return false;
            return true;
        }
    }
}
