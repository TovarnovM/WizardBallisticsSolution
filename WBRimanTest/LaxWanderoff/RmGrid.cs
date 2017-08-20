using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace WBRimanTest {
    public class RmGrid : WBGridBase {
        public RmGrid(string name, RmLayer initLayer) : base(name, initLayer) {
        }
        public double GetMnozj() {
            if(TimeCurr> 0.1) {
                return 0.7d;
            }
            return 0.05+ TimeCurr/0.1*(0.7-0.05);
        }
        public override double GetMaxTimeStep() {
            return GetMnozj() * ((RmLayer)CurrLayer).GetTauMax();
        }

        public override void InfoСommunication() {
            
        }

        public override void StepUpLogic(double deltaTau) {
            var lrN1 = ((RmLayer)CurrLayer).StepUp(deltaTau);
            AddLayer(lrN1);
        }
    }
}
