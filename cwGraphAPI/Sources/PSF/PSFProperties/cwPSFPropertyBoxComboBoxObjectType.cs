using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to select an object type from a combo box
    /// </summary>
    public class cwPSFPropertyBoxComboBoxObjectType : cwPSFPropertyBoxComboBox
    {
        private cwPSFTreeNode treeNodeObjectGroup = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxObjectType"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxComboBoxObjectType(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNode _treeNodeObjectGroup)
            : base(_helpName, _helpDescription, _keyName, new object[] {})
        {
            treeNodeObjectGroup = _treeNodeObjectGroup;
            if (treeNodeObjectGroup != null)
            {
                loadNodes(treeNodeObjectGroup.operationEditModeGUI.Model);
            }
        }


        /// <summary>
        /// Handles the SelectedValueChanged event of the CB control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void CB_SelectedValueChanged(object sender, EventArgs e)
        {
            cwPSFTreeNodeObjectNode treeNodeOT = treeNodeObjectGroup as cwPSFTreeNodeObjectNode;
            if (treeNodeOT != null)
            {
                treeNodeOT.updateObjectTypeDependendantPropertiesBoxes(getSelectedObjectType(), this);
            }     
        }


        /// <summary>
        /// Loads the object types CB.
        /// </summary>
        public override void loadNodes(object _currentModel)
        {
            enable();
            cwLightModel M = _currentModel as cwLightModel;
            if (0.Equals(boxComboBox.Items.Count))
            {
                foreach (cwLightObjectType OT in M.getPSFEnabledObjectTypes(false))
                {
                    boxComboBox.Items.Add(OT);
                }
            }
            disable();
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public cwLightObjectType getSelectedObjectType()
        {
            cwLightObjectType OT = boxComboBox.SelectedItem as cwLightObjectType;
            if (OT != null)
            {
                return OT;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="cwLightObjectTypeScriptName">Name of the object type script.</param>
        /// <returns></returns>
        internal override object searchInCollectionByScriptName(string cwLightObjectTypeScriptName)
        {
            foreach (cwLightObjectType OT in boxComboBox.Items)
            {
                if (OT.ScriptName.Equals(cwLightObjectTypeScriptName))
                {
                    return OT as object;
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
            cwLightObjectType OT = searchInCollectionByScriptName((string)_item)  as cwLightObjectType;
            if (OT != null)
            {
                boxComboBox.SelectedItem = OT;
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
            cwLightObjectType OT = getSelectedObjectType();
            if (OT != null) return OT.ScriptName;
            return "";
        }
    


    }
}
