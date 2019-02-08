using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Drawing;
using Casewise.GraphAPI.API;
using System.Windows.Forms;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI;

namespace Casewise.webDesigner.Nodes
{
    public class cwWebDesignerTreeNodeTab : cwPSFTreeNode
    {

        //public const string CONFIG_LAYOUT_TAB = "tab";
        public const string CONFIG_TAB_ICON = "tab_icon";
        public const string CONFIG_TAB_ID = "tab-id";
        public const string CONFIG_TAB_HIDE = "tab_hide";


        public cwWebDesignerTreeNodeTab(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
            updateText("Tab");
            ForeColor = Color.Chocolate;
            setIconForNodeUsingIndex(8);
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();

            cwPSFTreeNodeObjectNode parentNode = getParent() as cwPSFTreeNodeObjectNode;
            if (parentNode != null)
            {
                parentNode.addAssociationTypesToContextMenu<cwWebDesignerTreeNodeObjectNodeAssociationType, cwWebDesignerGUI>(menu_strip, this, null);
                add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodePropertiesGroup, cwWebDesignerGUI>("Add Properties Group", Properties.Resources.image_tvicon_propertiesgroup);
            }
            else
            {
                cwWebDesignerTreeNodePage parentPage = getParent() as cwWebDesignerTreeNodePage;
                if (parentPage != null)
                {
                    cwPSFTreeNodeObjectNode.addObjectTypesToContextMenu<cwWebDesignerTreeNodeObjectNodeObjectType, cwWebDesignerGUI>(menu_strip, this, operationEditModeGUI.Model, null);
                }
            }
           
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }


        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID {
            get {
                return getStringProperty(CONFIG_TAB_ID).ToString();
            }
        }

        public override void setPropertiesBoxes()
        {
            cwPSFPropertyBoxString id = new cwPSFPropertyBoxString("ID", "ID used for the tab functions identifier, do not change this name if you have done custom developpments", CONFIG_TAB_ID);
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).helpName = "Tab Name";
            id.setValue(cwTools.stringToID(getName()));
            propertiesBoxes.addPropertyBox(id);

            cwPSFPropertyBoxString icon = new cwPSFPropertyBoxString("Icon", "Icon for the tab, please report to http://jqueryui.com/themeroller/ for all available icon tags", CONFIG_TAB_ICON);
            icon.setValue("info");
            propertiesBoxes.addPropertyBox(icon);

            cwPSFPropertyBoxCheckBox hideTab = new cwPSFPropertyBoxCheckBox("Hide Tab", "hide tab if the content is empty",CONFIG_TAB_HIDE, false);
            propertiesBoxes.addPropertyBox(hideTab);
            //propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Disable Page", "Don't generate this page", CONFIG_ADD_DISABLE_PAGE, false));

        }

        /// <summary>
        /// Gets the children object nodes.
        /// </summary>
        /// <returns></returns>
        public List<cwPSFTreeNodeObjectNode> getChildrenObjectNodes()
        {
            return getChildrenGeneric<cwPSFTreeNodeObjectNode>();
        }
    }
}
;