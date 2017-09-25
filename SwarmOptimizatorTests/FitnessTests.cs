using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwarmOptimizator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.Oslo;

namespace SwarmOptimizator.Tests {
    [TestClass()]
    public class FitnessTests {
        [TestMethod()]
        public void FitnessTest() {
            var f1 = new FitInfo("f1");
            var f2 = new FitInfo("f2");
            var f = new Fitness(f1, f2);
            Assert.IsTrue(f.NeedCalc);
        }
        [TestMethod()]
        public void FitnessTest2() {
            var f1 = new FitInfo("f1");
            var f2 = new FitInfo("f2");
            var f = new Fitness(f1, f2);
            f["f1"] = 0;
            f["f2"] = -11;
            Assert.IsTrue(!f.NeedCalc);
        }

        [TestMethod()]
        public void CoolerThanTest() {
            var fi1 = new FitInfo("f1") { Extremum = FitExtremum.maximize};
            var fi2 = new FitInfo("f2") { Extremum = FitExtremum.minimize };
            var f1 = new Fitness(fi1, fi2);
            f1["f1"] = 0;
            f1["f2"] = -11;
            var f2 = new Fitness(fi1, fi2);
            f2["f1"] = 1;
            f2["f2"] = -11;
            Assert.AreEqual(-1, f1.CoolerThan(f2));
        }
        [TestMethod()]
        public void CoolerThanTest2() {
            var fi1 = new FitInfo("f1") { Extremum = FitExtremum.maximize };
            var fi2 = new FitInfo("f2") { Extremum = FitExtremum.minimize };
            var f1 = new Fitness(fi1, fi2);
            f1["f1"] = 0;
            f1["f2"] = -11;
            var f2 = new Fitness(fi1, fi2);
            f2["f1"] = -1;
            f2["f2"] = -11;
            Assert.AreEqual(1, f1.CoolerThan(f2));
        }
        [TestMethod()]
        public void CoolerThanTest3() {
            var fi1 = new FitInfo("f1") { Extremum = FitExtremum.maximize };
            var fi2 = new FitInfo("f2") { Extremum = FitExtremum.minimize };
            var f1 = new Fitness(fi1, fi2);
            f1["f1"] = 0;
            f1["f2"] = -11;
            var f2 = new Fitness(fi1, fi2);
            f2["f1"] = 1;
            f2["f2"] = -10;
            Assert.AreEqual(0, f1.CoolerThan(f2));
        }
    }
}