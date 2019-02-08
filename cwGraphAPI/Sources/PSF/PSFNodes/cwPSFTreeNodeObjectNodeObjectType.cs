using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.API;
using System.Drawing;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;

using System.Data;
using Casewise.Data.ICM;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Represents an object type tree node
    /// </summary>
    public static class cwPSFTreeNodeObjectNodeObjectType
    {
        /// <summary>
        /// CONFIG_OBJECT_TYPE
        /// </summary>
        public const string CONFIG_OBJECT_TYPE = "object-type";

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public static void setPropertiesBoxes(cwPSFTreeNodeObjectNode node)
        {
            cwPSFPropertyBoxComboBoxObjectType objectTypeNode = new cwPSFPropertyBoxComboBoxObjectType(Properties.Resources.PSF_TN_NODE_OBJECT_TYPE_NAME, Properties.Resources.PSF_TN_NODE_OBJECT_TYPE_HELP, CONFIG_OBJECT_TYPE, node);
            node.propertiesBoxes.addPropertyBox(objectTypeNode);
            node.propertiesBoxes.getPropertyBox(CONFIG_OBJECT_TYPE).disable();
        }


        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public static cwLightObjectType getSelectedObjectType(cwPSFTreeNodeObjectNode node)
        {
            return node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxObjectType>(cwPSFTreeNodeObjectNodeObjectType.CONFIG_OBJECT_TYPE).getSelectedObjectType();
        }

        /// <summary>
        /// Sets the type of the selected object.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="objectType">Type of the object.</param>
        public static void setSelectedObjectType(cwPSFTreeNodeObjectNode node, cwLightObjectType objectType)
        {
            node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxObjectType>(cwPSFTreeNodeObjectNodeObjectType.CONFIG_OBJECT_TYPE).setValue(objectType.ScriptName);
        }

        /// <summary>
        /// Updates the name.
        /// </summary>
        public static void updateName(cwPSFTreeNodeObjectNode node)
        {
            cwLightObjectType OT = getSelectedObjectType(node);
            if (OT != null)
            {
                node.updateText(OT.ToString());
                node.ID = OT.ToString().ToLower();
                node.ForeColor = Color.Blue;
            }
        }

        /// <summary>
        /// Gets the name of the box.
        /// </summary>
        /// <returns></returns>
        public static string getBoxName(cwPSFTreeNodeObjectNode node)
        {
            return getSelectedObjectType(node).ToString();
        }

    }
}
