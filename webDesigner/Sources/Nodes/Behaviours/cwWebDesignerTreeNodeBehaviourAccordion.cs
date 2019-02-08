using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.webDesigner.Nodes
{
    class cwWebDesignerTreeNodeBehaviourAccordion : cwWebDesignerTreeNodeBehaviour
    {
        public const string CONFIG_REMOVE_IF_EMPTY = "remove-if-empty";
        public const string CONFIG_KEEP_OPEN = "keep-open";

        //public string[] allowedLayouts = new string[] {"list"};

        public cwWebDesignerTreeNodeBehaviourAccordion(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Accordion");
            allowedLayouts.Add("list");
            allowedLayouts.Add("listbycategory");
            allowedLayouts.Add("list-box");
        }

        public override void setPropertiesBoxes()
        {
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Remove accordion header if empty", "Removes the accordion node if the accordion has no children", CONFIG_REMOVE_IF_EMPTY, true));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Collapse by default", "Will not collapse the accordion by default", CONFIG_KEEP_OPEN, true));
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override string ToString(string layoutItemKey, string pageName, string pageType)
        {
            string removeIfEmpty = propertiesBoxes.getPropertyBoxBoolean(CONFIG_REMOVE_IF_EMPTY).Checked.ToString().ToLower();
            string keepOpen = propertiesBoxes.getPropertyBoxBoolean(CONFIG_KEEP_OPEN).Checked.ToString().ToLower();
            
            if ("single".Equals(pageType))
            {
                return "cwBehaviours.cwAccordion.setAccordion(\"" + layoutItemKey + "\", " + removeIfEmpty + ", " + keepOpen + ");";
            }
            else
            {
                return "cwBehaviours.cwAccordion.setAccordion(\"" + layoutItemKey + "\", " + removeIfEmpty + ", " + keepOpen +" && !searching);";
            }
            //" + propertiesBoxes.getPropertyBoxBoolean(CONFIG_REMOVE_IF_EMPTY).ToString().ToLower() + "

        }
    }
}
