using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallistics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core.Tests {
    [TestClass()]
    public class WBNodeBaseTests {
        public class DataDummy : WBOneDemNode {
            public double P;
            public double Ro;
            public double p;
        }
        static string[] trueNames = new string[] { "X", "V", "P", "Ro", "p" };

        [TestMethod]
        public void GetDataFieldsNamesTest() {
            var d = new DataDummy();
            var names = d.GetDataFieldsNames();
            Assert.AreEqual(trueNames.Length, names.Count);
            foreach (var tn in trueNames) {
                Assert.IsTrue(names.Contains(tn));
            }

        }

        [TestMethod]
        public void ThisTest() {
            var tst = new DataDummy() {
                Index = 1,
                X = 2,
                V = 3,
                IndexInList = -1,
                P = 100,
                Ro = 200,
                p = 300

            };

            Assert.AreEqual(1, tst["Index"]);
            Assert.AreEqual(-1, tst["IndexInList"]);
            Assert.AreEqual(2d, tst["X"]);
            Assert.AreEqual(3d, tst["V"]);
            Assert.AreEqual(100d, tst["P"]);
            Assert.AreEqual(200d, tst["Ro"]);
            Assert.AreEqual(300d, tst["p"]);
            Assert.IsNull(tst["p "]);
        }

        [TestMethod]
        public void GetValuesTest() {
            var tst = Enumerable.Range(0, 100)
                .Select(i => new DataDummy() {
                    Index = 1,
                    X = 2,
                    V = 3,
                    IndexInList = -1,
                    P = 1 * i,
                    Ro = 200,
                    p = 300

                });

            var Ps = DataDummy.GetValues<double>(tst, "P").ToList();
            foreach (var i in Enumerable.Range(0, 100).Select(ii => (double)ii)) {
                Assert.IsTrue(Ps.Contains(i));

            }

        }

        [TestMethod]
        public void GetValuesTest2() {
            var tst = Enumerable.Range(0, 100)
                .Select(i => new DataDummy() {
                    Index = 1,
                    X = 2,
                    V = 3,
                    IndexInList = -1,
                    P = 1 * i,
                    Ro = 200,
                    p = 300

                });

            var Ps = tst.Values<double>("P").ToList();
            foreach (var i in Enumerable.Range(0, 100).Select(ii => (double)ii)) {
                Assert.IsTrue(Ps.Contains(i));

            }

        }


    }

        }
 
