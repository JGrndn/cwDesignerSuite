using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Represent a node
    /// </summary>
    public class cwLightNode
    {
        /// <summary>
        /// The object type of the node
        /// </summary>
        protected cwLightObjectType selectedObjectType = null;
        /// <summary>
        /// The parent of the node
        /// </summary>
        protected cwLightNode parent = null;
        /// <summary>
        /// The if of the node
        /// </summary>
        public string ID = "NOT_SET";
        /// <summary>
        /// List of objects
        /// </summary>
        public List<cwLightObject> usedOTLightObjects = new List<cwLightObject>();
        /// <summary>
        /// List of objects by ID
        /// </summary>
        public Dictionary<int, cwLightObject> usedOTLightObjectsByID = new Dictionary<int, cwLightObject>();
        /// <summary>
        /// List of objects by UUID
        /// </summary>
        public Dictionary<string, cwLightObject> usedOTLightObjectsByUUID = new Dictionary<string, cwLightObject>();
        /// <summary>
        /// List of objects by Name
        /// </summary>
        public Dictionary<string, cwLightObject> usedOTLightObjectsByNAME = new Dictionary<string, cwLightObject>();
        /// <summary>
        /// by source key propose the list of (intersection object, target object) using the object group association
        /// the target object should be available using usedOTLightObjectsByID
        /// new KeyValuePair(cwLightObject, cwLightObject)(intersectionObject, belowObject)
        /// </summary>
        public Dictionary<int, List<KeyValuePair<cwLightObject, cwLightObject>>> targetLightObjectBySourceKey = new Dictionary<int, List<KeyValuePair<cwLightObject, cwLightObject>>>();

        /// <summary>
        /// nodeName
        /// </summary>
        public string nodeName = "";

        /// <summary>
        /// layoutName
        /// </summary>
        public string layoutName = "";
        /// <summary>
        /// The list of children nodes
        /// </summary>
        public List<cwLightNode> childrenNodes = new List<cwLightNode>();
        /// <summary>
        /// The properties groups associated to the node
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> propertiesGroups = new Dictionary<string, Dictionary<string, object>>();
        /// <summary>
        /// If sort should be reversed
        /// </summary>
        public bool sortOnReverse = false;
        /// <summary>
        /// The property where the sort should be applied
        /// </summary>
        public string sortOnPropertyScriptName = null;
        /// <summary>
        /// The list of scriptnames of the selected properties to load
        /// </summary>
        public List<string> selectedPropertiesScriptName = new List<string>();
        /// <summary>
        /// The properties to load
        /// </summary>
        public List<cwLightProperty> selectedProperties = new List<cwLightProperty>();
        /// <summary>
        /// The AND filter list
        /// </summary>
        protected Dictionary<string, List<KeyValuePair<string, string>>> attributeFiltersKeep = new Dictionary<string, List<KeyValuePair<string, string>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNode"/> class.
        /// </summary>
        /// <param name="nodeTN">The node TN.</param>
        public cwLightNode(cwPSFTreeNodeObjectNode nodeTN)
        {
            if (nodeTN == null) {
                return;
            }

            this.ID = nodeTN.ID;
            this.sortOnPropertyScriptName = nodeTN.sortOnPropertyScriptName;
            this.sortOnReverse = nodeTN.sortOnReverse;
            this.selectedPropertiesScriptName = nodeTN.selectedCheckedPropertiesScriptNames;
            this.selectedProperties = nodeTN.selectedCheckedProperties;
            this.attributeFiltersKeep = nodeTN.attributeFiltersKeep;
            this.nodeName = nodeTN.getName();
            this.layoutName = nodeTN.layoutName;
          //  this.layoutName = nodeTN.layoutName;
        }

        /// <summary>
        /// Adds the attribute for filter AND.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOperation">The filter operation.</param>
        public void addAttributeForFilterAND(string propertyScriptName, string filterValue, string filterOperation)
        {
            //cwPSFPropertyBoxFilterProperties filterBox = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxFilterProperties>(CONFIG_FILTER_PROPERTIES_AND);
            if (!attributeFiltersKeep.ContainsKey(propertyScriptName))
            {
                attributeFiltersKeep[propertyScriptName] = new List<KeyValuePair<string, string>>();
            }
            attributeFiltersKeep[propertyScriptName].Add(new KeyValuePair<string, string> (filterOperation, filterValue));
        }

        /// <summary>
        /// Clears the attribute filter.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        public void clearAttributeFilter(string propertyScriptName)
        {
            if (attributeFiltersKeep.ContainsKey(propertyScriptName))
            {
                attributeFiltersKeep[propertyScriptName] = new List<KeyValuePair<string, string>>();
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNode"/> class.
        /// </summary>
        /// <param name="associationType">Type of the association.</param>
        public cwLightNode(cwLightAssociationType associationType)
        {
            this.ID = associationType.Target.ScriptName.ToLower();
        }
        /// <summary>
        /// Gets the type of the source object.
        /// </summary>
        /// <value>
        /// The type of the source object.
        /// </value>
        public cwLightObjectType sourceObjectType
        {
            get
            {
                return this.selectedObjectType;
            }
        }

        /// <summary>
        /// Gets the type of the target object.
        /// </summary>
        /// <value>
        /// The type of the target object.
        /// </value>
        public cwLightObjectType targetObjectType
        {
            get {
                if (this is cwLightNodeAssociationType)
                {
                    cwLightNodeAssociationType AT = this as cwLightNodeAssociationType;
                   return AT.AssociationType.Target;
                }
                else
                {
                    return this.sourceObjectType;
                }
            }
        }


        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns></returns>
        public cwLightNode getParent()
        {
            return parent;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void setParent(cwLightNode parent)
        {
            this.parent = parent;
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
        /// Preloads the light objects.
        /// </summary>
        public virtual void preloadLightObjects()
        {
            throw new NotImplementedException("preloadLightObjects");
        }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        public void addChildNode(cwLightNode childNode)
        {
            childNode.parent = this;
            this.childrenNodes.Add(childNode);
        }

        /// <summary>
        /// Adds the property to select.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        public virtual void addPropertyToSelect(string propertyScriptName)
        {
            if (!selectedPropertiesScriptName.Contains(propertyScriptName))
            {
                selectedProperties.Add(selectedObjectType.getProperty(propertyScriptName));
                selectedPropertiesScriptName.Add(propertyScriptName);
            }
        }

        /// <summary>
        /// Copies the node properties.
        /// </summary>
        /// <param name="node">The node.</param>
        public void copyNodeProperties(cwLightNode node)
        {
            this.sortOnPropertyScriptName = node.sortOnPropertyScriptName;
            this.sortOnReverse = node.sortOnReverse;
            this.selectedProperties = node.selectedProperties;
            this.selectedPropertiesScriptName = node.selectedPropertiesScriptName;
            this.attributeFiltersKeep = node.attributeFiltersKeep;
        }


        /// <summary>
        /// Clears the content of the node.
        /// </summary>
        protected virtual void clearNodeContent()
        {
            usedOTLightObjects.Clear();
            usedOTLightObjectsByID.Clear();
            usedOTLightObjectsByUUID.Clear();
            usedOTLightObjectsByNAME.Clear();
            targetLightObjectBySourceKey.Clear();
        }

    }
}
