using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.webDesigner.Nodes
{
    class cwWebDesignerTreeNodeBehaviourTreeView : cwWebDesignerTreeNodeBehaviour
    {
        //public const string CONFIG_REMOVE_IF_EMPTY = "remove-if-empty";
        public const string CONFIG_KEEP_OPEN = "keep-open";

        //public string[] allowedLayouts = new string[] {"list"};

        public cwWebDesignerTreeNodeBehaviourTreeView(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Tree view");
            allowedLayouts.Add("list");
            allowedLayouts.Add("listbycategory");
        }

        public override void setPropertiesBoxes()
        {
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Collapse by default", "Will not collapse the accordion by default", CONFIG_KEEP_OPEN, true));
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override string ToString(string layoutItemKey, string pageName, string pageType)
        {
            string keepOpen = propertiesBoxes.getPropertyBoxBoolean(CONFIG_KEEP_OPEN).Checked.ToString().ToLower();
            
            if ("single".Equals(pageType))
            {
                var x = layoutItemKey;
                return null;
                //return "cwBehaviours.cwTreeView.setTreeView('#sidetreecontrol', '#tab_process_elementaryprocess-business_hierarchy>ul', '#tab_process_activity-business_hierarchy', " + keepOpen + ");";
              //return "cwBehaviours.cwTreeView.setTreeView("#sidetreecontrol", ".indexprocess-index-area>ul:first", "#top_of_page", !searching);");
              //return "cwBehaviours.cwTreeView.setTreeView(\"" + layoutItemKey + "\", " + removeIfEmpty + ", " + keepOpen + ");";
            }
            else
            {
                var x = layoutItemKey;
               return "cwBehaviours.cwTreeView.setTreeView('#sidetreecontrol', '."+pageName+"-index-area>ul:first', '#zone_"+pageName+"', !searching);";
              //return "cwBehaviours.cwTreeView.setTreeView('#sidetreecontrol', '." + pageName + "-index-area>ul:first', '#zone_" + pageName + "', " + keepOpen + ");";
              //return "cwBehaviours.cwAccordion.setAccordion(\"" + layoutItemKey + "\", " + removeIfEmpty + ", " + keepOpen +" && !searching);";
            }
            //" + propertiesBoxes.getPropertyBoxBoolean(CONFIG_REMOVE_IF_EMPTY).ToString().ToLower() + "

        }
    }
}
