using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using System.Drawing;
using Casewise.webDesigner.GUI;


namespace Casewise.webDesigner.Nodes
{
    public class cwWebDesignerTreeNodeInterfaceLayout : cwPSFTreeNode
    {
        public cwWebDesignerTreeNodeInterfaceLayout(cwWebDesignerGUI _GUI, cwPSFTreeNode parent)
            : base(_GUI, parent)
        {
            updateText("ILayout");
            ForeColor = Color.Green;
            setIconForNodeUsingIndex(2);
        }

        public override void setPropertiesBoxes()
        {
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <returns></returns>
        public cwWebDesignerTreeNodeObjectNode getParentNode()
        {
            return getParent() as cwWebDesignerTreeNodeObjectNode;
        }
    }
}
