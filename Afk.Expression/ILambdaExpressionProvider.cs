using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Provides lambda expression
    /// </summary>
    public interface ILambdaExpressionProvider
    {
        /// <summary>
        /// Gets expression which retrieves property "name" from a <see cref="ParameterExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        System.Linq.Expressions.Expression GetExpression(System.Linq.Expressions.ParameterExpression expression, string name);

        /// <summary>
        /// Gets expression which from two expressions and operand
        /// </summary>
        /// <param name="left">Left expression</param>
        /// <param name="operand">Operand</param>
        /// <param name="right">Right expression</param>
        /// <returns>Returns expression, if default behavior is expected must return null</returns>
        System.Linq.Expressions.Expression GetExpression(System.Linq.Expressions.Expression left, string operand, System.Linq.Expressions.Expression right);
    }
}
