using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace WizardBallisticsCoreTests1 {
    [SolversFactory]
    public static class SolversFactory {
        [SolverGeneratorMethod("TstZadacha1")]
        public static WBSolver TimeStopFunc(WBProjectOptions options) {
            return new WBSolver(grids: null, Options: options);
        }

        [SolverGeneratorMethod("TstZadacha2")]
        public static WBSolver TimeStopFunc2(WBProjectOptions options) {
            return new WBSolver(grids:null, Options: options);
        }
    }
    [TestClass()]
    public class SolversFactoryTests {
        [TestMethod()]
        public void GetTest() {
            var solvers = WBSolver.FactoryVariants.ToList();
            var tstSolvers = new string[] { "TstZadacha1", "TstZadacha2" };
            foreach (var s in tstSolvers) {
                Assert.IsTrue(solvers.Contains(s));
            }
            var s1 = WBSolver.Factory("TstZadacha2", WBProjectOptions.Default);
        }
    }
}
