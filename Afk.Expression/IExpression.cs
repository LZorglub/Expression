﻿using System;

namespace Afk.Expression
{
    /// <summary>
    /// Represents an expression
    /// </summary>
    public interface IExpression
    {
        /// <summary>
        /// Gets the expression
        /// </summary>
        string Expression { get; }

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns></returns>
        object Evaluate();

        /// <summary>
        /// Evaluates the expression using the specified correlation id
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        object Evaluate(Guid correlationId);

        /// <summary>
        /// Reduces the expression
        /// </summary>
        /// <returns></returns>
        object Reduce();

        /// <summary>
        /// Gives representation of expression
        /// </summary>
        /// <returns></returns>
        string ToPolishString();
    }
}
