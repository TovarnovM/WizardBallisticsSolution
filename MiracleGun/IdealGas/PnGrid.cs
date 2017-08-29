using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {
    public class PnGrid : WBGridBase {

        public double BoundVel = 0d;

        public double m = 0.5;

        public GasLayer lrInit;

        public int smooth = 2;

        public PnGrid(string name, IWBNodeLayer initLayer) : base(name, initLayer) {
            lrInit = initLayer as GasLayer;
        }
        public double GetMnozj() {
            if (TimeCurr > 0.01) {
                return 0.5d;
            }
            return 0.05 + TimeCurr / 0.01 * (0.5 - 0.05);
        }
        public override double GetMaxTimeStep() {
            double tau = GetMnozj() * (CurrLayer as GasLayer).GetMaxTimeStep();
            //tau = tau < 1E-6 ? 1E-6 : tau;
            //tau =  Math.Min(0.01, tau);
            return tau;
        }

        public override void InfoСommunication() {
        }

        public override void StepUpLogic(double deltaTau) {
            var lrN0 = (GasLayer)CurrLayer;
            lrN0.RealBoundsRev[0].V = BoundVel;
            lrN0.SynchNodes_X_V();
            var lrN1 = lrN0.StepUp(deltaTau);
            var c_0 = lrN0.RealCellsRev[0];
            var c_1 = lrN1.RealCellsRev[0];
            var dV = deltaTau * (Get_a(lrN0.RealCellsRev) + Get_a(lrN1.RealCellsRev)) / 2;
            //var dV = deltaTau * Get_a(lrN1.RealCellsRev);
            BoundVel += dV;
            AddLayer(lrN1);
        }

        double Get_a(List<GasCell> revList) {
            double a = 0;
            for (int i = 0; i < smooth; i++) {
                a = revList[i].p * revList[i].RightBound.S / m;
                a += a;
            }
            return a / smooth;

        }

        public double Get_Vanal(double time) {
            var l = lrInit.RealCellsRev[0];
            //   var a0 = l.p * l.RightBound.S / m;
            var c0 = l.CSound;
            return  ((2 * c0) / (l.g[9])) * 
                (1 - Math.Pow(((l.RightBound.S * l.p * time) * (l.g[0] + 1)  / (2 *c0 * m) + 1) , -l.g[6]));
        }
    }
}
