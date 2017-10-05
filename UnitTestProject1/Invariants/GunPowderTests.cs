using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.Invariants;
using MiracleGun.OvBallistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGunTests {
    [TestClass()]
    public class GunPowderTests {
        [TestMethod()]
        public void PsiTest() {
            var p = OvSolverFactory.powder_AGARD();
            int pCount = 70;
            var psis = Enumerable.Range(0, pCount + 1)
                .Select(i => 2 * (double)i / pCount)
                .Select(z => (z, p.Psi(z)))
                .ToList();
            foreach (var tp in psis) {
                Assert.IsTrue(tp.Item2 <= 1d);
            }
        }
    }
}