using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// Fatal Exception
    /// </summary>
    public class cwExceptionFatal: Exception
    {
        private string p;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionFatal"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        public cwExceptionFatal(string p)
            :base(p)
        {            
            this.p = p;
        }
    }
}
