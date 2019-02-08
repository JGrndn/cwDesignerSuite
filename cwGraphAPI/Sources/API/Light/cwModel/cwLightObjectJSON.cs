using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Represents a serizable object in JSON
    /// </summary>
    public class cwLightObjectJSON
    {
        /// <summary>
        /// Gets or sets the object_id.
        /// </summary>
        /// <value>
        /// The object_id.
        /// </value>
        public int object_id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }
        //public string link_id { get; set; }
        /// <summary>
        /// Gets or sets the name of the object type script.
        /// </summary>
        /// <value>
        /// The name of the object type script.
        /// </value>
        public string objectTypeScriptName { get; set; }
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public Dictionary<string, object> properties { get; set; }
        /// <summary>
        /// the list of associated json objects sorted by node ID
        /// </summary>
        public Dictionary<string, List<cwLightObjectJSON>> associations = new Dictionary<string, List<cwLightObjectJSON>>();
        /// <summary>
        /// the list of associations with the node name and the target object type scriptname
        /// </summary>
        public Dictionary<string, associationTarget> associationsTargetObjectTypes = new Dictionary<string, associationTarget>();
        /// <summary>
        /// the list of properties groups properties and type
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> propertiesGroups = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// the list of intersections objects to the target object
        /// </summary>
        public Dictionary<string, string> iProperties = new Dictionary<string, string>();
        /// <summary>
        /// the name of the layout
        /// </summary>
        public string layoutName { get; set; }

        /// <summary>
        /// Gets or sets the diagrams showing internal event.
        /// </summary>
        /// <value>
        /// The diagrams showing internal event.
        /// </value>
        public List<cwLightObjectJSON> diagramsShowingInternalEvent { get; set; }
        /// <summary>
        /// Gets or sets the diagrams showing internal result.
        /// </summary>
        /// <value>
        /// The diagrams showing internal result.
        /// </value>
        public List<cwLightObjectJSON> diagramsShowingInternalResult { get; set; }

        /// <summary>
        /// associationTarget display names and target object type scriptname
        /// </summary>
        public struct associationTarget
        {
            /// <summary>
            /// targetScriptName
            /// </summary>
            public string targetScriptName;
            /// <summary>
            /// displayNodeName
            /// </summary>
            public string displayNodeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObjectJSON"/> class.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        public cwLightObjectJSON(cwLightObject mainObject)
        {
            object_id = mainObject.ID;
            name = mainObject.ToString();
            objectTypeScriptName = mainObject.getObjectType().ScriptName.ToLower();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObjectJSON" /> class.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The OG node.</param>
        /// <param name="loadDiagramForEventResult">if set to <c>true</c> [load diagram for event result].</param>
        public cwLightObjectJSON(cwLightObject mainObject, cwLightNode OGNode, bool loadDiagramForEventResult = false)
        {
            try
            {
                object_id = mainObject.ID;
                layoutName = OGNode.layoutName;
                name = mainObject.ToString();
                objectTypeScriptName = mainObject.getObjectType().ScriptName.ToLower();
                loadProperties(mainObject, OGNode);
                loadAssociations(mainObject, OGNode);
                loadPropertiesGroupFromNode(mainObject, OGNode);
                if (mainObject.getObjectType().ScriptName == cwLightObjectType.EVENTRESULT_SCRIPTNAME && loadDiagramForEventResult)
                {
                    this.diagramsShowingInternalEvent = new List<cwLightObjectJSON>();
                    this.diagramsShowingInternalResult = new List<cwLightObjectJSON>();

                    List<cwLightObject> diagForEvent = new List<cwLightObject>();
                    List<cwLightObject> diagForResult = new List<cwLightObject>();
                    Diagrams.cwDiagramLoader loader = new Diagrams.cwDiagramLoader(mainObject.Model);
                    loader.getDiagramsWhereEventResultIsShown(mainObject, diagForEvent, diagForResult);
                    foreach (cwLightObject d in diagForEvent)
                    {
                        this.diagramsShowingInternalEvent.Add(new cwLightObjectJSON(d));
                    }
                    foreach (cwLightObject d in diagForResult)
                    {
                        this.diagramsShowingInternalResult.Add(new cwLightObjectJSON(d));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObjectJSON" /> class.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The og node.</param>
        /// <param name="diagramIdsByEventResultId">The diagram ids by event result unique identifier.</param>
        public cwLightObjectJSON(cwLightObject mainObject, cwLightNode OGNode, Dictionary<int, List<int>> diagramIdsByEventResultId)
        {
            try
            {
                object_id = mainObject.ID;
                name = mainObject.ToString();
                objectTypeScriptName = mainObject.getObjectType().ScriptName.ToLower();
                loadProperties(mainObject, OGNode);
                loadAssociations(mainObject, OGNode, diagramIdsByEventResultId);
                loadPropertiesGroupFromNode(mainObject, OGNode);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Loads the properties.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The OG node.</param>
        private void loadProperties(cwLightObject mainObject, cwLightNode OGNode)
        {
            cwLightObjectType objectType = mainObject.getObjectType();
            properties = new Dictionary<string, object>();

            foreach (string p_Scriptname in OGNode.selectedPropertiesScriptName)
            {
                if (mainObject.properties.ContainsKey(p_Scriptname))
                {
                    string p_value = mainObject.properties[p_Scriptname];
                    string lowerScriptName = p_Scriptname.ToLower();
                    if (p_value != null)
                    {
                        string js_p_value = cwTools.escapeChars(p_value);
                        properties[lowerScriptName] = js_p_value;
                        if ("Lookup".Equals(objectType.getProperty(p_Scriptname).DataType))
                        {
                            properties[lowerScriptName + "_abbreviation"] = mainObject.properties[p_Scriptname + cwLookupManager.LOOKUPABBR_KEY];
                            properties[lowerScriptName + "_id"] = mainObject.properties[p_Scriptname + cwLookupManager.LOOKUPID_KEY];
                        }
                    }
                    else
                    {
                        properties[lowerScriptName] = "";
                    }
                }
            }
        }

        /// <summary>
        /// Loads the associations.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The og node.</param>
        /// <param name="diagramIdsByEventResult">The diagram ids by event result.</param>
        public void loadAssociations(cwLightObject mainObject, cwLightNode OGNode, Dictionary<int, List<int>> diagramIdsByEventResult)
        {
            foreach (cwLightNode childOGNode in OGNode.childrenNodes)
            {
                List<cwLightObjectJSON> childrenNodes = new List<cwLightObjectJSON>();
                if (childOGNode.targetLightObjectBySourceKey.ContainsKey(mainObject.ID))
                {
                    bool filterTargetUsingDico = false;
                    if ("EVENTRESULT".Equals(mainObject.getObjectType().ScriptName))
                    {
                        cwLightNodeAssociationType nodeAt = childOGNode as cwLightNodeAssociationType;
                        if (nodeAt != null && "ANYOBJECTSHOWNASSHAPEINDIAGRAM".Equals(nodeAt.AssociationType.ScriptName))
                        {
                            filterTargetUsingDico = true;
                        }
                    }
                    foreach (var o_child in childOGNode.targetLightObjectBySourceKey[mainObject.ID])
                    {
                        cwLightObjectJSON jsonObject = null;
                        if (filterTargetUsingDico)
                        {
                            if (!diagramIdsByEventResult.ContainsKey(mainObject.ID) || !diagramIdsByEventResult[mainObject.ID].Contains(o_child.Value.ID))
                            {
                                continue;
                            }
                        }
                        jsonObject = new cwLightObjectJSON(o_child.Value, childOGNode);
                        if (o_child.Key != null)
                        {
                            jsonObject.iProperties = o_child.Key.properties;
                        }
                        childrenNodes.Add(jsonObject);
                    }
                    associations[childOGNode.ID] = childrenNodes;
                }
                else
                {
                    associations[childOGNode.ID] = new List<cwLightObjectJSON>();
                }
                cwLightNodeAssociationType ATNode = childOGNode as cwLightNodeAssociationType;
                if (ATNode != null)
                {
                    associationTarget ATDisplays = new associationTarget();
                    ATDisplays.displayNodeName = ATNode.nodeName;
                    ATDisplays.targetScriptName = ATNode.AssociationType.Target.ScriptName.ToLower();
                    associationsTargetObjectTypes[childOGNode.ID] = ATDisplays;
                }

            }
        }

        /// <summary>
        /// Loads the associations.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The OG node.</param>
        public void loadAssociations(cwLightObject mainObject, cwLightNode OGNode)
        {
            foreach (cwLightNode childOGNode in OGNode.childrenNodes)
            {
                List<cwLightObjectJSON> childrenNodes = new List<cwLightObjectJSON>();
                if (childOGNode.targetLightObjectBySourceKey.ContainsKey(mainObject.ID))
                {
                    foreach (var o_child in childOGNode.targetLightObjectBySourceKey[mainObject.ID])
                    {
                        cwLightObjectJSON jsonObject = new cwLightObjectJSON(o_child.Value, childOGNode);
                        if (o_child.Key != null)
                        {
                            jsonObject.iProperties = o_child.Key.properties;
                        }
                        childrenNodes.Add(jsonObject);
                    }
                    associations[childOGNode.ID] = childrenNodes;
                }
                else
                {
                    associations[childOGNode.ID] = new List<cwLightObjectJSON>();
                }
                cwLightNodeAssociationType ATNode = childOGNode as cwLightNodeAssociationType;
                if (ATNode != null)
                {
                    associationTarget ATDisplays = new associationTarget();
                    ATDisplays.displayNodeName = ATNode.nodeName;
                    ATDisplays.targetScriptName = ATNode.AssociationType.Target.ScriptName.ToLower();
                    associationsTargetObjectTypes[childOGNode.ID] = ATDisplays;
                }

            }
        }

        /// <summary>
        /// Loads the properties group from node.
        /// </summary>
        /// <param name="mainObject">The main object.</param>
        /// <param name="OGNode">The OG node.</param>
        private void loadPropertiesGroupFromNode(cwLightObject mainObject, cwLightNode OGNode)
        {
            propertiesGroups = OGNode.propertiesGroups;
        }


    }
}
