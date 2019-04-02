using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealPiston {
    public class PistonGrid : WBGridBase {
        public double m1 = 0.0395, m2 = 0.0219;
       

        public int smooth = 2;
        public PistonGrid(string name, GasLayer initLayer) : base(name, initLayer) {
            initLayer.LeftBorder = new GasBorderWall() {
                OwnerLayer = initLayer,
                A_0_func = A_0_function_L,
                BorderPos = WBBorderPos.leftBorder
            };
            initLayer.RightBorder = new GasBorderWall() {
                OwnerLayer = initLayer,
                A_0_func = A_0_function_R,
                BorderPos = WBBorderPos.rightBorder
            };
        }

        public double A_0_function_L(double t, IList<GasCell> realCells) {
            return Get_a_lft(realCells);
        }

        public double A_0_function_R(double t, IList<GasCell> realCells) {
            return Get_a_rght(realCells);
        }

        public double Get_a_lft(IList<GasCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = (revList[i].p)* revList[i].LeftBound.S / m1;
                a += a;
            }
            return a / smooth;

        }

        public double Get_a_rght(IList<GasCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = (revList[i].p ) * revList[i].RightBound.S / m2;
                a += a;
            }
            return a / smooth;

        }

        public double GetMnozj() {
            if (TimeCurr > 0.001) {
                return 0.3d;
            }
            return 0.01 + TimeCurr / 0.001 * (0.3 - 0.01);
        }
        public override double GetMaxTimeStep() {
            double tau = GetMnozj() * (CurrLayer as GasLayer).GetMaxTimeStep();
            //tau = tau < 1E-6 ? 1E-6 : tau;
            //tau =  Math.Min(0.01, tau);
            return tau;
        }

        public override void InfoСommunication() {
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
        }
    }
}
