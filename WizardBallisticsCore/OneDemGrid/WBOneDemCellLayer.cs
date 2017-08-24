using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public class WBOneDemCellLayer<TCell,TBound>: WBOneDemLayer<WBOneDemNode> 
        where TCell : WBOneDemNode 
        where TBound : WBOneDemNode {

        [JsonIgnore]
        public List<TCell> RealCells, RealCellsRev ,LeftCells, RightCells;

        [JsonIgnore]
        public List<TBound> RealBounds, RealBoundsRev, LeftBounds, RightBounds;

        #region ListGenerators

        
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
    }
}
