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

        [TestMethod]
        public void TestIifFunction()
        {
            DateTime now = DateTime.UtcNow;
            ExpressionEval eval = new ExpressionEval("iif(hour(d + 1/24)>=8 && hour(d)<22, x, -x)");
            eval.AddVariables(new string[] { "x", "d" });
            eval.AddFunctions("hour"); eval.AddFunctions("iif");
            eval.UserExpressionEventHandler += (sender, e) =>
            {
                if (e.Name == "d") e.Result = now; else e.Result = 5d;
            };
            eval.UserFunctionEventHandler += Eval_UserFunctionEventHandler;
            var result = eval.Evaluate();

            Assert.AreEqual(5d, result);
        }

        private void Eval_UserFunctionEventHandler(object sender, UserFunctionEventArgs e)
        {
            switch(e.Name) {
                case "hour":
                    e.Result = 10;
                    break;
                case "iif":
                    e.Result = (Convert.ToBoolean(e.Parameters[0])) ? e.Parameters[1] : e.Parameters[2];
                    break;
            }
        }
    }
}
