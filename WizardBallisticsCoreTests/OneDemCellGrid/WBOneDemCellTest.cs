using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallisticsCore.OneDemCellGrid;
using System.Linq;

namespace WizardBallisticsCoreTests.OneDemCellGrid {
    [TestClass]
    public class WBOneDemCellTest {
        struct DataDummy {
            public double P;
            public double Ro;
            public double p;
        }
        static string[] trueNames = new string[] { "Index", "IndexInArray", "X", "V", "P", "Ro", "p" };

        [TestMethod]
        public void GetDataFieldsNamesTest() {
            var names = WBOneDemCell<DataDummy>.GetDataFieldsNames();
            foreach (var tn in trueNames) {
                Assert.IsTrue(names.Contains(tn));
            }

        }

        [TestMethod]
        public void thisTest() {
            var tst = new WBOneDemCell<DataDummy>() {
                Index = 1,
                X = 2,
                V = 3,
                IndexInArray = -1,
                Data = new DataDummy() {
                    P = 100,
                    Ro = 200,
                    p = 300
                }
            };

            Assert.AreEqual(1, tst["Index"]);
            Assert.AreEqual(-1, tst["IndexInArray"]);
            Assert.AreEqual(2d, tst["X"]);
            Assert.AreEqual(3d, tst["V"]);
            Assert.AreEqual(100d, tst["P"]);
            Assert.AreEqual(200d, tst["Ro"]);
            Assert.AreEqual(300d, tst["p"]);
            Assert.IsNull( tst["p "]);
        }

        [TestMethod]
        public void GetValuesTest() {
            var tst = Enumerable.Range(0, 100)
                .Select(i => new WBOneDemCell<DataDummy>() {
                    Index = 1,
                    X = 2,
                    V = 3,
                    IndexInArray = -1,
                    Data = new DataDummy() {
                        P = 1 * i,
                        Ro = 200,
                        p = 300
                    }
                });

            var Ps = WBOneDemCell<DataDummy>.GetValues<double>(tst, "P").ToList();
            foreach (var i in Enumerable.Range(0, 100).Select(ii => (double)ii)) {
                Assert.IsTrue(Ps.Contains(i));

            }

        }


    }
}
