using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// The node do not respect defined rules
    /// </summary>
    public class cwExceptionNodeValidation: Exception
    {
        private string p;
        /// <summary>
        /// errorNode
        /// </summary>
        public cwPSFTreeNode errorNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionNodeValidation"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="errorNode">The error node.</param>
        public cwExceptionNodeValidation(string p, cwPSFTreeNode errorNode)
            : base (p)
        {
            this.p = p;
            this.errorNode = errorNode;
            if (errorNode != null)
            {
                errorNode.Tag = p;
            }
           
        }

    }
}
