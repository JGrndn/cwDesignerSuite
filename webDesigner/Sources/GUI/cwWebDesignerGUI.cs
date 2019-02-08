using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.GUI;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.PSF;
using log4net;
using Casewise.GraphAPI.API;
using Casewise.webDesigner.Nodes;
using Casewise.webDesigner;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.ProgramManager;

namespace Casewise.webDesigner.GUI
{
    /// <summary>
    /// 
    /// </summary>
    public class cwWebDesignerGUI : cwEditModeGUI
    {
        public cwWebDesignerLayoutManager layoutManager = new cwWebDesignerLayoutManager(AppDomain.CurrentDomain.BaseDirectory + "/webDesigner/layouts");

        public cwWebDesignerGUI(cwLightModel _model, cwProgramManagerOptions options)
            : base(_model, options)
        {

            Text = "Casewise webUtility";
            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodeObjectNodeObjectType", "cwWebDesignerTreeNodeTab");
            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodeObjectNodeObjectType", "cwWebDesignerTreeNodePage");


            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodeObjectNodeAssociationType", "cwWebDesignerTreeNodeTab");
            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodeObjectNodeAssociationType", "cwPSFTreeNodeObjectNodeObjectType");

            addDragAndDropFor("Casewise.GraphAPI.Operations.Web.cwWebDesignerTreeNodeTab", "cwPSFTreeNodeObjectNodeObjectType");
            addDragAndDropFor("Casewise.GraphAPI.Operations.Web.cwWebDesignerTreeNodeTab", "cwWebDesignerTreeNodeTab");
            addDragAndDropFor("Casewise.GraphAPI.Operations.Web.cwWebDesignerTreeNodeTab", "cwWebDesignerTreeNodePage");

            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodePropertiesGroup", "cwWebDesignerTreeNodeObjectNodeObjectType");
            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodePropertiesGroup", "cwWebDesignerTreeNodeObjectNodeAssociationType");
            addDragAndDropFor("Casewise.webDesigner.Nodes.cwWebDesignerTreeNodePropertiesGroup", "cwWebDesignerTreeNodeTab");

            treeViewConfigurations.ImageList = new ImageList();
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_world);
            treeViewConfigurations.ImageList.Images.Add(Casewise.GraphAPI.Properties.Resources.image_tvicon_node);
            treeViewConfigurations.ImageList.Images.Add(Casewise.GraphAPI.Properties.Resources.image_tvicon_layout);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_pages);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_page_index);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_page_single);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_template_nodes);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_template_node);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_tab);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_propertiesgroup);
            treeViewConfigurations.ImageList.Images.Add(Properties.Resources.image_tvicon_behaviour);
        }

        /// <summary>
        /// Handles the Click event of the add_configurationItemToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void add_configurationItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cwWebDesignerTreeNodeSite _configNode = new cwWebDesignerTreeNodeSite(this);
            treeViewConfigurations.Nodes.Add(_configNode);            
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(cwWebDesignerGUI));
            this.SuspendLayout();
            // 
            // cwWebDesignerGUI
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "cwWebDesignerGUI";
            this.ResumeLayout(false);

        }

        public override bool isSaveRequired()
        {
            return false;
        }

    }
}
