using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;
using MiracleGun.IdealGas;

namespace MiracleGun.OvBallistic {

    public class OvCell : GasCell {

        public double[] z;

        public MixtureGunPowder mixture;


        public OvCell(GasConstants g, MixtureGunPowder mixture) : base(g, 3 + mixture.powders.Count) {
            this.mixture = mixture;
            z = new double[mixture.powders.Count];
        }

        public override IWBNode Clone() {
            var cl = base.Clone() as OvCell;
            Array.Copy(z, cl.z, z.Length);
            return cl;
        }

        public override double GetE() {
            var sumPsi = 0d;
            for (int i = 0; i < mixture.powders.Count; i++) {
                sumPsi += mixture.conc[i] * mixture.powders[i].Psi(z[i]);
            }
            return (p / (mixture.k - 1) * (1 / ro - (1 - sumPsi) / mixture.dest - mixture.alpha_k * sumPsi)) + (1 - sumPsi) * mixture.f / (mixture.k - 1);
        }
        public override double GetPressure() {
            var sumPsi = 0d;
            for (int i = 0; i < mixture.powders.Count; i++) {
                sumPsi += mixture.conc[i] * mixture.powders[i].Psi(z[i]);
            }
            return (e * (mixture.k - 1) - (1 - sumPsi) * mixture.f) / (1 / ro - (1 - sumPsi) / mixture.dest - mixture.alpha_k * sumPsi);
        }
        public override void InitQ() {
            q[1] = ro;
            q[2] = ro * u;
            q[3] = ro * (e + 0.5 * u * u);

            for (int i = 0; i < mixture.powders.Count; i++) {
                if (q[i + 4] / q[1] > mixture.powders[i].zk) {
                    q[i + 4] = q[1] * mixture.powders[i].zk;
                }
                else {
                   q[i + 4] = q[1] * z[i];
                }
            }
        }
        public override void Init_h() {
            h[1] = 0d;
            h[2] = p * Geom.Get_dS(X);
            h[3] = 0d;

            for (int i = 0; i < mixture.powders.Count; i++) {
                h[i + 4] = q[1] * Geom.Square(X) * p / mixture.powders[i].Ik;
            }

        }
        public override void SetQ(WBVec q) {
            this.q = q;
            ro = q[1];
            u = q[2] / ro;
            e = q[3] / ro - 0.5 * u * u;
            p = GetPressure();

            for (int i = 0; i < mixture.powders.Count; i++) {
                if (q[i + 4] > mixture.powders[i].zk * ro) {
                    z[i] = mixture.powders[i].zk;
                }
                else {
                  z[i] = q[i + 4] / ro;
                }
            }
            Init_h();
        }

        public override double GetCSound() {
            var sumPsi = 0d;
            for (int i = 0; i < mixture.powders.Count; i++) {
                sumPsi += mixture.conc[i] * mixture.powders[i].Psi(z[i]);
            }
            return Math.Sqrt(p * mixture.k / (1 / ro - (1 - sumPsi) / mixture.dest - mixture.alpha_k * sumPsi)) / ro;
        }
    }
}
