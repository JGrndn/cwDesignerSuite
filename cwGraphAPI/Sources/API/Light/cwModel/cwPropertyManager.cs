using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Manage the properties types
    /// </summary>
    internal class cwPropertyManager
    {

        private Dictionary<int, Dictionary<string, cwLightProperty>> propertiesByOTAndScriptName = new Dictionary<int, Dictionary<string, cwLightProperty>>();

        private cwLightModel currentModel = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPropertyManager"/> class.
        /// </summary>
        /// <param name="currentModel">The current model.</param>
        public cwPropertyManager(cwLightModel currentModel)
        { 
            this.currentModel = currentModel;
            loadProperties();
        }


        /// <summary>
        /// Gets the name of the properties by script.
        /// </summary>
        /// <param name="objectTypeID">The object type ID.</param>
        /// <returns></returns>
        internal Dictionary<string, cwLightProperty> getPropertiesByScriptName(int objectTypeID)
        {
            return propertiesByOTAndScriptName[objectTypeID];
        }
  
         /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public void loadProperties()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + cwTools.stringToStringSeparatedby(",", cwLightProperty.PROPERTIES_NAMES.ToList<string>(), false) + " FROM PROPERTYTYPE");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentModel.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pObjectTypeID = reader.GetInt32(0);
                            string pScriptName = reader.GetString(1);
                            if (!propertiesByOTAndScriptName.ContainsKey(pObjectTypeID))
                            {
                                propertiesByOTAndScriptName[pObjectTypeID] = new Dictionary<string,cwLightProperty>();
                            }
                            cwLightObjectType objectType = currentModel.getObjectTypeByID(pObjectTypeID);
                            if (!propertiesByOTAndScriptName[pObjectTypeID].ContainsKey(pScriptName))
                            {
                                propertiesByOTAndScriptName[pObjectTypeID][pScriptName] = new cwLightProperty(objectType, reader);
                            }
                            cwLightProperty property = propertiesByOTAndScriptName[pObjectTypeID][pScriptName];
                            if (property.paneID != 0)
                            {
                                cwLightPane pane = objectType.getPaneByID(property.paneID);
                                pane.addProperty(property.sequenceInPane, property);
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
    }
}
