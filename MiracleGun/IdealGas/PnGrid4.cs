using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class PnGrid4 : PnGrid2 {
        public PnGrid4(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
        }

        public override void StepUpLogic(double deltaTau) {
            var lr1 = (GasLayer)CurrLayer;
            var lr2 = lr1.EulerStep(0.5 * deltaTau, lr1);
            var lr3 = lr1.EulerStep(0.5 * deltaTau, lr2);
            var lr4 = lr1.EulerStep(1.0 * deltaTau, lr3);
            List<ComplexStepContainer<GasCell, GasBound>> rightPart = new List<ComplexStepContainer<GasCell, GasBound>> {
                new ComplexStepContainer<GasCell, GasBound>() {
                    calcFluxes = false,
                    synch = false,
                    multipl = 1d/6d,
                    dyLayer = lr1
                },
                new ComplexStepContainer<GasCell, GasBound>() {
                    calcFluxes = false,
                    synch = false,
                    multipl = 2d/6d,
                    dyLayer = lr2
                },
                new ComplexStepContainer<GasCell, GasBound>() {
                    calcFluxes = false,
                    synch = false,
                    multipl = 2d/6d,
                    dyLayer = lr3
                },
                new ComplexStepContainer<GasCell, GasBound>() {
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
