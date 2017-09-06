using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;
using static System.Math;

namespace MiracleGun.ElasticPiston {
    public class ElasticPistonBound: GasBound {
        public ElasticPistonConsts ge;
        public ElasticPistonBound(ElasticPistonConsts g) {
            ge = g;
        }
        public override WBVec AUSMp() {
            var fl = base.AUSMp();
            var dX = RightCell.X - LeftCell.X;
            double ul = LeftCell.u;
            double ur = RightCell.u;
            double u = 0.5 * (ul + ur);
            double dudx = (ur - ul) / dX;
            var dSdx = (RightCell.S - LeftCell.S) / dX;

            var ha = 2 * dudx - u * dSdx / S;
            var hh = (0.5 / Sqrt(3d)) * Abs(ha);
            double tauxx;
            if (Abs(hh) > 1E-11) {
                tauxx = (2.0 / 3.0) * (ge.mu + 0.5 * ge.taus / hh) * ha;
            } else {
                tauxx = 0d;
            }
            fl[2] -= tauxx;
            fl[3] -= tauxx * u;
            return fl;
        }
    }
}
