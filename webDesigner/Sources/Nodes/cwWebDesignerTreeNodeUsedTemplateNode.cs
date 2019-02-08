using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Drawing;
using Casewise.GraphAPI.API;
using System.Windows.Forms;


namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodeUsedTemplateNode : cwPSFTreeNode
    {
        public const string CONFIG_TEMPLATE_NAME = "template-name";

        //private cwWebDesignerTreeNodeTemplateNode templateNode = null;

        public cwWebDesignerTreeNodeUsedTemplateNode(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
            updateText("Used Template");
            ForeColor = Color.IndianRed;
            setIconForNodeUsingIndex(7);
        }

        public override void setPropertiesBoxes()
        {
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Template Name", "The name of the template used for drawing this node", CONFIG_TEMPLATE_NAME));
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).disable();
            propertiesBoxes.getPropertyBox(CONFIG_TEMPLATE_NAME).disable();

        }

        public string getTemplateName()
        {
            return propertiesBoxes.getPropertyBox(CONFIG_TEMPLATE_NAME).ToString();
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public void updateTemplateName(string templateName)
        { 
            propertiesBoxes.getPropertyBox(CONFIG_TEMPLATE_NAME).setValue(templateName);
            updateText("Used Template : " + templateName);
        }

        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            addDeleteOptionToContextMenu();
            menu_strip.Show(treeView, position);
        }

        public cwWebDesignerTreeNodeInterfaceLayout getFirstLayout(webDesignerOperation webDesigner)
        {
            string templateName = getStringProperty(cwWebDesignerTreeNodeUsedTemplateNode.CONFIG_TEMPLATE_NAME).ToString();
            cwWebDesignerTreeNodeObjectNode templateOG = webDesigner.templateTreeNodes[templateName];
            return templateOG.getLayout();
        }



    }
}
