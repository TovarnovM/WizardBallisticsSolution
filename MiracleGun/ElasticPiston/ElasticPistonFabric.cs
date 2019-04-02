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
            double d0 = 0.023, d1 = 0.016, l0 = 0.1, l1 = 0.1;
            geom.AddPoint(layerOpts.X_left - 10, d0);
            geom.AddPoint(l0, d0);
            geom.AddPoint(l0+l1, d1);
            geom.AddPoint(l0 + l1 + 1000, d1);
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
                    X_left = 0.09 - 0.0251/(3.14*0.023*0.023*0.25*921.0),
                    X_right = 0.09,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(100),
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
