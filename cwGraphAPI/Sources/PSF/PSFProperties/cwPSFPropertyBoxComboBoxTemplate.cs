using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// allow to select a template diagram from a combo box
    /// </summary>
    public class cwPSFPropertyBoxComboBoxTemplate : cwPSFPropertyBoxComboBox
    {

        /// <summary>
        /// Where the property box belongs to
        /// </summary>
        public cwPSFTreeNode _treeNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxTemplate"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxComboBoxTemplate(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNode _treeNodeObjectGroup)
            : base(_helpName, _helpDescription, _keyName, new cwLightObject[] { })
        {
            _treeNode = _treeNodeObjectGroup;
            loadNodes(_treeNode.operationEditModeGUI.Model);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBoxTemplate"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="model">The model.</param>
        public cwPSFPropertyBoxComboBoxTemplate(String _helpName, String _helpDescription, String _keyName, cwLightModel model)
            : base(_helpName, _helpDescription, _keyName, new cwLightObject[] { })
        {
            loadNodes(model);
        }



        /// <summary>
        /// Loads the object types CB.
        /// </summary>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            cwLightModel _currentModel = _sourcecwLightObjectType as cwLightModel;
            clearItems();

            //cwLightModel _currentModel = m.getLightModel();
            cwLightNodeObjectType OG = new cwLightNodeObjectType(_currentModel.getObjectTypeByScriptName("DIAGRAM"));

            OG.addAttributeForFilterAND("TEMPLATE", "1", "=");
            OG.sortOnPropertyScriptName = "NAME";
            OG.addPropertyToSelect("NAME");
            OG.addPropertyToSelect("TITLE");
            OG.addPropertyToSelect("UNIQUEIDENTIFIER");
            OG.addPropertyToSelect("TYPE");
            OG.preloadLightObjects();

            foreach (cwLightObject O in OG.usedOTLightObjects)
            {
                boxComboBox.Items.Add(O);
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
                cwLightObject P = node as cwLightObject;
                if (P == null)
                {
                    if (node.Equals(propertyScriptName))
                    {
                        return node.ToString();
                    }
                }
                else
                {
                    if (P.ID.ToString().Equals(propertyScriptName))
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
            cwLightObject P = getSelectedObject();
            if (P != null) return P.ID.ToString();
            return "";
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public cwLightObject getSelectedObject()
        {
            cwLightObject P = boxComboBox.SelectedItem as cwLightObject;
            if (P != null)
            {
                return P;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the template diagram.
        /// </summary>
        /// <returns></returns>
        public cwLightDiagram getTemplateDiagram()
        {
            cwLightObject template = this.getSelectedObject();
            if (template == null) return null;
            cwLightDiagram d= new cwLightDiagram(this._treeNode.operationEditModeGUI.Model, template.ID, template.properties["NAME"], template.properties["TITLE"], template.properties["UNIQUEIDENTIFIER"]);
            d.properties = template.properties;
            return d;
        }



    }
}
