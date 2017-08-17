using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.OneDemGrid;
using static System.Math;

namespace WBRimanTest {
    public class RmLayer : WBOneDemLayer<RmNode> {
        public double GetTauMax() {
            double maxVal = -1;
            foreach (var nd in RealNodes) {
                var s = nd.s;
                var k = RmNode.k;
                var s2divs1 = s[2] / s[1];
                var currVal = Abs(s2divs1) + Sqrt(k * (k - 1) * (s[3] / s[1] - 0.5 * s2divs1 * s2divs1));
                if (currVal > maxVal)
                    maxVal = currVal;
            }
            return Opt.H / maxVal;
        }
        

        public void InitGrUslov_Wall() {
            var s = RealNodes[1];
            LeftNodes[0].p = s.p;
            LeftNodes[0].u= -s.u;
            LeftNodes[0].ro = s.ro;
            LeftNodes[0].Synch();

            s = RealNodes[RealNodes.Count-2];
            RightNodes[0].p = s.p;
            RightNodes[0].u = -s.u;
            RightNodes[0].ro = s.ro;
            RightNodes[0].Synch();
        }


        public RmLayer StepUp(double tau) {
            InitGrUslov_Wall();
            var nxtLayer = (RmLayer)Clone();
            nxtLayer.Time = Time + tau;

            var s12 = new WBVec[Opt.RealNodesCount + 1];
            for (int n = 0; n < Nodes.Count -1; n++) {
                s12[n] = 0.5 * (Nodes[n].s + Nodes[n + 1].s) - tau / 2 / Opt.H * (Nodes[n + 1].f - Nodes[n].f);
            }

            for (int n = Opt.LeftNodesCount; n < nxtLayer.Nodes.Count - Opt.RightNodesCount; n++) {
                var s_k1 = Nodes[n].s - tau / Opt.H * (RmNode.F_func(s12[n]) - RmNode.F_func(s12[n - 1])) + RmNode.vyaz*(Nodes[n+1].s - 2d* Nodes[n].s+ Nodes[n-1].s);
                nxtLayer.Nodes[n].F(s_k1);
            }
            return nxtLayer;
        }
    }
}
