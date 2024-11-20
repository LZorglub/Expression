using Afk.Expression;

namespace UnitTestEval
{
    [TestClass]
    public class DateTest
    {
        [TestMethod]
        public void TestSimpleDate()
        {
            ExpressionEval eval = new ExpressionEval("@D(2022-01-20)", CaseSensitivity.None);
            Assert.AreEqual(eval.Evaluate(), new DateTime(2022, 1, 20, 0, 0, 0));

            eval = new ExpressionEval("@D(2022-08-14 20:14)", CaseSensitivity.None);
            Assert.AreEqual(new DateTime(2022, 8, 14, 20, 14, 0), eval.Evaluate());
        }

        [TestMethod]
        public void TestDateAddition()
        {
            ExpressionEval eval = new ExpressionEval("@D(2022-01-20) + 1.5", CaseSensitivity.None);
            Assert.AreEqual(eval.Evaluate(), new DateTime(2022, 1, 21, 12, 0, 0));

            eval = new ExpressionEval("7 + @D(2022-08-11)", CaseSensitivity.None);
            Assert.AreEqual(new DateTime(2022, 8, 18, 0, 0, 0), eval.Evaluate());
        }

    }
}
