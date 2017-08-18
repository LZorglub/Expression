using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represent the delegate of <see cref="UserExpressionEventArgs"/>
    /// </summary>
    public delegate void UserExpressionEventHandler(object sender, UserExpressionEventArgs e);

    /// <summary>
    /// Represents the event for user expression
    /// </summary>
    public class UserExpressionEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new isntance of <see cref="UserExpressionEventArgs"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="correlationId"></param>
        public UserExpressionEventArgs(string name, Guid correlationId)
        {
            this.Name = name;
            this.CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the name of events
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the correlation id
        /// </summary>
        public Guid CorrelationId { get; private set; }

        /// <summary>
        /// Gets the result of user expression
        /// </summary>
        public object Result { get; set; }
    }
}
