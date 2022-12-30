using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents common functions (case, upper, lower...)
    /// </summary>
    class WellKnowFunctionsExpression : IExpression
    {
        /// <summary>
        /// Initialize a new isntance of <see cref="WellKnowFunctionsExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameters"></param>
        public WellKnowFunctionsExpression(string expression, object[] parameters)
        {
            this.Expression = expression;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the expression of <see cref="WellKnowFunctionsExpression"/>
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
        public object Evaluate()
        {
            return this.Evaluate(Guid.Empty);
        }

        /// <summary>
        /// Evaluate the user expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate(Guid correlationId)
        {
            // Evaluate parameters
            object[] values = EvaluateParameters(correlationId);

            switch (this.Expression.ToLower())
            {
                case "lower":
                    return values[0].ToString().ToLower();
                case "upper":
                    return values[0].ToString().ToUpper();
                case "case":
                    return PerformCase(values);
                case "replace":
                    return PerformReplace(values);
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

        /// <summary>
        /// Perform case operator
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private object PerformCase(object[] values)
        {
            if (values == null || values.Length < 3) throw new ExpressionException(string.Format("Invalid number of arguments {0}", this.Expression), 0, 0);

            var value = values[0];
            for(int index=1; index < values.Length; index+=2)
            {
                if (index + 1 > values.Length) return values[index]; // <= valeur par défaut
                if (Object.Equals(value, values[index])) return values[index + 1];
            }
            // Si on a un nombre pair d'éléments c'est qu'on a une valeur par défaut
            if (values.Length % 2 == 0) return values.Last();

            throw new ExpressionException(string.Format("No default value {0}", this.Expression), 0, 0);
        }

        /// <summary>
        /// Perform replace operator
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private object PerformReplace(object[] values)
        {
            if (values == null || values.Length < 3) throw new ExpressionException(string.Format("Invalid number of arguments {0}", this.Expression), 0, 0);
            var value = values[0].ToString();
            return value.Replace(values[1].ToString(), values[2].ToString());
        }
    }
}
