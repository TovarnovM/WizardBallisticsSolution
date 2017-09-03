using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class PnGrid2 : PnGrid {
        
        public PnGrid2(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
            lrInit.LeftBorder = new GasBorderWall() {
                OwnerLayer = lrInit,
                BorderPos = WBBorderPos.leftBorder
            };
            lrInit.RightBorder = new GasBorderWall() {
                OwnerLayer = lrInit,
                A_0_func = A_0_function,
                BorderPos = WBBorderPos.rightBorder
            };
        }
        public double A_0_function(double t, IList<GasCell> realCells) {
            return Get_a(realCells);
        }

        public override void StepUpLogic(double deltaTau) {
            var lr0 = (GasLayer)CurrLayer;
            var lr05 = lr0.EulerStep(0.5*deltaTau, lr0);

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
