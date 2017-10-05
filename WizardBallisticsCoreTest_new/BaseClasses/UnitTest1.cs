using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallistics.Core;

namespace WizardBallisticsCoreTest_new {
    [TestClass]
    public class UnitTest1_ {
        [TestMethod]
        public void WBVecCopyTest1() {
            WBVec a = new WBVec(1, 2, 3, 4, 5);
            var b = a.Clone();
            b[1] = 999;
            Assert.AreNotEqual(a[1], b[1]);
        }
        [TestMethod]
        public void WBVecCopyTest2() {
            WBVec a = new WBVec(1, 2, 3, 4, 5);
            var b = 1*a;
            b[1] = 999;
            Assert.AreNotEqual(a[1], b[1]);
        }
        [TestMethod]
        public void WBVecCopyTest3() {
            WBVec a = new WBVec(1, 2, 3, 4, 5);
            var b = a;
            b[1] = 999;
            Assert.AreEqual(a[1], b[1]);
        }
    }
}
