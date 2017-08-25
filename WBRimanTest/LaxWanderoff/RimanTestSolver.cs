using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics;
using WizardBallistics.Core;

namespace SolverDrawTsts {
    [SolversFactory]
    public class RimanFactory {
        [SolverGeneratorMethod("Лакс-вандеркто-то идеальный газ, задача Римана")]
        public static WBSolver GetNewSolver(WBProjectOptions options) {
            var layerOpts = StandartOpts;
            var initLayer = new RmLayer();
            initLayer.InitLayer(0d, layerOpts, InitNodeF);
            var grid = new RmGrid("RimanGrid", initLayer);
            var solver = new WBSolver(grid, options);
            return solver;
        }

        public static WBOneDemLayerOptions StandartOpts {
            get {
                return new WBOneDemLayerOptions() {
                    LeftNodesCount = 1,
                    RightNodesCount = 1,
                    X_left = -1,
                    X_right = 1,
                    RealNodesCount = 200,
                    H = 0.01
                };
            }
        }
        public static RmNode InitNodeF(double t, double x) {
            var answ = new RmNode();
            if (x <= 0d) {
                answ.ro = 1;
                answ.u = 0;
                answ.p = 1;
            } else {
                answ.ro = 0.125;
                answ.u = 0;
                answ.p = 0.1;
            }
            answ.e = answ.GetE();
            answ.InitS();
            answ.InitF();
            return answ;
        }
    }
}
