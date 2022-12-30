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
    public class FunctionsTest
    {
        [TestMethod]
        public void TestUpperFunction()
        {
            ExpressionEval eval = new ExpressionEval("lower(upper('test'))");
            Assert.AreEqual("test", eval.Evaluate());
        }

        [TestMethod]
        public void TestCaseFunction()
        {
            ExpressionEval eval = new ExpressionEval("case(0, 1, 'one', 2, 'two', 'zero')");
            Assert.AreEqual("zero", eval.Evaluate());

            eval = new ExpressionEval("case(3+6, 1, 'one', 2, 'two', 9, 'nine')");
            Assert.AreEqual("nine", eval.Evaluate());

            eval = new ExpressionEval("case('dog', 'd' + 'og', 1, 'cat', 2, 'horse', 3)");
            Assert.AreEqual(1d, eval.Evaluate());
        }

        [TestMethod]
        public void TestReplaceFunction()
        {
            ExpressionEval eval = new ExpressionEval("replace('one dog', 'dog', 'cat')");
            Assert.AreEqual("one cat", eval.Evaluate());

            eval = new ExpressionEval("replace('one dog', 'o', 'a')");
            Assert.AreEqual("ane dag", eval.Evaluate());
        }
    }
}
