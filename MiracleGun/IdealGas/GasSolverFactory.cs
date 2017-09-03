using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.IdealGas {

    [SolversFactory]
    public class GasSolverFactory {
        [SolverGeneratorMethod("Euler-AUSMp-IdealGas-RimanTest")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts.X_left-10, 0.2);
            geom.AddPoint(layerOpts.X_right+10, 0.2);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts, InitGasCell, InitGasBound);
            var grid = new GasGrid("GasGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            return solver;
        }

        [SolverGeneratorMethod("Euler-AUSMp-IdealGas-RimanTest-movingMesh")]
        public static WBSolver GetNewSolver1(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var geom = new GunShape();
            geom.AddPoint(layerOpts.X_left - 10, 0.2);
            geom.AddPoint(layerOpts.X_right + 10, 0.2);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts, InitGasCell, InitGasBound);
            var grid = new GasGrid("GasGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.RealBoundsRev[0].V = 0;
            initLayer.RealBounds[0].V = 0.5;
            // initLayer.RealCells.ForEach(n => n.u = 0.5);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        [SolverGeneratorMethod("Pneumatics-simple_happening")]
        public static WBSolver GetNewSolver2(WBProjectOptions options) {
            var layerOpts1 = StandartOpts1;
            var geom = new GunShape();
            geom.AddPoint(layerOpts1.X_left-0.2, 0.2);
            geom.AddPoint(layerOpts1.X_right*100, 0.2);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts1, InitGasCell1, InitGasBound);
            var grid = new PnGrid("GasGrid_tst1", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        [SolverGeneratorMethod("Pneumatics-simple_happening2")]
        public static WBSolver GetNewSolver3(WBProjectOptions options) {
            var layerOpts1 = StandartOpts1;
            var geom = new GunShape();
            geom.AddPoint(layerOpts1.X_left - 0.2, 0.2);
            geom.AddPoint(layerOpts1.X_right * 100, 0.2);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts1, InitGasCell1, InitGasBound);
            var grid = new PnGrid2("GasGrid_tst2", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        [SolverGeneratorMethod("Pneumatics-simple_happening4")]
        public static WBSolver GetNewSolver4(WBProjectOptions options) {
            var layerOpts1 = StandartOpts1;
            var geom = new GunShape();
            geom.AddPoint(layerOpts1.X_left - 0.2, 0.2);
            geom.AddPoint(layerOpts1.X_right * 100, 0.2);
            var initLayer = new GasLayer();
            initLayer.Geom = geom;
            initLayer.InitLayer(0d, layerOpts1, InitGasCell1, InitGasBound);
            var grid = new PnGrid4("GasGrid_tst4", initLayer);
            var solver = new WBSolver(grid, options);
            initLayer.SynchNodes_X_V();
            return solver;
        }

        public static WBOneDemLayerOptions StandartOpts {
            get {
                var lo =  new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = -2,
                    X_right = 2,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(400),                  
                };
                lo.SynchH();
                return lo;
            }
        }

        public static WBOneDemLayerOptions StandartOpts1 {
            get {
                var lo = new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = 0,
                    X_right = 1,
                    RealNodesCount = GasLayer.GetNumOfRealNodes(100),
                };
                lo.SynchH();
                return lo;
            }
        }

        static GasConstants gc = new GasConstants(1.4, 0);
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
            return answ;
        }

        public static GasCell InitGasCell1(double t, double x) {
            var answ = new GasCell(gc);
            answ.ro = 40;
            answ.u = 0;
            answ.p = 4e5;
            answ.X = x;
            answ.V = 0;
            return answ;
        }

        public static GasBound InitGasBound(double t, double x) {
            return new GasBound() { X = x };
        }
    }
}
