using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Used to have an Association type as a tree node
    /// </summary>
    internal class cwTreeNodeLightAssociationType : TreeNode
    {

        /// <summary>
        /// The Association type
        /// </summary>
        public cwLightAssociationType AST = null;
        /// <summary>
        /// The ScriptName
        /// </summary>
        public string ScriptName = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwTreeNodeLightAssociationType"/> class.
        /// </summary>
        /// <param name="_AST">The _ AST.</param>
        public cwTreeNodeLightAssociationType(cwLightAssociationType _AST)
            : base(_AST.ToString())
        {
            AST = _AST;
        }
    }
}
