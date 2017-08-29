using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiracleGun.OvBallistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallistics.Core;

namespace MiracleGun.OvBallistic.Tests {
    [TestClass()]
    public class OvLayerTests {
        [TestMethod()]
        public void InitBoundCellRefsTest() {
            var opts = new WBOneDemLayerOptions() {
                LeftNodesCount = 3,
                RightNodesCount = 4,
                RealNodesCount = OvLayer.GetNumOfRealNodes(5),
                X_left = 0,
                X_right = 10
            };
            opts.SynchH();

            Assert.AreEqual(18, opts.AllNodesCount);

            var lr = new OvLayer();
            
            lr.InitLayer(0, opts, (t,x) => new OvCell(), (t, x) => new OvBound());
            lr = lr.Clone() as OvLayer;

            Assert.AreEqual(5, lr.RealCells.Count);
            Assert.AreEqual(5, lr.RealCellsRev.Count);

            Assert.AreEqual(6, lr.RealBounds.Count);
            Assert.AreEqual(6, lr.RealBoundsRev.Count);

            Assert.AreEqual(2, lr.LeftCells.Count);
            Assert.AreEqual(2, lr.RightCells.Count);

            Assert.AreEqual(1, lr.LeftBounds.Count);
            Assert.AreEqual(2, lr.RightBounds.Count);

            Assert.AreEqual(9, lr.AllCells.Count);
            Assert.AreEqual(9, lr.AllCellsRev.Count);

            Assert.AreEqual(9, lr.AllBounds.Count);
            Assert.AreEqual(9, lr.AllBoundsRev.Count);

            Assert.IsNull(lr.AllCells[0].LeftBound);
            Assert.IsNotNull(lr.AllCellsRev[0].RightBound);

            Assert.IsNull(lr.AllBoundsRev[0].RightCell);
            Assert.IsNotNull(lr.AllBounds[0].LeftCell);

            foreach (var c in lr.AllCells) {
                Assert.IsTrue(c.RightBound != null || c.LeftBound != null);
            }
            foreach (var b in lr.AllBounds) {
                Assert.IsTrue(b.RightCell != null || b.LeftCell != null);
            }
        }

        [TestMethod()]
        public void InitBoundCellRefsTest1() {
            var opts = new WBOneDemLayerOptions() {
                LeftNodesCount = 4,
                RightNodesCount = 4,
                RealNodesCount = OvLayer.GetNumOfRealNodes(5),
                X_left = 0,
                X_right = 10
            };
            opts.SynchH();

            Assert.AreEqual(19, opts.AllNodesCount);

            var lr = new OvLayer();
            lr.InitLayer(0, opts, (t, x) => new OvCell(), (t, x) => new OvBound());
            lr.InitBoundCellRefs();

            Assert.AreEqual(5, lr.RealCells.Count);
            Assert.AreEqual(5, lr.RealCellsRev.Count);

            Assert.AreEqual(6, lr.RealBounds.Count);
            Assert.AreEqual(6, lr.RealBoundsRev.Count);

            Assert.AreEqual(2, lr.LeftCells.Count);
            Assert.AreEqual(2, lr.RightCells.Count);

            Assert.AreEqual(2, lr.LeftBounds.Count);
            Assert.AreEqual(2, lr.RightBounds.Count);

            Assert.AreEqual(9, lr.AllCells.Count);
            Assert.AreEqual(9, lr.AllCellsRev.Count);

            Assert.AreEqual(10, lr.AllBounds.Count);
            Assert.AreEqual(10, lr.AllBoundsRev.Count);

            Assert.IsNotNull(lr.AllCells[0].LeftBound);
            Assert.IsNotNull(lr.AllCellsRev[0].RightBound);

            Assert.IsNull(lr.AllBoundsRev[0].RightCell);
            Assert.IsNull(lr.AllBounds[0].LeftCell);

            foreach (var c in lr.AllCells) {
                Assert.IsTrue(c.RightBound != null || c.LeftBound != null);
            }
            foreach (var b in lr.AllBounds) {
                Assert.IsTrue(b.RightCell != null || b.LeftCell != null);
            }
        }

