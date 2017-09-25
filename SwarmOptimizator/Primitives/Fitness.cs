using Microsoft.Research.Oslo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {

    public class Fitness {
        public List<FitInfo> FitInfos { get; set; }
        public Vector fVec;
        public double GetFbyName(string name) {
            int ind = -1;
            for (int i = 0; i < FitInfos.Count; i++) {
                if(FitInfos[i].Name == name) {
                    ind = i;
                    break;
                }
            }
            return ind >= 0 ? fVec[ind] : throw new ArgumentOutOfRangeException($"{name} такого фитнеса нет");
        }
        public void SetFbyName(string name, double val) {
            int ind = -1;
            for (int i = 0; i < FitInfos.Count; i++) {
                if (FitInfos[i].Name == name) {
                    ind = i;
                    break;
                }
            }
            if (ind >= 0) {
                fVec[ind] = val;
            } else {
                throw new ArgumentOutOfRangeException($"{name} такого элемента нет");
            }
        }
        public double this[string fName] {
            get {
                return GetFbyName(fName);
            }
            set {
                SetFbyName(fName, value);
            }
        }
        public bool NeedCalc {
            get {
                for (int i = 0; i < fVec.Length; i++) {
                    if (Double.IsNaN(fVec[i]))
                        return true;
                }
                return false;
            }
        }
        public Fitness(List<FitInfo> FitInfos) {
            this.FitInfos = FitInfos;
            fVec = Vector.Zeros(FitInfos.Count);
            for (int i = 0; i < fVec.Length; i++) {
                fVec[i] = Double.NaN;
            }
        }
        public Fitness(params FitInfo[] fitInfos):this(fitInfos.ToList()) {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="that"></param>
        /// <returns>1 = me cooler, -1 = f2Vec Cooler, 0 = pareto poh</returns>
        public int CoolerThan(Fitness that) {
            return CoolerThan(that.fVec);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fVec2"></param>
        /// <returns>1 = me cooler, -1 = f2Vec Cooler, 0 = pareto poh</returns>
        public int CoolerThan(Vector fVec2) {
            var par = new int[fVec.Length];
            for (int i = 0; i < fVec.Length; i++) {
                par[i] = fVec[i] > fVec2[i]
                    ? 1
                    : fVec[i] < fVec2[i]
                    ? -1
                    : 0;
                par[i] *= !FitInfos[i].Matters
                    ? 0
                    : FitInfos[i].Extremum == FitExtremum.maximize
                    ? 1
                    : -1;
            }
            int max = -1, min = 1;
            for (int i = 0; i < par.Length; i++) {
                if (par[i] > max)
                    max = par[i];
                if (par[i] < min)
                    min = par[i];
            }
            return max == 1 && min > -1
                ? 1
                : min == -1 && max < 1
                ? -1
                : 0;
        }
    }
    
}
