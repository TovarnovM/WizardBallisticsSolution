using MiracleGun.Invariants;
using MiracleGun.IdealGas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic {
    [SolversFactory]
    public class OvSolverFactory {

        [SolverGeneratorMethod("One-velocity-gunpowder_one-powder")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts.X_left - 0.2, 0.2);
            geom.AddPoint(layerOpts.X_right * 100, 0.2);
            var initLayer = new OvLayer();
            initLayer.Geom = geom;
            GunPowder powder = GunPowder.Factory("ВТМ");
            powder.Get_powder_SI();
            List<GunPowder> powder_list = new List<GunPowder>() { powder };
            List<double> conc_list = new List<double>() { 1 };
            MixtureGunPowder mixture = new MixtureGunPowder(powder_list, conc_list);
            initLayer.InitLayer(0d, layerOpts, MySuoerFunc(mixture), InitOvBound);
            var grid = new OvGridRK("OvGrid_tst", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        [SolverGeneratorMethod("One-velocity-gunpowder_lot-of-powder")]
        public static WBSolver GetNewSolver1(WBProjectOptions options) {
            var layerOpts1 = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts1.X_left - 0.2, 0.132);
            geom.AddPoint(layerOpts1.X_right + 5, 0.132);
            var initLayer = new OvLayer();
            initLayer.Geom = geom;
            GunPowder powder = new GunPowder();
            powder = powder_AGARD();
            List<GunPowder> powder_list = new List<GunPowder>() {powder, powder};
            List<double> conc_list = new List<double>() { 0.7, 0.3 };
            MixtureGunPowder mixture = new MixtureGunPowder(powder_list, conc_list);
            mixture.GetMixture();
            initLayer.InitLayer(0d, layerOpts1, MySuoerFunc(mixture), InitOvBound);
            var grid = new OvGridRK("OvGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        [SolverGeneratorMethod("One-velocity-gunpowder_AGARD")]
        public static WBSolver GetNewSolver2(WBProjectOptions options) {
            var layerOpts2 = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts2.X_left - 0.2, 0.132);
            geom.AddPoint(layerOpts2.X_right + 5, 0.132);
            var initLayer = new OvLayer();
            initLayer.Geom = geom;
            GunPowder powder = new GunPowder();
            powder = powder_AGARD();
            List<GunPowder> powder_list = new List<GunPowder>() { powder };
            List<double> conc_list = new List<double>() { 1 };
            MixtureGunPowder mixture = new MixtureGunPowder(powder_list, conc_list);
            initLayer.InitLayer(0d, layerOpts2, MySuoerFunc(mixture), InitOvBound);
            var grid = new OvGridRK("OvGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        public static WBOneDemLayerOptions StandartOpts {
            get {
                var lo = new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = 0,
                    X_right = 0.762,
                    RealNodesCount = OvLayer.GetNumOfRealNodes(200),
                };
                lo.SynchH();
                return lo;
            }
        }

        public static Func<double, double, OvCell> MySuoerFunc(MixtureGunPowder mixture) {
            return (t, x) => {
                var answ = new OvCell(null, mixture);
                answ.ro = 840;
                answ.u = 0;
                answ.p = 1e5;
                answ.X = x;
                answ.V = 0;
                for (int i = 0; i < answ.mixture.powders.Count; i++) {
                    answ.z[i] = 0;
                }
                return answ;
            };
        }

        public static OvBound InitOvBound(double t, double x) {
            return new OvBound() { X = x };
        }

        public static GunPowder powder_AGARD() {
            var powder = new GunPowder();
            powder.f = 1.009e6;
            powder.alpha_k = 1.0838e-3;
            powder.dest = 1575;
            powder.k = 1.27;
            powder.nu = 0.9;
            powder.T1 = 2585;
            powder.lambda_1 = 0.2049;
            powder.lambda_2 = -0.8977;
            powder.kappa_1 = 0.7185;
            powder.kappa_2 = 0.5386;
            powder.Ik = 1.276e6;
            powder.zk = 1.5;
            return powder;
        }

        public static GunPowder powder_ign_AGARD() {
            var powder = new GunPowder();
            powder.f = 1.009e6;
            powder.alpha_k = 1.0838e-3;
            powder.dest = 1575;
            powder.k = 1.25;
            powder.nu = 0.9;
            powder.T1 = 2585;
            powder.lambda_1 = 0.2049;
            powder.lambda_2 = -0.8977;
            powder.kappa_1 = 0.7185;
            powder.kappa_2 = 0.5386;
            powder.Ik = 1.276e6;
            powder.zk = 1.5;
            return powder;
        }
    }
}

