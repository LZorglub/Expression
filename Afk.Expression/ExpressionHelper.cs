﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Afk.Expression
{
    /// <summary>
    /// Provides method to analyze an <see cref="IExpression"/>
    /// </summary>
    public class ExpressionHelper
    {
        /// <summary>
        /// Builds an <see cref="System.Linq.Expressions.Expression"/> from <see cref="IExpression"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <param name="expressionFunc"></param>
        /// <returns></returns>
        public static System.Linq.Expressions.Expression<Func<T, TResult>> BuildLambda<T, TResult>(IExpression expr, 
            Func<System.Linq.Expressions.Expression, string, System.Linq.Expressions.Expression> expressionFunc)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));

            object parse = (expr is ExpressionEval) ? ((ExpressionEval)expr).Parse() : expr;

            ParameterExpression argParam = System.Linq.Expressions.Expression.Parameter(typeof(T), "s");
            System.Linq.Expressions.Expression exp = BuildLambda(parse, argParam, expressionFunc);

            System.Linq.Expressions.Expression<Func<T, TResult>> lambda = System.Linq.Expressions.Expression.Lambda<Func<T, TResult>>(exp, argParam);

            return lambda;
        }

        private static System.Linq.Expressions.Expression BuildLambda(object o, 
            System.Linq.Expressions.Expression parameter, 
            Func<System.Linq.Expressions.Expression, string, System.Linq.Expressions.Expression> expressionFunc)
        {
            System.Linq.Expressions.Expression result = null;

            if (o is UserExpression)
            {
                result = expressionFunc(parameter, ((UserExpression)o).Expression);
            }
            else if (o is UnaryNode)
            {
                #region UnaryNode
                UnaryNode node = (UnaryNode)o;
                System.Linq.Expressions.Expression e1 = BuildLambda(node.Operand, parameter, expressionFunc);

                switch (node.Op.Op)
                {
                    case "+":
                        result = e1;
                        break;
                    case"-":
                        result = System.Linq.Expressions.Expression.Negate(e1);
                        break;
                    case"!":
                        result = System.Linq.Expressions.Expression.Not(e1);
                        break;
                    case"~":
                        result = System.Linq.Expressions.Expression.Not(e1);
                        break;
                }
                #endregion
            }
            else if (o is BinaryNode)
            {
                #region BinaryNode
                BinaryNode node = (BinaryNode)o;
                System.Linq.Expressions.Expression e1 = BuildLambda(node.Operand1, parameter, expressionFunc);
                System.Linq.Expressions.Expression e2 = BuildLambda(node.Operand2, parameter, expressionFunc);

                switch (node.Op.Op.ToLower())
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
                                if (LambdaExpressionBuilder.implicitConversions.Any(e => e.Key == e1.Type && e.Value.Contains(e2.Type)))
                                {
                                    result = System.Linq.Expressions.Expression.Add(
                                        System.Linq.Expressions.Expression.Convert(e1, e2.Type),
                                        e2
                                    );
                                }
                                else if (LambdaExpressionBuilder.implicitConversions.Any(e => e.Key == e2.Type && e.Value.Contains(e1.Type)))
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
                    case "in":
                        if (e2.Type == typeof(object[]))
                        {
                            MethodInfo mis = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Single(m => m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(object));
                            result = System.Linq.Expressions.Expression.Call(null, mis, new System.Linq.Expressions.Expression[] { e2, System.Linq.Expressions.Expression.Convert(e1, typeof(object)) });

                            //MethodInfo mi = typeof(Enumerable).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(object));
                            //result = System.Linq.Expressions.Expression.Call(mi, System.Linq.Expressions.Expression.Convert(e2, typeof(IEnumerable<object>)), System.Linq.Expressions.Expression.Convert(e1, typeof(object)));
                        } 
                        else
                        {
                            throw new InvalidOperationException("in operator not supported");
                        }
                        break;
                    case "==":
                    case "=":
                        result = System.Linq.Expressions.Expression.Equal(e1, e2);
                        break;
                    case "<>":
                    case "!=":
                        result = System.Linq.Expressions.Expression.NotEqual(e1, e2);
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
#endregion
            }
            else if (o is object[])
            {
                List<System.Linq.Expressions.Expression> items = new List<System.Linq.Expressions.Expression>();
                foreach (var item in (object[])o)
                {
                    items.Add(System.Linq.Expressions.Expression.TypeAs(
                        BuildLambda(item, parameter, expressionFunc), typeof(object)));
                }
                result = System.Linq.Expressions.Expression.NewArrayInit(typeof(object), items.ToArray());
                //result = System.Linq.Expressions.Expression.Constant(items.ToArray());
            }
            else
            {
                result = System.Linq.Expressions.Expression.Constant(o);
            }
            return result;
        }
    }
    
}
