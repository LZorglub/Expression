using Afk.Expression;

namespace UnitTestEval
{
    class CustomArrayOperand : ICustomOperand
    {
        private IEnumerable<string> values;

        public CustomArrayOperand(IEnumerable<string> values)
        {
            this.values = values;
        }

        public object HandleOperation(string @operator, object otherOperand, bool isLeftOperand)
        {
            if (@operator == "=" && otherOperand != null)
            {
                return values.Contains(otherOperand.ToString());
            }
            else if (@operator == "!=" && otherOperand != null)
            {
                return !values.Contains(otherOperand.ToString());
            }

            return false;
        }
    }

    class CustomStringOperand : ICustomOperand
    {
        private readonly string customValue;

        public CustomStringOperand(string customValue)
        {
            this.customValue = customValue;
        }

        public object HandleOperation(string @operator, object otherOperand, bool isLeftOperand)
        {
            if (@operator == "+" && otherOperand != null)
            {
                return (isLeftOperand) ? $"{customValue}{otherOperand}" : $"{otherOperand}{customValue}";
            }

            return string.Empty;
        }
    }

    [TestClass]
    public class CustomOperandTest
    {
        [TestMethod]
        [DataRow("user1='val3'", true)]
        [DataRow("'val3'=user1", true)]
        [DataRow("'val4'=user1", false)]
        [DataRow("user1!='val4'", true)]
        public void TestCustomOperand(string expression, bool expected)
        {
            ExpressionEval eval = new ExpressionEval(expression, CaseSensitivity.None);
            eval.AddVariable("user1");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            Assert.AreEqual(expected, eval.Evaluate());
        }

        [TestMethod]
        [DataRow("stringValue + ' blue'", "red blue")]
        [DataRow("'blue ' + stringValue", "blue red")]
        [DataRow("'blue ' + stringValue + ' orange'", "blue red orange")]
        public void TestCustomStringOperand(string expression, string expected)
        {
            ExpressionEval eval = new ExpressionEval(expression, CaseSensitivity.None);
            eval.AddVariable("stringValue");

            eval.UserExpressionEventHandler += Eval_UserExpressionEventHandler;
            Assert.AreEqual(expected, eval.Evaluate());
        }

        private void Eval_UserExpressionEventHandler(object sender, UserExpressionEventArgs e)
        {
            if (e.Name == "user1")
                e.Result = new CustomArrayOperand(new List<string>() { "val1", "val2", "val3" });
            else if (e.Name == "stringValue")
                e.Result = new CustomStringOperand("red");
        }
    }
}
