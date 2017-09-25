using System.Collections.Generic;
using System.Linq;

namespace SwarmOptimizator {
    public class BeeColony {
        public Dictionary<int,Bee> bees;
        public BeeColony(ParamsInfo paramsInfo, List<FitInfo> FitInfos, int popCount) {
            ParamsInfo = paramsInfo;
            this.FitInfos = FitInfos;
            PopCount = popCount;
            bees = new Dictionary<int, Bee>(popCount + 10);
        }
        public BeeColony() {

        }
        public void InitFirstGen() {
            bees.Clear();
            for (int i = 0; i < PopCount; i++) {
                var b = new Bee(i, ParamsInfo, FitInfos);
                bees.Add(i, b);
                var bp = b.GetNew();
                b.AddNew(bp);
            }
        }
        public IEnumerable<BeePoint> GetUnknowns() {
            return bees.Values
                .SelectMany(b => b.unknuwnHistory.AsEnumerable());
        }
        public ParamsInfo ParamsInfo { get; }
        public List<FitInfo> FitInfos { get; }
        public int PopCount { get; }

    }
}
