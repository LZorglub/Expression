using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an expression evaluator
    /// </summary>
    public class ExpressionEval : IExpression
    {
        private readonly CaseSensitivity caseSensitivity;
        private readonly OperatorType operatorType;

        private readonly ExpressionArguments arguments;
        private object bTree;

        /// <summary>
        /// Represents the method that handles the user expression event
        /// </summary>
        public event UserExpressionEventHandler UserExpressionEventHandler;

        /// <summary>
        /// Represents the method that handles the user function event
        /// </summary>
        public event UserFunctionEventHandler UserFunctionEventHandler;

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionEval"/>
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="caseSensitivity">Case sensitivity</param>
        public ExpressionEval(string expression, CaseSensitivity caseSensitivity = CaseSensitivity.UserConstants & CaseSensitivity.UserExpression & CaseSensitivity.String & CaseSensitivity.UserFunction)
        {
            this.Expression = expression;
            this.arguments = new ExpressionArguments(caseSensitivity);
            this.arguments.PropertyChanged += OnArgumentChanged;

            this.caseSensitivity = caseSensitivity;
            this.operatorType = OperatorType.Binary;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionEval"/>
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="operatorType">Operator type</param>
        /// <param name="caseSensitivity">Case sensitivity</param>
        public ExpressionEval(string expression, OperatorType operatorType, CaseSensitivity caseSensitivity = CaseSensitivity.UserConstants & CaseSensitivity.UserExpression & CaseSensitivity.String & CaseSensitivity.UserFunction) : this(expression, caseSensitivity)
        {
            this.operatorType = operatorType;
        }

        /// <summary>
        /// Add a user variable in expression
        /// </summary>
        /// <param name="name"></param>
        public void AddVariable(string name)
        {
            this.arguments.AddVariable(name);
        }

        /// <summary>
        /// Add user variables in expression
        /// </summary>
        /// <param name="names"></param>
        public void AddVariables(string[] names)
        {
            foreach(string name in names)
                this.arguments.AddVariable(name);
        }

        /// <summary>
        /// Adds a user function in expression
        /// </summary>
        /// <param name="name"></param>
        public void AddFunctions(string name)
        {
            this.arguments.AddFunctions(name);
        }

        /// <summary>
        /// Add a user constant in expression
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddConstant(string name, object value)
        {
            this.arguments.AddConstants(name, value);
        }

        /// <summary>
        /// Occurs when a argument is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnArgumentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bTree = null;
        }

        /// <summary>
        /// Evaluate the expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            return this.Evaluate(Guid.Empty);
        }

        /// <summary>
        /// Evaluate the expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate(Guid correlationId) {
			this.Parse();

			if (bTree != null) return EvaluateObject(bTree, correlationId);
			return null;
		}

        /// <summary>
        /// Parse the expression
        /// </summary>
        /// <returns></returns>
		public object Parse() {
            object tempTree = bTree;

            if (tempTree == null)
            {
				ExpressionParser parser = new ExpressionParser(this.Expression, this.arguments, this.caseSensitivity, this.operatorType);

                parser.UserExpressionEventHandler += OnUserExpressionEvaluated;
                parser.UserFunctionEventHandler += OnUserFunctionEvaluated;

                try
                {
                    tempTree = parser.Evaluate();
                    bTree = tempTree;
                }
                finally
                {
                    parser.UserExpressionEventHandler -= OnUserExpressionEvaluated;
                    parser.UserFunctionEventHandler -= OnUserFunctionEvaluated;
                }
			}
            return tempTree;
		}

        /// <summary>
        /// Reduce the expression
        /// </summary>
        /// <returns></returns>
        public object Reduce()
        {
            this.Parse();

            if (bTree != null)
            {
                if (bTree is IExpression)
                {
                    return ((IExpression)bTree).Reduce();
                }
                else
                    return bTree;
            }
            return null;
        }

        /// <summary>
        /// Evaluation d'un noeud du bTree
        /// </summary>
        /// <param name="node">Noeud à évaluer</param>
        /// <param name="correlationId"></param>
        /// <returns>Valeur de l'objet évalué</returns>
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
        /// Occurs when an evaluation of user expression is required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserExpressionEvaluated(object sender, UserExpressionEventArgs e)
        {
            this.UserExpressionEventHandler?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when an evaluation of user function is required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserFunctionEvaluated(object sender, UserFunctionEventArgs e)
        {
            this.UserFunctionEventHandler?.Invoke(this, e);
        }

        /// <summary>
        /// Gets the expression of parser
        /// </summary>
        public string Expression
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            this.Parse();

            if (bTree != null)
            {
                if (bTree is IExpression)
                {
                    return ((IExpression)bTree).ToPolishString();
                }
                else
                    return bTree.ToString();
            }
            return null;
        }

        /// <summary>
        /// Extracts user expression from expression
        /// </summary>
        /// <param name="expression">Expression to analyze</param>
        /// <param name="types">Types of user expression to extract</param>
        /// <returns></returns>
        public static IEnumerable<string> Extract(string expression, UserExpressionTypes types = UserExpressionTypes.Function | UserExpressionTypes.Variable)
        {
            return ExpressionParser.Extract(expression, types);
        }
    }
}
