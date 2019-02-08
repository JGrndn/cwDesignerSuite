using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.IO;

using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.GUI;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// a configuration tree node
    /// </summary>
    class cwDiagramDesignerDesignNode : cwPSFTreeNode
    {
        public const string CONFIG_LAYOUT_NAME = "layout_name";
        public const string CONFIG_DIAGRAM_NAME = "diagram_name";
        public const string CONFIG_DIAGRAM_PREFIX = "diagram_prefix";
        public const string CONFIG_DIAGRAM_SUFIX = "diagram_sufix";
        public const string CONFIG_DIAGRAM_MODE = "diagram_mode";
        public const string CONFIG_DESIGN_ESCAPE_LEVEL1_NO_CHILD = "escape_level1_no_children";
        public const string CONFIG_DESIGN_AUTO_CHECK_CHILDREN = "auto_check_children";
        public const string CONFIG_TEMPLATE = "template_id";
        //private cwDesignerTreeNodeDesignRule rootRule = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwDesignerConfigurationNode"/> class.
        /// </summary>
        /// <param name="_name">The _name.</param>
        /// <param name="_designerGUI">The _designer GUI.</param>
        public cwDiagramDesignerDesignNode(cwWebDesignerGUI _GUI, cwPSFTreeNode parent)
            : base(_GUI, parent)
        {
            updateText("Automatic Diagram Design");
        }
        
        public override void setPropertiesBoxes()
        {
            //base.setPropertiesBoxes();
            cwPSFPropertyBoxComboBox cb_diagramType = new cwPSFPropertyBoxComboBox("Diagram Mode", @"The mode of generation : 
//Single will create one diagram from the root nodes you will select.
//Group will create only one diagram containing all selected items", CONFIG_DIAGRAM_MODE, new string[] { "single", "group" });
            cb_diagramType.SelectedIndexChanged(new EventHandler(CB_DiagramMode_SelectedValueChanged));
            propertiesBoxes.addPropertyBox(cb_diagramType);

//            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Layout Name", "The usable name of the Layout, this name will appear in the context menu in CM suite tools", CONFIG_LAYOUT_NAME));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Diagram Name", "The name of the diagram if group mode only", "diagram_name"));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Diagram Name Prefix", "The prefix of the name of the diagram if single mode only", CONFIG_DIAGRAM_PREFIX));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Diagram Name Suffix", "The suffix of the name of the diagram if single mode only", CONFIG_DIAGRAM_SUFIX));
//            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Escape If No Children", "Escape the diagram if there is no children on the first level", CONFIG_DESIGN_ESCAPE_LEVEL1_NO_CHILD, false));
 //           propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Check Root Children", "If all the root level children should be checked by default", CONFIG_DESIGN_AUTO_CHECK_CHILDREN, false));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxComboBoxTemplate("Diagram Template", "The diagram template which will be used by the design", CONFIG_TEMPLATE, this));

            propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_MODE).setValue("group");            
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }
        /// <summary>
        /// Handles the SelectedValueChanged event of the CB_DiagramMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void CB_DiagramMode_SelectedValueChanged(object sender, EventArgs e)
        {
            string diagram_mode = propertiesBoxes.getPropertyBoxComboBox(CONFIG_DIAGRAM_MODE).ToString();            
            if ("group".Equals(diagram_mode))
            {
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_PREFIX).cleanAndDisable();
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_SUFIX).cleanAndDisable();
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_NAME).enable();
            }
            if ("single".Equals(diagram_mode))
            {
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_PREFIX).enable();
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_SUFIX).enable();
                propertiesBoxes.getPropertyBox(CONFIG_DIAGRAM_NAME).cleanAndDisable();
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
            base.createContextMenu(treeView, position);
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

    }
}
