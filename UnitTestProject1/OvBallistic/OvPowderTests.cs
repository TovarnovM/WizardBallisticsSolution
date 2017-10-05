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
    public class OvPowderTests {
        [TestMethod()]
        public void PsiTest() {
            var powders = GunPowderFactory.GetAllPowderNames();
            var badPowders = new List<string>();
            for (int i = 0; i < powders.Length; i++) {
                var powder = GunPowder.Factory(powders[i]);
                bool goodPwdr = powder.Psi(-10) == 0d &&
                    powder.Psi(999) == 1d;
                if (!goodPwdr)
                    badPowders.Add(powders[i]);
            }
            Assert.AreEqual(0, badPowders.Count);
        }
    }
}