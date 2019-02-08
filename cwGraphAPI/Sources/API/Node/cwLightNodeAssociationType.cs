using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.Exceptions;
using System.Data;
using Casewise.Data.ICM;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Represents an Association Type
    /// </summary>
    public class cwLightNodeAssociationType : cwLightNode
    {
        private cwLightAssociationType selectedAssociationType = null;
        /// <summary>
        /// intersectionPropertiesToLoad
        /// </summary>
        public List<string> intersectionPropertiesToLoad = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNodeAssociationType"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="sourceObjectType">Type of the source object.</param>
        /// <param name="selectedAssociationType">Type of the selected association.</param>
        public cwLightNodeAssociationType(cwLightNode parent, cwLightObjectType sourceObjectType, cwLightAssociationType selectedAssociationType)
            : base(selectedAssociationType)
        {
            this.selectedAssociationType = selectedAssociationType;
            this.selectedObjectType = sourceObjectType;
            parent.addChildNode(this);
            //loadPropertiesGroup(selectedAssociationType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNodeAssociationType"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="associationNode">The association node.</param>
        /// <param name="selectedAssociationType">Type of the selected association.</param>
        public cwLightNodeAssociationType(cwLightNode parent, cwPSFTreeNodeObjectNode associationNode, cwLightAssociationType selectedAssociationType)
            : base(associationNode)
        {
            this.selectedAssociationType = selectedAssociationType;
            this.selectedObjectType = associationNode.getSelectedObjectType();
            parent.addChildNode(this);
        }

        /// <summary>
        /// Gets the type of the association.
        /// </summary>
        /// <value>
        /// The type of the association.
        /// </value>
        public cwLightAssociationType AssociationType
        {
            get
            {
                return this.selectedAssociationType;
            }
        }

        /// <summary>
        /// Gets the target objects by ID.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, cwLightObject> getTargetObjectsByID()
        {
            Dictionary<int, cwLightObject> _targetsByID = new Dictionary<int, cwLightObject>();
            foreach (var targetVar in this.targetLightObjectBySourceKey)
            {
                foreach (var objectVar in targetVar.Value)
                {
                    if (!_targetsByID.ContainsKey(objectVar.Value.ID))
                    {
                        _targetsByID.Add(objectVar.Value.ID, objectVar.Value);
                    }
                }
            }
            return _targetsByID;
        }

        /// <summary>
        /// Preloads the light objects.
        /// </summary>
        public override void preloadLightObjects()
        {
            clearNodeContent();
            cwLightAssociationType AT_FATHER_TO_CHILD = selectedAssociationType;
            if (selectedObjectType.hasProperty(sortOnPropertyScriptName))
            {
                addPropertyToSelect(sortOnPropertyScriptName);
            }
            switch (AT_FATHER_TO_CHILD.ScriptName)
            {
                case "ANYOBJECTEXPLODEDASDIAGRAM":
                    isDiagramExploded();
                    break;
                case "ANYOBJECTSHOWNASSHAPEINDIAGRAM":
                    isDiagramOn();
                    break;
                default:
                    cwLightObjectType targetOTForDefault = selectedObjectType;
                    // load the objects by ID
                    cwLightNode parentOT = getParent();
                    if (parentOT == null)
                    {
                        throw new cwExceptionFatal("Association type node [" + ToString() + "] need an Object type node as parent");
                    }
                    usedOTLightObjectsByID = new Dictionary<int, cwLightObject>(parentOT.getTargetObjectsByID());

                    cwLightNodeObjectType OTTargetNode = new cwLightNodeObjectType(selectedAssociationType.Target);
                    OTTargetNode.copyNodeProperties(this);
                    OTTargetNode.preloadLightObjects();

                    if (usedOTLightObjectsByID.Count() > 0)
                    {
                        loadLightObjectsTargets(AT_FATHER_TO_CHILD, ref targetLightObjectBySourceKey, selectedObjectType.Model, OTTargetNode.usedOTLightObjectsByID, selectedPropertiesScriptName, attributeFiltersKeep, intersectionPropertiesToLoad, usedOTLightObjectsByID);
                    }

                    sortAssociationsTargets();
                    break;
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
            return ID + " - " + selectedAssociationType.ToString();
        }

        /// <summary>
        /// Loads the light objects targets.
        /// </summary>
        /// <param name="AT">The AT.</param>
        /// <param name="_objects">The _objects.</param>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_usedOTTargetsLightObjectsByID">The _used OT targets light objects by ID.</param>
        /// <param name="_propertiesToLoad">The _properties to load.</param>
        /// <param name="_propertiesFiltersKeep">The _properties filters keep.</param>
        /// <param name="_selectIntersectionPropertiesScriptNames">The _select intersection properties script names.</param>
        /// <param name="_usedOTSourcesLightObjectsByID">The _used OT sources light objects by ID.</param>
        public static void loadLightObjectsTargets(cwLightAssociationType AT, ref Dictionary<int, List<KeyValuePair<cwLightObject, cwLightObject>>> _objects, cwLightModel _lightModel, Dictionary<int, cwLightObject> _usedOTTargetsLightObjectsByID, List<string> _propertiesToLoad, Dictionary<string, List<KeyValuePair<string, string>>> _propertiesFiltersKeep, List<string> _selectIntersectionPropertiesScriptNames, Dictionary<int, cwLightObject> _usedOTSourcesLightObjectsByID)
        {
            cwLightObjectType Intersection_OT = AT.Intersection;
            //Model _model = OT.getFather();
            Dictionary<int, string> _IntersectionPropertiesTypeByIterationNumber = new Dictionary<int, string>();
            Dictionary<string, string> _IntersectionPropertiesTypeByScriptName = new Dictionary<string, string>();

            try
            {
                string intersectionPropertiesToLoad = cwPSFQueryManager.createPropertiesToSelectQueryString(_selectIntersectionPropertiesScriptNames, _IntersectionPropertiesTypeByIterationNumber, _IntersectionPropertiesTypeByScriptName, Intersection_OT);
                string orderToLoadBelowAbove = cwPSFQueryManager.queryOrderForIntersectionOT(AT);
                string sourcePropertyName = cwPSFQueryManager.sourceIDForIntersectionOT(AT);
                int attrToLoadCount = _selectIntersectionPropertiesScriptNames.Count();
                List<string> idsAsListOfString = new List<string>();
                _usedOTSourcesLightObjectsByID.Keys.ToList().ForEach(k =>
                {
                    idsAsListOfString.Add(k.ToString());
                });
                int end = _usedOTSourcesLightObjectsByID.Keys.Count;
                int MAX = _lightModel.DataBaseType.Equals(DbManagementSystem.SQL_Server) ? 2000 : 900; // limitation imposée par la technologie utilisée
                int iterationMax = int.Parse((end / MAX).ToString());

                int i = 0;
                while (i < iterationMax)
                {
                    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, MAX);
                    cwLightNodeAssociationType.executeQueryForLoadLightObjectsTargets(intersectionPropertiesToLoad + orderToLoadBelowAbove, Intersection_OT, sourcePropertyName, ids, _lightModel, attrToLoadCount, _selectIntersectionPropertiesScriptNames, ref _IntersectionPropertiesTypeByIterationNumber, _usedOTTargetsLightObjectsByID, ref _objects);
                    i++;
                }
                if ((end % MAX) < MAX)
                {
                    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, end % MAX);
                    cwLightNodeAssociationType.executeQueryForLoadLightObjectsTargets(intersectionPropertiesToLoad + orderToLoadBelowAbove, Intersection_OT, sourcePropertyName, ids, _lightModel, attrToLoadCount, _selectIntersectionPropertiesScriptNames, ref _IntersectionPropertiesTypeByIterationNumber, _usedOTTargetsLightObjectsByID, ref _objects);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Executes the query for load light objects targets.
        /// </summary>
        private static void executeQueryForLoadLightObjectsTargets(string intersectionPropertiesToLoad, cwLightObjectType Intersection_OT, string sourcePropertyName, string sourcesIDs,
            cwLightModel _lightModel, int attrToLoadCount, List<string> _selectIntersectionPropertiesScriptNames, ref Dictionary<int, string> _IntersectionPropertiesTypeByIterationNumber, Dictionary<int, cwLightObject> _usedOTTargetsLightObjectsByID,
            ref Dictionary<int, List<KeyValuePair<cwLightObject, cwLightObject>>> _objects)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT " + intersectionPropertiesToLoad + " FROM " + Intersection_OT.ScriptName + " WHERE " + sourcePropertyName + " IN (" + sourcesIDs + ")");


            using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> _properties = new Dictionary<string, string>();

                        cwPSFQueryManager.updatePropertiesFromQueryResponse(reader, _properties, _selectIntersectionPropertiesScriptNames, _IntersectionPropertiesTypeByIterationNumber, _lightModel);

                        int InterSection_ID = reader.GetInt32(attrToLoadCount);
                        string InterSection_UUID = reader.GetString(attrToLoadCount + 1);
                        int Source_ID = reader.GetInt32(attrToLoadCount + 2);
                        int Target_ID = reader.GetInt32(attrToLoadCount + 3);
                        cwLightObject intersectionObject = new cwLightObject(_lightModel, InterSection_ID, Intersection_OT.ID);
                        intersectionObject.properties = _properties;
                        intersectionObject.properties[cwLightObject.UNIQUEIDENTIFIER] = InterSection_UUID;
                        if (!_usedOTTargetsLightObjectsByID.ContainsKey(Target_ID))
                        {
                            continue;
                        }
                        if (!_objects.ContainsKey(Source_ID))
                        {
                            _objects[Source_ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                        }
                        cwLightObject belowObject = _usedOTTargetsLightObjectsByID[Target_ID];
                        _objects[Source_ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(intersectionObject, belowObject));

                    }
                }
            }
        }

        /// <summary>
        /// Loads the light targets for diagrams exploded.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_sourceObjects">The _source objects.</param>
        /// <param name="_targetObjects">The _target objects.</param>
        private void loadLightTargetsForDiagramsExploded(cwLightModel _lightModel, Dictionary<int, cwLightObject> _sourceObjects, Dictionary<int, cwLightObject> _targetObjects)
        {
            if (0.Equals(_lightModel.diagramsExploded.Count()))
            {
                _lightModel.loadDiagramsExploded();
            }
            foreach (var sourceOGVar in _sourceObjects)
            {
                if (_lightModel.diagramsExploded.ContainsKey(sourceOGVar.Value.OTID) && _lightModel.diagramsExploded[sourceOGVar.Value.OTID].ContainsKey(sourceOGVar.Value.ID))
                {
                    foreach (cwLightDiagram diagram in _lightModel.diagramsExploded[sourceOGVar.Value.OTID][sourceOGVar.Value.ID])
                    {
                        if (!_targetObjects.ContainsKey(diagram.ID))
                        {
                            continue;
                        }
                        if (!targetLightObjectBySourceKey.ContainsKey(sourceOGVar.Value.ID))
                        {
                            targetLightObjectBySourceKey[sourceOGVar.Value.ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                        }
                        targetLightObjectBySourceKey[sourceOGVar.Value.ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, diagram));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is diagram exploded].
        /// </summary>
        private void isDiagramExploded()
        {
            cwLightModel _lightModel = this.selectedObjectType.Model;

            cwLightNodeObjectType OGSource = new cwLightNodeObjectType(this.getParent().sourceObjectType);
            //OGSource.copyNodeProperties(this);
            OGSource.preloadLightObjects();

            cwLightNodeObjectType targetOG = new cwLightNodeObjectType(selectedAssociationType.Target);
            string[] properties = new string[] { "ID", "TITLE", "NAME", "TYPE", "DIAGRAMMER", "TABLENUMBER", "OBJECTID", "DESCRIPTION", "UNIQUEIDENTIFIER", "VALIDATED" };
            targetOG.addPropertiesToSelect(properties);
            targetOG.copyNodeProperties(this);
            targetOG.preloadLightObjects();

            this.usedOTLightObjectsByID = targetOG.usedOTLightObjectsByID;
            this.usedOTLightObjectsByNAME = targetOG.usedOTLightObjectsByNAME;
            this.usedOTLightObjectsByUUID = targetOG.usedOTLightObjectsByUUID;
            this.loadLightTargetsForDiagramsExploded(_lightModel, OGSource.usedOTLightObjectsByID, targetOG.usedOTLightObjectsByID);
            sortAssociationsTargets();
        }


        private void loadLightTargetsForAssociationType(cwLightModel _lightModel, cwLightNode objectOnNode, cwLightNodeObjectType diagramsNode)
        {

            if (0.Equals(_lightModel.objectsOnDiagramsByObject.Count()))
            {
                _lightModel.loadDiagramsOn();
            }
            cwLightObjectType OT = objectOnNode.targetObjectType;
            if (_lightModel.objectsOnDiagramsByObject.ContainsKey(OT.ID))
            {
                foreach (var targetsVar in objectOnNode.targetLightObjectBySourceKey)
                {
                    foreach (var targetsObject in targetsVar.Value)
                    {
                        cwLightObject targetObject = targetsObject.Value;
                        if (!this.targetLightObjectBySourceKey.ContainsKey(targetObject.ID))
                        {
                            this.targetLightObjectBySourceKey[targetObject.ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                        }
                        List<int> diagramsOn = new List<int>();
                        if (_lightModel.objectsOnDiagramsByObject.ContainsKey(targetObject.OTID) && _lightModel.objectsOnDiagramsByObject[targetObject.OTID].ContainsKey(targetObject.ID))
                        {
                            diagramsOn = _lightModel.objectsOnDiagramsByObject[targetObject.OTID][targetObject.ID];
                        }

                        diagramsOn.ForEach(onDiagramID =>
                        {
                            if (diagramsNode.usedOTLightObjectsByID.ContainsKey(onDiagramID))
                            {
                                cwLightObject onDiagramObject = diagramsNode.usedOTLightObjectsByID[onDiagramID];
                                this.targetLightObjectBySourceKey[targetObject.ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, onDiagramObject));
                            }
                        });
                    }
                }

            }

        }


        private void loadLightTargetsForObjectType(cwLightModel _lightModel, cwLightNode objectOnNode, cwLightNodeObjectType diagramsNode)
        {
            if (0.Equals(_lightModel.objectsOnDiagramsByObject.Count()))
            {
                _lightModel.loadDiagramsOn();
            }
            cwLightObjectType OT = objectOnNode.targetObjectType;
            if (_lightModel.objectsOnDiagramsByObject.ContainsKey(OT.ID))
            {
                foreach (var sourceObjectVar in objectOnNode.usedOTLightObjectsByID)
                {
                    cwLightObject sourceObject = sourceObjectVar.Value;
                    if (!this.targetLightObjectBySourceKey.ContainsKey(sourceObject.ID))
                    {
                        this.targetLightObjectBySourceKey[sourceObject.ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                    }
                    List<int> diagramsOn = new List<int>();
                    if (_lightModel.objectsOnDiagramsByObject.ContainsKey(sourceObject.OTID) && _lightModel.objectsOnDiagramsByObject[sourceObject.OTID].ContainsKey(sourceObject.ID))
                    {
                        diagramsOn = _lightModel.objectsOnDiagramsByObject[sourceObject.OTID][sourceObject.ID];
                    }

                    diagramsOn.ForEach(onDiagramID =>
                    {
                        if (diagramsNode.usedOTLightObjectsByID.ContainsKey(onDiagramID))
                        {
                            cwLightObject onDiagramObject = diagramsNode.usedOTLightObjectsByID[onDiagramID];
                            this.targetLightObjectBySourceKey[sourceObject.ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, onDiagramObject));
                        }
                    });
                }
            }

        }

        /// <summary>
        /// Loads the light targets for on diagrams.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="objectOnNode">The object on node.</param>
        /// <param name="diagramsNode">The diagrams node.</param>
        //[Obsolete("Not implemented yet / under implementation")]

        private void loadLightTargetsForObjectType_(cwLightModel _lightModel, cwLightNode objectOnNode, cwLightNodeObjectType diagramsNode)
        {
            if (0.Equals(_lightModel.objectsOnDiagramsByObject.Count()))
            {
                _lightModel.loadDiagramsOn();
            }

            cwLightObjectType OT = objectOnNode.targetObjectType;

            if (objectOnNode is cwLightNodeAssociationType)
            {
                if (_lightModel.objectsOnDiagramsByObject.ContainsKey(OT.ID))
                {
                    foreach (var targetsVar in objectOnNode.targetLightObjectBySourceKey)
                    {
                        foreach (var targetsObject in targetsVar.Value)
                        {
                            cwLightObject targetObject = targetsObject.Value;
                            if (!targetLightObjectBySourceKey.ContainsKey(targetObject.ID))
                            {
                                targetLightObjectBySourceKey[targetObject.ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                            }
                            List<int> diagramsOn = new List<int>();
                            if (_lightModel.objectsOnDiagramsByObject.ContainsKey(targetObject.OTID) && _lightModel.objectsOnDiagramsByObject[targetObject.OTID].ContainsKey(targetObject.ID))
                            {
                                diagramsOn = _lightModel.objectsOnDiagramsByObject[targetObject.OTID][targetObject.ID];
                            }

                            diagramsOn.ForEach(onDiagramID =>
                            {
                                if (diagramsNode.usedOTLightObjectsByID.ContainsKey(onDiagramID))
                                {
                                    cwLightObject onDiagramObject = diagramsNode.usedOTLightObjectsByID[onDiagramID];
                                    targetLightObjectBySourceKey[targetObject.ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, onDiagramObject));
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                if (_lightModel.objectsOnDiagramsByObject.ContainsKey(OT.ID))
                {

                    foreach (var sourceObjectVar in objectOnNode.usedOTLightObjectsByID)
                    {
                        cwLightObject sourceObject = sourceObjectVar.Value;
                        if (!targetLightObjectBySourceKey.ContainsKey(sourceObject.ID))
                        {
                            targetLightObjectBySourceKey[sourceObject.ID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                        }
                        List<int> diagramsOn = new List<int>();
                        if (_lightModel.objectsOnDiagramsByObject.ContainsKey(sourceObject.OTID) && _lightModel.objectsOnDiagramsByObject[sourceObject.OTID].ContainsKey(sourceObject.ID))
                        {
                            diagramsOn = _lightModel.objectsOnDiagramsByObject[sourceObject.OTID][sourceObject.ID];
                        }

                        diagramsOn.ForEach(onDiagramID =>
                        {
                            if (diagramsNode.usedOTLightObjectsByID.ContainsKey(onDiagramID))
                            {
                                cwLightObject onDiagramObject = diagramsNode.usedOTLightObjectsByID[onDiagramID];
                                targetLightObjectBySourceKey[sourceObject.ID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, onDiagramObject));
                            }
                        });
                    }
                }
            }
        }


        /// <summary>
        /// Loads the light targets for on diagrams.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_requiredObjects">The _required objects.</param>
        /// <param name="_selectedDiagrams">The _selected diagrams.</param>
        private void loadLightTargetsForOnDiagrams(cwLightModel _lightModel, Dictionary<int, cwLightObject> _requiredObjects, List<int> _selectedDiagrams)
        {
            if (0.Equals(_lightModel.objectsOnDiagramsByObject.Count()))
            {
                _lightModel.loadDiagramsOn();
            }
            foreach (var requiredObjectVar in _requiredObjects)
            {
                cwLightObject requiredObject = requiredObjectVar.Value;
                if (_lightModel.objectsOnDiagramsByObject.ContainsKey(requiredObject.OTID) && _lightModel.objectsOnDiagramsByObject[requiredObject.OTID].ContainsKey(requiredObject.ID))
                {
                    foreach (int diagramID in _lightModel.objectsOnDiagramsByObject[requiredObject.OTID][requiredObject.ID])
                    {
                        if (!_selectedDiagrams.Contains(diagramID))
                        {
                            continue;
                        }
                        if (!targetLightObjectBySourceKey.ContainsKey(diagramID))
                        {
                            targetLightObjectBySourceKey[diagramID] = new List<KeyValuePair<cwLightObject, cwLightObject>>();
                        }
                        targetLightObjectBySourceKey[diagramID].Add(new KeyValuePair<cwLightObject, cwLightObject>(null, requiredObject));
                    }
                }
            }
        }
        /// <summary>
        /// Determines whether [is diagram on].
        /// </summary>
        private void isDiagramOn()
        {
            cwLightModel _lightModel = selectedObjectType.Model;


            cwLightNodeObjectType OGTargetOnDiagram = new cwLightNodeObjectType(selectedObjectType);
            OGTargetOnDiagram.copyNodeProperties(this);
            OGTargetOnDiagram.preloadLightObjects();

            //cwPSFTreeNode OGsParentNode = OGNode.getParent();

            cwLightNode diagramParentOG = getParent();
            cwLightObjectType parentOfParentOTRoot = diagramParentOG.sourceObjectType;
            if (!"DIAGRAM".Equals(parentOfParentOTRoot.ScriptName))
            {
                if (parentOfParentOTRoot != null)
                {
                    if (diagramParentOG is cwLightNodeAssociationType)
                    {
                        loadLightTargetsForAssociationType(_lightModel, diagramParentOG, OGTargetOnDiagram);
                    }
                    else if (diagramParentOG is cwLightNodeObjectType)
                    {
                        loadLightTargetsForObjectType(_lightModel, diagramParentOG, OGTargetOnDiagram);
                    }
                    //loadLightTargetsForObjectType(_lightModel, diagramParentOG, OGTargetOnDiagram);
                }
                //throw new cwExceptionFatal("ANYOBJECTSHOWNASSHAPEINDIAGRAM parent of parent should have one cwWebDesignerTreeNodeObjectGroup as a children with Object Type DIAGRAM");
            }
            else
            {
                loadLightTargetsForOnDiagrams(_lightModel, OGTargetOnDiagram.usedOTLightObjectsByID, diagramParentOG.usedOTLightObjectsByID.Keys.ToList<int>());
            }
            sortAssociationsTargets();

        }


        /// <summary>
        /// Sorts the associations targets.
        /// </summary>
        private void sortAssociationsTargets()
        {
            // sort associations
            if (sortOnPropertyScriptName != null)
            {
                if (this.selectedObjectType.propertiesByScriptName.ContainsKey(sortOnPropertyScriptName))
                {
                    string dataType = this.selectedObjectType.propertiesByScriptName[sortOnPropertyScriptName].DataType;
                    foreach (var associationsTargets in targetLightObjectBySourceKey)
                    {
                        switch (dataType)
                        {
                            case "Integer":
                                associationsTargets.Value.Sort(delegate(KeyValuePair<cwLightObject, cwLightObject> keyPair1, KeyValuePair<cwLightObject, cwLightObject> keyPair2) { return Convert.ToInt32(keyPair1.Value.properties[sortOnPropertyScriptName]).CompareTo(Convert.ToInt32(keyPair2.Value.properties[sortOnPropertyScriptName])); });
                                break;
                            default:
                                associationsTargets.Value.Sort(delegate(KeyValuePair<cwLightObject, cwLightObject> keyPair1, KeyValuePair<cwLightObject, cwLightObject> keyPair2) { return keyPair1.Value.properties[sortOnPropertyScriptName].CompareTo(keyPair2.Value.properties[sortOnPropertyScriptName]); });
                                break;
                        }
                        if (sortOnReverse)
                        {
                            associationsTargets.Value.Reverse();
                        }
                    }
                }
            }
        }
    }
}