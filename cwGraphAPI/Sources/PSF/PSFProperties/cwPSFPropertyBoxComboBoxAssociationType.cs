using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// combox box to select association types
    /// </summary>
    public class cwPSFPropertyBoxComboBoxAssociationType : cwPSFPropertyBoxComboBox
    {
        private cwPSFTreeNodeObjectNode treeNodeObjectGroup = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxAssociationType"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxComboBoxAssociationType(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeObjectNode _treeNodeObjectGroup)
            : base(_helpName, _helpDescription, _keyName, new object[] { })
        {
            treeNodeObjectGroup = _treeNodeObjectGroup;
            disable();               
        }

 
        /// <summary>
        /// Handles the SelectedValueChanged event of the CBcwLightObjectTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void CB_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                cwLightObjectType targetOT = getSelectedTargetObjectType();
                if (treeNodeObjectGroup != null)
                {
                    treeNodeObjectGroup.updateObjectTypeDependendantPropertiesBoxes(targetOT, this);
                }
            }
            catch (Exception exception)
            {
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }


        /// <summary>
        /// Gets the type of the selected association.
        /// </summary>
        /// <returns></returns>
        public cwLightAssociationType getSelectedAssociationType()
        {
            return boxComboBox.SelectedItem as cwLightAssociationType;
        }

        /// <summary>
        /// Gets the type of the selected intersection object.
        /// </summary>
        /// <returns></returns>
        private cwLightObjectType getSelectedIntersectionObjectType()
        {
            cwLightAssociationType AT = boxComboBox.SelectedItem as cwLightAssociationType;
            if (AT != null)
            {
                return AT.Intersection;
            }
            return null;
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public cwLightObjectType getSelectedTargetObjectType()
        {
            cwLightAssociationType AT = boxComboBox.SelectedItem as cwLightAssociationType;
            if (AT != null)
            {
                return AT.Target;
            }
            return null;
        }

        /// <summary>
        /// Loads the object types CB.
        /// </summary>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            clearItems();
            cwLightObjectType OT = _sourcecwLightObjectType as cwLightObjectType;
            // if not OT has been loaded, load the model OTs
            if (OT != null)
            {
                enable();
                boxComboBox.Items.Add("_None_");
                boxComboBox.SelectedItem = "_None_";

                foreach (cwLightAssociationType AT in OT.AssociationsType)
                {
                    boxComboBox.Items.Add(AT);
                }
                disable();
            }
            else
            {
                disable();
            }
        }

        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="cwLightAssociationTypeScriptName">Name of the cw light association type script.</param>
        /// <returns></returns>
        private object searchInItemsByProperty(string cwLightAssociationTypeScriptName)
        {
            foreach (object node in boxComboBox.Items)
            {
                cwLightAssociationType AT_Node = node as cwLightAssociationType;
                if (AT_Node == null)
                {
                    if (node.Equals(cwLightAssociationTypeScriptName))
                    {
                        return node.ToString();
                    }
                }
                else
                {
                    if (AT_Node.ScriptName.Equals(cwLightAssociationTypeScriptName))
                    {
                        return AT_Node;
                    }
                }                
            }
            return null;
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public override void setValue(object _item)
        {
            enable();
            object AT_Node = searchInItemsByProperty((string)_item);
            if (AT_Node != null)
            {
                boxComboBox.SelectedItem = AT_Node;
            }
            disable();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            cwLightAssociationType AT_Node = getSelectedAssociationType();
            if (AT_Node != null) return AT_Node.ScriptName;
            return "Root-Mode";
        }


    }
}
