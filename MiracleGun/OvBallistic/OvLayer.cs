using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using static System.Math;

namespace MiracleGun.OvBallistic {
    public class OvLayer : WBOneDemCellLayer<OvCell, OvBound> {
        public GunShape Geom { get; set; }
        public MixtureGunPowder mixture;

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

        public void InitBoundaryCells() {

            LeftCells[0].p = RealCells[0].p;
            LeftCells[0].ro = RealCells[0].ro;
            LeftCells[0].u = -RealCells[0].u + 2 * RealBounds[0].V;
            for (int i = 0; i < mixture.powders.Count; i++){
                LeftCells[0].z[i] = RealCells[0].z[i];
            }
            LeftCells[0].Sync();

            RightCells[0].p = RealCellsRev[0].p;
            RightCells[0].ro = RealCellsRev[0].ro;
            RightCells[0].u = -RealCellsRev[0].u + 2 * RealBoundsRev[0].V;
            for (int i = 0; i < mixture.powders.Count; i++) {
                RightCells[0].z[i] = RealCellsRev[0].z[i];
            }
            RightCells[0].Sync();

        }

        public void StrechMe(double deltaT) {
            Time += deltaT;
            RealNodes[0].X += RealNodes[0].V * deltaT;
            RealNodesRev[0].X += RealNodesRev[0].V * deltaT;
            SynchNodes_X_V();
        }

        public OvLayer EulerStep(double tau, OvLayer dqLayer, bool synchMe = true, bool syncDqLayer = true, bool calcDqLayerFluxes = true) {
            var lr0 = this;
            if (synchMe) {
                foreach (var c in lr0.RealCells) {
                    c.Sync();
                }
                lr0.SetBounds();
            }

            if (calcDqLayerFluxes)
                foreach (var b in dqLayer.RealBounds) {
                    b.SetFlux();
                }

            var lr1 = lr0.Clone() as OvLayer;
            lr1.StrechStep(tau);
            for (int i = 0; i < lr1.RealCells.Count; i++) {
                var c_0 = lr0.RealCells[i];
                var c_n = lr1.RealCells[i];
                var c_dq = dqLayer.RealCells[i];
                var qs = (c_0.q * c_0.W + tau * c_dq.Get_dQS()) / c_n.W;

                c_n.SetQ(qs);
            }
            return lr1;
        }

        public OvLayer StepUp(double tau) {
            var lr0 = this;
            foreach (var c in lr0.RealCells) {
                c.Sync();
            }
            lr0.InitBoundaryCells();

            foreach (var b in lr0.RealBounds) {
                b.SetFlux();
            }

            var lr05 = lr0.Clone() as OvLayer;
            lr05.StrechMe(tau * 0.5);

            for (int i = 0; i < lr05.RealCells.Count; i++) {
                var c_0 = lr0.RealCells[i];
                var c_05 = lr05.RealCells[i];
                var qs = (c_0.q * c_0.W + 0.5 * tau * c_0.Get_dQS()) / c_05.W;

                c_05.SetQ(qs);
            }

            lr05.InitBoundaryCells();
            foreach (var b in lr05.RealBounds) {
                b.SetFlux();
            }

            var lr1 = lr05.Clone() as OvLayer;
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

        public override void InitLayer(double time, WBOneDemLayerOptions opts, Func<double, double, OvCell> initCellFunc, Func<double, double, OvBound> initBoundFunc) {
            base.InitLayer(time, opts, initCellFunc, initBoundFunc);
            foreach (var c in AllCells) {
                c.Sync();
            }
        }



        public override WBOneDemCellLayer<OvCell, OvBound> ComplexStep(double tau, bool synch, IList<ComplexStepContainer<OvCell, OvBound>> rightPart) {

            if (synch) {
                SetBounds();
                foreach (var c in RealCells) {
                    c.Sync();
                }
            }

            var res = (OvLayer)Clone();
            res.Time += tau;
            foreach (var tp in rightPart) {
                res.RightBorder.X_0 += tp.dyLayer.RightBorder.V_0 * tp.multipl * tau;
                res.LeftBorder.X_0 += tp.dyLayer.LeftBorder.V_0 * tp.multipl * tau;

                res.RightBorder.V_0 += tp.dyLayer.RightBorder.A_0 * tp.multipl * tau;
                res.LeftBorder.V_0 += tp.dyLayer.LeftBorder.A_0 * tp.multipl * tau;
            }

            res.SynchNodes_X_V();

            foreach (var tp in rightPart) {
                if (tp.synch) {
                    foreach (var c in tp.dyLayer.RealCells) {
                        c.Sync();
                    }
                }
                if (tp.calcFluxes) {
                    foreach (var b in tp.dyLayer.RealBounds) {
                        b.SetFlux();
                    }
                }
            }

            for (int i = 0; i < RealCells.Count; i++) {
                var c_0 = RealCells[i];
                var qn = c_0.q * c_0.W;
                foreach (var tp in rightPart) {
                    var c_rp = tp.dyLayer.RealCells[i];
                    qn += tau * tp.multipl * c_rp.Get_dQS();
                }
                qn /= res.RealCells[i].W;
                res.RealCells[i].SetQ(qn);
            }

            return res;
        }
    }
}
