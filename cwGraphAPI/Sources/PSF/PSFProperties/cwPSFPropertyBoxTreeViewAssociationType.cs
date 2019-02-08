using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to check or uncheck a list of association types in a treeview
    /// </summary>
    public class cwPSFPropertyBoxTreeViewcwLightAssociationType : cwPSFPropertyBoxTreeView
    {

        private cwPSFTreeNodeObjectNode treeNodeObjectGroup = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxTreeViewcwLightAssociationType"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxTreeViewcwLightAssociationType(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeObjectNode _treeNodeObjectGroup)
            : base (_helpName, _helpDescription, _keyName)
        {
            treeNodeObjectGroup = _treeNodeObjectGroup;
            treeView.CheckBoxes = true;
            disable();
        }

        /// <summary>
        /// Loads the nodes.
        /// </summary>
        /// <param name="_sourcecwLightObjectType">should be cwLightObjectType</param>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            cwLightObjectType OT = _sourcecwLightObjectType as cwLightObjectType;
            clearItems();
            // if not OT has been loaded, load the model OTs
            if (OT != null)
            {
                enable();
                foreach (cwLightAssociationType AT in OT.AssociationsType)
                {
                    treeView.Nodes.Add(new cwTreeNodeLightAssociationType(AT));
                }
            }
            else
            {
                disable();
            }
            treeView.Sort();
            treeView.Height = treeView.GetNodeCount(true) * TREEVIEW_ITEM_SIZE;
        }

        /// <summary>
        /// Gets the content of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> getCollectionContent<T>()
        {
            List<T> assos = new List<T>();
            foreach (TreeNode _node in treeView.Nodes)
            {
                cwTreeNodeLightAssociationType tnAT = _node as cwTreeNodeLightAssociationType;
                if (tnAT == null) continue;
                assos.Add(tnAT.AST as T);
            }
            return assos;
        }

        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public override string ToStringCollection()
        {
            String attributes = "";
            foreach (TreeNode _node in treeView.Nodes)
            {
                cwTreeNodeLightAssociationType tnAT = _node as cwTreeNodeLightAssociationType;
                if (tnAT == null) continue;
                if (true.Equals(_node.Checked))
                {
                    attributes += tnAT.AST.ToString() + ",";
                }
            }
            return attributes;
        }

        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <returns></returns>
        private cwTreeNodeLightAssociationType searchInNodesByProperty(string propertyScriptName)
        {
            foreach (cwTreeNodeLightAssociationType P in treeView.Nodes)
            {                
                if (P == null)
                {
                    throw new cwExceptionWarning("Missing Requested Property [" + propertyScriptName + "]"); 
                }
                else
                {
                    if (P.AST.ScriptName.Equals(propertyScriptName))
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
            if (treeNodeObjectGroup.getSelectedObjectType() == null) return;
            String _attributes = _item as String;
            String[] _attributesArray = _attributes.Split(',');

            int i = 0;
            foreach (String _a in _attributesArray)
            {
                // escape the last element
                if (i == _attributesArray.Length - 1) break;
                cwTreeNodeLightAssociationType P = searchInNodesByProperty(_a);
                if (P == null) continue;
                P.Checked = true;
                i++;
            }
        }

        /// <summary>
        /// Gets the content of the checked collection.
        /// </summary>
        /// <returns></returns>
        public List<cwLightAssociationType> getCheckedCollectionContent()
        {
            List<cwLightAssociationType> assos = new List<cwLightAssociationType>();
            foreach (TreeNode _node in treeView.Nodes)
            {
                cwTreeNodeLightAssociationType tnAT = _node as cwTreeNodeLightAssociationType;
                if (tnAT == null) continue;
                if (!tnAT.Checked) continue;
                assos.Add(tnAT.AST as cwLightAssociationType);
            }
            return assos;
        }
    }
}
