using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Used to have a pan as a tree node
    /// </summary>
    internal class cwTreeNodePane : TreeNode
    {

        /// <summary>
        /// The Pane
        /// </summary>
        public cwLightPane pane = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwTreeNodePane"/> class.
        /// </summary>
        /// <param name="pane">The pane.</param>
        public cwTreeNodePane(cwLightPane pane)
            : base(pane.ToString())
        {
            this.pane = pane;
        }
    }
}
