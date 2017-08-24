using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {

    class OvLayer : WBOneDemLayer<OvCell> {

        public OvCell node;

        double vmax; 

        public double TimeStep() {
            var cs = node.Csound();
            var vel = cs + Math.Abs(node.Get_u());
            vmax = 0;
            if (vmax < vel) {
                vmax = vel;
            }
            return vmax;
        }

        public void Mboundary() {

        }
    }
}
