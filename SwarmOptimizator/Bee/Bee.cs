using System.Collections.Generic;
using System.Linq;

namespace SwarmOptimizator {
    public class Bee {
        public string Name { get; set; }
        public int id;
        public ParamsInfo paramsInfo;
        public List<BeePoint> history;
        public Queue<BeePoint> unknuwnHistory;
        public BeePoint BestBee { get; private set; }
        public List<FitInfo> FitInfos { get; set; }
        public BeePoint GetNew(int t = 0) {
            var bp = new BeePoint() {
                time = t,
                beeId = id,
                par = paramsInfo.GetRnd(),
                fitness = new Fitness(FitInfos)
            };
            return bp;
        }
        public bool AddNew(BeePoint bp) {
            if (bp.fitness.NeedCalc) {
                unknuwnHistory.Enqueue(bp);
                return true;
            }

            history.Add(bp);
            if (BestBee != null) {
                if (bp.fitness.CoolerThan(BestBee.fitness) > -1) {
                    BestBee = bp;
                }
            } else
                BestBee = bp;
            return false;
        }

        public Bee(int id, ParamsInfo paramsInfo, List<FitInfo> FitInfos, string name = "") {
            this.id = id;
            this.paramsInfo = paramsInfo;
            this.FitInfos = FitInfos;
            Name = name != ""
                ? name 
                : MarkovNameGenerator.Instance.GetNextName();
        }

        public void SynchQueue() {
            
            while (unknuwnHistory.Count > 0  && !unknuwnHistory.Peek().fitness.NeedCalc) {
                var bp = unknuwnHistory.Dequeue();
                AddNew(bp);
            }
        }

        public static (int[] ids, int[,] paretoMatr) GetFitnessparetoMatrix(IEnumerable<Bee> bs) {
            var blist = bs.ToList();
            var res = new int[blist.Count, blist.Count];
            var ids = new int[blist.Count];
            for (int i = 0; i < blist.Count; i++) {
                ids[i] = blist[i].id;
                for (int j = i + 1; j < blist.Count; j++) {
                    res[i, j] = blist[i].BestBee.fitness.CoolerThan(blist[j].BestBee.fitness);
                    res[j, i] = -res[i, j];
                }
            }
            return (ids, res);
        }

        public static List<int> GetParetoIds(int[] ids, int[,] paretoMatr, List<int> ignoreList = null) {
            ignoreList = ignoreList == null ? new List<int>() : ignoreList;
            var res = new List<int>();
            var goodInds = new int[ids.Length];
            for (int i = 0; i < goodInds.Length; i++) {
                goodInds[i] = i;
            }
            int IndexOfIds(int id) {
                for (int i = 0; i < ids.Length; i++) {
                    if (ids[i] == id)
                        return i;
                }
                return -1;
            }

            foreach (var badId in ignoreList) {
                int ii = IndexOfIds(badId);
                if(ii >= 0) {
                    goodInds[ii] = -1;
                }
            }

            for (int i = 0; i < ids.Length; i++) {
                if (goodInds[i]<0)
                    continue;
                bool pareto_I = true;
                for (int j = 0; j < paretoMatr.GetLength(1); j++) {
                    if (goodInds[j] < 0)
                        continue;
                    if (paretoMatr[i, j] < 0) {
                        pareto_I = false;
                        break;
                    }
                }
                if (pareto_I)
                    res.Add(ids[i]);
            }
            return res;
        }

    }
}
