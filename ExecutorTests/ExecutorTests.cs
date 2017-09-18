using Microsoft.VisualStudio.TestTools.UnitTesting;
using Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.Tests {
    [TestClass()]
    public class ExecutorTests {
        
        [TestMethod()]
        public void ExecutorTest() {
            var en = Enumerable.Range(10, 4);
            var lst = en.ToList();
            var enumer = en.GetEnumerator();
            var c = enumer.Current;
            Assert.Fail();
        }
    }
}