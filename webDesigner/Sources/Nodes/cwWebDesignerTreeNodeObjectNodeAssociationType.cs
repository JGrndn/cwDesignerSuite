using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.API;
using System.Drawing;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.Operations.Web;
using System.Data;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.webDesigner.Nodes
{
    public class cwWebDesignerTreeNodeObjectNodeAssociationType : cwWebDesignerTreeNodeObjectNode
    {


        public cwWebDesignerTreeNodeObjectNodeAssociationType(cwWebDesignerGUI _cwEditModeGUI, cwPSFTreeNode _parent)
            : base(_cwEditModeGUI, _parent)
        {
            updateText("Association Type Node");
            ForeColor = Color.DarkRed;
        }

        public override void setPropertiesBoxes()
        {
            cwPSFTreeNodeObjectNodeAssociationType.setPropertiesBoxes(this);
            base.setPropertiesBoxes();

            // check if parent is a tab
            cwPSFPropertyBoxComboBoxAssociationType associationTypeNode = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE);
            if (0.Equals(associationTypeNode.getCollectionContent<cwLightAssociationType>().Count))
            {
                cwWebDesignerTreeNodeTab parentTab = getParent() as cwWebDesignerTreeNodeTab;
                if (parentTab != null)
                {
                    cwPSFTreeNodeObjectNode parentOT = parentTab.getParent() as cwPSFTreeNodeObjectNode;
                    if (parentOT != null)
                    {
                        associationTypeNode.loadNodes(parentOT.getSelectedObjectType());
                    }

                }
            }
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }
        /// <summary>
        /// Sets the type of the selected object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public override void setSelectedObjectType(cwLightObjectType objectType)
        {
            cwPSFTreeNodeObjectNodeAssociationType.setSelectedObjectType(this, objectType);
        }


        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public override cwLightObjectType getSelectedObjectType()
        {
            return cwPSFTreeNodeObjectNodeAssociationType.getSelectedObjectType(this);
        }


        public override void updateName()
        {
            cwPSFTreeNodeObjectNodeAssociationType.updateName(this);          
        }

        public override string getBoxName()
        {
            return getName();           
        }

    }
}
