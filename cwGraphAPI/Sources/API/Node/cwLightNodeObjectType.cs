using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.Exceptions;
using Casewise.Data.ICM;
using System.Data;
using log4net;

namespace Casewise.GraphAPI.API
{

    /// <summary>
    /// Represents an Object Type
    /// </summary>
    public class cwLightNodeObjectType : cwLightNode
    {
        /// <summary>
        /// log
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(typeof(cwLightNodeObjectType));


        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNodeObjectType"/> class.
        /// </summary>
        /// <param name="selectedObjectTypeNode">The selected object type node.</param>
        public cwLightNodeObjectType(cwPSFTreeNodeObjectNode selectedObjectTypeNode)
            : base(selectedObjectTypeNode)
        {
            this.selectedObjectType = selectedObjectTypeNode.getSelectedObjectType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightNodeObjectType"/> class.
        /// </summary>
        /// <param name="selectedObjectType">Type of the selected object.</param>
        public cwLightNodeObjectType(cwLightObjectType selectedObjectType)
            : base(null as cwPSFTreeNodeObjectNode)
        {
            this.selectedObjectType = selectedObjectType;
            this.ID = selectedObjectType.ToString().ToLower();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ID + " - " + selectedObjectType.ToString();
        }

        /// <summary>
        /// Gets the target objects by ID.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, cwLightObject> getTargetObjectsByID()
        {
            return usedOTLightObjectsByID;
        }

        /// <summary>
        /// Loads the light objects.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        private void loadLightObjects(cwLightModel _lightModel)
        {
            try
            {
                cwLightObjectType OT = selectedObjectType;
                Dictionary<int, string> _propertiesTypeByIterationNumber = new Dictionary<int, string>();
                Dictionary<string, string> _propertiesTypeByScriptName = new Dictionary<string, string>();
                int OTID = OT.ID;
                string propertiesToSelect = cwPSFQueryManager.createPropertiesToSelectQueryString(selectedPropertiesScriptName, _propertiesTypeByIterationNumber, _propertiesTypeByScriptName, OT);

                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + propertiesToSelect + " FROM " + OT.ScriptName);
                string _queryClause = cwPSFQueryManager.createPropertiesWhereQueryString(attributeFiltersKeep, OT);
                query.Append(_queryClause);
                using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                {
                    cwPSFQueryManager.updateCommandUsingQueryForFilterProperties(attributeFiltersKeep, command, OT);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> _properties = new Dictionary<string, string>();
                            int attrToLoadCount = selectedPropertiesScriptName.Count;
                            cwPSFQueryManager.updatePropertiesFromQueryResponse(reader, _properties, selectedPropertiesScriptName, _propertiesTypeByIterationNumber, _lightModel);

                            int ID = reader.GetInt32(attrToLoadCount);
                            string UUID = reader.GetString(attrToLoadCount + 1);

                            string _name = "";
                            if (!OT.isIntersectionObjectType)
                            {
                                _name = reader.GetValue(attrToLoadCount + 2).ToString();
                            }
                            cwLightObject o = new cwLightObject(_lightModel, ID, OTID);
                            o.properties = _properties;
                            o.properties[cwLightObject.UNIQUEIDENTIFIER] = UUID;
                            o.properties["NAME"] = _name;
                            usedOTLightObjects.Add(o);
                            usedOTLightObjectsByID[ID] = o;
                            usedOTLightObjectsByUUID[UUID] = o;
                            if (!OT.isIntersectionObjectType)
                            {
                                usedOTLightObjectsByNAME[_name] = o;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Preloads the light objects.
        /// </summary>
        public override void preloadLightObjects()
        {
            clearNodeContent();
            if (selectedObjectType == null) throw new cwExceptionFatal("Requested Object Type is missing in the model while loading the Object Type for objectGroup [" + this.ToString() + "]");
            // load the objects 
            if (selectedObjectType.hasProperty(sortOnPropertyScriptName))
            {
                addPropertyToSelect(sortOnPropertyScriptName);
            }

            loadLightObjects(selectedObjectType.Model);
            sortLightObjects();
            usedOTLightObjects.Count();
        }

        /// <summary>
        /// Sorts the light objects.
        /// </summary>
        public void sortLightObjects()
        {
            if (sortOnPropertyScriptName != null)
            {
                if (this.selectedObjectType.propertiesByScriptName.ContainsKey(sortOnPropertyScriptName))
                {
                    switch (this.selectedObjectType.propertiesByScriptName[sortOnPropertyScriptName].DataType)
                    {
                        case "Integer":
                            usedOTLightObjects.Sort(delegate(cwLightObject o1, cwLightObject o2){
                                return Convert.ToInt32(o1.properties[sortOnPropertyScriptName]).CompareTo(Convert.ToInt32(o2.properties[sortOnPropertyScriptName]));
                            });
                            break;
                        default:
                            usedOTLightObjects.Sort(delegate(cwLightObject o1, cwLightObject o2) { return o1.properties[sortOnPropertyScriptName].CompareTo(o2.properties[sortOnPropertyScriptName]); });
                            break;
                    }
                    
                }
            }
            if (sortOnReverse)
            {
                usedOTLightObjects.Reverse();
            }
        }

        /// <summary>
        /// Adds the properties to select.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void addPropertiesToSelect(string[] properties)
        {
            foreach (string property in properties)
            {
                addPropertyToSelect(property);
            }
        }
    }
}
