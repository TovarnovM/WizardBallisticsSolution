using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiracleGun.Tests {
    [TestClass()]
    public class GunShapeTests {
        [TestMethod()]
        public void STest() {
            var gs = new GunShape();
            gs.AddPoint(2, 3);
            gs.AddPoint(1, 3);
            gs.AddPoint(3, 2);
            gs.AddPoint(-2, 1);
            gs.AddPoint(7, 2);
            Assert.AreEqual(0d, gs.GetV(-10));
            Assert.AreEqual(0d, gs.GetV(10));
            Assert.AreEqual(1d, gs.GetV(-2));
            Assert.AreEqual(2d, gs.GetV(3));
            Assert.AreEqual(3d, gs.GetV(1));
            Assert.AreEqual(2d, gs.GetV(7));
            Assert.AreEqual(3d, gs.GetV(2));
            Assert.AreEqual(2d, gs.GetV(5));
            Assert.AreEqual(2.5, gs.GetV(2.5));
            Assert.AreEqual(2d, gs.GetV(-0.5), 0.00000001);
            Assert.AreEqual(2.25, gs.GetV(2.75), 0.00000001);
        }

        [TestMethod()]
        public void InvalidDataTest() {
            var gs = new GunShape();
            gs.AddPoint(2, 3);
            gs.AddPoint(1, 3);
            gs.AddPoint(3, 2);
            gs.AddPoint(-2, 1);
            gs.AddPoint(7, 2);
            gs.AddPoint(3+gs.x_delta*0.7, 2);
            gs.AddPoint(-2, 1);
            gs.AddPoint(-2, 1);
            var s = gs.InvalidData();
            Assert.AreNotEqual("", s);
        }
    }
}