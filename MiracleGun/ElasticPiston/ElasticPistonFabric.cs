using MiracleGun.IdealGas;
using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.ElasticPiston {
    [SolversFactory]
    public class ElasticPistonFabric {
        static ElasticPistonConsts gp = new ElasticPistonConsts(1.63098);
        static double v0 = 576;

        [SolverGeneratorMethod("ElasticPiston по инерции")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts.X_left - 10, 0.23);
            geom.AddPoint(layerOpts.X_right + 0.01, 0.23);
            geom.AddPoint(layerOpts.X_right + 0.01+0.1, 0.16);
            geom.AddPoint(layerOpts.X_right + 1000, 0.16);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts, InitIdealPCell, InitElasticPBound);
            var grid = new IdealPiston.PistonGrid("ElasticP_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            return solver;
        }
        public static WBOneDemLayerOptions StandartOpts {
            get {
                var lo = new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = 0,
                    X_right = 0.1,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(150),
                };
                lo.SynchH();
                return lo;
            }
        }
        public static ElasticPistonCell InitIdealPCell(double t, double x) {
            var answ = new ElasticPistonCell(gp);
            answ.ro = 921.0;
            answ.u = v0;
            answ.p = 0;
            answ.X = x;
            answ.V = v0;
            return answ;
        }
        public static ElasticPistonBound InitElasticPBound(double t, double x) {
            return new ElasticPistonBound(gp) { X = x, V = v0 };
        }
    }
}
