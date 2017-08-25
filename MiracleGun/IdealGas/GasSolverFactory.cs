using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {

    [SolversFactory]
    public class GasSolverFactory {
        [SolverGeneratorMethod("IdealGasTest1")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var initLayer = new GasLayer();
            initLayer.InitLayer(0d, layerOpts, InitGasCell, InitGasBound);
            var grid = new GasGrid("GasGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            return solver;
        }

        public static WBOneDemLayerOptions StandartOpts {
            get {
                var lo =  new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = -1,
                    X_right = 1,
                    RealNodesCount = 200,                  
                };
                lo.SynchH();
                return lo;
            }
        }
        static GasConstants gc = new GasConstants(1.4);
        public static GasCell InitGasCell(double t, double x) {
            var answ = new GasCell(gc);
            if (x <= 0d) {
                answ.ro = 1;
                answ.u = 0;
                answ.p = 1;
                
            } else {
                answ.ro = 0.125;
                answ.u = 0;
                answ.p = 0.1;
            }
            answ.X = x;
            answ.V = 0;
            answ.Sync();
            return answ;
        }
        public static GasBound InitGasBound(double t, double x) {
            return new GasBound();
        }
    }
}
