using Afk.Expression;

namespace UnitTestEval
{
    [TestClass]
    public class NullTest
    {
        [TestMethod]
        public void SumWithNullMustReturnNull()
        {
            ExpressionEval eval = new ExpressionEval("x + y", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y"); 

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void SubstractWithNullMustReturnNull()
        {
            ExpressionEval eval = new ExpressionEval("y - x", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void MulWithNullMustReturnNull()
        {
            ExpressionEval eval = new ExpressionEval("x * y", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DivWithNullMustReturnNull()
        {
            ExpressionEval eval = new ExpressionEval("y /x ", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void EqualsWithNullMustReturnFalse()
        {
            ExpressionEval eval = new ExpressionEval("y ==  x ", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void EqualsNullValuesMustReturnTrue()
        {
            ExpressionEval eval = new ExpressionEval("y ==  y ", CaseSensitivity.None);
            eval.AddVariable("x");
            eval.AddVariable("y");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            var result = eval.Evaluate();
            Assert.AreEqual(true, result);
        }

        private void Eval_UserExpressionEventHandler(object sender, UserExpressionEventArgs e)
        {
            if (e.Name == "x") e.Result = 2;
            else e.Result = null;
        }
    }
}
