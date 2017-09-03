using MiracleGun.IdealGas;
using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealPiston {
    [SolversFactory]
    public class IdealPistonFabric {
        [SolverGeneratorMethod("IdealPiston по инерции")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts.X_left - 10, 0.2);
            geom.AddPoint(layerOpts.X_right + 0.1, 0.2);
            geom.AddPoint(layerOpts.X_right + 2.4, 0.05);
            geom.AddPoint(layerOpts.X_right + 1000, 0.05);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts, InitIdealPCell, InitIdealPBound);
            var grid = new PistonIdealGrid("IdealP_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            return solver;
        }
        public static WBOneDemLayerOptions StandartOpts {
            get {
                var lo = new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = 0,
                    X_right = 1,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(300),
                };
                lo.SynchH();
                return lo;
            }
        }
        static IdealPistonConstants gp = new IdealPistonConstants(1.235);
        static double v0 = 700;
        public static PistonIdealCell InitIdealPCell(double t, double x) {
            var answ = new PistonIdealCell(gp);
            answ.ro = 921.0;
            answ.u = v0;
            answ.p = 0;
            answ.X = x;
            answ.V = v0;
            return answ;
        }

        public static PistonIdealBound InitIdealPBound(double t, double x) {
            return new PistonIdealBound() { X = x, V = v0 };
        }
    }
}
