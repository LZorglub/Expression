﻿using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Afk.Expression
{
    /// <summary>
    /// Represents a binary operator
    /// </summary>
    internal class BinaryOp
    {
        /// <summary>
        /// Gets the operator type
        /// </summary>
        public OperatorType OperatorType { get; }

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
        /// <param name="operatorType">Operator type : bianry or arithmetic</param>
        public BinaryOp(string op, OperatorType operatorType)
        {
            this.OperatorType = operatorType;
            this.Op = op; Priority = BinaryOpPriority.GetPriority(op, operatorType);
        }

        /// <summary>
        /// Evaluate the binary operation with the two speicifed operand
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="caseSensitivity"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public object DoBinaryOp(object v1, object v2, CaseSensitivity caseSensitivity, Guid correlationId)
        {
            if (v1 is IExpression tv)
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
        private object DoBinarySimpleOp(object v1, object v2, CaseSensitivity caseSensitivity, Guid correlationId)
        {
            if (v1 is IExpression tv)
                v1 = tv.Evaluate(correlationId);

            if (v1 is ICustomOperand)
            {
                object value = (v2 is IExpression) ? ((IExpression)v2).Evaluate(correlationId) : v2;
                return ((ICustomOperand)v1).HandleOperation(this.Op.ToLower(), v2, true);
            }

            // La deuxieme opérande est évalué suivant l'opérateur (les opérateurs
            // logiques ne nécessitent pas l'évaluation de v2 si v1 est déjà respecté)
            switch (this.Op.ToLower())
            {
                case "*":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    return (Convert.ToDouble(v1, CultureInfo.InvariantCulture) *
                              Convert.ToDouble(v2, CultureInfo.InvariantCulture));
                case "/":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    return (Convert.ToDouble(v1, CultureInfo.InvariantCulture) /
                              Convert.ToDouble(v2, CultureInfo.InvariantCulture));
                case "%":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    return (Convert.ToInt64(v1, CultureInfo.InvariantCulture) %
                              Convert.ToInt64(v2, CultureInfo.InvariantCulture));
                case "<<":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    return (Convert.ToInt64(v1, CultureInfo.InvariantCulture) <<
                               Convert.ToInt32(v2, CultureInfo.InvariantCulture));
                case ">>":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
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
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    return DoSpecialOperator(v1, v2, caseSensitivity);
                case "in":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null || !(v2 is object[])) return false;
                    return ((object[])v2).Contains(v1);
                case "&":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    return (Convert.ToUInt64(v1, CultureInfo.InvariantCulture) &
                              Convert.ToUInt64(v2, CultureInfo.InvariantCulture));
                case "^":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
                    if (this.OperatorType == OperatorType.Arithmetic)
                    {
                        return DoSpecialOperator(v1, v2, caseSensitivity);
                    }
                    else
                    {
                        return (Convert.ToUInt64(v1, CultureInfo.InvariantCulture) ^
                                  Convert.ToUInt64(v2, CultureInfo.InvariantCulture));
                    }
                case "|":
                    tv = v2 as IExpression;
                    if (tv != null)
                        v2 = tv.Evaluate(correlationId);
                    if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
                    if (v1 is null || v2 is null) return null;
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
                        if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
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
                        if (v2 is ICustomOperand) return ((ICustomOperand)v2).HandleOperation(Op.ToLower(), v1, false);
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
            if (v1 is null && v2 is null)
            {
                // If operands are null do special op
                switch (this.Op.ToLower())
                {
                    case "+":
                    case "-": return null;
                    case "<":
                    case ">":
                    case "<>":
                    case "!=":
                        return false;
                    case "<=":
                    case ">=":
                    case "like":
                    case "==":
                    case "=": return true;
                }
            }
            // If operand are string, concat the string
            else if (v1 is string || v2 is string)
            {
                string str1 = v1?.ToString();
                string str2 = v2?.ToString();
                string stru1, stru2;
                if ((caseSensitivity & CaseSensitivity.String) == CaseSensitivity.None)
                {
                    stru1 = str1?.ToUpper();
                    stru2 = str2?.ToUpper();
                }
                else
                {
                    stru1 = str1; stru2 = str2;
                }

                switch (this.Op.ToLower())
                {
                    case "+": return string.Format("{0}{1}", str1, str2);
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
            else if (v1 is DateTime && v2 is DateTime)
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
            else if (v1 is DateTime && IsNumeric(v2))
            {
                DateTime d1 = Convert.ToDateTime(v1, CultureInfo.CurrentCulture);
                double d2 = Convert.ToDouble(v2, CultureInfo.InvariantCulture);

                switch (this.Op.ToLower())
                {
                    case "+": return d1.AddDays(d2);
                    case "-": return d1.AddDays(-d2);
                    case "<": 
                    case "<=":
                    case ">": 
                    case ">=":
                    case "like":
                    case "==":
                    case "=": 
                    case "<>":
                    case "!=":
                        throw new ArgumentException($"Operator '{this.Op}' invalid for date and numeric.");
                }
            }
            else if (IsNumeric(v1) && v2 is DateTime)
            {
                double d1 = Convert.ToDouble(v1, CultureInfo.InvariantCulture);
                DateTime d2 = Convert.ToDateTime(v2, CultureInfo.CurrentCulture);

                switch (this.Op.ToLower())
                {
                    case "+": return d2.AddDays(d1);
                    case "-": 
                    case "<":
                    case "<=":
                    case ">":
                    case ">=":
                    case "like":
                    case "==":
                    case "=":
                    case "<>":
                    case "!=":
                        throw new ArgumentException($"Operator '{this.Op}' invalid for numeric and date.");
                }
            }
            else if (v1 is null || v2 is null)
            {
                // One operand is null, another must match a double
                switch (this.Op.ToLower())
                {
                    case "+": return null;
                    case "-": return null;
                    case "<": return false;
                    case "<=": return false;
                    case ">": return false;
                    case ">=": return false;
                    case "like":
                    case "==":
                    case "=": return false;
                    case "<>":
                    case "!=": return true;
                    case "^": return null;
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
                case "^": return Math.Pow(f1, f2);
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
                case "in":
                case "<>":
                case "!=":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value which indicates whether the expression is numeric
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            return expression is sbyte
                    || expression is byte
                    || expression is short
                    || expression is ushort
                    || expression is int
                    || expression is uint
                    || expression is long
                    || expression is ulong
                    || expression is float
                    || expression is double
                    || expression is decimal;
        }
    }
}
