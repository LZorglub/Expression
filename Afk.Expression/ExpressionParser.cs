using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a parser of expression, the evaluation provides a binary tree of <see cref="IExpression"/>
    /// </summary>
    public class ExpressionParser : IExpression
    {
        private CaseSensitivity caseSensitivity;

        private ExpressionArguments arguments;

        private Regex regUserExpression;
        private Regex regUserFunction;
        private Regex regUserConstant;

        /// <summary>
        /// Represents the method that handles the user expression event
        /// </summary>
        public event UserExpressionEventHandler UserExpressionEventHandler;

        /// <summary>
        /// Represents the method that handles the user function event
        /// </summary>
        public event UserFunctionEventHandler UserFunctionEventHandler;

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionParser"/>
        /// </summary>
        public ExpressionParser(string expression) : this(expression, new ExpressionArguments(CaseSensitivity.UserConstants & CaseSensitivity.UserExpression & CaseSensitivity.String & CaseSensitivity.UserFunction), CaseSensitivity.UserConstants & CaseSensitivity.UserExpression & CaseSensitivity.String & CaseSensitivity.UserFunction)
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="ExpressionParser"/>
        /// </summary>
        internal ExpressionParser(string expression, ExpressionArguments arguments, CaseSensitivity caseSensitivity)
        {
            this.Expression = expression;
            this.arguments = arguments;
            this.arguments.PropertyChanged += OnArgumentChanged;

            this.caseSensitivity = caseSensitivity;
        }

        /// <summary>
        /// Occurs when an argument changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnArgumentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Variables":
                    regUserExpression = null;
                    break;
                case "Functions":
                    regUserFunction = null;
                    break;
                case "Constants":
                    regUserConstant = null;
                    break;
            }
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
        /// Evaluate the parser
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            return this.Evaluate(null);
        }

        /// <summary>
        /// Evaluate the parser
        /// </summary>
        /// <returns></returns>
        public object Evaluate(int? correlationId)
        {
            if (regUserExpression == null && this.arguments.Variables.Count > 0)
            {
                string expr = string.Join("|", this.arguments.Variables.Select(e => Regex.Escape(e)).ToArray());
                regUserExpression = new Regex(expr,
                    ((caseSensitivity & Afk.Expression.CaseSensitivity.UserExpression) == Afk.Expression.CaseSensitivity.UserExpression) ? (RegexOptions.Compiled) : (RegexOptions.Compiled | RegexOptions.IgnoreCase)
                );
            }

            if (regUserFunction == null && this.arguments.Functions.Count > 0)
            {
                string expr = "(?<Function>" + string.Join("|", this.arguments.Functions.Select(e => Regex.Escape(e)).ToArray()) + @")\s*" + DefinedRegex.FunctionParameters;
                regUserFunction = new Regex(expr,
                    ((caseSensitivity & Afk.Expression.CaseSensitivity.UserFunction) == Afk.Expression.CaseSensitivity.UserFunction) ? (RegexOptions.Compiled) : (RegexOptions.Compiled | RegexOptions.IgnoreCase)
                );
            }

            if (regUserConstant == null && this.arguments.ConstantNames.Count > 0)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach(string name in this.arguments.ConstantNames)
                {
                    if (builder.Length > 0) builder.Append("|");
                    builder.Append(Regex.Escape(name));
                }
                regUserConstant = new Regex(builder.ToString(),
                    ((caseSensitivity & Afk.Expression.CaseSensitivity.UserConstants) == Afk.Expression.CaseSensitivity.UserConstants) ? (RegexOptions.Compiled) : (RegexOptions.Compiled | RegexOptions.IgnoreCase)
                );
            }

            return BuildTree(correlationId);
        }

        /// <summary>
        /// Reduces the expression
        /// </summary>
        /// <returns></returns>
        public object Reduce()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPolishString()
        {
            object obj = this.Evaluate();

            if (obj != null)
            {
                if (obj is IExpression)
                {
                    return ((IExpression)obj).ToPolishString();
                }
                else
                    return obj.ToString();
            }
            return null;
        }

        /// <summary>
        /// Gets the bTree of current expression
        /// </summary>
        /// <returns></returns>
        private object BuildTree(int? correlationId)
        {
            Match mRet = null;
            int nIdx = 0;
            object val = null; // Node currently analysis
            object root = null; // Root node (bTree)

            string currentExpression = Expression;

            while (nIdx < currentExpression.Length)
            {
                mRet = null;

                // Ignore les caractères séparateurs (espaces, tabs, line feed, new line, carriage return)
                Match m = DefinedRegex.WhiteSpace.Match(Expression, nIdx);
                if (m.Success && m.Index == nIdx)
                {
                    nIdx += m.Length; continue;
                }

                // Comment line
                m = DefinedRegex.CommentLine.Match(Expression, nIdx);
                if (m.Success && m.Index == nIdx)
                {
                    nIdx += m.Length; continue;
                }

                // Comment block
                m = DefinedRegex.CommentBlock.Match(Expression, nIdx);
                if (m.Success && m.Index == nIdx)
                {
                    nIdx += m.Length; continue;
                }

                // Jeu de parenthèses
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.Parenthesis.Match(Expression, nIdx);
                    if (m.Success)
                    {
                        mRet = m;
                        ExpressionParser expr = new ExpressionParser(m.Groups["Parenthesis"].Value, this.arguments, this.caseSensitivity);
                        if (this.UserExpressionEventHandler != null)
                        {
                            expr.UserExpressionEventHandler += this.UserExpressionEventHandler;
                        }
                        if (this.UserFunctionEventHandler != null)
                        {
                            expr.UserFunctionEventHandler += this.UserFunctionEventHandler;
                        }
                        try
                        {
                            val = expr.Evaluate(correlationId);
                            // Un jeu de parenthèse est une entité et est prioritaire sur toute considération
                            // d'opérateur binaire
                            if (val is BinaryNode) ((BinaryNode)val).IsEntity = true;
                        }
                        catch (ExpressionException e)
                        {
                            // Pos + 1 pour la parenthèse
                            throw new ExpressionException(e.Message, nIdx + 1 + e.Index, e.Length);
                        }
                    }
                }

                // Bracket
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.Bracket.Match(Expression, nIdx);
                    if (m.Success)
                    {
                        mRet = m;
                        ArrayExpression expr = new ArrayExpression(m.Groups["Bracket"].Value, this.arguments, this.caseSensitivity);

                        if (this.UserExpressionEventHandler != null)
                        {
                            expr.UserExpressionEventHandler += this.UserExpressionEventHandler;
                        }
                        if (this.UserFunctionEventHandler != null) {
                            expr.UserFunctionEventHandler += this.UserFunctionEventHandler;
                        }

                        try
                        {
                            val = expr.Evaluate(correlationId);
                        }
                        catch (ExpressionException e)
                        {
                            // Pos + 1 pour le premier crochet
                            throw new ExpressionException(e.Message, nIdx + 1 + e.Index, e.Length);
                        }
                    }
                }

                // Opérateur unaire
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.UnaryOp.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = new UnaryOp(m.Value);
                    }
                }

                // Valeur hexadécimale
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.Hexadecimal.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = Convert.ToInt32(m.Value, 16);
                    }
                }

                // Valeur booléene
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.Boolean.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = bool.Parse(m.Value);
                    }
                }

                // Valeur DateTime
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.DateTime.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = Convert.ToDateTime(m.Groups["DateString"].Value, CultureInfo.InvariantCulture);
                    }
                }

                // Valeur numérique
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.Numeric.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        while (m.Success && ("" + m.Value == ""))
                            m = m.NextMatch();
                        if (m.Success)
                        {
                            mRet = m;
                            val = double.Parse(m.Value, CultureInfo.InvariantCulture);
                        }
                    }
                }

                // Chaine de caractère
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.String.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = m.Groups["String"].Value.Replace(string.Format("{0}{0}", DefinedRegex.QuoteCharacter), DefinedRegex.QuoteCharacter.ToString());
                    }
                }

                // Opérateur binaire
                if (mRet == null || mRet.Index > nIdx)
                {
                    m = DefinedRegex.BinaryOp.Match(Expression, nIdx);
                    if (m.Success && (mRet == null || m.Index < mRet.Index))
                    {
                        mRet = m; val = new BinaryOp(m.Value);
                    }
                }

                // Constantes
                if (mRet == null || mRet.Index > nIdx)
                {
                    if (regUserConstant != null)
                    {
                        m = regUserConstant.Match(Expression, nIdx);
                        if (m.Success && (mRet == null || m.Index < mRet.Index))
                        {
                            mRet = m;
                            val = this.arguments.GetConstantValue(m.Value);
                        }
                    }
                }

                // Variable utilisateur
                if (mRet == null || mRet.Index > nIdx)
                {
                    if (regUserExpression != null)
                    {
                        m = regUserExpression.Match(Expression, nIdx);
                        if (m.Success && (mRet == null || m.Index < mRet.Index))
                        {
                            UserExpression user = new UserExpression(m.Value);
                            if (this.UserExpressionEventHandler != null)
                                user.UserExpressionHandler += UserExpressionEventHandler;
                            mRet = m; val = user;
                        }
                    }
                }

                // Function
                if (mRet == null || mRet.Index > nIdx)
                {
                    if (regUserFunction != null)
                    {
                        m = regUserFunction.Match(Expression, nIdx);
                        if (m.Success && (mRet == null || m.Index < mRet.Index))
                        {
                            mRet = m;
                            // Function name
                            string functionName = m.Groups["Function"].Value;
                            #region Function parameters
                            ArrayExpression expr = new ArrayExpression(m.Groups["Parenthesis"].Value, this.arguments, this.caseSensitivity);

                            if (this.UserExpressionEventHandler != null)
                            {
                                expr.UserExpressionEventHandler += this.UserExpressionEventHandler;
                            }
                            if (this.UserFunctionEventHandler != null)
                            {
                                expr.UserFunctionEventHandler += this.UserFunctionEventHandler;
                            }

                            try
                            {
                                val = expr.Evaluate(correlationId);
                            }
                            catch (ExpressionException e)
                            {
                                // Pos + 1 pour le premier crochet
                                throw new ExpressionException(e.Message, nIdx + 1 + e.Index, e.Length);
                            }
                            #endregion

                            FunctionExpression fnc = new FunctionExpression(functionName, val as object[]);
                            if (this.UserFunctionEventHandler != null)
                                fnc.FunctionHandler += UserFunctionEventHandler;
                            val = fnc;
                        }
                    }
                }

                if (mRet == null)
                    throw new ExpressionException("Invalid expression construction: \"" + Expression + "\".", nIdx, Expression.Length - nIdx);

                if (mRet.Index != nIdx)
                {
                    throw new ExpressionException(
                        "Invalid token in expression: [" +
                        Expression.Substring(nIdx, mRet.Index - nIdx).Trim() + "]",
                        nIdx, mRet.Index - nIdx
                        );
                }

                // D'apres la description syntaxique l'opérateur unaire est forcement suivi d'un élément
                // numéric ou d'une parenthèse, on se sert de cette hypothese pour les tests suivants

                // Construction de l'arbre
                if (val is BinaryOp)
                {
                    // Root ne peut pas être null, la règle syntaxique interdit de reconnaitre
                    // un opérateur binaire en première position

                    // Si root is BinaryNode && obj1 is null || obj2 is null ==> exception
                    if (root is BinaryNode && (((BinaryNode)root).Operand1 == null || ((BinaryNode)root).Operand2 == null))
                    {
                        throw new Exception();
                    }
                    // Si root is BinaryNode il faut respecter les priorités des opérateurs
                    if (root is BinaryNode && !((BinaryNode)root).IsEntity && ((BinaryNode)root).Op.Priority > ((BinaryOp)val).Priority)
                    {
                        // Exemple : 1 + 3 * 4
                        BinaryNode treeNode = root as BinaryNode;
                        while (treeNode.Operand2 is BinaryNode &&
                            !((BinaryNode)treeNode.Operand2).IsEntity &&
                            ((BinaryNode)treeNode.Operand2).Op.Priority > ((BinaryOp)val).Priority)
                            treeNode = treeNode.Operand2 as BinaryNode;

                        // Priorité élevé ==> opérateur plus faible
                        BinaryNode node = new BinaryNode();
                        node.Op = val as BinaryOp;
                        node.Operand1 = treeNode.Operand2;
                        node.CaseSensitivity = this.caseSensitivity;
                        treeNode.Operand2 = node;
                        //						node.obj1 = ((BinaryNode)root).obj2;
                        //						((BinaryNode)root).obj2 = node;
                    }
                    else
                    {
                        BinaryNode node = new BinaryNode();
                        node.Op = val as BinaryOp; node.Operand1 = root;
                        node.CaseSensitivity = this.caseSensitivity;
                        root = node;
                    }
                }
                else if (val is UnaryOp)
                {
                    if (root == null)
                    {
                        root = new UnaryNode() { Op = (UnaryOp)val };
                    }
                    else
                    {
                        // La regle syntaxique impose que root est un opérateur binaire
                        BinaryNode node = root as BinaryNode;
                        while (node.Operand2 is BinaryNode) node = node.Operand2 as BinaryNode;
                        node.Operand2 = new UnaryNode() { Op = (UnaryOp)val };
                    }
                }
                else
                {
                    if (root == null)
                        root = val;
                    else if (root is BinaryNode)
                    {
                        BinaryNode node = root as BinaryNode;
                        while (node.Operand2 is BinaryNode) node = node.Operand2 as BinaryNode;

                        // Si obj2 not null ==> exception
                        if (node.Operand2 != null)
                        {
                            if (node.Operand2 is UnaryNode)
                                ((UnaryNode)node.Operand2).Operand = val;
                            //							else if (node.obj2 is BinaryNode && ((BinaryNode)node.obj2).obj2 == null)
                            //								((BinaryNode)node.obj2).obj2 = val;
                            else
                                throw new ExpressionException("Invalid expression construction: \"" + Expression + "\".", nIdx, Expression.Length - nIdx);
                        }
                        else
                        {
                            node.Operand2 = val;
                        }
                    }
                    else if (root is UnaryNode)
                    {
                        ((UnaryNode)root).Operand = val;
                    }
                    else
                    {
                        throw new ExpressionException("Invalid expression construction: \"" + Expression + "\".", nIdx, Expression.Length - nIdx);
                    }
                }

                nIdx = mRet.Index + mRet.Length;
            } // End while

            // Test des opérandes
            if (root is BinaryNode && !ValidateBinaryNode((BinaryNode)root))
            {
                throw new ExpressionException("Missing arguments", nIdx, 1);
            }

            return root;
        }

        private bool ValidateBinaryNode(BinaryNode node)
        {
            if (node.Operand1 == null || node.Operand2 == null) return false;
            if (node.Operand1 is BinaryNode && !ValidateBinaryNode((BinaryNode)node.Operand1)) return false;
            if (node.Operand2 is BinaryNode && !ValidateBinaryNode((BinaryNode)node.Operand2)) return false;
            return true;
        }
   }
}
