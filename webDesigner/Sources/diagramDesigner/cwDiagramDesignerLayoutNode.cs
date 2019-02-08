using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;

namespace Casewise.webDesigner.Nodes
{
    public class cwDiagramDesignerLayoutNode : cwWebDesignerTreeNodeInterfaceLayout
    {
        public cwDiagramDesignerLayoutNode(cwWebDesignerGUI _GUI, cwPSFTreeNode parent)
            : base(_GUI, parent)
        {
        }

        public override void setPropertiesBoxes()
        {
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

        public void addLayoutNode(cwWebDesignerWriter layout_file, string nodeID, int level, List<string> childrenNames)
        {
            float paddingRight = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_PADDING_RIGHT).update_value * 60;
            float paddingLeft = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_PADDING_LEFT).update_value * 60;
            float paddingTop = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_PADDING_TOP).update_value * 60;
            float paddingBottom = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_PADDING_BOTTOM).update_value * 60;
            float spaceX = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_SPACE_X).update_value * 60;
            float spaceY = propertiesBoxes.getPropertyBoxFloat(cwDiagramDesignerLayoutIncludeType.CONFIG_SPACE_Y).update_value * 60;
            int columns = propertiesBoxes.getPropertyBoxInt(cwDiagramDesignerLayoutIncludeType.CONFIG_MAX_COLUMNS).update_value;

            layout_file.writeToFile(nodeID + " = DiagramDesignerAPI.diagramDesignerCreateIncludeNode('" + nodeID + "', " + level + ", " + columns + ", " + paddingTop + ", " + paddingBottom + ", " + paddingLeft + ", " + paddingRight + ", " + spaceX + ", " + spaceY + ", [" + cwTools.stringToStringSeparatedby(",", childrenNames, false) + "]);");
        }
    }
}
