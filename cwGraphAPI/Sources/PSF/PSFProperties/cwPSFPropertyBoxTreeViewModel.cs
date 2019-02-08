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
    /// Allow to check models in a tree view
    /// </summary>
    public class cwPSFPropertyBoxTreeViewModel : cwPSFPropertyBoxTreeView
    {

        private cwPSFTreeNodeConfigurationNode treeNodeConfiguration = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxTreeViewModel"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_treeNodeConfiguration">The _tree node configuration.</param>
        /// <param name="_currentConnection">The _current connection.</param>
        public cwPSFPropertyBoxTreeViewModel(String _helpName, String _helpDescription, String _keyName, cwPSFTreeNodeConfigurationNode _treeNodeConfiguration, cwConnection _currentConnection)
            : base (_helpName, _helpDescription, _keyName)
        {
            treeNodeConfiguration = _treeNodeConfiguration;
            treeView.CheckBoxes = true;
            loadNodes(_currentConnection);
        }

        /// <summary>
        /// Loads the nodes.
        /// </summary>
        public override void loadNodes(object source)
        {
            cwConnection _currentConnection = source as cwConnection;
            clearItems();
            // if not OT has been loaded, load the model OTs
            if (_currentConnection != null)
            {
                enable();
                foreach (cwLightModel M in _currentConnection.getModels())
                {
                    int tn_pos = treeView.Nodes.Add(new cwTreeNodeModel(M));
                    TreeNode tn = treeView.Nodes[tn_pos];
                    if (treeNodeConfiguration.operationEditModeGUI.Model.FileName.Equals(M.FileName))
                    {
                        tn.Checked = true;
                    }
                }
            }
            else
            {
                disable();
            }
            treeView.Sort();
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
                cwTreeNodeModel tnP = _node as cwTreeNodeModel;
                if (tnP == null) continue;
                if (true.Equals(_node.Checked))
                {
                    attributes += tnP.model.FileName + ",";
                }
            }
            return attributes;
        }


        /// <summary>
        /// Searches the in items by property.
        /// </summary>
        /// <param name="modelFileName">Name of the model file.</param>
        /// <returns></returns>
        private cwTreeNodeModel searchInNodesByProperty(string modelFileName)
        {
            foreach (cwTreeNodeModel M in treeView.Nodes)
            {                
                if (M == null)
                {
                    throw new cwExceptionWarning("Missing Requested Property [" + modelFileName + "]"); 
                }
                else
                {
                    if (M.model.FileName.Equals(modelFileName))
                    {
                        return M;
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
            //if (treeNodeConfiguration.getSelectedcwLightObjectType() == null) return;
            String _attributes = _item as String;
            String[] _attributesArray = _attributes.Split(',');

            int i = 0;
            foreach (String _a in _attributesArray)
            {
                // escape the last element
                if (i == _attributesArray.Length - 1) break;
                cwTreeNodeModel P = searchInNodesByProperty(_a);
                if (P == null) continue;
                P.Checked = true;
                i++;
            }
        }
    }
}
