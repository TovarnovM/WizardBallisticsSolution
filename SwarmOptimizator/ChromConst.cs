using Microsoft.Research.Oslo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator {
    public class ParamsInfo {
        public List<DblRange> DRanges;
        public List<StrRange> SRanges;
        public 
    }

    public class 

    public class ChromConst {
        public List<DblRange> DRanges;
        public Vector dPrms;

        public List<StrRange> SRanges;
        public List<string> sPrms;

        public string Name { get; set; }

        public List<FitInfo> FitInfos { get; set; }
        public Vector fitVector;

        public bool NeedCalc {
            get {
                for (int i = 0; i < fitVector.Length; i++) {
                    if (Double.IsNaN(fitVector[i]))
                        return true;
                }
                return false;
            }
        }

        public ChromConst(ChromConst cloneMe) {
            dPrms = cloneMe.dPrms.Clone();
            sPrms = new List<string>(cloneMe.sPrms);
            fitVector = cloneMe.fitVector.Clone();
            DRanges = cloneMe.DRanges;
            SRanges = cloneMe.SRanges;
            FitInfos = cloneMe.FitInfos;
            Name = cloneMe.Name;
        }
        public ChromConst(List<DblRange> DRanges, List<StrRange> SRanges, List<FitInfo> FitInfos, string name = "") {
            this.DRanges = DRanges;
            this.SRanges = SRanges;
            this.FitInfos = FitInfos;
            RndInit(name);
        }
        public void RndInit(string name = "") {
            dPrms = Vector.Zeros(DRanges.Count);
            for (int i = 0; i < DRanges.Count; i++) {
                var dr = DRanges[i];
                dPrms[i] = dr.GetRandValue();
            }
            sPrms = new List<string>(SRanges.Capacity);
            foreach (var sr in SRanges) {
                sPrms.Add(sr.GetRandValue());
            }
            fitVector = Vector.Zeros(FitInfos.Count);
            for (int i = 0; i < FitInfos.Count; i++) {
                fitVector[i] = Double.NaN;
            }
            Name = name == "" ? MarkovNameGenerator.Instance.GetNextName() : name;
        }
        public ChromConst GetNew() {
            return new ChromConst(DRanges, SRanges, FitInfos);
        }
    }
}
