using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI;
using Casewise.GraphAPI.Operations;
using System.Drawing;
using System.Xml;
using System.IO;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;
using System.Xml.Linq;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// a configuration tree node
    /// </summary>
    public class cwWebDesignerTreeNodePage : cwPSFTreeNode
    {
       
        public const string CONFIG_PAGE_TYPE = "page_type";
        public const string CONFIG_PAGE_DISPLAY_NAME = "page-display-name";
        public const string CONFIG_ADD_SEARCH_ENGINE = "add_search_engine";
        public const string CONFIG_INSTANT_SEARCH = "is_instant_search";
        public const string CONFIG_ADD_DISABLE_PAGE = "disable_page";



        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerTreeNodePage"/> class.
        /// </summary>
        /// <param name="_cwWebIndexEditGUI">The _CW web index edit GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwWebDesignerTreeNodePage(cwWebDesignerGUI _cwWebIndexEditGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexEditGUI, _parent)
        {
            updateText("Page");
        }

        /// <summary>
        /// Gets the parent site.
        /// </summary>
        /// <returns></returns>
        public cwWebDesignerTreeNodeSite getParentSite()
        {
            return this.getParent().getParent() as cwWebDesignerTreeNodeSite;
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).helpName = "Page Name";
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Page Display Name", "The display name of the page, used for the menu", CONFIG_PAGE_DISPLAY_NAME));
            cwPSFPropertyBoxComboBox _page_type = new cwPSFPropertyBoxComboBox("Page Type", "Index will create one page with all the items inside, single will create one page for each root item", CONFIG_PAGE_TYPE, new string[] { "index", "single" });
            _page_type.SelectedIndexChanged(new EventHandler(CBPageType_ValueChanged));
            propertiesBoxes.addPropertyBox(_page_type);
            _page_type.disable();
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Disable Page", "Don't generate this page", CONFIG_ADD_DISABLE_PAGE, false));


            cwPSFPropertyBoxCheckBox _search_engine = new cwPSFPropertyBoxCheckBox("Add SearchEngine", "Provide search capabilities", CONFIG_ADD_SEARCH_ENGINE, false);
            propertiesBoxes.addPropertyBox(_search_engine);
            propertiesBoxes.getPropertyBox(CONFIG_ADD_SEARCH_ENGINE).disable();

            _search_engine.checkBoxChanged(new EventHandler(SEARCH_ENGINE_ValueChanged));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Instant Search", "Instant search or search by clicking search button", CONFIG_INSTANT_SEARCH, false));
            propertiesBoxes.getPropertyBox(CONFIG_INSTANT_SEARCH).disable();
        }
      
        /// <summary>
        /// Handles the ValueChanged event of the CBPageType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void CBPageType_ValueChanged(object sender, EventArgs e)
        {
            cwPSFPropertyBoxComboBox _page_type = propertiesBoxes.getPropertyBoxComboBox(CONFIG_PAGE_TYPE);
            string _pageTypeValue = _page_type.ToString();
            if ("index".Equals(_pageTypeValue))
            {
                propertiesBoxes.getPropertyBoxBoolean(CONFIG_ADD_SEARCH_ENGINE).enable();
                setIconForNodeUsingIndex(4);
                return;
            }
            if ("single".Equals(_pageTypeValue))
            {
                propertiesBoxes.getPropertyBoxBoolean(CONFIG_ADD_SEARCH_ENGINE).cleanAndDisable();
                propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_PAGE_DISPLAY_NAME).cleanAndDisable();
                setIconForNodeUsingIndex(5);
                return;
            }
        }

        public void SEARCH_ENGINE_ValueChanged(object sender, EventArgs e)
        {
            cwPSFPropertyBoxCheckBox _search_engine = propertiesBoxes.getPropertyBoxBoolean(CONFIG_ADD_SEARCH_ENGINE);
            if (_search_engine.Checked)
            {
                propertiesBoxes.getPropertyBox(CONFIG_INSTANT_SEARCH).enable();
            }
            else {
                propertiesBoxes.getPropertyBox(CONFIG_INSTANT_SEARCH).disable();
            }
        }


        public List<cwPSFTreeNodeObjectNode> getChildrenObjectNodes()
        {
            List<cwPSFTreeNodeObjectNode> _groups = new List<cwPSFTreeNodeObjectNode>();
            if (hasAtLeastOnChildNode<cwWebDesignerTreeNodeTab>())
            {
                foreach (cwWebDesignerTreeNodeTab tab in getChildrenNodes<cwWebDesignerTreeNodeTab>())
                {
                    tab.getChildrenObjectNodes().ForEach(node => _groups.Add(node));
                }
            }
            else
            {
                if (hasAtLeastOnChildNode<cwPSFTreeNodeObjectNode>())
                {
                    getChildrenNodes<cwPSFTreeNodeObjectNode>().ForEach(tn => _groups.Add(tn as cwPSFTreeNodeObjectNode));
                }
            }
            return _groups;
        }


        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            base.createContextMenu(treeView, position);


            //if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeTab>())
            //{
            //    addAssociationTypesToContextMenu(menu_strip, this);
            //}

            string _pageType = propertiesBoxes.getPropertyBox(CONFIG_PAGE_TYPE).ToString();
            switch (_pageType)
            {
                case "single":
                    if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeObjectNodeObjectType>())
                    {
                        cwPSFTreeNodeObjectNode.addObjectTypesToContextMenu<cwWebDesignerTreeNodeObjectNodeObjectType, cwWebDesignerGUI>(menu_strip, this, operationEditModeGUI.Model, null);
                    }
                    break;
                case "index":
                    // add a tab if has no children
                    if (!hasAtLeastOnChildNode<cwWebDesignerTreeNodeObjectNodeObjectType>())
                    {
                        add_AddChildMenuItemToContextMenuLast<cwWebDesignerTreeNodeTab, cwWebDesignerGUI>("Add Tab", Properties.Resources.image_tvicon_tab);
                    }
                    cwPSFTreeNodeObjectNode.addObjectTypesToContextMenu<cwWebDesignerTreeNodeObjectNodeObjectType, cwWebDesignerGUI>(menu_strip, this, operationEditModeGUI.Model, null);
                    //if (!hasAtLeastOnChildNode<cwPSFTreeNodeObjectNodeObjectType>())
                    //{
                    //    add_AddChildMenuItemToContextMenuLast<cwPSFTreeNodeObjectNodeObjectType, cwWebDesignerGUI>("Add Object Type");
                    //}

                    break;
            }


            ToolStripItem generateNow_item = menu_strip.Items.Add("Generate Now");
            generateNow_item.Image = Casewise.GraphAPI.Properties.Resources.image_tvicon_generate;
            generateNow_item.Click += new System.EventHandler(this.generateNow_item_ToolStripMenuItem_Click);

            addDeleteOptionToContextMenu();


            menu_strip.Show(treeView, position);
        }






        /// <summary>
        /// Handles the Click event of the generateNow_item_ToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void generateNow_item_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = DateTime.Now;
                operationEditModeGUI.appendInfo("Start generating " + this.Text + "...");
                cwWebDesignerTreeNodeSite siteNode = getParentSite();
                siteNode.operationEditModeGUI.Cursor = Cursors.WaitCursor;
                webDesignerOperation _webDesigner = new webDesignerOperation(operationEditModeGUI.Model, getParentSite());
                if (0.Equals(_webDesigner.nodes[this.getName()].Count))
                {
                    throw new cwExceptionFatal(String.Format(@"The page {0} should have at least one Object Type as a child", this.getName()));
                }
                cwLightNodeObjectType OT = _webDesigner.nodes[this.getName()].First();

                
                string _pageType = propertiesBoxes.getPropertyBox(CONFIG_PAGE_TYPE).ToString();
                switch (_pageType)
                {
                    case "single":
                        _webDesigner.createSinglePage(this);
                        if (OT.sourceObjectType.ScriptName.Equals("DIAGRAM"))
                        {
                            _webDesigner.exportDiagrams(OT.selectedPropertiesScriptName);
                        }
                        break;
                    case "index":
                        _webDesigner.createIndexPage(this);
                        break;
                }

                operationEditModeGUI.appendInfo("Page " + this.Text + " generated in " + DateTime.Now.Subtract(start).ToString() + "s.");
                siteNode.operationEditModeGUI.Cursor = Cursors.Default;
            }
            catch (Exception _exception)
            {
                cwPSFTreeNode.log.Error(_exception.ToString());
                appendError(_exception.Message.ToString());
                appendError("An error occured while generating your page.");
            }
        }
    }
}
