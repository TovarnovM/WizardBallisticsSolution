using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {
    public class OvBorderWall : WBEulerBorder<OvCell, OvBound> {
        public override void InitFakeCells() {
            var c_f = FakeCells[0];
            var c = RealCells[0];
            var b = BorderNode;
            c_f.p = c.p;
            c_f.ro = c.ro;
            c_f.u = -c.u + 2 * b.V;
            c_f.z = c.z;
            c_f.Sync();
        }
    }
}
