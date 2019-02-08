using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// a combo box to select a property type
    /// </summary>
    public class cwPSFPropertyBoxComboBoxProperties : cwPSFPropertyBoxComboBox
    {

        /// <summary>
        /// The node where the property belongs to
        /// </summary>
        public cwPSFTreeNodeObjectNode treeNodeObjectGroup = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxProperties"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxComboBoxProperties(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeObjectNode _treeNodeObjectGroup)
            : base(_helpName, _helpDescription, _keyName, new object[] { })
        {

            treeNodeObjectGroup = _treeNodeObjectGroup;
            disable();
            if (treeNodeObjectGroup != null)
            {
                loadNodes(treeNodeObjectGroup.getSelectedObjectType());
            }
            
        }


        /// <summary>
        /// Loads the object types CB.
        /// </summary>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            cwLightObjectType OT = _sourcecwLightObjectType as cwLightObjectType;
            clearItems();
            // if not OT has been loaded, load the model OTs
            if (OT != null)
            {
                enable();
                boxComboBox.Items.Add("_None_");
                boxComboBox.SelectedItem = "_None_";
                foreach (cwLightProperty P in OT.getProperties())
                {
                    boxComboBox.Items.Add(P);
                }
                setValue("NAME");
            }
            else
            {
                disable();
            }
        }



        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <returns></returns>
        private object searchInItemsByProperty(string propertyScriptName)
        {
            foreach (object node in boxComboBox.Items)
            {
                cwLightProperty P = node as cwLightProperty;
                if (P == null)
                {
                    if (node.Equals(propertyScriptName))
                    {
                        return node.ToString();
                    }
                }
                else
                {
                    if (P.ScriptName.Equals(propertyScriptName))
                    {
                        return P;
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
            object P = searchInItemsByProperty((string)_item);
            if (P != null)
            {
                boxComboBox.SelectedItem = P;
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
            cwLightProperty P = getSelectedProperty();
            if (P != null) return P.ScriptName;
            return "";
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public cwLightProperty getSelectedProperty()
        {
            cwLightProperty P = boxComboBox.SelectedItem as cwLightProperty;
            if (P != null)
            {
                return P;
            }
            else
            {
                return null;
            }
        }



    }
}
