using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afk.Expression
{
    /// <summary>
    /// Represent the delegate of <see cref="UserFunctionEventArgs"/>
    /// </summary>
    public delegate void UserFunctionEventHandler(object sender, UserFunctionEventArgs e);

    /// <summary>
    /// Represents the event for user functions
    /// </summary>
    public class UserFunctionEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new isntance of <see cref="UserExpressionEventArgs"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="correlationId"></param>
        public UserFunctionEventArgs(string name, object[] parameters, Guid correlationId)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the name of events
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the correlationId
        /// </summary>
        public Guid CorrelationId { get; private set; }

        /// <summary>
        /// Gets the parameters of event
        /// </summary>
        public object[] Parameters { get; private set; }

        /// <summary>
        /// Gets the result of user expression
        /// </summary>
        public object Result { get; set; }    
    }
}
