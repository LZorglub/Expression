using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Extensions for <see cref="IExpression"/>
    /// </summary>
    public static class IExpressionExtensions
    {
        
        /// <summary>
        /// Gets a lambda expression from <see cref="IExpression"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static System.Linq.Expressions.Expression<Func<T, TResult>> ToLambda<T, TResult>(this IExpression expr, ILambdaExpressionProvider provider)
        {
            LambdaExpressionBuilder builder = new LambdaExpressionBuilder(provider);
            return builder.BuildLambda<T, TResult>(expr);
        }
    }
}
