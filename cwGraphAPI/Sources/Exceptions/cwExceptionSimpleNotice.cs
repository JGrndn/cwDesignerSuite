using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// Simple notice exception
    /// </summary>
    public class cwExceptionSimpleNotice: Exception
    {
        private string p;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionSimpleNotice"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        public cwExceptionSimpleNotice(string p)
            : base (p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

    }
}
