using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwarmOptimizator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmOptimizator.Tests {
    [TestClass()]
    public class MarkovNameGeneratorTests {
        [TestMethod()]
        public void MarkovNameGeneratorTest() {
            var m = MarkovNameGenerator.Instance;
            var nms = Enumerable.Range(0, 2)
                .Select(_ => m.GetNextName())
                .ToList();
            var nms2 = MarkovNameGenerator.GetNamesByParents(nms)
                .Take(10)
                .ToList();
            int f = 10;
        }
    }
}