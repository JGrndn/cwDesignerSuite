using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Used to have a property as a tree node
    /// </summary>
    internal class cwTreeNodeProperty : TreeNode
    {

        /// <summary>
        /// The property
        /// </summary>
        public cwLightProperty Property = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwTreeNodeProperty"/> class.
        /// </summary>
        /// <param name="_Property">The _ property.</param>
        public cwTreeNodeProperty(cwLightProperty _Property)
            : base(_Property.ToString())
        {
            Property = _Property;
        }
    }
}
