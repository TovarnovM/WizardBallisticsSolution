using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using static System.Math;

namespace MiracleGun.IdealGas {
    public class GasLayer : WBOneDemCellLayer<GasCell, GasBound> {
        public GunShape Geom { get; set; }
        public override void InitBoundCellRefs() {
            foreach (var tpl in GetCellBoundNeibs()) {
                tpl.cell.LeftBound = tpl.leftB;
                if (tpl.leftB != null)
                    tpl.leftB.RightCell = tpl.cell;

                tpl.cell.RightBound = tpl.rightB;
                if (tpl.rightB != null)
                    tpl.rightB.LeftCell = tpl.cell;

            }
        }

        public override void InitDataRefs() {
            foreach (var c in AllCells) {
                c.Geom = Geom;
            }
            foreach (var b in AllBounds) {
                b.Geom = Geom;
            }
        }

        public double GetMaxTimeStep() {
            double vmax = 0d;
            foreach (var cell in RealCells) {
                var vel = cell.CSound + Abs(cell.q[2]) / cell.q[1];
                if (vel > vmax)
                    vmax = vel;
            }
            vmax = Max(vmax, Max(Abs(RealBounds[0].V), Abs(RealBoundsRev[0].V)));
            return RealCells[0].dx / vmax;
        }

        public void InitBoundaryCells_wall() {
            ////Согласно godstep.f90 строка 172
            //var ql = RealCells[0].q;
            //var qr = ql;
            //ql[2] = -qr[2] + 2 * RealBounds[0].V * qr[1];

            //LeftCells[0].SetQ(ql);

            //qr = RealCellsRev[0].q;
            //ql = qr;
            ////Однако в godstep.f90 строка 204 вместо последней ql[1] стоит qr[1].... странно, возможно это ошибка при копировании, хотя это и не важно
            //qr[2] = -ql[2] + 2 * RealBoundsRev[0].V * ql[1];

            //RightCells[0].SetQ(qr);

            LeftCells[0].p = RealCells[0].p;
            LeftCells[0].ro = RealCells[0].ro;
            LeftCells[0].u = -RealCells[0].u + 2 * RealBounds[0].V;
            LeftCells[0].Sync();

            RightCells[0].p = RealCellsRev[0].p;
            RightCells[0].ro = RealCellsRev[0].ro;
            RightCells[0].u = -RealCellsRev[0].u + 2 * RealBoundsRev[0].V;
            RightCells[0].Sync();

        }

        public void StrechMe(double deltaT) {
            Time += deltaT;
            RealNodes[0].X += RealNodes[0].V * deltaT;
            RealNodesRev[0].X += RealNodesRev[0].V * deltaT;
            SynchNodes_X_V();
        }

        public GasLayer EulerStepUp(double tau) {
            var lr0 = this;
            foreach (var c in lr0.RealCells) {
                c.Sync();
            }
            lr0.InitBoundaryCells_wall();

            foreach (var b in lr0.RealBounds) {
                b.SetFlux();
            }

            var lr05 = lr0.Clone() as GasLayer;

            lr05.StrechMe(tau);

            for (int i = 0; i < lr05.RealCells.Count; i++) {
                var c_0 = lr0.RealCells[i];
                var c_05 = lr05.RealCells[i];
                var qs = (c_0.q * c_0.W + tau * c_0.Get_dQS()) / c_05.W;

                c_05.SetQ(qs);
            }
            return lr05;
        }



        public GasLayer StepUp(double tau) {
            var lr0 = this;        
            foreach (var c in lr0.RealCells) {
                c.Sync();
            }
            lr0.InitBoundaryCells_wall();

            foreach (var b in lr0.RealBounds) {
                b.SetFlux();
            }

            var lr05 = lr0.Clone() as GasLayer;
            lr05.StrechMe(tau * 0.5);

            for (int i = 0; i < lr05.RealCells.Count; i++) {
                var c_0 = lr0.RealCells[i];
                var c_05 = lr05.RealCells[i];
                var qs = (c_0.q * c_0.W + 0.5 * tau * c_0.Get_dQS()) /c_05.W;

                c_05.SetQ(qs);
            }

            lr05.InitBoundaryCells_wall();
            foreach (var b in lr05.RealBounds) {
                b.SetFlux();
            }

            var lr1 = lr05.Clone() as GasLayer;
            lr1.StrechMe(tau * 0.5);

            for (int i = 0; i < lr1.RealCells.Count; i++) {
                var c_0 = lr0.RealCells[i];
                var c_05 = lr05.RealCells[i];
                var c_1 = lr1.RealCells[i];

                var qn = (c_0.q * c_0.W + tau * c_05.Get_dQS()) / c_1.W;

                c_1.SetQ(qn);
            }

            return lr1;
        }

        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, GasCell> initCellFunc, Func<double, double, GasBound> initBoundFunc) {
            base.InitLayer(time, opts, initCellFunc, initBoundFunc);
            foreach (var c in AllCells) {
                c.Sync();
            }
        }
    }
}
