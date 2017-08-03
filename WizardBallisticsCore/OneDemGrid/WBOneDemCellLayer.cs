using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.Interfaces;

namespace WizardBallisticsCore.OneDemGrid {
    public class WBOneDemCellLayer<T> : WBNodeLayerBase where T : struct {
        WBOneDemCell<T> Cast(IWBNode node) {
            return (WBOneDemCell<T>)node;
        }
        public override void CloneLogic() {
            if (Nodes.Count < 2)
                return;
            var leftC = Cast(Nodes[0]);
            var rightC = leftC;
            leftC.LeftCell = null;
            
            for (int i = 1; i < Nodes.Count; i++) {
                rightC = Cast(Nodes[i]);
                leftC.RightCell = rightC;
                rightC.LeftCell = leftC;
                leftC = rightC;
            }

            rightC.RightCell = null;
        }

        public void SynchXAndVel() {

        }
    }
}
