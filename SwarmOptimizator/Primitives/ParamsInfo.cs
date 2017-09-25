using Microsoft.Research.Oslo;
using System.Collections.Generic;

namespace SwarmOptimizator {
    public class ParamsInfo {
        public List<DblRange> DRanges;
        public List<StrRange> SRanges;
        public ParConst GetRnd() {
            int dc = DRanges.Count , sc = SRanges.Count;
            var ans = new ParConst() {
                dNames = new List<string>(dc),
                dVec = Vector.Zeros(dc),
                sDict = new Dictionary<string, string>(sc)
            };
            for (int i = 0; i < dc; i++) {
                ans.dNames.Add(DRanges[i].Name);
                ans.dVec[i] = DRanges[i].GetRandValue();
            }
            foreach (var sr in SRanges) {
                ans.sDict.Add(sr.Name, sr.GetRandValue());
            }
            return ans;
        }
    }
    
}
