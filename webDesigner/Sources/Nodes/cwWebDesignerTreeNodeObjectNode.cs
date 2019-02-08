using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.Operations.Web;
using Casewise.GraphAPI.API;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;
using Casewise.webDesigner.Libs;

namespace Casewise.webDesigner.Nodes
{
    public class cwWebDesignerTreeNodeObjectNode : cwPSFTreeNodeObjectNode
    {
        public cwWebDesignerTreeNodeObjectNode(cwWebDesignerGUI _cwEditModeGUI, cwPSFTreeNode _parent)
            : base(_cwEditModeGUI, _parent)
        {
            updateText("Web Designer Object Node");
            cwWebDesignerTreeNodeLayout defaultLayout = LayoutManager.getLayoutNodeByKeyName("list", this);
            if (defaultLayout != null)
            {
                addChildNodeLast(defaultLayout);
            }
            setIconForNodeUsingIndex(1);
        }

        public override void setPropertiesBoxes()
        {
            base.setPropertiesBoxes();
        }

        public cwWebDesignerLayoutManager LayoutManager
        {
            get
            {
                return getGUI<cwWebDesignerGUI>().layoutManager;
            }
        }

    

        public List<T> getAllChildrenGeneric<T>() where T : cwPSFTreeNode
        {
            List<T> nodes = new List<T>();
            nodes.AddRange(getChildrenGeneric<T>());
            nodes.AddRange(getChildrenGenericFromTabs<T>());
            return nodes;
        }

        public List<cwPSFTreeNodeObjectNode> getAllChildrenNodes()
        {
            List<cwPSFTreeNodeObjectNode> nodes = new List<cwPSFTreeNodeObjectNode>();
            nodes.AddRange(getChildrenObjectNodes());
            nodes.AddRange(getChildrenNodesFromTabs());
            return nodes;
        }

