using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {

    public class OvCell : WBOneDemNode {

        public double u, rho, p, e, z;

        public GunPowder powder;

        public GunShape geom;

        public OvBound LeftBound, RightBound;

        public double Get_E() {
            return (p / (powder.k - 1)) *
            ((1 / rho) - ((1 - powder.Psi(z)) / powder.dest + powder.alpha_k * powder.Psi(z))) +
            (1 - powder.Psi(z)) * powder.f / (powder.k - 1);
        }

        public double Get_p() {
            return ((powder.k - 1) * e - (1 - powder.Psi(z)) * powder.f) /
            (1 / rho - (1 - powder.Psi(z)) / powder.dest - powder.alpha_k * powder.Psi(z));
        }

        public double Get_z() {
            return vect_q[4] / vect_q[1];
        }

        public double Get_u() {
            return vect_q[2] / vect_q[1];
        }

        public void Init_q() {
            vect_q[1] = rho;
            vect_q[2] = rho * u;
            vect_q[3] = rho * (e + 0.5 * Math.Pow(u, 2));
            vect_q[4] = rho * z;
        }

        public void Synch() {
            e = Get_E();
            Init_q();
        }

        public void Get_h() {
            vect_h[1] = 0;
            vect_h[2] = p * geom.Get_dS(X);
            vect_h[3] = 0;
            vect_h[4] = vect_q[1] * geom.Square(X) * Math.Pow(p, powder.nu) / powder.Ik;

        }

        public double Csound() {
            var z = Get_z();
            return Math.Sqrt(p / ((1 / powder.k) * (1 / rho - (1 - powder.Psi(z)) / powder.dest - powder.alpha_k * powder.Psi(z)))) / rho;
        }

        public WBVec vect_f = new WBVec(0, 0, 0, 0), vect_q = new WBVec(0, 0, 0, 0), vect_h = new WBVec(0, 0, 0, 0);

        public override IWBNode Clone() {
            var cl = (OvCell)base.Clone();
            cl.vect_q = vect_q.Clone();
            cl.vect_f = vect_f.Clone();
            cl.vect_h = vect_h.Clone();
            return cl;
        }
       
    }
}
