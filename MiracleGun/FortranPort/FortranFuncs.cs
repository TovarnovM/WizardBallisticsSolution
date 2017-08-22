using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using static System.Math;

namespace MiracleGun.FortranPort {
    public static class FortranFuncs {
        /// <summary>
        /// Согласно AUSMp.f90
        /// </summary>
        /// <param name="nodeL"></param>
        /// <param name="nodeR"></param>
        /// <param name="vbi"></param>
        /// <returns>flux</returns>
        public static WBVec AUSMp(GasNode nodeL, GasNode nodeR, double vbi) {
            double r1 = nodeL.ro;
            double u1 = nodeL.u;
            double e1 = nodeL.e;

            double r2 = nodeR.ro;
            double u2 = nodeR.u;
            double e2 = nodeR.e;

            double p1 = nodeL.p;
            double p2 = nodeR.p;

            double H1 = nodeL.H;
            double H2 = nodeR.H;

            double c1 = nodeL.CSound;
            double c2 = nodeR.CSound;

            double cs = 0.5 * (c1 + c2);
            double Mr1 = (u1 - vbi) / cs;
            double Mr2 = (u2 - vbi) / cs;
            double du = u2 - u1;

            double M4p = Abs(Mr1) >= 1d
                ? 0.5 * (Mr1 + Abs(Mr1))
                : 0.25 * ((Mr1 + 1.0) * (Mr1 + 1.0)) * (1.0 + 2.0 * 0.25 * (Mr1 - 1.0) * (Mr1 - 1.0));
            double P5p = Abs(Mr1) >= 1d
                ? 0.5 * (Mr1 + Abs(Mr1)) / Mr1
                : 0.25 * ((Mr1 + 1.0) * (Mr1 + 1.0)) * ((2.0 - Mr1) + 3.0 * Mr1 * 0.25 * (Mr1 - 1.0) * (Mr1 - 1.0));


            double M4m = Abs(Mr2) >= 1d
                ? 0.5 * (Mr2 - Abs(Mr2))
                : -0.25 * ((Mr2 - 1.0) * (Mr2 - 1.0)) * (1.0 + 2.0 * 0.25 * (Mr2 + 1.0) * (Mr2 + 1.0));

            double P5m = Abs(Mr2) >= 1d
                ? 0.5 * (Mr2 - Abs(Mr2)) / Mr2
                : 0.25 * ((Mr2 - 1.0) * (Mr2 - 1.0)) * ((2.0 + Mr2) - 3.0 * Mr2 * 0.25 * (Mr2 + 1.0) * (Mr2 + 1.0));

            double Mrf = M4p + M4m;
            double pf = P5p  * p1 + P5m  * p2;

            double flux1 = 0.5 * (cs * Mrf * (r1 + r2) - cs * Abs(Mrf) * (r2 - r1));
            double flux2 = 0.5 * (cs * Mrf * (r1 * u1 + r2 * u2) - cs * Abs(Mrf) * (r2 * u2 - r1 * u1)) + pf;
            double flux3 = 0.5 * (cs * Mrf * (r1 * H1 + r2 * H2) - cs * Abs(Mrf) * (r2 * H2 - r1 * H1)) + pf * vbi;
            return new WBVec(flux1, flux2, flux3);
        }
        
    }
}
