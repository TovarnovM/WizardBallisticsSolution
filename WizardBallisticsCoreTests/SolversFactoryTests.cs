using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallisticsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;

namespace WizardBallisticsCore.Tests {
    [SolversFactory]
    public static class SolversFactory {
        [SolverGeneratorMethod("TstZadacha1")]
        public static WBSolver TimeStopFunc(WBProjectOptions options) {
            return new WBSolver(null, options);
        }

        [SolverGeneratorMethod("TstZadacha2")]
        public static WBSolver TimeStopFunc2(WBProjectOptions options) {
            return new WBSolver(null, options);
        }
    }
    [TestClass()]
    public class SolversFactoryTests {
        [TestMethod()]
        public void GetTest() {
            var solvers = WBSolver.FactoryVariants.ToList();
            var tstSolvers = new string[] { "TstZadacha1", "TstZadacha2" };
            foreach (var s in tstSolvers) {
                Assert.IsTrue( solvers.Contains(s));
            }
             var s1 = WBSolver.Factory("TstZadacha2", WBProjectOptions.Default);
        }
    }
}