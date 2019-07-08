using Afk.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestEval
{
    [TestClass]
    public class ExtractTest
    {
        [TestMethod]
        public void TestSimpleExtract()
        {
            IEnumerable<string> userExpressions = ExpressionEval.Extract("5 * (3 + 8)");
            Assert.AreEqual(userExpressions.Count(), 0);
        }

        [TestMethod]
        public void TestFormulaExtract()
        {
            IEnumerable<string> userExpressions = ExpressionEval.Extract("5=a and y = z");
            Assert.AreEqual(3, userExpressions.Count());
            Assert.AreEqual("a", userExpressions.ElementAt(0));
            Assert.AreEqual("y", userExpressions.ElementAt(1));
            Assert.AreEqual("z", userExpressions.ElementAt(2));
        }

        [TestMethod]
        public void TestComplexExtract()
        {
            IEnumerable<string> userExpressions = ExpressionEval.Extract("5=(a.b + b.c) and y = 15 and [z, i] = 75");
            Assert.AreEqual(5, userExpressions.Count());
            Assert.AreEqual("a.b", userExpressions.ElementAt(0));
            Assert.AreEqual("b.c", userExpressions.ElementAt(1));
            Assert.AreEqual("y", userExpressions.ElementAt(2));
            Assert.AreEqual("z", userExpressions.ElementAt(3));
            Assert.AreEqual("i", userExpressions.ElementAt(4));
        }

        [TestMethod]
        public void TestFunctionExtract()
        {
            IEnumerable<string> userExpressions = ExpressionEval.Extract("fn(a, (b), c) = result.value");
            Assert.AreEqual(5, userExpressions.Count());
            Assert.AreEqual("fn", userExpressions.ElementAt(0));
            Assert.AreEqual("a", userExpressions.ElementAt(1));
            Assert.AreEqual("b", userExpressions.ElementAt(2));
            Assert.AreEqual("c", userExpressions.ElementAt(3));
            Assert.AreEqual("result.value", userExpressions.ElementAt(4));
        }

    }
}
