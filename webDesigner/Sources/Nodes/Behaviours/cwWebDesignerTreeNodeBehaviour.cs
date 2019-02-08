using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using System.Windows.Forms;
using System.Drawing;
using Casewise.webDesigner.GUI;

namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodeBehaviour : cwPSFTreeNode
    {

        public List<string> allowedLayouts = new List<string>();

        public cwWebDesignerTreeNodeBehaviour(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Behaviour");
            setIconForNodeUsingIndex(10);
        }

        public override void setPropertiesBoxes() {}

        public virtual string ToString(string layoutItemKey, string pageName, string pageType)
        {
            throw new NotImplementedException("ToString(string layoutItemKey, string pageName, string pageType)");
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

        public cwWebDesignerTreeNodeLayout getParentLayout()
        {
            return getParent() as cwWebDesignerTreeNodeLayout;
        }

        /// <summary>
        /// Determines whether [is layout allowed for behaviour] [the specified layout name].
        /// </summary>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns>
        ///   <c>true</c> if [is layout allowed for behaviour] [the specified layout name]; otherwise, <c>false</c>.
        /// </returns>
        public bool isLayoutAllowedForBehaviour(string layoutName)
        {
            return allowedLayouts.Contains(layoutName);
        }

        public ToolStripItem getToolStripItem(cwWebDesignerTreeNodeLayout layoutNode)
        {
            ToolStripItem behaviour_item = menu_strip.Items.Add(getName());
            behaviour_item.Click += (sender, args) => ctx_addBehaviourToLayoutOnClick(layoutNode, args);
            return behaviour_item;
        }

        private void ctx_addBehaviourToLayoutOnClick(cwWebDesignerTreeNodeLayout layoutNode, EventArgs e)
        {
            layoutNode.addChildNodeFirst(this);
        }
    }
}
