using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizardBallisticsCore.OneDemGrid;
using Newtonsoft.Json;

namespace WizardBallisticsCoreTests {
    [TestClass]
    public class SerialTest {
        struct TstStr {
            public double a, b, c;
            public int i;
        }

        interface ITst<T> where T : struct {
            double Dprop { get; set; }
            int IntProp { get; set; }
            WBOneDemNode<T> Node {get;set;}
        }

        interface IProp {
            double Value { get; set; }
        }

        class PropC : IProp {
            public double Value { get; set; }
            public TstC Owner { get; set; }
        }

        class TstC : ITst<TstStr> {
            public double Dprop { get; set; }
            public int IntProp { get; set; }
            public IProp InterfaceProp { get; set; }
            public WBOneDemNode<TstStr> Node { get; set; }

        }

        class TstC2: TstC, IEquatable<TstC2> {
            public double Dfield;
            public int intField;

            public bool Equals(TstC2 other) {
                return Dprop == other.Dprop &&
                    IntProp == other.IntProp &&
                    Node.Equals(other.Node) &&
                    Dfield == other.Dfield &&
                    intField == other.intField; 

            }
        }

        [TestMethod]
        public void TestMethod1() {
            var tst2 = new TstC2() {
                Dprop = 1d,
                IntProp = 2,
                Dfield = 3d,
                intField = 4,
                InterfaceProp = new PropC() {
                    Value = 777d,
                },
                Node = new WBOneDemNode<TstStr>() {
                    Index = 1,
                    IndexInArray = 2,
                    X = 77d,
                    V = 88d,
                    Data = new TstStr() {
                        a = 11,
                        b = 12,
                        c = 13,
                        i = 14
                    }
                }
            };

            var settings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects
            };
            var s = JsonConvert.SerializeObject(tst2, settings);

            var tst22 = JsonConvert.DeserializeObject<TstC>(s, settings);
            var n = tst22.Node;
            //n.Data.i++;
            tst22.Node = n;

            Assert.IsTrue(((TstC2)tst22).Equals(tst2));
        }
    }
}
