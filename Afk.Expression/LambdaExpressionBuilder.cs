using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a builder of lambda expression from <see cref="IExpression"/>
    /// </summary>
    class LambdaExpressionBuilder
    {
        internal static Dictionary<Type, List<Type>> implicitConversions = new Dictionary<Type, List<Type>>()
        {
            { typeof(int), new List<Type>(){ typeof(long), typeof(float), typeof(double), typeof(decimal)} },
            { typeof(short), new List<Type>(){ typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(ushort), new List<Type>(){ typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(uint), new List<Type>(){ typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(long), new List<Type>(){ typeof(float), typeof(double), typeof(decimal) } },
            { typeof(char), new List<Type>(){ typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(float), new List<Type>(){ typeof(double) } },
            { typeof(ulong), new List<Type>(){ typeof(float), typeof(double), typeof(decimal) } },
        };

        private readonly ILambdaExpressionProvider lambdaExpressionProvider;

        /// <summary>
        /// Initialize a new instance of <see cref="LambdaExpressionBuilder"/>
        /// </summary>
        public LambdaExpressionBuilder(ILambdaExpressionProvider lambdaExpressionProvider)
        {
            this.lambdaExpressionProvider = lambdaExpressionProvider;
        }

        /// <summary>
        /// Builds an <see cref="System.Linq.Expressions.Expression"/> from <see cref="IExpression"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression<Func<T, TResult>> BuildLambda<T, TResult>(IExpression expr)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));

            object parse = (expr is ExpressionEval) ? ((ExpressionEval)expr).Parse() : expr;

            ParameterExpression argParam = System.Linq.Expressions.Expression.Parameter(typeof(T), "s");
            System.Linq.Expressions.Expression exp = BuildLambda(parse, argParam);

            System.Linq.Expressions.Expression<Func<T, TResult>> lambda = System.Linq.Expressions.Expression.Lambda<Func<T, TResult>>(exp, argParam);

            return lambda;
        }

        /// <summary>
        /// Builds lambda expression recursively
        /// </summary>
        /// <param name="o"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression BuildLambda(object o, System.Linq.Expressions.ParameterExpression parameter)
        {
            System.Linq.Expressions.Expression result = null;

            if (o is UserExpression)
            {
                result = this.lambdaExpressionProvider.GetExpression(parameter, ((UserExpression)o).Expression);
            }
            else if (o is UnaryNode unaryNode)
            {
                #region UnaryNode
                System.Linq.Expressions.Expression e1 = BuildLambda(unaryNode.Operand, parameter);

                switch (unaryNode.Op.Op)
                {
                    case "+":
                        result = e1;
                        break;
                    case "-":
                        result = System.Linq.Expressions.Expression.Negate(e1);
                        break;
                    case "!":
                        result = System.Linq.Expressions.Expression.Not(e1);
                        break;
                    case "~":
                        result = System.Linq.Expressions.Expression.Not(e1);
                        break;
                }
                #endregion
            }
            else if (o is BinaryNode binaryNode)
            {
                #region BinaryNode
                System.Linq.Expressions.Expression e1 = BuildLambda(binaryNode.Operand1, parameter);
                System.Linq.Expressions.Expression e2 = BuildLambda(binaryNode.Operand2, parameter);

                result = this.lambdaExpressionProvider.GetExpression(e1, binaryNode.Op.Op.ToLower(), e2);

                if (result == null)
                {
                    switch (binaryNode.Op.Op.ToLower())
                    {
                        case "*":
                            result = System.Linq.Expressions.Expression.Multiply(e1, e2);
                            break;
                        case "/":
                            result = System.Linq.Expressions.Expression.Divide(e1, e2);
                            break;
                        case "%":
                            result = System.Linq.Expressions.Expression.Modulo(e1, e2);
                            break;
                        case "+":
                            {
                                if (e1.Type == typeof(string) || e2.Type == typeof(string))
                                {
                                    var concatMethod = typeof(string).GetRuntimeMethod("Concat", new[] { typeof(object), typeof(object) });
                                    result = System.Linq.Expressions.Expression.Add(e1, e2, concatMethod);
                                }
                                else if (e1.Type != e2.Type)
                                {
                                    // Convert e1 to e2
                                    if (implicitConversions.Any(e => e.Key == e1.Type && e.Value.Contains(e2.Type)))
                                    {
                                        result = System.Linq.Expressions.Expression.Add(
                                            System.Linq.Expressions.Expression.Convert(e1, e2.Type),
                                            e2
                                        );
                                    }
                                    else if (implicitConversions.Any(e => e.Key == e2.Type && e.Value.Contains(e1.Type)))
                                    {
                                        result = System.Linq.Expressions.Expression.Add(
                                            e1,
                                            System.Linq.Expressions.Expression.Convert(e2, e1.Type)
                                        );
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"Can not add {e1.Type.ToString()} and {e2.Type.ToString()}");
                                    }
                                }
                                else
                                {
                                    result = System.Linq.Expressions.Expression.Add(e1, e2);
                                }
                            }
                            break;
                        case "-":
                            result = System.Linq.Expressions.Expression.Subtract(e1, e2);
                            break;
                        case ">>":
                            result = System.Linq.Expressions.Expression.RightShift(e1, e2);
                            break;
                        case "<<":
                            result = System.Linq.Expressions.Expression.LeftShift(e1, e2);
                            break;
                        case "<":
                            result = System.Linq.Expressions.Expression.LessThan(e1, e2);
                            break;
                        case "<=":
                            result = System.Linq.Expressions.Expression.LessThanOrEqual(e1, e2);
                            break;
                        case ">":
                            result = System.Linq.Expressions.Expression.GreaterThan(e1, e2);
                            break;
                        case ">=":
                            result = System.Linq.Expressions.Expression.GreaterThanOrEqual(e1, e2);
                            break;
                        case "like":
                            if (e1.Type == typeof(string) && e2.Type == typeof(string))
                            {
                                MethodInfo mi = typeof(string).GetRuntimeMethod("Contains", new Type[] { typeof(string) });
                                result = System.Linq.Expressions.Expression.Call(e1, mi, e2);
                            }
                            else
                            {
                                throw new InvalidOperationException("like operator not supported");
                            }
                            break;
                        case "==":
                        case "=":
                            if (e1.Type == e2.Type)
                            {
                                result = System.Linq.Expressions.Expression.Equal(e1, e2);
                            } else
                            {
                                result = System.Linq.Expressions.Expression.Equal(e1, System.Linq.Expressions.Expression.Convert(e2, e1.Type));
                            }
                            break;
                        case "<>":
                        case "!=":
                            if (e1.Type == e2.Type)
                            {
                                result = System.Linq.Expressions.Expression.NotEqual(e1, e2);
                            }
                            else
                            {
                                result = System.Linq.Expressions.Expression.NotEqual(e1, System.Linq.Expressions.Expression.Convert(e2, e1.Type));
                            }
                            break;
                        case "&":
                            result = System.Linq.Expressions.Expression.And(e1, e2);
                            break;
                        case "^":
                            throw new NotImplementedException();
                        //result = System.Linq.Expressions.Expression.Not(e1, e2);
                        //break;
                        case "|":
                            result = System.Linq.Expressions.Expression.Or(e1, e2);
                            break;
                        case "and":
                        case "&&":
                            result = System.Linq.Expressions.Expression.AndAlso(e1, e2);
                            break;
                        case "or":
                        case "||":
                            result = System.Linq.Expressions.Expression.OrElse(e1, e2);
                            break;
                    }
                }
                #endregion
            }
            else
            {
                result = System.Linq.Expressions.Expression.Constant(o);
            }
            return result;
        }
    }
}
