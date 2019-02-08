using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;

namespace Casewise.webDesigner.Nodes
{
    class cwWebDesignerTreeNodeBehaviourWorldMap : cwWebDesignerTreeNodeBehaviour
    {
        public cwWebDesignerTreeNodeBehaviourWorldMap(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("World Map");
            allowedLayouts.Add("list");
            allowedLayouts.Add("list-box");
        }

        public override void setPropertiesBoxes()
        {
        }
        
        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override string ToString(string layoutItemKey, string pageName, string pageType)
        {
            cwWebDesignerTreeNodeLayout layout = getParentLayout();
            cwPSFTreeNodeObjectNode parentNode = layout.getParentNode();
            string nodeID = parentNode.ID;
            string layoutKey = layout.getStringProperty(cwWebDesignerTreeNodeLayout.CONFIG_LAYOUT_NAME);

            string s = "";
            switch (layoutKey)
            {
                case "list":
                    cwWebDesignerTreeNodeObjectNodeAssociationType ATNode = parentNode.getFirstChildNode<cwWebDesignerTreeNodeObjectNodeAssociationType>();
                    s += "_.each(all_items." + nodeID + ", function (o) {";
                    s += "cwBehaviours.cwWorldMap.countriesToMapFromItems(\"" + ATNode.ID + "-\" + o.object_id, \"world-map-\" + o.object_id, o.associations." + ATNode.ID + ");";
                    s += "});";
                    break;
                case "list-box":
                    s += "cwBehaviours.cwWorldMap.countriesToMapFromItems(\"" + parentNode.ID + "\", \"world-map-" + parentNode.ID + "\", " + pageName + ".associations." + parentNode.ID + ");";
                    break;
            }


            return s;
        }
    }
}
