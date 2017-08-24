using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public abstract class WBOneDemCellLayer<TCell,TBound>: WBOneDemLayer<WBOneDemNode> 
        where TCell : WBOneDemNode 
        where TBound : WBOneDemNode {

        public static int GetNumOfRealNodes(int CellCount) {
            return CellCount * 2 + 1;
        }

        [JsonIgnore]
        public List<TCell> RealCells, RealCellsRev ,LeftCells, RightCells, AllCells, AllCellsRev;

        [JsonIgnore]
        public List<TBound> RealBounds, RealBoundsRev, LeftBounds, RightBounds, AllBounds, AllBoundsRev;

        #region ListGenerators
        public IEnumerable<TCell> GetAllCells() {
            int indexOfFirstCell = Opt.LeftNodesCount % 2 == 0
                ? 1
                : 0;
            for (int i = indexOfFirstCell; i < Nodes.Count; i+=2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetAllBounds() {
            int indexOfFirstBound = Opt.LeftNodesCount % 2 == 0
                ? 0
                : 1;
            for (int i = indexOfFirstBound; i < Nodes.Count; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }

        public IEnumerable<TCell> GetLeftCells() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            for (int i = realBoundsIndexLeft - 1; i >= 0; i-=2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TCell> GetRightCells() {
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount-1;
            for (int i = realBoundsIndexRight +1; i < Nodes.Count; i += 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TCell> GetRealCells() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexLeft + 1; i < realBoundsIndexRight; i += 2) {
                yield return (TCell)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetLeftBounds() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            for (int i = realBoundsIndexLeft - 2; i >= 0; i-=2) {
                yield return (TBound)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetRightBounds() {
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexRight + 2; i < Nodes.Count; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }
        public IEnumerable<TBound> GetRealBounds() {
            int realBoundsIndexLeft = Opt.LeftNodesCount;
            int realBoundsIndexRight = Opt.LeftNodesCount + Opt.RealNodesCount - 1;
            for (int i = realBoundsIndexLeft ; i <= realBoundsIndexRight; i += 2) {
                yield return (TBound)Nodes[i];
            }
        }
        #endregion

        public override void NodeIndexing() {
            int ind = -5 * Opt.LeftNodesCount;
            for (int i = 0; i < Nodes.Count; i++) {
                Nodes[i].IndexInList = i;
                Nodes[i].Index = ind;
                ind += 5;
            }


        }

        public override void InitLists() {
            base.InitLists();
            AllBounds = new List<TBound>(GetAllBounds());
            AllBoundsRev = new List<TBound>(AllBounds.Count);
            for (int i = AllBounds.Count - 1; i >= 0; i--) {
                AllBoundsRev.Add(AllBounds[i]);
            }

            AllCells = new List<TCell>(GetAllCells());
            AllCellsRev = new List<TCell>(AllCells.Count);
            for (int i = AllCells.Count - 1; i >= 0; i--) {
                AllCellsRev.Add(AllCells[i]);
            }

            RealCells = new List<TCell>(GetRealCells());
            RealCellsRev = new List<TCell>(RealCells.Capacity);
            for (int i = RealCells.Count - 1; i >= 0; i--) {
                RealCellsRev.Add(RealCells[i]);
            }

            RealBounds = new List<TBound>(GetRealBounds());

            RealBoundsRev = new List<TBound>(RealBounds.Capacity);
            for (int i = RealBounds.Count - 1; i >= 0; i--) {
                RealBoundsRev.Add(RealBounds[i]);
            }

            LeftCells = new List<TCell>(GetLeftCells());
            LeftBounds = new List<TBound>(GetLeftBounds());

            RightBounds = new List<TBound>(GetRightBounds());
            RightCells = new List<TCell>(GetRightCells());
        }

        public virtual void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, TCell> initCellFunc, Func<double, double, TBound> initBoundFunc) {
            Opt = opts;
            Time = time;
            Nodes?.Clear();
            int indexOfFirstBound = Opt.LeftNodesCount % 2 == 0
                    ? 1
                    : 0;
            var nds = Enumerable.Range(0, Opt.AllNodesCount)
                .Select(ind => {
                    double x = Opt.X_left + ind * Opt.H;
                    WBOneDemNode nd = ind % 2 == indexOfFirstBound
                      ? (WBOneDemNode)initCellFunc(Time, x)
                      : (WBOneDemNode)initBoundFunc(Time, x);
                    nd.X = x;
                    return nd;
                });
            Nodes.AddRange(nds);

            InitLists();
            NodeIndexing();

            InitBoundCellRefs();
            InitDataRefs();
        }

        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, WBOneDemNode> initCondFunc) {
            base.InitLayer(time, opts, initCondFunc);
            InitBoundCellRefs();
            InitDataRefs();
        }

        public class CellBoundNeibs {
            public TCell cell;
            public TBound leftB, rightB;
        }

        public abstract void InitBoundCellRefs();
        public abstract void InitDataRefs();

        public IEnumerable<CellBoundNeibs> GetCellBoundNeibs() {
            int indexOfFirstCell = Opt.LeftNodesCount % 2 == 0
                ? 1
                : 0;
            int indShifter = 0;
            switch (indexOfFirstCell) {
                case 0:
                    yield return new CellBoundNeibs() {
                        cell = AllCells[0],
                        leftB = null,
                        rightB = AllBounds[0]
                    };
                    indShifter = 1;
                    break;
                default:
                    yield return new CellBoundNeibs() {
                        cell = AllCells[0],
                        leftB = AllBounds[0],
                        rightB = AllBounds[1]
                    };
                    break;
            }

            for (int i = 1; i < AllCells.Count - 1; i++) {
                yield return new CellBoundNeibs() {
                    cell = AllCells[i],
                    leftB = AllBounds[i- indShifter],
                    rightB = AllBounds[i+1- indShifter]
                };
            }

            bool cellLast = AllCells.Count == (AllBounds.Count + indShifter);
            if (cellLast) {
                yield return new CellBoundNeibs() {
                    cell = AllCellsRev[0],
                    leftB = AllBoundsRev[0],
                    rightB = null
                };
            } else {
                yield return new CellBoundNeibs() {
                    cell = AllCellsRev[0],
                    leftB = AllBoundsRev[1],
                    rightB = AllBoundsRev[0]
                };
            }


        }

        public override void ActionWhenLoad() {
            base.ActionWhenLoad();
            InitBoundCellRefs();
        }

        public override void CloneLogic(IWBNodeLayer clone) {
            base.CloneLogic(clone);
            (clone as WBOneDemCellLayer<TCell, TBound>).InitBoundCellRefs();
        }


    }
}
