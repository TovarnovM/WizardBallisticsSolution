using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.IdealPiston {
    public class PistonIdealCell : GasCell {
        public IdealPistonConstants pg;
        public PistonIdealCell(IdealPistonConstants g) : base(g) {
            pg = g;
        }
        public override double GetCSound() {
            var Ro = ro / pg.rho0;
            if (Ro < 1d)
                Ro = 1d;
            return Math.Sqrt(pg.B * pg.rho0 * (ro - pg.Ck * (2.0 * ro - pg.rho0)) / Math.Pow(ro - pg.Ck * pg.rho0,3.0));
        }
        public override double GetPressure() {
            var Ro = ro / pg.rho0;
            return ro > pg.rho0 
                ? pg.B * Ro * (Ro - 1d) / ((pg.Ck - Ro) * (pg.Ck - Ro)) 
                : 0d;
        }
        public override double GetE() {
            return (p / g[9]) * (1d / ro);
        }
    }

    public class IdealPistonConstants : GasConstants {
        public double B = 1.19E9, Ck = 1.73, rho0 = 920d;
        public IdealPistonConstants(double gamma) : base(gamma, 0d) {
        }
    }
}
