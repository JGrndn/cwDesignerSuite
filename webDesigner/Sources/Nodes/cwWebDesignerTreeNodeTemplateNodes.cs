using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using System.Drawing;
using Casewise.GraphAPI.API;
using System.Windows.Forms;
using Casewise.webDesigner.GUI;

namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodeTemplateNodes : cwPSFTreeNode
    {

        public cwWebDesignerTreeNodeTemplateNodes(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
            updateText("Template Nodes");
            setIconForNodeUsingIndex(6);
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            //addObjectTypesToContextMenu(menu_strip, this);
            add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeTemplateNode, cwWebDesignerGUI>("Add Template Node", Properties.Resources.image_tvicon_template_node);
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

    }
}
