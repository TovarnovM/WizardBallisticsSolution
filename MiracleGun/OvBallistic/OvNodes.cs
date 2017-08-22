using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {

    public class OvNodes : WBOneDemNode {

        public double u, rho, p, e, z;

        public OvPowder powder;

        public double GetE() {
            return (p / (powder.k - 1)) *
            ((1 / rho) - ((1 - powder.Psi(z)) / powder.dest + powder.alpha_k * powder.Psi(z))) +
            (1 - powder.Psi(z)) * powder.f / (powder.k - 1);
        }

        public double GetP() {
            return ((powder.k - 1) * e - (1 - powder.Psi(z)) * powder.f) /
            (1 / rho - (1 - powder.Psi(z)) / powder.dest - powder.alpha_k * powder.Psi(z));
        }

        public void Init_q() {
            vect_q[1] = rho;
            vect_q[2] = rho * u;
            vect_q[3] = rho * (e + 0.5 * Math.Pow(u, 2));
            vect_q[4] = rho * z;
        }

        public void Synch() {
            e = GetE();
            Init_q();
        }

        public WBVec vect_f = new WBVec(0, 0, 0), vect_q = new WBVec(0, 0, 0);

        public override IWBNode Clone() {
            var cl = (OvNodes)base.Clone();
            cl.vect_q = vect_q.Clone();
            cl.vect_f = vect_f.Clone();
            return cl;
        }
    }
}
