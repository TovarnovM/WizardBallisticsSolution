using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallisticsCore.OneDemGrid;
using System.Linq;
using WizardBallisticsCore;

namespace WizardBallisticsCoreTests.OneDemCellGrid {
    [TestClass]
    public class WBOneDemCellTest {
        struct DataDummy {
            public double P;
            public double Ro;
            public double p;
        }
        static string[] trueNames = new string[] { "X", "V", "P", "Ro", "p" };

        [TestMethod]
        public void GetDataFieldsNamesTest() {
            var names = WBOneDemNode<DataDummy>.GetDataFieldsNames();
            Assert.AreEqual(trueNames.Length, names.Count);
            foreach (var tn in trueNames) {
                Assert.IsTrue(names.Contains(tn));
            }

        }

        [TestMethod]
        public void ThisTest() {
            var tst = new WBOneDemNode<DataDummy>() {
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
                .Select(i => new WBOneDemNode<DataDummy>() {
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

            var Ps = WBOneDemNode<DataDummy>.GetValues<double>(tst, "P").ToList();
            foreach (var i in Enumerable.Range(0, 100).Select(ii => (double)ii)) {
                Assert.IsTrue(Ps.Contains(i));

            }

        }

        [TestMethod]
        public void GetValuesTest2()
        {
            var tst = Enumerable.Range(0, 100)
                .Select(i => new WBOneDemNode<DataDummy>()
                {
                    Index = 1,
                    X = 2,
                    V = 3,
                    IndexInArray = -1,
                    Data = new DataDummy()
                    {
                        P = 1 * i,
                        Ro = 200,
                        p = 300
                    }
                });

            var Ps = tst.Values<DataDummy,double>("P").ToList();
            foreach (var i in Enumerable.Range(0, 100).Select(ii => (double)ii))
            {
                Assert.IsTrue(Ps.Contains(i));

            }

        }


    }
}
