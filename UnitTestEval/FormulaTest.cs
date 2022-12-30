using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Afk.Expression;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestEval
{
    [TestClass]
    public class FormulaTest
    {
        [TestMethod]
        public void TestSimpleFormula()
        {
            ExpressionEval eval = new ExpressionEval("5 * (3 + 8)");
            Assert.AreEqual(eval.Evaluate(), 55d);
        }

        [TestMethod]
        public void TestDoubleValue()
        {
            ExpressionEval eval = new ExpressionEval("5.8");
            Assert.AreEqual(eval.Evaluate(), 5.8d);
        }

        [TestMethod]
        public void TestSimpleStringFormula()
        {
            ExpressionEval eval = new ExpressionEval("'one ' + 'and' + ' two'");
            Assert.AreEqual(eval.Evaluate(), "one and two");
        }

        [TestMethod]
        public void TestPowerFormula()
        {
            ExpressionEval eval = new ExpressionEval("5E2");
            Assert.AreEqual(500d, eval.Evaluate());
        }

        [TestMethod]
        public void TestBoolean()
        {
            ExpressionEval eval = new ExpressionEval("true or false");
            Assert.AreEqual(eval.Evaluate(), true);
        }

        [TestMethod]
        public void TestArithmetic()
        {
            ExpressionEval eval = new ExpressionEval("5 * x + y");
            eval.AddVariable("x"); eval.AddVariable("y");
            eval.UserExpressionEventHandler += (s, e) => {
                if (e.Name == "x")
                    e.Result = 8d;
                else if (e.Name == "y")
                    e.Result = 5d;
            }; 
            var result = eval.Evaluate();
            Assert.AreEqual(result, 45d);
        }

        [TestMethod]
        public void TestConstants()
        {
            ExpressionEval eval = new ExpressionEval("5 * x + y - z");
            eval.AddConstant("x", 8);
            eval.AddConstant("y", 2);
            eval.AddConstant("z", 3);
            var result = eval.Evaluate();
            Assert.AreEqual(result, 39d);
        }

        [TestMethod]
        public void TestConstantsNotSensitive()
        {
            ExpressionEval eval = new ExpressionEval("5 * x + Y - Z", CaseSensitivity.None);
            eval.AddConstant("x", 8);
            eval.AddConstant("y", 2);
            eval.AddConstant("z", 3);
            var result = eval.Evaluate();
            Assert.AreEqual(result, 39d);
        }

        [TestMethod]
        public void TestConstantsSensitive()
        {
            ExpressionEval eval = new ExpressionEval("5 * xx + xX - Xx", CaseSensitivity.UserConstants);
            eval.AddConstant("xx", 8);
            eval.AddConstant("xX", 2);
            eval.AddConstant("Xx", 3);
            
            var result = eval.Evaluate();
            Assert.AreEqual(result, 39d);
        }

        [TestMethod]
        public void TestUserExpression()
        {
            ExpressionEval eval = new ExpressionEval("var1 + var2");
            eval.AddVariable("var1");
            eval.AddVariable("var2");
            eval.UserExpressionEventHandler += OnUserExpression;
            var result = eval.Evaluate();
            Assert.AreEqual(result, 9d);
        }

        [TestMethod]
        public void TestBracket()
        {
            ExpressionEval eval = new ExpressionEval("[ var1, 15, 'un '] + [var2, (25+9), 'deux']");
            eval.AddVariable("var1");
            eval.AddVariable("var2");
            eval.UserExpressionEventHandler += OnUserExpression;
            var result = eval.Evaluate();
            Assert.IsInstanceOfType(result, typeof(object[]));
            object[] values = result as object[];
            Assert.AreEqual(values[0], 9d);
            Assert.AreEqual(values[1], 49d);
            Assert.AreEqual(values[2], "un deux");
        }

        [TestMethod]
        public void TestFncConcat()
        {
            ExpressionEval eval = new ExpressionEval("'quand la caravane ' + Concat(  Concat(  Upper (verbe), ' les '), 'chiens ', 'aboient') + '.'");
            eval.AddVariable("var1");
            eval.AddVariable("var2");
            eval.AddVariable("verbe");

            eval.AddFunctions("Concat");
            eval.AddFunctions("Upper");

            eval.UserExpressionEventHandler += OnUserExpression;
            eval.UserFunctionEventHandler += OnFunctionHandler;
            var result = eval.Evaluate();

            Assert.AreEqual(result, "quand la caravane PASSE les chiens aboient.");
        }

        [TestMethod]
        public void TestOperatorType()
        {
            ExpressionEval expr1 = new ExpressionEval("4^2", OperatorType.Arithmetic);
            Assert.AreEqual(16d, expr1.Evaluate());

            ExpressionEval expr2 = new ExpressionEval("8^2", OperatorType.Binary);
            Assert.AreEqual((ulong)10, expr2.Evaluate());
        }

        [TestMethod]
        public void TestBuildLambaExpression()
        {
            ExpressionEval expr = new ExpressionEval("8+x");
            expr.AddVariable("x");

            var lambda = ExpressionHelper.BuildLambda<string, double>(expr,
                (e, name) => System.Linq.Expressions.Expression.Property(e, "Length"))
                .Compile();

            Assert.AreEqual(lambda("test"), 12);
            Assert.AreEqual(lambda("12345678"), 16);
        }

        [TestMethod]
        public void TestBuildLambaAdditionExpression()
        {
            ExpressionEval expr = new ExpressionEval("x + y");
            expr.AddVariable("x");
            expr.AddVariable("y");

            var lambda = ExpressionHelper.BuildLambda<Dictionary<string,int>, int>(expr,
                (e, name) => {
                    MethodInfo mi = e.Type.GetMethod("get_Item");
                    return System.Linq.Expressions.Expression.Call(e, mi, System.Linq.Expressions.Expression.Constant(name));
                })
                .Compile();

            Assert.AreEqual(13, lambda(new Dictionary<string, int> { { "x", 5 }, { "y", 8 } }));
            Assert.AreNotEqual(3, lambda(new Dictionary<string, int> { { "x", 1 }, { "y", 1 } }));
            Assert.AreEqual(3, lambda(new Dictionary<string, int> { { "x", 1 }, { "y", 2 } }));
        }

        [TestMethod]
        public void TestBuildLambaDoubleValue()
        {
            ExpressionEval expr = new ExpressionEval("x = 3");
            expr.AddVariable("x");

            var lambda = ExpressionHelper.BuildLambda<double, Boolean>(expr,
                (e, name) =>
                {
                    return e;
                })
                .Compile();
            Assert.AreEqual(true, lambda(3));
            Assert.AreNotEqual(true, lambda(6));
        }

        [TestMethod]
        public void TestBuildLambaInArrayExpression()
        {
            ExpressionEval expr = new ExpressionEval("x in [2,3,5,6, 3+6]");
            expr.AddVariable("x");

            var lambda = ExpressionHelper.BuildLambda<double, Boolean>(expr,
                (e, name) =>
                {
                    return e;
                })
                .Compile();

            Assert.AreEqual(true, lambda(3));
            Assert.AreEqual(true, lambda(6));
            Assert.AreEqual(false, lambda(8));
            Assert.AreEqual(true, lambda(9));

            expr = new ExpressionEval("(x + y) in [2, 4, 8]");
            expr.AddVariable("x");
            expr.AddVariable("y");

            var lambdaXY = ExpressionHelper.BuildLambda<Dictionary<string, double>, bool>(expr,
                (e, name) => {
                    MethodInfo mi = e.Type.GetMethod("get_Item");
                    return System.Linq.Expressions.Expression.Call(e, mi, System.Linq.Expressions.Expression.Constant(name));
                })
                .Compile();

            Assert.AreEqual(true, lambdaXY(new Dictionary<string, double> { { "x", 5 }, { "y", 3 } }));

            expr = new ExpressionEval("(x + y) in [2, 4, 8, z]");
            expr.AddVariable("x");
            expr.AddVariable("y");
            expr.AddVariable("z");

            var lambdaXYZ = ExpressionHelper.BuildLambda<Dictionary<string, double>, bool>(expr,
                (e, name) => {
                    MethodInfo mi = e.Type.GetMethod("get_Item");
                    return System.Linq.Expressions.Expression.Call(e, mi, System.Linq.Expressions.Expression.Constant(name));
                })
                .Compile();

            Assert.AreEqual(true, lambdaXYZ(new Dictionary<string, double> { { "x", 5 }, { "y", 3 }, { "z", 9 } }));
            Assert.AreEqual(false, lambdaXYZ(new Dictionary<string, double> { { "x", 5 }, { "y", 5 }, { "z", 9 } }));
            Assert.AreEqual(true, lambdaXYZ(new Dictionary<string, double> { { "x", 5 }, { "y", 4 }, { "z", 9 } }));
        }

        private void OnFunctionHandler(object sender, UserFunctionEventArgs e)
        {
            if (e.Name == "Concat")
            {
                e.Result = string.Join("", e.Parameters);
            }
            else if (e.Name == "Upper")
            {
                e.Result = e.Parameters[0].ToString().ToUpper();
            }
        }

        private void OnUserExpression(object sender, UserExpressionEventArgs e)
        {
            e.Result= 0;
            if (e.Name == "x") e.Result = 8d;
            if (e.Name == "var1") e.Result= 4;
            if (e.Name == "var2") e.Result= 5;
            if (e.Name == "verbe") e.Result = "passe";
        }
    }
}
