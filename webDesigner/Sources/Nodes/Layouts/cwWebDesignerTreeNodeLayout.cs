using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Operations.Web;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.API;

namespace Casewise.webDesigner.Nodes
{
    public class cwWebDesignerTreeNodeLayout : cwWebDesignerTreeNodeInterfaceLayout
    {

        private Dictionary<string, string> properties = new Dictionary<string, string>();
        public const string CONFIG_LAYOUT_DESIGN_SET_LINK = "set-link-to-object";
        public const string CONFIG_LAYOUT_JS_CLASS_NAME = "js-class-name";
        public const string CONFIG_LAYOUT_NAME = "layout-name";
        public const string CONFIG_LAYOUT_LINK_NAME = "link-name";
        public const string CONFIG_LAYOUT_ID = "layout-id";
        //public const string CONFIG_LAYOUT_MERGE = "merge-tree-leaf";
        public const string CONFIG_LAYOUT_ENABLE_BEHAVIOUR = "set-behaviour";


        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerTreeNodeLayout"/> class.
        /// </summary>
        /// <param name="_cwWebIndexGUI">The _CW web index GUI.</param>
        /// <param name="_parent">The _parent.</param>
        /// <param name="properties">The properties.</param>
        public cwWebDesignerTreeNodeLayout(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent, Dictionary<string, string> properties)
            : base(_cwWebIndexGUI, _parent)
        {
            this.properties = properties;
            
            updateText("Layout " + getProperty("display-name"));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_JS_CLASS_NAME).setValue(getProperty("js-class-name"));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_NAME).setValue(getProperty("name"));
            _parent.layoutName = getProperty("js-class-name");

