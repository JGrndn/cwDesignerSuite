using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Drawing;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.Exceptions;
using System.Windows.Forms;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI;

namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodePropertiesGroup : cwPSFTreeNode
    {

        //public const string CONFIG_LAYOUT_LIST = "tab";
        public const string CONFIG_LAYOUT_DESIGN = "layout-design";
        public const string CONFIG_SELECTED_PROPERTIES = "selected-properties";


        public cwWebDesignerTreeNodePropertiesGroup(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
            updateText("Properties Group");
            ForeColor = Color.DarkSeaGreen;
            setIconForNodeUsingIndex(9);
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns></returns>
        public cwPSFTreeNodeObjectNode getParentNode()
        {
            cwPSFTreeNodeObjectNode parentOT = getParent() as cwPSFTreeNodeObjectNode;
            if (parentOT != null)
            {
                return parentOT;
            }
            else
            {
                cwWebDesignerTreeNodeTab tab = getParent() as cwWebDesignerTreeNodeTab;
                if (tab != null)
                {
                    parentOT = tab.getParent() as cwPSFTreeNodeObjectNode;
                    if (parentOT != null)
                    {
                        return parentOT;
                    }
                }
            }
            throw new cwExceptionFatal("The parent of a Properties Group node [" + getName() + "] should be an Object Group Node or a Tab with a valid parent");
        }

        public string ID
        {
            get
            {
                return cwTools.stringToID(getName());
            }
        }

        public List<cwLightProperty> getCheckedPropertiesList()
        {
            cwPSFPropertyBoxTreeViewPaneProperties selectedProperties = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewPaneProperties>(CONFIG_SELECTED_PROPERTIES);
            return selectedProperties.getCheckedPropertiesList();
        }

        public override void setPropertiesBoxes()
        {
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).helpName = "Property Group Name";
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxComboBox("Layout", "Layout", CONFIG_LAYOUT_DESIGN, new string[] { "list", "table", "memotext", "Property Box", "Property Box(hyperlink)"}));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_DESIGN).setValue("table");
            cwPSFPropertyBoxTreeViewPaneProperties _selectedProperties = new cwPSFPropertyBoxTreeViewPaneProperties("Selected Properties", "Select the properties to export for the Web Designer", CONFIG_SELECTED_PROPERTIES, null);
            cwPSFTreeNodeObjectNode parentOG = getParentNode() as cwPSFTreeNodeObjectNode;
            _selectedProperties.loadNodes(parentOG.getSelectedObjectType());
            propertiesBoxes.addPropertyBox(_selectedProperties);
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

        public override void OnClick()
        {
            
        }
        
    }
}
