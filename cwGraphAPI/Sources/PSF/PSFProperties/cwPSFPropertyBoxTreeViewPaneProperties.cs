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
    /// Allow to select property types by pane 
    /// </summary>
    public class cwPSFPropertyBoxTreeViewPaneProperties : cwPSFPropertyBoxTreeView
    {

        private cwPSFTreeNodeObjectNode treeNodeObjectGroup = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxTreeViewPaneProperties"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeObjectGroup">The _tree node object group.</param>
        public cwPSFPropertyBoxTreeViewPaneProperties(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeObjectNode _treeNodeObjectGroup)
            : base(_helpName, _helpDescription, _keyName)
        {
            treeNodeObjectGroup = _treeNodeObjectGroup;
            treeView.CheckBoxes = true;
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
                foreach (cwLightPane Pane in OT.getSortedPanes())
                {
                    TreeNode paneTN = new cwTreeNodePane(Pane);
                    foreach (cwLightProperty p in Pane.sortedProperties)
                    {
                        paneTN.Nodes.Add(new cwTreeNodeProperty(p));
                    }
                    treeView.Nodes.Add(paneTN);
                }
            }
            else
            {
                disable();
            }
            //treeView.Sort();
            treeView.Height = treeView.GetNodeCount(true) * TREEVIEW_ITEM_SIZE;
        }


        /// <summary>
        /// Gets the checked properties list.
        /// </summary>
        /// <returns></returns>
        public List<cwLightProperty> getCheckedPropertiesList()
        {
            List<cwLightProperty> _elements = new List<cwLightProperty>();
            foreach (cwTreeNodePane pane in treeView.Nodes)
            {
                foreach (cwTreeNodeProperty tn in pane.Nodes)
                {
                    if (tn.Checked)
                    {
                        _elements.Add(tn.Property);
                    }
                }
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
            foreach (cwTreeNodePane pane in treeView.Nodes)
            {
                foreach (cwTreeNodeProperty p in pane.Nodes)
                {
                    if (true.Equals(p.Checked))
                    {
                        attributes += p.Property.ScriptName + ",";
                    }
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
            foreach (cwTreeNodePane pane in treeView.Nodes)
            {
                foreach (cwTreeNodeProperty p in pane.Nodes)
                {
                    if (p.Property.ScriptName.Equals(propertyScriptName))
                    {
                        return p;
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
                if (P.Parent != null)
                {
                    P.Parent.Checked = true;
                    P.Parent.ExpandAll();
                }
                i++;
            }
        }
    }
}