        protected void addLayoutsToContextMenu()
        {
            if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeLayout>())
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Add Layout");
                foreach (Dictionary<string, string> layoutProperties in LayoutManager.layoutsProperties.Values)
                {
                    cwWebDesignerTreeNodeLayout layout = new cwWebDesignerTreeNodeLayout(getGUI<cwWebDesignerGUI>(), getParent(), layoutProperties);
                    ToolStripItem item = menuItem.DropDownItems.Add(layout.getProperty("display-name"));
                    item.Click += (sender, args) => ctx_addChildLayout(layout, args);
                }
                menu_strip.Items.Add(menuItem);
            }
        }

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
                    return parentOT;
                }
            }
            return null;
            //throw new cwExceptionFatal("The parent of a node [" + getName() + "] should be an Object Group Node or a Tab");
        }

        public cwWebDesignerTreeNodeSite getParentSiteNode()
        {
            cwPSFTreeNode parentNode = getParent();
            while (true)
            {
                if (parentNode as cwWebDesignerTreeNodeSite != null)
                {
                    break;
                }
                parentNode = parentNode.getParent();
            }
            return parentNode as cwWebDesignerTreeNodeSite;
        }


        private void add_DiagramDesignerLayouts(ContextMenuStrip menuStrip)
        {
            if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeLayout>())
            {
                ToolStripMenuItem c = new ToolStripMenuItem("Add Diagram Designer Layouts");
                add_AddChildMenuItemToContextMenuFirst<cwDiagramDesignerLayoutIncludeType, cwWebDesignerGUI>(c, "Diagram Include Layout", Casewise.GraphAPI.Properties.Resources.image_tvicon_layout);
                menuStrip.Items.Add(c);
            }
        }

        private void add_DiagramDesignerDesignNode(ContextMenuStrip menuStrip)
        {
            //ToolStripMenuItem c = new ToolStripMenuItem("Add Diagram Designer");
            add_AddChildMenuItemToContextMenuFirst<cwDiagramDesignerDesignNode, cwWebDesignerGUI>("Add Diagram Design", Casewise.GraphAPI.Properties.Resources.image_tvicon_layout);
            //menuStrip.Items.Add(c);  
        }

        public void add_DiagramDesignerOptions(ContextMenuStrip menuStrip)
        {

            if (!hasAtLeastOnChildNode<cwDiagramDesignerDesignNode>())
            {
                cwPSFTreeNodeObjectNode parent = getParentNode();
                if (parent != null)
                {
                    // if parent has not a design layout
                    if (!parent.hasAtLeastOnChildNode<cwDiagramDesignerLayoutNode>())
                    { // can add a designer node
                        add_DiagramDesignerDesignNode(menuStrip);
                    }
                    else
                    { // can have design layout
                        add_DiagramDesignerLayouts(menuStrip);
                    }
                }
                else
                {
                    add_DiagramDesignerDesignNode(menuStrip);
                }
            }
            else
            {
                add_DiagramDesignerLayouts(menuStrip);
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

            add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeTab, cwWebDesignerGUI>("Add Tab", Properties.Resources.image_tvicon_tab);

            cwLightObjectType OT = getSelectedObjectType();
            
            if ("DIAGRAM".Equals(OT.ScriptName))
            {
                add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeObjectNodeOnDiagram, cwWebDesignerGUI>("Add On Diagram Object Node", Casewise.GraphAPI.Properties.Resources.image_tvicon_node);
            }

            add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodePropertiesGroup, cwWebDesignerGUI>("Add Properties Group", Properties.Resources.image_tvicon_propertiesgroup);

            // no association if has tabs
            addLayoutsToContextMenu();
            if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeTab>())
            {
                addAssociationTypesToContextMenu<cwWebDesignerTreeNodeObjectNodeAssociationType, cwWebDesignerGUI>(menu_strip, this, null);
            }

            if (this is cwWebDesignerTreeNodeObjectNodeAssociationType)
            {
                add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeObjectNodeIntersectionObjectType, cwWebDesignerGUI>("Add Intersection Object Type", Properties.Resources.image_tvicon_tab);
            }
            

            add_DiagramDesignerOptions(menu_strip);

            addDeleteOptionToContextMenu();

            menu_strip.Show(treeView, position);
        }

        /// <summary>
        /// Ctx_adds the child layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_addChildLayout(cwWebDesignerTreeNodeLayout layout, EventArgs e)
        {
            addChildNodeFirst(layout);
            Expand();
        }



        internal cwWebDesignerTreeNodeInterfaceLayout getLayout()
        {

            if (hasAtLeastOnChildNode<cwWebDesignerTreeNodeLayout>())
            {
                cwWebDesignerTreeNodeInterfaceLayout _ILayout = getFirstChildNode<cwWebDesignerTreeNodeLayout>();
                this.layoutName = _ILayout.getStringProperty("js-class-name");
                return _ILayout;
            }
            if (hasAtLeastOnChildNode<cwDiagramDesignerLayoutNode>())
            {
                cwWebDesignerTreeNodeInterfaceLayout _ILayout = getFirstChildNode<cwDiagramDesignerLayoutNode>();
                this.layoutName = _ILayout.getStringProperty("js-class-name");
                return _ILayout;
            }

            string errorMessage = "Impossible to run the object node [" + getName() + "] without a valid layout";
            operationEditModeGUI.appendInfo(errorMessage);
            BackColor = Color.Red;
            throw new cwExceptionNodeValidation(errorMessage, this);
        }


        /// <summary>
        /// Gets the children object nodes.
        /// </summary>
        /// <returns></returns>
        public List<cwWebDesignerTreeNodeTab> getChildrenTabs()
        {
            return getChildrenGeneric<cwWebDesignerTreeNodeTab>();
        }

        /// <summary>
        /// Gets the children generic from tabs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> getChildrenGenericFromTabs<T>() where T : cwPSFTreeNode
        {
            List<T> nodes = new List<T>();
            foreach (cwWebDesignerTreeNodeTab tab in getChildrenTabs())
            {
                nodes.AddRange(tab.getChildrenGeneric<T>());
            }
            return nodes;
        }


        /// <summary>
        /// Gets the children nodes from tabs.
        /// </summary>
        /// <returns></returns>
        public List<cwPSFTreeNodeObjectNode> getChildrenNodesFromTabs()
        {
            return getChildrenGenericFromTabs<cwPSFTreeNodeObjectNode>();
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <returns></returns>
        public string getViewName()
        {
            return getSelectedObjectType().ScriptName.ToLower().ToString();
        }
    }
}
