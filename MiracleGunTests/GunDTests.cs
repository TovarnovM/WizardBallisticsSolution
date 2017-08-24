using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun;
using MiracleGun.Invariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.Tests {
    [TestClass()]
    public class GunDTests {
        [TestMethod()]
        public void GetWTest() {
            var g = new GunShape();
            g.AddPoint(0, 3);
            g.AddPoint(3, 0);
            Assert.AreEqual(0d, g.GetW(-3, -1));
            Assert.AreEqual(0d, g.GetW(4, 17));
            Assert.AreEqual(0d, g.GetW(-3, -0.00001));
            Assert.AreEqual(0d, g.GetW(-3, 0));
        }

        [TestMethod()]
        public void GetWTest2() {
            var g = new GunShape();
            g.AddPoint(0, 3);
            g.AddPoint(3, 0);
            Assert.AreEqual(7.0685834705770345, g.GetW(0, 3), 0.0000001);
            Assert.AreEqual(7.0685834705770345, g.GetW(-10, 30), 0.0000001);
            Assert.AreEqual(1.8326, g.GetW(1, 2), 0.0001);
        }

        [TestMethod()]
        public void GetWTest3() {
            var g = new GunShape();
            g.AddPoint(0, 3);
            g.AddPoint(1, 2);
            g.AddPoint(2, 1);
            g.AddPoint(3, 0);
            Assert.AreEqual(7.0685834705770345, g.GetW(-10, 10), 0.0000001);
            Assert.AreEqual(7.0685834705770345, g.GetW(0, 3), 0.0000001);
            Assert.AreEqual(1.8326, g.GetW(1, 2), 0.0001);
            
        }
        [TestMethod()]
        public void GetWTest4() {
            var g = new GunShape();
            g.AddPoint(0, 2);
            g.AddPoint(1, 2);
            g.AddPoint(2, 1);
            g.AddPoint(3, 1);
            Assert.AreEqual(1.8326+ 3.1415926535897931+ 0.78539816339744828, g.GetW(-10, 10), 0.0001);
            Assert.AreEqual(1.8326 + 3.1415926535897931*0.5 + 0.78539816339744828*0.5, g.GetW(0.5, 2.5), 0.0001);
            Assert.AreEqual(1.8326 + 3.1415926535897931 * 0 + 0.78539816339744828 * 0.5, g.GetW(1, 2.5), 0.0001);
            Assert.AreEqual(1.8326 + 3.1415926535897931 * 0 + 0.78539816339744828 * 0, g.GetW(1, 2), 0.0001);
            Assert.AreEqual(1.8326 + 3.1415926535897931 * 0.5 + 0.78539816339744828 * 0, g.GetW(0.5, 2), 0.0001);
            //Assert.AreEqual(7.0685834705770345, g.GetW(0, 3), 0.0000001);
            //Assert.AreEqual(1.8326, g.GetW(1, 2), 0.0001);

        }
    }
}