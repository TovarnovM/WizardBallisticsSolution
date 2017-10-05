using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.ElasticPiston;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGunTests {
    [TestClass()]
    public class ElasticPistonCell_shockEOSTests {
        [TestMethod()]
        public void GetPressureTest() {
            var g = new ElasticPistonConsts_shockEOS(1.6);
            var c = new ElasticPistonCell_shockEOS(g);
            var p = c.GetPressure();
            c.p = p;
            var e = c.GetE();
            Assert.AreEqual(e, c.e);
        }
    }
}