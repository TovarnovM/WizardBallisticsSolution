using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.OvBallistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiracleGun.Invariants;

namespace MiracleGun.OvBallistic.Tests {
    [TestClass()]
    public class OvCellTests {

        MixtureGunPowder mixture;
        [TestMethod(), TestInitialize()]
        public void GetPressureTest() {
            var powder = new GunPowder();
            powder.f = 1.009e6;
            powder.alpha_k = 1.0838e-3;
            powder.dest = 1575;
            powder.k = 1.25;
            powder.nu = 0.9;
            powder.T1 = 2585;
            powder.lambda_1 = 0.2049;
            powder.lambda_2 = -0.8977;
            powder.kappa_1 = 0.7185;
            powder.kappa_2 = 0.5386;
            powder.Ik = 1.276e6;
            powder.zk = 1.5;
            List<GunPowder> powder_list = new List<GunPowder>() { powder };
            List<double> conc_list = new List<double>() { 1 };
            mixture = new MixtureGunPowder(powder_list, conc_list);
            var cell = new OvCell(null, mixture) {
                ro = 840,
                u = 0,
                p = 1e5,
                X = 0,
                V = 0
            };
            cell.e = cell.GetE();
            var p = cell.GetPressure();
            Assert.AreEqual(cell.p, p, 0.1);
        }

        [TestMethod()]
        public void CloneTest() {
            var c1 = new OvCell(null, mixture);
            var c2 = (OvCell)c1.Clone();

            Assert.AreNotSame(c1.z,c2.z);
        }
    }
}