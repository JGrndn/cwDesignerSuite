using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Xml;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    internal class cwWebDesignerTreeNodePagesSingle : cwWebDesignerTreeNodePages
    {


        public cwWebDesignerTreeNodePagesSingle(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Single Pages");
            disableName();
            setIconForNodeUsingIndex(3);
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
        }
        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            ToolStripItem rule_item = menu_strip.Items.Add("Add Single Page");
            rule_item.Click += new System.EventHandler(this.addRuleToolStripMenuItem_Click);

            //ToolStripItem loadPages_item = menu_strip.Items.Add("Load Missing Pages");
            //loadPages_item.Click += new System.EventHandler(this.loadPagesToolStripMenuItem_Click);
            //ToolStripItem cleanPages_item = menu_strip.Items.Add("Clean Pages");
            //cleanPages_item.Click += new System.EventHandler(this.cleanPagesToolStripMenuItem_Click);
            base.createContextMenu(treeView, position);
            menu_strip.Show(treeView, position);
        }


        /// <summary>
        /// Handles the Click event of the addRuleToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void addRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cwWebDesignerTreeNodePage _page = new cwWebDesignerTreeNodePage(getGUI<cwWebDesignerGUI>(), this);
            cwPSFPropertyBox _type = _page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_PAGE_TYPE);
            _type.setValue("single");
            _type.disable();
            addChildNodeLast(_page);
        }

       

        /// <summary>
        /// Handles the Click event of the loadPagesToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void loadPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //createMissingNodes("single", true, getGUI<cwWebDesignerGUI>());
            }
            catch (Exception _exception)
            {
                cwPSFTreeNode.log.Error(_exception.ToString());
            }            
        }

    }
}
