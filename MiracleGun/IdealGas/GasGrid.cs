using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class GasGrid : WBGridBase {
        public GasGrid(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
        }

        public override double GetMaxTimeStep() {
            double tau =  0.4*(CurrLayer as GasLayer).GetMaxTimeStep();
            tau =  Math.Min(0.001, tau);
            return tau;
        }

        public override void InfoСommunication() {
        }

        public override void StepUpLogic(double deltaTau) {
            var lrN1 = ((GasLayer)CurrLayer).StepUp(deltaTau);
            AddLayer(lrN1);
        }
    }
}
