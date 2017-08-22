using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

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

            double y1 = 0d, y2 = 0d;

            double p1 = nodeL.p;
            double p2 = nodeR.p;
        }

        public static double Pressure()
    }
}
