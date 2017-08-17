using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an user expression
    /// </summary>
    class UserExpression : IExpression
    {
        /// <summary>
        /// Represents the method that handles the user expression event
        /// </summary>
        public event UserExpressionEventHandler UserExpressionHandler;

        /// <summary>
        /// Initialize a new isntance of <see cref="UserExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        public UserExpression(string expression)
        {
            this.Expression = expression;
        }

        /// <summary>
        /// Gets the expression of <see cref="UserExpression"/>
        /// </summary>
        public string Expression
        {
            get;
            private set;
        }

        /// <summary>
        /// Evalate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            return this.Evaluate(null);
        }

        /// <summary>
        /// Evalate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate(int? correlationId)
        {
            if (UserExpressionHandler != null)
            {
                UserExpressionEventArgs e = new UserExpressionEventArgs(this.Expression, correlationId);
                UserExpressionHandler(this, e);
                return e.Result;
            }

            throw new ExpressionException(string.Format("Unable to evaluate expression {0}", this.Expression), 0, 0);
        }

        /// <summary>
        /// Reduce an user expression
        /// </summary>
        /// <returns></returns>
        public object Reduce() {
            // User expression can not be reduce, coz can change
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            return "[UserExpr]";
        }
    }
}
