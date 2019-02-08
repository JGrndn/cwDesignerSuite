using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to select models in a combo box
    /// </summary>
    public class cwPSFPropertyBoxComboBoxModel : cwPSFPropertyBoxComboBox
    {
        private cwPSFTreeNode treeNodeParent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxModel"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeParent">The _tree node parent.</param>
        public cwPSFPropertyBoxComboBoxModel(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNode _treeNodeParent)
            : base(_helpName, _helpDescription, _keyName, new object[] {})
        {
            treeNodeParent = _treeNodeParent;
            loadNodes(treeNodeParent.operationEditModeGUI.Model.getConnection());


        }

        /// <summary>
        /// Handles the SelectedValueChanged event of the CBcwLightObjectTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void CB_SelectedValueChanged(object sender, EventArgs e)
        {
            //treeNodeObjectGroup.setSelectedcwLightObjectType(getSelectedcwLightObjectType(), this);
        }


        /// <summary>
        /// Loads the object types CB.
        /// </summary>
        public override void loadNodes(object _currentConnection)
        {
            //Model M = _currentModel as Model;
            cwConnection C = _currentConnection as cwConnection;
            if (0.Equals(boxComboBox.Items.Count))
            {
                foreach (cwLightModel M in C.getModels())
                {                    
                    boxComboBox.Items.Add(M);                    
                }
            }
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public cwLightModel getSelectedModel()
        {
            cwLightModel M = boxComboBox.SelectedItem as cwLightModel;
            if (M != null)
            {
                return M;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="ModelFileName">Name of the model file.</param>
        /// <returns></returns>
        private cwLightModel searchInItemsByProperty(string ModelFileName)
        {
            foreach (cwLightModel M in boxComboBox.Items)
            {
                if (M.FileName.Equals(ModelFileName))
                {
                    return M;
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
            cwLightModel M = searchInItemsByProperty((string)_item);
            if (M != null)
            {
                boxComboBox.SelectedItem = M;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            cwLightModel M = getSelectedModel();
            if (M != null) return M.FileName;
            return "";
        }




        ///// <summary>
        ///// Gets the selected object type from combo box.
        ///// </summary>
        ///// <param name="treeNode">The tree node.</param>
        ///// <param name="keyName">Name of the key.</param>
        ///// <returns></returns>
        //public static Model getSelectedModelFromComboBox(cwPSFTreeNode treeNode, String keyName)
        //{
        //    cwPSFPropertyBoxComboBox box = treeNode.propertiesBoxes.getPropertyBoxComboBox(keyName);
        //    cwPSFPropertyBoxComboBoxObjectType OTBox = box as cwPSFPropertyBoxComboBoxObjectType;
        //    if (OTBox == null)
        //    {
        //        throw new cwExceptionWarning("Requested cwLightObjectType ComboBox is not an cwLightObjectType ComboBox");
        //    }
        //    return OTBox.getSelectedcwLightObjectType();

        //}

    }
}
