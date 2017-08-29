using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    public abstract class WBOneDemGrid<T> : WBGridBase where T : WBOneDemNode {
        public WBOneDemGrid(string name, WBOneDemLayer<T> initLayer) : base(name, initLayer) {
        }

        public override void StepUpLogic(double deltaTau) {
            throw new NotImplementedException();
        }
    }

    public class WBBoundaryCond {

    }
}
