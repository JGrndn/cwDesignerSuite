using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.API;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// The Object Node tree node
    /// </summary>
    public class cwPSFTreeNodeObjectNode : cwPSFTreeNode
    {
        internal List<cwPSFPropertyBoxCollection> objectTypeDependantsPropertiesBoxes = new List<cwPSFPropertyBoxCollection>();
        /// <summary>
        /// CONFIG_OG_ID
        /// </summary>
        public const string CONFIG_OG_ID = "object-node-id";
        internal const string CONFIG_SORT_ON_PROPERTY = "sort-on-property";
        internal const string CONFIG_SORT_ON_REVERSE = "sort-reverse";
        /// <summary>
        /// List the selected properties
        /// </summary>
        public const string CONFIG_SELECTED_PROPERTIES = "selected-properties";
        internal const string CONFIG_FILTER_PROPERTIES_AND = "filter-properties-and";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTreeNodeObjectNode"/> class.
        /// </summary>
        /// <param name="_cwEditModeGUI">The _CW edit mode GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwPSFTreeNodeObjectNode(cwEditModeGUI _cwEditModeGUI, cwPSFTreeNode _parent)
            : base(_cwEditModeGUI, _parent)
        {
            updateText("Object Node");
        }

        /// <summary>
        /// Adds the attribute for filter AND.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <param name="filterValue">The filter value.</param>
        public void addAttributeForFilterAND(string propertyScriptName, string filterValue)
        {
            cwPSFPropertyBoxFilterProperties filterBox = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxFilterProperties>(CONFIG_FILTER_PROPERTIES_AND);
            filterBox.getDataGrid().addDataGridRow("=", propertyScriptName, filterValue);
        }
        /// <summary>
        /// Gets or sets the name of the sort on property script.
        /// </summary>
        /// <value>
        /// The name of the sort on property script.
        /// </value>
        internal string sortOnPropertyScriptName
        {
            get
            {
                return propertiesBoxes.getPropertyBoxCollection(CONFIG_SORT_ON_PROPERTY).ToString();
            }
            set
            {
                propertiesBoxes.getPropertyBoxCollection(CONFIG_SORT_ON_PROPERTY).setValue(value);
            }
        }


        /// <summary>
        /// Gets the target objects by ID.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<int, cwLightObject> getTargetObjectsByID()
        {
            throw new NotImplementedException("getTargetObjectsByID");
        }

        /// <summary>
        /// Gets the name of the box.
        /// </summary>
        /// <returns></returns>
        public virtual string getBoxName()
        {
            throw new NotImplementedException("getBoxName");
        }

        internal bool sortOnReverse
        {
            get
            {
                return propertiesBoxes.getPropertyBoxBoolean(CONFIG_SORT_ON_REVERSE).Checked;
            }
            set
            {
                propertiesBoxes.getPropertyBoxBoolean(CONFIG_SORT_ON_REVERSE).Checked = value;
            }
        }

        //public string layoutName
        //{
        //    get
        //    {
        //        return propertiesBoxes.getPropertyBox(CONFIG_LAYOUT_ID).Text;
        //    }
        //    set
        //    {
        //        propertiesBoxes.getPropertyBox(CONFIG_SORT_ON_REVERSE).Checked = value;
        //    }
        //}



        /// <summary>
        /// Checks the name of the property by script.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        internal void checkPropertyByScriptName(string propertyScriptName)
        {
            cwPSFPropertyBoxTreeViewProperties properties = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewProperties>(CONFIG_SELECTED_PROPERTIES);
            cwTreeNodeProperty propertyNode = properties.searchInNodesByPropertyScriptname(propertyScriptName) as cwTreeNodeProperty;
            if (propertyNode != null)
            {
                propertyNode.Checked = true;
            }
        }

        /// <summary>
        /// Gets or sets the selected checked properties script names.
        /// </summary>
        /// <value>
        /// The selected checked properties script names.
        /// </value>
        public List<string> selectedCheckedPropertiesScriptNames
        {
            get
            {
                List<string> _selectPropertiesScriptNames = new List<string>();
                propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewProperties>(CONFIG_SELECTED_PROPERTIES).getCheckedPropertiesList().ForEach(p => _selectPropertiesScriptNames.Add(p.ScriptName));
                return _selectPropertiesScriptNames;
            }
            set
            {
                cwPSFPropertyBoxTreeViewProperties properties = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewProperties>(CONFIG_SELECTED_PROPERTIES);
                foreach (string propertyScriptName in value)
                {
                    checkPropertyByScriptName(propertyScriptName);
                }
            }
        }
        /// <summary>
        /// Gets the selected checked properties.
        /// </summary>
        public List<cwLightProperty> selectedCheckedProperties
        {
            get
            {
                return propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewProperties>(CONFIG_SELECTED_PROPERTIES).getCheckedPropertiesList();
            }
        }


        
        /// <summary>
        /// Adds the association types to context menu.
        /// </summary>
        public void addAssociationTypesToContextMenu<AssociationTypeNode, GUI>(ContextMenuStrip menuStrip, cwPSFTreeNode toNode, List<string> availableObjectTypeScriptNames)
            where AssociationTypeNode : cwPSFTreeNodeObjectNode
            where GUI : cwEditModeGUI
        {
            if (getSelectedObjectType() == null)
            {
                return;
            }
            cwLightObjectType selectedOT = getSelectedObjectType();
            ToolStripMenuItem c = new ToolStripMenuItem(Properties.Resources.PSF_TN_NODE_CTX_ADD_ASSOCIATION_TYPE);
            c.Image = Casewise.GraphAPI.Properties.Resources.image_tvicon_node;
            foreach (var targetOTVar in selectedOT.getAssociationTypesByTargetObjectType())
            {
                ToolStripMenuItem menuOTTarget = new ToolStripMenuItem(targetOTVar.Key.ToString());
                for (var i = 0; i < targetOTVar.Value.Count(); ++i)
                {
                    cwLightAssociationType AT = targetOTVar.Value[i];
                    if (availableObjectTypeScriptNames != null && availableObjectTypeScriptNames.Count > 0)
                    {
                        if (!availableObjectTypeScriptNames.Contains(AT.Target.ScriptName))
                        {
                            continue;
                        }
                    }
                    ToolStripItem item = menuOTTarget.DropDownItems.Add(AT.ToString());
                    item.Click += (sender, args) => ctx_addChildAssociationTypeNode<AssociationTypeNode, GUI>(toNode, menuStrip, AT, args);
                }
                if (menuOTTarget.DropDownItems.Count > 0)
                {
                    c.DropDownItems.Add(menuOTTarget);
                }
                
            }
            menuStrip.Items.Add(c);
        }



        /// <summary>
        /// Handles the Click event of the addObjectGroupToolStripMenuItem control.
        /// </summary>
        /// <param name="toNode">To node.</param>
        /// <param name="menuStrip">The menu strip.</param>
        /// <param name="associationTypeNode">The association type node.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_addChildAssociationTypeNode<AssociationTypeNode, GUI>(cwPSFTreeNode toNode, ContextMenuStrip menuStrip, cwLightAssociationType associationTypeNode, EventArgs e)
            where AssociationTypeNode : cwPSFTreeNodeObjectNode
            where GUI : cwEditModeGUI
        {
            AssociationTypeNode ATNode = Activator.CreateInstance(typeof(AssociationTypeNode), new object[] { toNode.getGUI<GUI>(), toNode }) as AssociationTypeNode;
            ATNode.addAndSetAssociationType(associationTypeNode);
            toNode.addChildNodeLast(ATNode);
            toNode.Expand();
            toNode.checkNodeStructureRec();
        }


        internal void clearAttributeFiltersGrid()
        {
            cwPSFPropertyBoxFilterProperties filterBox = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxFilterProperties>(CONFIG_FILTER_PROPERTIES_AND);
            if (filterBox != null)
            {
                cwPSFPropertyBoxDataGrid _dg = filterBox.getDataGrid();
                _dg.clearGrid();
            }
        }

        internal Dictionary<string, List<KeyValuePair<string, string>>> attributeFiltersKeep
        {
            get
            {
                Dictionary<string, List<KeyValuePair<string, string>>> _attributeFiltersKeep = new Dictionary<string, List<KeyValuePair<string, string>>>();
                cwPSFPropertyBoxFilterProperties filterBox = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxFilterProperties>(CONFIG_FILTER_PROPERTIES_AND);
                if (filterBox != null)
                {
                    cwPSFPropertyBoxDataGrid _dg = filterBox.getDataGrid();
                    return _dg.getAttributesFiltered();
                }
                return _attributeFiltersKeep;
            }
            set
            {
                cwPSFPropertyBoxFilterProperties filterBox = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxFilterProperties>(CONFIG_FILTER_PROPERTIES_AND);
                if (filterBox != null)
                {
                    foreach (var attributesKeep in value)
                    {
                        foreach (KeyValuePair<string, string> attributesValue in attributesKeep.Value)
                        {
                            filterBox.getDataGrid().addDataGridRow(attributesValue.Key, attributesKeep.Key, attributesValue.Value);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Copies from another node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void copyFromAnotherNode(cwPSFTreeNodeObjectNode node)
        {
            selectedCheckedPropertiesScriptNames = node.selectedCheckedPropertiesScriptNames;
            sortOnPropertyScriptName = node.sortOnPropertyScriptName;
            sortOnReverse = node.sortOnReverse;
            attributeFiltersKeep = node.attributeFiltersKeep;
        }



        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public virtual cwLightObjectType getSelectedObjectType()
        {
            throw new NotImplementedException("getSelectedObjectType");
        }

        /// <summary>
        /// Sets the type of the selected object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public virtual void setSelectedObjectType(cwLightObjectType objectType)
        {
            throw new NotImplementedException("setSelectedObjectType");
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
            cwPSFPropertyBoxString OGId = new cwPSFPropertyBoxString(Properties.Resources.PSF_TN_NODE_OG_OGID_NAME, Properties.Resources.PSF_TN_NODE_OG_OGID_HELP, CONFIG_OG_ID);
            propertiesBoxes.addPropertyBox(OGId);

            cwPSFPropertyBoxComboBoxProperties sortOn = new cwPSFPropertyBoxComboBoxProperties(Properties.Resources.PSF_TN_NODE_OG_SORTON_NAME, Properties.Resources.PSF_TN_NODE_OG_SORTON_HELP, CONFIG_SORT_ON_PROPERTY, this);
            cwPSFPropertyBoxCheckBox sortReverse = new cwPSFPropertyBoxCheckBox(Properties.Resources.PSF_TN_NODE_OG_SORTON_REVERSE_NAME, Properties.Resources.PSF_TN_NODE_OG_SORTON_REVERSE_HELP, CONFIG_SORT_ON_REVERSE, false);
            cwPSFPropertyBoxTreeViewProperties selectedProperties = new cwPSFPropertyBoxTreeViewProperties(Properties.Resources.PSF_TN_NODE_OG_SELECTEDPROPERTIES_NAME, Properties.Resources.PSF_TN_NODE_OG_SELECTEDPROPERTIES_HELP, CONFIG_SELECTED_PROPERTIES, this);
            cwPSFPropertyBoxFilterProperties filterPropertiesAND = new cwPSFPropertyBoxFilterProperties(Properties.Resources.PSF_TN_NODE_OG_FILTER_AND_NAME, Properties.Resources.PSF_TN_NODE_OG_FILTER_AND_HELP, CONFIG_FILTER_PROPERTIES_AND);
            filterPropertiesAND.setPSFParentTreeNode(this);
            objectTypeDependantsPropertiesBoxes.Add(sortOn);
            objectTypeDependantsPropertiesBoxes.Add(selectedProperties);
            objectTypeDependantsPropertiesBoxes.Add(filterPropertiesAND);

            propertiesBoxes.addPropertyBox(sortOn);
            propertiesBoxes.addPropertyBox(sortReverse);
            propertiesBoxes.addPropertyBox(selectedProperties);
            propertiesBoxes.addPropertyBox(filterPropertiesAND);


        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get
            {
                return getStringProperty(CONFIG_OG_ID);
            }
            set
            {
                string v = cwTools.stringToID(value);
                propertiesBoxes.getPropertyBox(CONFIG_OG_ID).setValue(v);
            }
        }

        /// <summary>
        /// Updates the object type dependendant properties boxes.
        /// </summary>
        /// <param name="selectedObjectType">Type of the selected object.</param>
        /// <param name="sourceUpdatePropertyCollection">The source update property collection.</param>
        public void updateObjectTypeDependendantPropertiesBoxes(cwLightObjectType selectedObjectType, cwPSFPropertyBoxCollection sourceUpdatePropertyCollection)
        {
            foreach (cwPSFPropertyBoxCollection collection in objectTypeDependantsPropertiesBoxes)
            {
                collection.loadNodes(selectedObjectType);
            }
            updateName();
            if (hasAtLeastOnChildNode<cwPSFTreeNodeObjectNode>())
            {
                foreach (cwPSFTreeNodeObjectNode childNode in getChildrenNodes<cwPSFTreeNodeObjectNode>())
                {
                    childNode.setSelectedObjectType(selectedObjectType);
                }
            }
        }


        /// <summary>
        /// Gets the children object nodes.
        /// </summary>
        /// <returns></returns>
        public List<cwPSFTreeNodeObjectNode> getChildrenObjectNodes()
        {
            return getChildrenGeneric<cwPSFTreeNodeObjectNode>();
        }



        /// <summary>
        /// Updates the name.
        /// </summary>
        public virtual void updateName()
        {
            throw new NotImplementedException("updateName");
        }


        /// <summary>
        /// Adds the object types to context menu.
        /// </summary>
        /// <typeparam name="ObjectTypeNode">The type of the bject type node.</typeparam>
        /// <typeparam name="GUI">The type of the UI.</typeparam>
        /// <param name="menuStrip">The menu strip.</param>
        /// <param name="toNode">To node.</param>
        /// <param name="model">The model.</param>
        /// <param name="acceptedObjectTypesScriptNames">The accepted object types script names.</param>
        public static void addObjectTypesToContextMenu<ObjectTypeNode, GUI>(ContextMenuStrip menuStrip, cwPSFTreeNode toNode, cwLightModel model, List<string> acceptedObjectTypesScriptNames)
            where ObjectTypeNode : cwPSFTreeNodeObjectNode
            where GUI : cwEditModeGUI
        {
            ToolStripMenuItem c = new ToolStripMenuItem(Properties.Resources.PSF_TN_ADD_OBJECT_TYPE);
            c.Image = Casewise.GraphAPI.Properties.Resources.image_tvicon_node;

            List<cwLightObjectType> OTs = model.getPSFEnabledObjectTypes(false);
            OTs.Sort();
            for (int i = 0; i < OTs.Count; ++i)
            {
                cwLightObjectType OT = OTs[i];
                if (acceptedObjectTypesScriptNames != null && acceptedObjectTypesScriptNames.Count > 0)
                {
                    if (!acceptedObjectTypesScriptNames.Contains(OT.ScriptName))
                    {
                        continue;
                    }
                }

                ToolStripItem item = c.DropDownItems.Add(OT.ToString());
                item.Click += (sender, args) => ctx_addChildObjectTypeNode<ObjectTypeNode, GUI>(toNode, menuStrip, OT, args);
                c.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(c);
        }

        /// <summary>
        /// Handles the Click event of the addObjectGroupToolStripMenuItem control.
        /// </summary>
        /// <param name="toNode">To node.</param>
        /// <param name="menuStrip">The menu strip.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void ctx_addChildObjectTypeNode<ObjectTypeNode, GUI>(cwPSFTreeNode toNode, ContextMenuStrip menuStrip, cwLightObjectType objectType, EventArgs e)
            where ObjectTypeNode : cwPSFTreeNodeObjectNode
            where GUI : cwEditModeGUI
        {
            ObjectTypeNode OTNode = Activator.CreateInstance(typeof(ObjectTypeNode), new object[] { toNode.getGUI<GUI>(), toNode }) as ObjectTypeNode;
            OTNode.setSelectedObjectType(objectType);
            toNode.addChildNodeLast(OTNode);
            toNode.Expand();
            toNode.checkNodeStructureRec();
        }


        #region AssociationTypes

        /// <summary>
        /// Gets the type of the selected association.
        /// </summary>
        /// <returns></returns>
        public cwLightAssociationType getSelectedAssociationType(cwPSFTreeNodeObjectNode node)
        {
            return node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE).getSelectedAssociationType();
        }


        /// <summary>
        /// Sets the type of the selected association.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="associationType">Type of the association.</param>
        public void setSelectedAssociationType(cwPSFTreeNodeObjectNode node, cwLightAssociationType associationType)
        {
            node.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE).setValue(associationType.ScriptName);
        }


        /// <summary>
        /// Gets the type of the selected association.
        /// </summary>
        /// <returns></returns>
        public cwLightAssociationType getSelectedAssociationType()
        {
            return propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE).getSelectedAssociationType();
        }


        /// <summary>
        /// Sets the type of the selected association.
        /// </summary>
        /// <param name="associationType">Type of the association.</param>
        public void addAndSetAssociationType(cwLightAssociationType associationType)
        {
            propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE) .add(associationType);
            setSelectedAssociationType(this, associationType);
        }




        #endregion
    }
}
