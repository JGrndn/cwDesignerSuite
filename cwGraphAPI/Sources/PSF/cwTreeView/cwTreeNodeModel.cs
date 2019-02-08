using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Used to have a model as a tree node
    /// </summary>
    internal class cwTreeNodeModel : TreeNode
    {
        /// <summary>
        /// The Model
        /// </summary>
        public cwLightModel model = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwTreeNodeModel"/> class.
        /// </summary>
        /// <param name="_model">The _model.</param>
        public cwTreeNodeModel(cwLightModel _model)
            : base(_model.ToString())
        {
            model = _model;
        }
    }
}
