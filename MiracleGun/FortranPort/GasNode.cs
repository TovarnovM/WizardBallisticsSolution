using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.FortranPort {
    /// <summary>
    /// 
    /// </summary>
    public class GasNode:WBOneDemNode {
        public double u;
        /// <summary>
        /// d(i)  Согласно constants.f90  
        /// </summary>
        public double ro;
        public double p;
        public double e;
        public GasConstants g;
        public WBVec q = new WBVec(0, 0, 0);
        public double GetE() {
            return (p / g[9]) * (1 / ro - g.covolume);
        }
        public double GetPressure() {
            return g[9]*e *(1 / ro - g.covolume);
        }
        public void InitQ() {
            q[1] = ro;
            q[2] = ro * u;
            q[3] = ro *(e + 0.5 * u * u);
        }
        public void SetQ(WBVec q) {
            this.q = q;
            ro = q[1];
            u = q[2] / ro;
            e = q[3] / ro - 0.5 * u * u;
            p = GetPressure();
        }

    }

    /// <summary>
    /// Согласно constants.f90
    /// </summary>
    public class GasConstants {
        public double[] g = new double[9];
        public double gamma;
        /// <summary>
        /// Согласно constants.f90   c(1,8)
        /// </summary>
        public double covolume = 0.001027d; 
        public GasConstants(double gamma) {
            SynchArr(gamma);
        }
        public void SynchArr(double gamma) {
            this.gamma = gamma;
            g[1] = 0.5 * (gamma - 1d) / gamma;
            g[2] = 0.5 * (gamma + 1d) / gamma;
            g[3] = 2d* gamma / (gamma - 1d);
            g[4] = 2d/ (gamma - 1d);
            g[5] = 2d/ (gamma + 1d);
            g[6]= (gamma - 1d) / (gamma + 1d);
            g[7] = 0.5 * (gamma - 1d);
            g[8] = 1d/ gamma;
            g[9] = gamma - 1d;
        }
        public double this[int ind] {
            get {
                return g[ind];
            }
        }
    }
}
