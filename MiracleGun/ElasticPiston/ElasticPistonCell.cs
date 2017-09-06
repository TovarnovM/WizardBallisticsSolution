using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using static System.Math;

namespace MiracleGun.ElasticPiston {
    public class ElasticPistonCell : GasCell {
        public ElasticPistonConsts ge;
        public ElasticPistonCell(ElasticPistonConsts g) : base(g,3) {
            ge = g;
        }
        public override double GetE() {
            return (p - ge.c0 * ge.c0 * (ro - ge.rho0)) / (ge[9] * ro);
        }
        public override double GetCSound() {
            return Math.Sqrt((ge[0] * p + ge.rho0 * ge.c0 * ge.c0) / ro);
        }
        public override double GetPressure() {
            return ro > ge.rho0
                ? (ge[9]) * ro * e + ge.c0 * ge.c0 * (ro - ge.rho0)
                : 0d;
        }
        public override void Init_h() {
            double sigmaxx = 0d;
            double tauxx;

            double qw = 0d;

            double ul = 0.5 * (LeftBound.LeftCell.u + u);
            double ur = 0.5 * (RightBound.RightCell.u + u);
            double dudx = (ur - ul) / dx;

            double Sr = LeftBound.S,
                Sl = LeftBound.S,
                Ssr = 0.5 * (Sr + Sl),
                dSdx = (Sr-Sl)/dx;

            double ha = 2d * dudx - u * dSdx / Ssr;//2.0 * (ur - ul - u * (Sr - Sl) / (Sr + Sl)) / dx;
            double hh = (0.5 / Sqrt(3.0)) * Abs(ha);

            if (Abs(hh) > 1E-11) {
                tauxx = (2.0 / 3.0) * (ge.mu + 0.5 * ge.taus / hh) * ha;
            } else {
                tauxx = 0d;
            }

            sigmaxx = -p + tauxx;
            double sigmannw = sigmaxx - 1.5 * tauxx,
                sigmantw;

            if (p < ge.sigmas) {
                sigmantw = -p * ge.k0 * (1d + ge.b1 * u) * Exp(-ge.b2 * u) * Sign(q[2]); //p - "давление осевого сжати¤"
            } else {
                sigmantw = -ge.sigmas * ge.k0 * (1d + ge.b1 * u) * Exp(-ge.b2 * u) * Sign(q[2]);
            }

            double Dsred = 0.5 * (RightBound.D + LeftBound.D);

            h[1] = 0d;
            h[2] = PI * Dsred * sigmantw - sigmannw * dSdx;
            h[3] = PI * Dsred * (sigmantw * u - qw);
        }
    }
}
