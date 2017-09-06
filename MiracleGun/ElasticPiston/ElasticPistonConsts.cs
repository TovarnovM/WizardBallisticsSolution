using MiracleGun.IdealGas;

namespace MiracleGun.ElasticPiston {
    public class ElasticPistonConsts: GasConstants {
        public double 
            c0 = 2380d, 
            rho0 = 919.03,
            sigmas = 25.2E6, 
            k0 = 0.054, 
            b1 = 0.027, 
            b2 = 0.00675,
            mu = 0.001,
            taus = 1.0E6;
        public ElasticPistonConsts(double gamma):base(gamma,0d) {

        }
    }
}