            setEnableBehaviourValue();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerTreeNodeLayout"/> class.
        /// </summary>
        /// <param name="_cwWebIndexGUI">The _CW web index GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwWebDesignerTreeNodeLayout(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Layout");
            setEnableBehaviourValue();
        }

        private void setEnableBehaviourValue()
        {
            string val = "false";
            properties.TryGetValue("enable-behaviour", out val);
            if (!string.IsNullOrEmpty(val) && val.ToLower().Equals("true"))
            {
                cwPSFPropertyBoxCheckBox enableBehaviour = propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_ENABLE_BEHAVIOUR) as cwPSFPropertyBoxCheckBox;
                if (enableBehaviour != null)
                {
                    enableBehaviour.setValue(val);
                }
            }
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }


        private void addBehavioursToLayoutContextMenu(ContextMenuStrip contextMenu)
        {
            ToolStripMenuItem c = new ToolStripMenuItem("Add Behaviours");
            cwWebDesignerBehaviourManager behaviourManager = new cwWebDesignerBehaviourManager(getGUI<cwWebDesignerGUI>());
            foreach (cwWebDesignerTreeNodeBehaviour behaviour in behaviourManager.getAllBehaviours())
            {
                if (this.isBehaviorEnabled())
                {
                    c.DropDownItems.Add(behaviour.getToolStripItem(this));
                }
            }
            //cwWebDesignerBehaviourManager behaviourManager = new cwWebDesignerBehaviourManager(getGUI<cwWebDesignerGUI>());
            //foreach (cwWebDesignerTreeNodeBehaviour behaviour in behaviourManager.getBehavioursForLayout(ToString()))
            //{
            //    if (behaviour.getParentLayout().isBehaviorEnabled())
            //    {
            //        c.DropDownItems.Add(behaviour.getToolStripItem(this));
            //    }
            //}
            if (c.DropDownItems.Count > 0)
            {
                contextMenu.Items.Add(c);
            }
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();

            if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeUsedTemplateNode>())
            {
                addTemplateNodesToContextMenu(objectType);
            }

            addBehavioursToLayoutContextMenu(menu_strip);
            addSwitchLayoutOptionToContextMenu(menu_strip);
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

        public void addSwitchLayoutOptionToContextMenu(ContextMenuStrip contextMenu)
        {
            cwWebDesignerLayoutManager LayoutManager = getGUI<cwWebDesignerGUI>().layoutManager;
            ToolStripMenuItem menuItem = new ToolStripMenuItem("Switch Layout");
            foreach (Dictionary<string, string> layoutProperties in LayoutManager.layoutsProperties.Values)
            {
                cwWebDesignerTreeNodeLayout layout = new cwWebDesignerTreeNodeLayout(getGUI<cwWebDesignerGUI>(), getParent(), layoutProperties);
                ToolStripItem item = menuItem.DropDownItems.Add(layout.getProperty("display-name"));
                item.Click += (sender, args) => ctx_switchLayout(getParent() as cwWebDesignerTreeNodeObjectNode, layout, args);
                //item.Click += (sender, args) => ctx_switchChildLayout(layout, args);
            }
            menu_strip.Items.Add(menuItem);
        }

        public void addTemplateNodesToContextMenu(cwLightObjectType selectedObjectType)
        {
            cwWebDesignerTreeNodeSite siteNode = getParentNode().getParentSiteNode();
            cwWebDesignerTreeNodeTemplateNodes templateNodes = siteNode.getChildrenGeneric<cwWebDesignerTreeNodeTemplateNodes>().First();
            List<cwWebDesignerTreeNodeTemplateNode> templateNodesItems = templateNodes.getChildrenGeneric<cwWebDesignerTreeNodeTemplateNode>();

            List<cwWebDesignerTreeNodeTemplateNode> matchedTemplateNodes = new List<cwWebDesignerTreeNodeTemplateNode>();

            for (int i = 0; i < templateNodesItems.Count; ++i)
            {
                cwWebDesignerTreeNodeTemplateNode templateNode = templateNodesItems[i];
                if (selectedObjectType.ScriptName.Equals(templateNode.getSelectedObjectType().ScriptName))
                {
                    matchedTemplateNodes.Add(templateNode);
                }
            }

            if (matchedTemplateNodes.Count > 0)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Add Template Node");
                for (int i = 0; i < matchedTemplateNodes.Count; ++i)
                {
                    cwWebDesignerTreeNodeTemplateNode templateNode = matchedTemplateNodes[i];
                    ToolStripItem item = menuItem.DropDownItems.Add(templateNode.getName());
                    item.Click += (sender, args) => ctx_addChildTemplateNode(templateNode, args);
                }
                menu_strip.Items.Add(menuItem);
            }

        }

        /// <summary>
        /// Ctx_adds the child template node.
        /// </summary>
        /// <param name="templateNode">The template node.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_addChildTemplateNode(cwWebDesignerTreeNodeTemplateNode templateNode, EventArgs e)
        {

            cwWebDesignerTreeNodeUsedTemplateNode usedTemplate = new cwWebDesignerTreeNodeUsedTemplateNode(getGUI<cwWebDesignerGUI>(), this);
            usedTemplate.updateTemplateName(templateNode.getName());
            addChildNodeFirst(usedTemplate);
            Expand();
        }

        private void ctx_switchLayout(cwWebDesignerTreeNodeObjectNode parent, cwWebDesignerTreeNodeLayout layout, EventArgs e)
        {
            parent.removeChild(this);
            parent.addChildNodeFirst(layout);
            parent.checkNodeStructureRec();

            Expand();
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string getProperty(string key)
        {
            return properties[key];
        }

        public override string ToString()
        {
            return getStringProperty(CONFIG_LAYOUT_NAME);
        }

        public cwLightObjectType objectType
        {
            get
            {
                return getParentNode().getSelectedObjectType();
            }
        }



        /// <summary>
        /// Determines whether [is set link equals to true].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is set link equals to true]; otherwise, <c>false</c>.
        /// </returns>
        public bool isSetLinkEqualsToTrue()
        {
            return propertiesBoxes.getPropertyBoxBoolean(CONFIG_LAYOUT_DESIGN_SET_LINK).Checked;
        }

        public bool isBehaviorEnabled()
        {
            return propertiesBoxes.getPropertyBoxBoolean(CONFIG_LAYOUT_ENABLE_BEHAVIOUR).Checked;
        }

        public string getLinkViewName()
        {
            return propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_LINK_NAME).Text;
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Set Link", "Set a link to the object", CONFIG_LAYOUT_DESIGN_SET_LINK, true));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("JS Class Name", "The name of the JS Class Object.", CONFIG_LAYOUT_JS_CLASS_NAME));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_JS_CLASS_NAME).disable();
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Layout Name", "The name of the layout, used also as a key for the layout.", CONFIG_LAYOUT_NAME));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_NAME).disable();
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Target View Name", "Set the name of the view you want to be redirected to", CONFIG_LAYOUT_LINK_NAME));
            //propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Set Object To Be Merged", "Set the scriptname of the object type to be merged", CONFIG_LAYOUT_MERGE));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Enable behaviours", "", CONFIG_LAYOUT_ENABLE_BEHAVIOUR, false));
            propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_ENABLE_BEHAVIOUR).disable();
        }


    }
}
