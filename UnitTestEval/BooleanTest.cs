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
    public class BooleanTest
    {
        [TestMethod]
        public void TestVariableWithSamePrefix()
        {
            ExpressionEval eval = new ExpressionEval("prm.attribut.test2='good'", CaseSensitivity.None);
            eval.AddVariable("PRM.Attribut.TEST");
            eval.AddVariable("PRM.Attribut.TEST2"); // => MUST MATCH Before first variable

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            Assert.AreEqual(eval.Evaluate(), true);
        }

        [TestMethod]
        public void TestVariableWithQuote()
        {
            ExpressionEval eval = new ExpressionEval("prm.attribut.test-a2='good'", CaseSensitivity.None);
            //eval.AddVariable("PRM.Attribut.TEST");
            eval.AddVariable("PRM.Attribut.TEST-A2"); // => Minus sign must be in quote

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            Assert.AreEqual(true, eval.Evaluate());
        }

        private void Eval_UserExpressionEventHandler(object sender, UserExpressionEventArgs e)
        {
            if (e.Name== "prm.attribut.test2" || e.Name == "prm.attribut.test-a2")
            {
                e.Result = "good";
            }
        }
    }
}
