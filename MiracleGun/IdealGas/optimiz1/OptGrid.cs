using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas.optimiz1 {
    public class OptGrid : WBGridBase {
        public GasLayer lrInit;
        public double m = 0.5;
        public OptGrid(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
            lrInit = initLayer as GasLayer;
            lrInit.LeftBorder = new GasBorderWall() {
                OwnerLayer = lrInit,
                BorderPos = WBBorderPos.leftBorder
            };
            lrInit.RightBorder = new GasBorderWall() {
                OwnerLayer = lrInit,
                A_0_func = A_0_function,
                BorderPos = WBBorderPos.rightBorder
            };
            kinE.Add((0d, 0d));
            AddStopFunc(StopF_My);
        }
        public List<(double t, double ke)> kinE = new List<(double t, double ke)>();
        public double InnerEnergy0;
        public double GetKinE() {
            var cl = (GasLayer)CurrLayer;
            var v = cl.RealBoundsRev[0].V;
            var kinEnergy = m * v * v * 0.5;
            return kinEnergy;
        }
        public bool StopF_My() {
            int l = kinE.Count;
            if (l < 2)
                return false;
            return kinE[l - 1].ke - kinE[l - 2].ke < -1E-10;

        }

        public double GetMnozj() {
            if (TimeCurr > 0.01) {
                return 0.3d;
            }
            return 0.01 + TimeCurr / 0.01 * (0.3 - 0.01);
        }
        public override double GetMaxTimeStep() {
            double tau = GetMnozj() * (CurrLayer as GasLayer).GetMaxTimeStep();
            //tau = tau < 1E-6 ? 1E-6 : tau;
            //tau =  Math.Min(0.01, tau);
            return tau;
        }

        public override void InfoСommunication() {
            
        }


        public double A_0_function(double t, IList<GasCell> realCells) {
            return Get_a(realCells);
        }
        public double Get_a(IList<GasCell> revList) {
            double a = 0;
            int smooth = 2;
            for (int i = 0; i < smooth; i++) {
                a = (revList[i].p - 1e5) * revList[i].RightBound.S / m;
                a += a;
            }
            return a / smooth;

        }
        public override void StepUpLogic(double deltaTau) {
            var lr0 = (GasLayer)CurrLayer;
            var lr05 = lr0.EulerStep(0.5 * deltaTau, lr0);

            List<ComplexStepContainer<GasCell, GasBound>> rightPart = new List<ComplexStepContainer<GasCell, GasBound>> {
                new ComplexStepContainer<GasCell, GasBound>() {
                    calcFluxes = true,
                    synch = true,
                    multipl = 1d,
                    dyLayer = lr05
                }
            };

            var lrr = lr0.ComplexStep(deltaTau, false, rightPart);
            AddLayer(lrr);
            kinE.Add((lrr.Time,GetKinE()));
        }
    }
}
