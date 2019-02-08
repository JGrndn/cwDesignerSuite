using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Drawing;
using Casewise.GraphAPI.API;
using System.Windows.Forms;


namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodeTemplateNode : cwPSFTreeNode
    {

        public cwWebDesignerTreeNodeTemplateNode(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
            updateText("Template Node");
            setIconForNodeUsingIndex(7);
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            //addObjectTypesToContextMenu(menu_strip, this);
            add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeObjectNodeObjectType, cwWebDesignerGUI>("Add Object Type", Casewise.GraphAPI.Properties.Resources.image_tvicon_node);
            //addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override void setPropertiesBoxes()
        {
        }

        public cwLightObjectType getSelectedObjectType()
        {
            cwPSFTreeNodeObjectNode node = getChildrenGeneric<cwWebDesignerTreeNodeObjectNodeObjectType>().First();
            return node.getSelectedObjectType();
        }

        public cwWebDesignerTreeNodeInterfaceLayout getFirstNodeLayout()
        {
            return getFirstChildNode<cwWebDesignerTreeNodeObjectNode>().getLayout();
        }


    }
}
