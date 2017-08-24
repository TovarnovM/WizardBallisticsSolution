using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {

    public class OvLayer : WBOneDemCellLayer<OvCell,OvBound> {
        public OvGunShape geom;
        public OvPowder powder;

        public void InitBoundCellRefs() {
            int indexOfFirstCell = Opt.LeftNodesCount % 2 == 0
                ? 1
                : 0;
            int indShifter = 0;
            switch (indexOfFirstCell) {
                case 0:
                    AllCells[0].LeftBound = null;
                    AllCells[0].RightBound = AllBounds[0];
                    AllBounds[0].LeftCell = AllCells[0];
                    indShifter = 1;
                    break;
                default:
                    AllBounds[0].LeftCell = null;
                    break;
            }

            for (int i = indShifter; i < AllCells.Count-1; i++) {
                AllCells[i].LeftBound = AllBounds[i- indShifter];
                AllBounds[i- indShifter].RightCell = AllCells[i];

                AllCells[i].RightBound = AllBounds[i + 1- indShifter];
                AllBounds[i + 1- indShifter].LeftCell = AllCells[i];
            }

            bool cellLast = AllCells.Count == (AllBounds.Count + indShifter);
            if (cellLast) {
                AllCellsRev[0].RightBound = null;
                AllCellsRev[0].LeftBound = AllBoundsRev[0];
                AllBoundsRev[0].RightCell = AllCellsRev[0];
            } else {
                AllCellsRev[0].LeftBound = AllBoundsRev[1];
                AllBoundsRev[1].RightCell = AllCellsRev[0];

                AllCellsRev[0].RightBound = AllBoundsRev[0];
                AllBoundsRev[0].LeftCell = AllCellsRev[0];

                AllBoundsRev[0].RightCell = null;
            }


        }
        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, WBOneDemNode> initCondFunc) {
            base.InitLayer(time, opts, initCondFunc);
            foreach (var c in AllCells) {
                c.geom = geom;
                c.powder = powder;
            }
            foreach (var b in AllBounds) {
                b.geom = geom;
            }
        }
        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, OvCell> initCellFunc, Func<double, double, OvBound> initBoundFunc) {
            base.InitLayer(time, opts, initCellFunc, initBoundFunc);
            foreach (var c in AllCells) {
                c.geom = geom;
                c.powder = powder;
            }
            foreach (var b in AllBounds) {
                b.geom = geom;
            }
        }
        public override void ActionWhenLoad() {
            base.ActionWhenLoad();
            InitBoundCellRefs();
        }
        public override void CloneLogic(IWBNodeLayer clone) {
            base.CloneLogic(clone);
            (clone as OvLayer).InitBoundCellRefs();
        }


    }
}
