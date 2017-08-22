using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.OvBallistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.OvBallistic.Tests {
    [TestClass()]
    public class OvPowderTests {
        [TestMethod()]
        public void PsiTest() {
            var powder = new OvPowder() {
                f = 1.047,
                k = 1.236,
                alpha_k = 1.039,
                T1 = 3000,
                dest = 1.6,
                Ik = 0.17,
                zk = 1.331,
                kappa_1 = 0.309,
                lambda_1 = 1.7,
                kappa_2 = 0.743,
                lambda_2 = -0.996
            };

            powder.Psi(0.3);
            Assert.Fail();
        }
    }
}