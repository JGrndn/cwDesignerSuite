using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// Warning
    /// </summary>
    public class cwExceptionWarning : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionWarning"/> class.
        /// </summary>
        /// <param name="_text">The _text.</param>
        public cwExceptionWarning(string _text)
            : base(_text)
        {
        }
    }
}
