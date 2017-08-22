using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.OvBallistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.OvBallistic.Tests {
    [TestClass()]
    public class OvPowderFactoryTests {
        [TestMethod()]
        public void GetPowderTest() {
            string[] allp = null;
            try {
                allp = OvPowderFactory.GetAllPowderNames();
            } catch (Exception) {
                Assert.Fail("Фабрика не создается");
            }
            Assert.IsNotNull(allp);
        }
    }
}