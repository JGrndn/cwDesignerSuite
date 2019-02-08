using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using System.Drawing;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;
using Casewise.Data.ICM;
using System.Data;
using Casewise.GraphAPI.GUI;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Represents an assoication type node
    /// </summary>
    public static class cwPSFTreeNodeObjectNodeAssociationType
    {

        /// <summary>
        /// the association type selected
        /// </summary>
        public const string CONFIG_ASSOCIATION_TYPE = "association-type";

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public static void setPropertiesBoxes(cwPSFTreeNodeObjectNode node)
        {
            cwPSFPropertyBoxComboBoxAssociationType associationTypeNode = new cwPSFPropertyBoxComboBoxAssociationType(Properties.Resources.PSF_TN_NODE_ASSOCIATION_TYPE_NAME, Properties.Resources.PSF_TN_NODE_ASSOCIATION_TYPE_HELP, CONFIG_ASSOCIATION_TYPE, node);
            node.propertiesBoxes.addPropertyBox(associationTypeNode);
            node.propertiesBoxes.getPropertyBox(CONFIG_ASSOCIATION_TYPE).disable();
            cwPSFTreeNodeObjectNode parentOT = node.getParent() as cwPSFTreeNodeObjectNode;
            if (parentOT != null)
            {
                associationTypeNode.loadNodes(parentOT.getSelectedObjectType());
            }
        }



        /// <summary>
        /// Sets the type of the selected object.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="objectType">Type of the object.</param>
        public static void setSelectedObjectType(cwPSFTreeNodeObjectNode node, cwLightObjectType objectType)
        {
            node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE).loadNodes(objectType);

        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public static cwLightObjectType getSelectedObjectType(cwPSFTreeNodeObjectNode node)
        {
            return node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE).getSelectedTargetObjectType();
        }

        /// <summary>
        /// Updates the name.
        /// </summary>
        public static void updateName(cwPSFTreeNodeObjectNode node)
        {
            cwLightAssociationType AT = node.getSelectedAssociationType();
            if (AT != null)
            {
                if ("Association Type Node".Equals(node.getName()))
                {
                    node.updateText(AT.Target.ToString() + " (" + AT.getVerbeName() + ")");
                }                

                string newIDName = AT.Target.ToString().ToLower();
                if (AT.Intersection != null)
                {
                    newIDName += "_" + AT.Intersection.ID.ToString();
                }
                node.ID = newIDName;
                
                node.ForeColor = Color.BlueViolet;
            }
            else
            {
                node.updateText("Association Type Node");
                node.ForeColor = Color.DarkRed;
            }
        }





    }
}
