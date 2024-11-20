using System;
using System.Collections.Generic;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a function
    /// </summary>
    class FunctionExpression : IExpression
    {
        public event UserFunctionEventHandler FunctionHandler;

        /// <summary>
        /// Initialize a new instance of <see cref="FunctionExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameters"></param>
        public FunctionExpression(string expression, object[] parameters)
        {
            this.Expression = expression;
            this.Parameters = parameters;
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
        /// Gets the function parameters
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Evaluate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate() {
            return this.Evaluate(Guid.Empty);
        }

        /// <summary>
        /// Evaluate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate(Guid correlationId)
        {
            if (FunctionHandler != null)
            {
                // Evaluate parameters
                object[] values = EvaluateParameters(correlationId);

                UserFunctionEventArgs e = new UserFunctionEventArgs(this.Expression, values, correlationId);
                FunctionHandler(this, e);
                return e.Result;
            }

            throw new ExpressionException(string.Format("Unable to evaluate expression {0}", this.Expression), 0, 0);
        }

        /// <summary>
        /// Reduces the function expression
        /// </summary>
        /// <returns></returns>
        public object Reduce()
        {
            return this;
        }

        /// <summary>
        /// Evaluates the function parameters
        /// </summary>
        /// <returns></returns>
        private object[] EvaluateParameters(Guid correlationId)
        {
            if (this.Parameters == null) return null;
            if (this.Parameters.Length == 0) return new object[] { };

            List<object> values = new List<object>();
            foreach (var item in this.Parameters)
            {
                values.Add(EvaluateObject(item, correlationId));
            }
            return values.ToArray();
        }

        /// <summary>
        /// Evaluates a object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private object EvaluateObject(object node, Guid correlationId)
        {
            if (node is IExpression)
                return ((IExpression)node).Evaluate(correlationId);
            else if (node is object[])
            {
                for (int i = 0; i < ((object[])node).Length; i++)
                    ((object[])node)[i] = EvaluateObject(((object[])node)[i], correlationId);
                return node;
            }
            else
                return node;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            return "[FuncExpr]";
        }
    }
}
