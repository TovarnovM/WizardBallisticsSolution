using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using MiracleGun.IdealGas;

namespace MiracleGun.OvBallistic {

    public class OvGridRK : WBGridBase {

        public double m = 43.359;

        public OvLayer lrInit;

        public int smooth = 2;

        public OvGridRK(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
            lrInit = initLayer as OvLayer;
            lrInit.LeftBorder = new OvBorderWall() {
                OwnerLayer = lrInit,
                BorderPos = WBBorderPos.leftBorder
            };
            lrInit.RightBorder = new OvBorderWall() {
                OwnerLayer = lrInit,
                A_0_func = A_0_function,
                BorderPos = WBBorderPos.rightBorder
            };
        }
        public double A_0_function(double t, IList<OvCell> realCells) {
            return Get_a(realCells);
        }

        public double Get_a(IList<OvCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = revList[i].p * revList[i].RightBound.S / m;
                a += a;
            }
            return a / smooth;

        }

        public override void InfoСommunication() {
        }

        public override double GetMaxTimeStep() {
            //double tau = GetMnozj() * (CurrLayer as OvLayer).GetMaxTimeStep();
            double tau = 0.5 * (CurrLayer as OvLayer).GetMaxTimeStep();
            return tau;
        }

        public double GetMnozj() {
            if (TimeCurr > 0.01) {
                return 0.3d;
            }
            return 0.01 + TimeCurr / 0.01 * (0.3 - 0.01);
        }

        public override void StepUpLogic(double deltaTau) {
            var lr1 = (OvLayer)CurrLayer;
            var lr2 = lr1.EulerStep(0.5 * deltaTau, lr1);
            var lr3 = lr1.EulerStep(0.5 * deltaTau, lr2);
            var lr4 = lr1.EulerStep(1.0 * deltaTau, lr3);
            List<ComplexStepContainer<OvCell, OvBound>> rightPart = new List<ComplexStepContainer<OvCell, OvBound>> {
                    new ComplexStepContainer<OvCell, OvBound>() {
                        calcFluxes = false,
                        synch = false,
                        multipl = 1d/6d,
                        dyLayer = lr1
                    },
                    new ComplexStepContainer<OvCell, OvBound>() {
                        calcFluxes = false,
                        synch = false,
                        multipl = 2d/6d,
                        dyLayer = lr2
                    },
                    new ComplexStepContainer<OvCell, OvBound>() {
                        calcFluxes = false,
                        synch = false,
                        multipl = 2d/6d,
                        dyLayer = lr3
                    },
                    new ComplexStepContainer<OvCell, OvBound>() {
                        calcFluxes = true,
                        synch = true,
                        multipl = 1d/6d,
                        dyLayer = lr4
                    }
                };
            var lrr = lr1.ComplexStep(deltaTau, false, rightPart);
            AddLayer(lrr);
        }
    }
}