        [TestMethod()]
        public void InitBoundCellRefsTest2() {
            var opts = new WBOneDemLayerOptions() {
                LeftNodesCount = 4,
                RightNodesCount = 3,
                RealNodesCount = OvLayer.GetNumOfRealNodes(5),
                X_left = 0,
                X_right = 10
            };
            opts.SynchH();

            Assert.AreEqual(18, opts.AllNodesCount);

            var lr = new OvLayer();
            lr.InitLayer(0, opts, (t, x) => new OvCell(), (t, x) => new OvBound());
            lr.InitBoundCellRefs();

            Assert.AreEqual(5, lr.RealCells.Count);
            Assert.AreEqual(5, lr.RealCellsRev.Count);

            Assert.AreEqual(6, lr.RealBounds.Count);
            Assert.AreEqual(6, lr.RealBoundsRev.Count);

            Assert.AreEqual(2, lr.LeftCells.Count);
            Assert.AreEqual(2, lr.RightCells.Count);

            Assert.AreEqual(2, lr.LeftBounds.Count);
            Assert.AreEqual(1, lr.RightBounds.Count);

            Assert.AreEqual(9, lr.AllCells.Count);
            Assert.AreEqual(9, lr.AllCellsRev.Count);

            Assert.AreEqual(9, lr.AllBounds.Count);
            Assert.AreEqual(9, lr.AllBoundsRev.Count);

            Assert.IsNotNull(lr.AllCells[0].LeftBound);
            Assert.IsNull(lr.AllCellsRev[0].RightBound);

            Assert.IsNotNull(lr.AllBoundsRev[0].RightCell);
            Assert.IsNull(lr.AllBounds[0].LeftCell);

            foreach (var c in lr.AllCells) {
                Assert.IsTrue(c.RightBound != null || c.LeftBound != null);
            }
            foreach (var b in lr.AllBounds) {
                Assert.IsTrue(b.RightCell != null || b.LeftCell != null);
            }
        }

        [TestMethod()]
        public void InitBoundCellRefsTestMoscCommonScen() {
            var opts = new WBOneDemLayerOptions() {
                LeftNodesCount = 1,
                RightNodesCount = 1,
                RealNodesCount = OvLayer.GetNumOfRealNodes(5),
                X_left = 0,
                X_right = 10
            };
            opts.SynchH();

            Assert.AreEqual(13, opts.AllNodesCount);

            var lr = new OvLayer();
            lr.InitLayer(0, opts, (t, x) => new OvCell(), (t, x) => new OvBound());
            lr.InitBoundCellRefs();

            Assert.AreEqual(5, lr.RealCells.Count);
            Assert.AreEqual(5, lr.RealCellsRev.Count);

            Assert.AreEqual(6, lr.RealBounds.Count);
            Assert.AreEqual(6, lr.RealBoundsRev.Count);

            Assert.AreEqual(1, lr.LeftCells.Count);
            Assert.AreEqual(1, lr.RightCells.Count);

            Assert.AreEqual(0, lr.LeftBounds.Count);
            Assert.AreEqual(0, lr.RightBounds.Count);

            Assert.AreEqual(7, lr.AllCells.Count);
            Assert.AreEqual(7, lr.AllCellsRev.Count);

            Assert.AreEqual(6, lr.AllBounds.Count);
            Assert.AreEqual(6, lr.AllBoundsRev.Count);

            Assert.IsNull(lr.AllCells[0].LeftBound);
            Assert.IsNull(lr.AllCellsRev[0].RightBound);

            Assert.IsNotNull(lr.AllBoundsRev[0].RightCell);
            Assert.IsNotNull(lr.AllBounds[0].LeftCell);

            foreach (var c in lr.AllCells) {
                Assert.IsTrue(c.RightBound != null || c.LeftBound != null);
            }
            foreach (var b in lr.AllBounds) {
                Assert.IsTrue(b.RightCell != null || b.LeftCell != null);
            }
        }
    }
}