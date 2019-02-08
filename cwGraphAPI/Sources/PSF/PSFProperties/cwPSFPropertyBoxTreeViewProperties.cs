using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to select properties in a tree view
    /// </summary>
    public class cwPSFPropertyBoxTreeViewProperties : cwPSFPropertyBoxTreeView
    {

        private cwPSFTreeNodeObjectNode treeNodeObjectGroup = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxTreeViewProperties"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxTreeViewProperties(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeObjectNode _treeNodeObjectGroup)
            : base (_helpName, _helpDescription, _keyName)
        {
            treeNodeObjectGroup = _treeNodeObjectGroup;
            treeView.CheckBoxes = true;
        }

        /// <summary>
        /// Checks all items excepted ID.
        /// </summary>
        public void checkAllItemsExceptedID()
        {
            foreach (TreeNode tn in treeView.Nodes)
            { 
                cwTreeNodeProperty tnP = tn as cwTreeNodeProperty;
                if (tnP != null)
                {
                    if ("ID".Equals(tnP.Property.ScriptName)) continue;
                    if (tnP.Property.ScriptName.StartsWith("WHO")) continue;
                    if (tnP.Property.ScriptName.Equals("DATEVALIDATED")) continue;
                    if (tnP.Property.ScriptName.StartsWith("WHEN")) continue;
                    tnP.Checked = true;
                }
            }
        }

        /// <summary>
        /// Loads the nodes.
        /// </summary>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            cwLightObjectType OT = _sourcecwLightObjectType as cwLightObjectType;
            clearItems();
            // if not OT has been loaded, load the model OTs
            if (OT != null)
            {
                enable();
                foreach (cwLightProperty P in OT.getProperties())
                {
                    int tn_pos = treeView.Nodes.Add(new cwTreeNodeProperty(P));
                    TreeNode tn = treeView.Nodes[tn_pos];
                    if ("NAME".Equals(P.ScriptName)) tn.Checked = true;
                    //if ("DESCRIPTION".Equals(P.Scriptname)) tn.Checked = true;
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
        /// Gets the checked properties list.
        /// </summary>
        /// <returns></returns>
        public List<cwLightProperty> getCheckedPropertiesList()
        {
            List<cwLightProperty> _elements = new List<cwLightProperty>();
            foreach (cwTreeNodeProperty tn in treeView.Nodes)
            {
                if (tn.Checked)
                {
                    _elements.Add(tn.Property);
                }
            }
            return _elements;
        }

        /// <summary>
        /// Gets the content of the collection.
        /// </summary>
        /// <returns></returns>
        public List<cwLightProperty> getPropertiesList()
        {
            List<cwLightProperty> _elements = new List<cwLightProperty>();
            foreach (cwTreeNodeProperty tn in treeView.Nodes)
            {
                _elements.Add(tn.Property);
            }
            return _elements;
        }

        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public override String ToStringCollection()
        {
            String attributes = "";
            foreach (TreeNode _node in treeView.Nodes)
            {
                cwTreeNodeProperty tnP = _node as cwTreeNodeProperty;
                if (tnP == null) continue;
                if (true.Equals(_node.Checked))
                {
                    attributes += tnP.Property.ScriptName + ",";
                }
            }
            return attributes;
        }



        /// <summary>
        /// Searches the in nodes by property scriptname.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <returns></returns>
        internal cwTreeNodeProperty searchInNodesByPropertyScriptname(string propertyScriptName)
        {
            foreach (cwTreeNodeProperty P in treeView.Nodes)
            {                
                if (P == null)
                {
                    throw new cwExceptionWarning("Missing Requested Property [" + propertyScriptName + "]"); 
                }
                else
                {
                    if (P.Property.ScriptName.Equals(propertyScriptName))
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
            if (treeNodeObjectGroup != null && treeNodeObjectGroup.getSelectedObjectType() == null) return;
            String _attributes = _item as String;
            String[] _attributesArray = _attributes.Split(',');

            int i = 0;
            foreach (String _a in _attributesArray)
            {
                // escape the last element
                if (i == _attributesArray.Length - 1) break;
                cwTreeNodeProperty P = searchInNodesByPropertyScriptname(_a);
                if (P == null) continue;
                P.Checked = true;
                i++;
            }
        }
    }
}
