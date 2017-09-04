using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace MiracleGun.ElasticPiston {
    public class PistonElasticCell : GasCell {
        public ElasticPistonConsts ge;
        public PistonElasticCell(ElasticPistonConsts g) : base(g,3) {
            ge = g;
        }
        public override double GetE() {
            return (p / g[9]) * (1d / ro);
        }
        public override double GetCSound() {
            return Math.Sqrt((ge.k * p + ge.rho0 * ge.c0 * ge.c0) / ro);
        }
        public override double GetPressure() {
            return ro > ge.rho0
                ? (ge.k - 1.0) * ro * e + ge.c0 * ge.c0 * (ro - ge.rho0)
                : 0d;
        }
        public override void Init_h() {
            double sigmaxx = 0d;
            double tauxx;

            double qw = 0d;

            double ul = 0.5 * (LeftBound.LeftCell.u + u);
            double ur = 0.5 * (RightBound.RightCell.u + u);

            double Sr = LeftBound.S, Sl = LeftBound.S;

            double ha = 2.0 * (ur - ul - u * (Sr - Sl) / (Sr + Sl)) / dx;
            double hh = (0.5 / Sqrt(3.0)) * Abs(ha);

            if (hh > 1E-11) {
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
                
        


    //tauxx on the boundaries
    //        ha = 4.0 * (u(nl) - vb0) / dx !производна¤ dS / dx выброшена дл¤ простоты
    //      hh = (0.5 / sqrt(3.0)) * abs(ha)
    //if (hh.ne.0) then
    //    tauxxl(nl) = (2.0 / 3.0) * (mu + 0.5 * taus / hh) * ha
    //else
    //    tauxxl(nl) = 0.0
    //endif
    //do i = nl, nr - 1
    //    ha = (2.0 * (u(i + 1) - u(i)) - 0.25 * Sr(i) * (u(i + 1) + u(i)) * (Sr(i + 1) + Sl(i + 1) - Sr(i) - Sl(i))) / dx
    //    hh = (0.5 / sqrt(3.0)) * abs(ha)
    //    if (hh.ne.0) then
    //        tauxxr(i) = (2.0 / 3.0) * (mu + 0.5 * taus / hh) * ha
    //    else
    //        tauxxr(i) = 0.
    //    endif
    //    tauxxl(i + 1) = tauxxr(i)
    //enddo
    //ha = 4.0 * (vb(nr) - u(nr)) / dx
    //hh = (0.5 / sqrt(3.0)) * abs(ha)
    //if (hh.ne.0) then
    //    tauxxr(nr) = (2.0 / 3.0) * (mu + 0.5 * taus / hh) * ha
    //else
    //    tauxxr(nr) = 0.
    //endif


    //do i = nl,nr
    //    h(1, i) = 0.
    //    h(2, i) = -(sigmannw(i) * (Sr(i) - Sl(i)) / dx + 2.0 * sqrt(0.5 * (Sr(i) + Sl(i))) * sigmantw(i)) / sqrt(pi)
    //    h(3, i) = 2.0 * sqrt(0.5 * (Sr(i) + Sl(i))) * (sigmantw(i) * u(i) - qw(i)) / Sqrt(pi)
    //enddo


        }
    }
}
