using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.ElasticPiston {
    public class ElasticPistonConsts_shockEOS : ElasticPistonConsts {
        public double Grun = 1.55,
            C_1 = 2.486000e+003,
            S_1 = 1.577000,
            p0 = 1e5;

        public ElasticPistonConsts_shockEOS(double gamma) : base(gamma) {
            rho0 = 1.05e3;
        }
    }

    public class ElasticPistonCell_shockEOS : ElasticPistonCell {
        public ElasticPistonConsts_shockEOS g_shock;
        public ElasticPistonCell_shockEOS(ElasticPistonConsts_shockEOS g) : base(g) {
            g_shock = g;
        }
        public override double GetPressure() {
            var mu = ro / g_shock.rho0;
            var p_h = g_shock.rho0 * g_shock.c0 * g_shock.c0 * mu * (1 + mu) / Math.Pow(1 - (g_shock.S_1 - 1) * mu, 2);
             var e_h = 0.5 * p_h / g_shock.p0 * mu / (1 + mu);
            return p_h + g_shock.Grun * ro * (e - e_h);
        }
        public override double GetE() {
            var mu = ro / g_shock.rho0;
            var p_h = g_shock.rho0 * g_shock.c0 * g_shock.c0 * mu * (1 + mu) / Math.Pow(1 - (g_shock.S_1 - 1) * mu, 2);
            var e_h = 0.5 * p_h / g_shock.p0 * mu / (1 + mu);
            return e_h + (p - p_h) / (g_shock.Grun * ro);
        }
    }
}
