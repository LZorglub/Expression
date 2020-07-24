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
    public class PowTest
    {
        [TestMethod]
        public void TestVariableWithPower()
        {
            ExpressionEval eval = new ExpressionEval("x2+y", CaseSensitivity.None);
            eval.AddVariables(new string[] { "x", "y"});
            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(11d, result);
        }

        [TestMethod]
        public void TestVariablesWithPower()
        {
            ExpressionEval eval = new ExpressionEval("x2+y1", CaseSensitivity.None);
            eval.AddVariables(new string[] { "x", "y" });
            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(11d, result);
        }

        [TestMethod]
        public void TestOperatorTypePowerPriority()
        {
            ExpressionEval expr1 = new ExpressionEval("0.0000001504*222916^2+0.0281*222916+2752.28", OperatorType.Arithmetic);
            var result = Math.Round((double)expr1.Evaluate(), 0, MidpointRounding.AwayFromZero);
            Assert.AreEqual(16490d, result);
        }

        [TestMethod]
        public void TestConstantWithPower()
        {
            ExpressionEval eval = new ExpressionEval("deux3", CaseSensitivity.None);
            eval.AddConstant("deux", 2d);
            var result = eval.Evaluate();
            Assert.AreEqual(8d, result);
        }

        private void Eval_UserExpressionEventHandler(object sender, UserExpressionEventArgs e)
        {
            if (e.Name == "x") e.Result = 3;
            if (e.Name == "y") e.Result = 2;
        }
    }
}
