using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a binary operator
    /// </summary>
    internal class BinaryOp
    {
        /// <summary>
        /// Gets the binary operator
        /// </summary>
        public string Op { get; private set; }

        /// <summary>
        /// Gets the binary operator priority
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Initialize a new instance of <see cref="BinaryOp"/>
        /// </summary>
        /// <param name="op"></param>
        public BinaryOp(string op)
        {
            this.Op = op; Priority = BinaryOpPriority.GetPriority(op);
        }

        /// <summary>
        /// Evaluate the binary operation with the two speicifed operand
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="caseSensitivity"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public object DoBinaryOp(object v1, object v2, CaseSensitivity caseSensitivity, int? correlationId)
        {
            IExpression tv = v1 as IExpression;
            if (tv != null)
                v1 = tv.Evaluate(correlationId);

            if (v1 is System.Collections.ICollection)
            {
                tv = v2 as IExpression;
                if (tv != null)
                    v2 = tv.Evaluate(correlationId);

                object[] tv1 = (object[])v1;
                object[] tv2 = (object[])v2;

                if (tv1.Length != tv2.Length)
                    throw new ArgumentException("Array length invalid.");

                switch (this.Op.ToLower())
                {
                    case "*":
                    case "/":
                    case "%":
                    case "<<":
                    case ">>":
                    case "+":
                    case "-":
                    case "&":
                    case "^":
                    case "|":
                    case "and":
                    case "&&":
                    case "or":
                    case "||":
                        object[] vr = new object[tv1.Length];

                        for (int index = 0; index < tv1.Length; index++)
                        {
                            vr[index] = DoBinaryOp(tv1[index], tv2[index], caseSensitivity, correlationId);
                        }
                        return vr;

                    case "<":
                    case "<=":
                    case ">":
                    case ">=":
                    case "==":
                    case "=":
                    case "like":
                    case "<>":
                    case "!=":
                        for (int index = 0; index < tv1.Length; index++)
                        {
                            if (!Convert.ToBoolean(DoBinaryOp(tv1[index], tv2[index], caseSensitivity, correlationId)))
                                return false;
                        }
                        return true;
                }
                throw new ArgumentException("Binary Operator " + this.Op + " not defined.");
            }
            else
                return DoBinarySimpleOp(v1, v2, caseSensitivity, correlationId);
        }

        /// <summary>
        /// Evaluate binary operation on simple operand
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="caseSensitivity"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private object DoBinarySimpleOp(object v1, object v2, CaseSensitivity caseSensitivity, int? correlationId)
        {
            IExpression tv = v1 as IExpression;
            if (tv != null)
                v1 = tv.Evaluate(correlationId);

            // La deuxieme opérande est évalué suivant l'opérateur (les opérateurs
            // logiques ne nécessitent pas l'évaluation de v2 si v1 est déjà respecté)
            switch (this.Op.ToLower())
            {
                case "*":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToDouble(v1, CultureInfo.InvariantCulture) *
                              Convert.ToDouble(v2, CultureInfo.InvariantCulture));
                case "/":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToDouble(v1, CultureInfo.InvariantCulture) /
                              Convert.ToDouble(v2, CultureInfo.InvariantCulture));
                case "%":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToInt64(v1, CultureInfo.InvariantCulture) %
                              Convert.ToInt64(v2, CultureInfo.InvariantCulture));
                case "<<":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToInt64(v1, CultureInfo.InvariantCulture) <<
                               Convert.ToInt32(v2, CultureInfo.InvariantCulture));
                case ">>":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToInt64(v1, CultureInfo.InvariantCulture) >>
                               Convert.ToInt32(v2, CultureInfo.InvariantCulture));
                case "+":
                case "-":
                case "<":
                case "<=":
                case ">":
                case ">=":
                case "==":
                case "=":
                case "like":
                case "<>":
                case "!=":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return DoSpecialOperator(v1, v2, caseSensitivity);
                case "&":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToUInt64(v1, CultureInfo.InvariantCulture) &
                              Convert.ToUInt64(v2, CultureInfo.InvariantCulture));
                case "^":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToUInt64(v1, CultureInfo.InvariantCulture) ^
                              Convert.ToUInt64(v2, CultureInfo.InvariantCulture));
                case "|":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    return (Convert.ToUInt64(v1, CultureInfo.InvariantCulture) |
                              Convert.ToUInt64(v2, CultureInfo.InvariantCulture));
                case "and":
                case "&&":
                    if (!Convert.ToBoolean(v1, CultureInfo.InvariantCulture))
                        return false;
                    else
                    {
                        tv = v2 as IExpression;
                        if (tv != null)
                            v2 = tv.Evaluate(correlationId);
                        return (Convert.ToBoolean(v1, CultureInfo.InvariantCulture) &&
                            Convert.ToBoolean(v2, CultureInfo.InvariantCulture));
                    }
                case "or":
                case "||":
                    if (Convert.ToBoolean(v1, CultureInfo.InvariantCulture))
                        return true;
                    else
                    {
                        tv = v2 as IExpression;
                        if (tv != null)
                            v2 = tv.Evaluate(correlationId);
                        return (Convert.ToBoolean(v1, CultureInfo.InvariantCulture) ||
                            Convert.ToBoolean(v2, CultureInfo.InvariantCulture));
                    }
            }
            throw new ArgumentException("Binary Operator " + this.Op + " not defined.");
        }

        /// <summary>
        /// Evaluate the binary operation on specified operand
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="caseSensitivity"></param>
        /// <returns></returns>
        private object DoSpecialOperator(object v1, object v2, CaseSensitivity caseSensitivity)
        {
            // If operand are string, concat the string
            if (v1 is string || v2 is string)
            {
                string str1 = (v1 != null) ? v1.ToString() : null;
                string str2 = (v2 != null) ? v2.ToString() : null;
                string stru1, stru2;
                if ((caseSensitivity & CaseSensitivity.String) == CaseSensitivity.None)
                {
                    stru1 = (str1 != null) ? str1.ToUpper() : null;
                    stru2 = (str2 != null) ? str2.ToUpper() : null;
                }
                else
                {
                    stru1 = str1; stru2 = str2;
                }

                switch (this.Op.ToLower())
                {
                    case "+": return string.Format("{0}{1}", str1 , str2);
                    case "-": throw new ArgumentException("Operator '-' invalid for strings.");
                    case "<": return (stru1 == null || stru2 == null) ? false : stru1.CompareTo(stru2) < 0;
                    case "<=": return (stru1 == null || stru2 == null) ? false : stru1.CompareTo(stru2) < 0 || stru1 == stru2;
                    case ">": return (stru1 == null || stru2 == null) ? false : stru1.CompareTo(stru2) > 0;
                    case ">=": return (stru1 == null || stru2 == null) ? false : stru1.CompareTo(stru2) > 0 || stru1 == stru2; ;
                    case "==":
                    case "=": return (stru1 == null || stru2 == null) ? false : stru1.Equals(stru2);
                    case "like": return (stru1 == null || stru2 == null) ? false : Regex.IsMatch(stru1, "^.*" + Regex.Escape(stru2).Replace("_", ".{1}").Replace("%", ".*") + ".*$");
                    case "<>":
                    case "!=": // return (stru1 == null || stru2 == null) ? false : stru1 != stru2;
                        return stru1 != stru2;
                }
            }
            else if (v1 is DateTime || v2 is DateTime)
            {
                DateTime
                    d1 = Convert.ToDateTime(v1, CultureInfo.CurrentCulture),
                    d2 = Convert.ToDateTime(v2, CultureInfo.CurrentCulture);

                switch (this.Op.ToLower())
                {
                    case "+": throw new ArgumentException("Operator '+' invalid for dates.");
                    case "-": return d1 - d2;
                    case "<": return d1 < d2;
                    case "<=": return d1 <= d2;
                    case ">": return d1 > d2;
                    case ">=": return d1 >= d2;
                    case "like":
                    case "==":
                    case "=": return d1 == d2;
                    case "<>":
                    case "!=": return d1 != d2;
                }
            }

            double
                f1 = Convert.ToDouble(v1, CultureInfo.InvariantCulture),
                f2 = Convert.ToDouble(v2, CultureInfo.InvariantCulture);
            switch (this.Op.ToLower())
            {
                case "+": return f1 + f2;
                case "-": return f1 - f2;
                case "<": return f1 < f2;
                case "<=": return f1 <= f2;
                case ">": return f1 > f2;
                case ">=": return f1 >= f2;
                case "like":
                case "==":
                case "=": return f1 == f2;
                case "<>":
                case "!=": return f1 != f2;
            }

            throw new ArgumentException("Operator '" + this.Op + "' not specified.");
        }

        public bool IsBooleanOperator()
        {
            switch (this.Op.ToLower())
            {
                case "and":
                case "&&":
                case "or":
                case "||":
                case "<":
                case "<=":
                case ">":
                case ">=":
                case "==":
                case "=":
                case "like":
                case "<>":
                case "!=":
                    return true;
            }
            return false;
        }
    }
}
