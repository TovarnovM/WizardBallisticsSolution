using Microsoft.Research.Oslo;
using System;
using System.Collections.Generic;

namespace SwarmOptimizator {
    public class ParConst {
        public Vector dVec;
        public List<string> dNames;
        public Dictionary<string, string> sDict;
        public double GetDbyName(string name) {
            int ind = dNames.IndexOf(name);
            return ind >= 0 ? dVec[ind] : throw new ArgumentOutOfRangeException($"{name} такого элемента нет");
        }
        public void SetDbyName(string name, double val) {
            int ind = dNames.IndexOf(name);
            if (ind >= 0) {
                dVec[ind] = val;
            } else {
                throw new ArgumentOutOfRangeException($"{name} такого элемента нет");
            }
        }
        public double this[string dKey] {
            get {
                return GetDbyName(dKey);
            }
            set {
                SetDbyName(dKey, value);
            }
        }
        public double this[int dIndex] {
            get {
                return dVec[dIndex];
            }
            set {
                dVec[dIndex] = value;
            }
        }
    }
    
}
