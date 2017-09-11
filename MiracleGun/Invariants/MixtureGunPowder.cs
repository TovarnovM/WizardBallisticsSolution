using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.Invariants {
    public class MixtureGunPowder : GunPowder {
        public List<GunPowder> powders;
        public List<double> conc;

        public MixtureGunPowder(List<GunPowder> powderList, List<double> concList) {
            powders = powderList;
            conc = concList;
            GetMixture();
        }

        public void GetMixture() {
            this.f = 0;
            this.alpha_k = 0;
            this.k = 0;
            this.dest = 0;
            for (int i = 0; i < powders.Count; i++) {
                this.f += conc[i] * powders[i].f;
                this.alpha_k += conc[i] * powders[i].alpha_k;
                this.k += conc[i] * powders[i].k;
                this.dest += conc[i] * powders[i].dest;
            }
        }
    }
}
