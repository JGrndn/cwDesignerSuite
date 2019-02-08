using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using Casewise.GraphAPI.API;
using System.Data;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Manage the association types
    /// </summary>
    internal class cwAssociationTypeManager
    {
        private const int ANYOBJECT_ID = 6273;

        private cwLightModel currentModel = null;
        private Dictionary<int, List<cwLightAssociationType>> associationTypesBySourceObjectTypeID = new Dictionary<int, List<cwLightAssociationType>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwAssociationTypeManager"/> class.
        /// </summary>
        /// <param name="currentModel">The current model.</param>
        public cwAssociationTypeManager(cwLightModel currentModel)
        {
            this.currentModel = currentModel;
            List<cwLightObjectType> OTs = this.currentModel.getPSFEnabledObjectTypes(false);
            foreach (cwLightObjectType ot in OTs) { 
            if (!associationTypesBySourceObjectTypeID.ContainsKey(ot.ID))
            {
                associationTypesBySourceObjectTypeID[ot.ID] = new List<cwLightAssociationType>();
            }
            }
            loadAssociationTypes();

            //
            List<cwLightAssociationType> toAnyObject = new List<cwLightAssociationType>(associationTypesBySourceObjectTypeID[ANYOBJECT_ID]);


            foreach (KeyValuePair<int, List<cwLightAssociationType>> ATVar in associationTypesBySourceObjectTypeID)
            {
                foreach (cwLightAssociationType AT in toAnyObject)
                {
                    if (!associationTypesBySourceObjectTypeID.ContainsKey(AT.Target.ID))
                    {
                        associationTypesBySourceObjectTypeID[AT.Target.ID] = new List<cwLightAssociationType>();
                    }
                    associationTypesBySourceObjectTypeID[ATVar.Key].Add(AT);
                }

            }


        }

        /// <summary>
        /// Gets the type of the association types for source object.
        /// </summary>
        /// <param name="sourceObjectTypeID">The source object type ID.</param>
        /// <returns></returns>
        public List<cwLightAssociationType> getAssociationTypesForSourceObjectType(int sourceObjectTypeID)
        {
            if (!associationTypesBySourceObjectTypeID.ContainsKey(sourceObjectTypeID))
            {
                return new List<cwLightAssociationType>();
            }
            return associationTypesBySourceObjectTypeID[sourceObjectTypeID];
        }

        /// <summary>
        /// Loads the association types.
        /// </summary>
        public void loadAssociationTypes()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ID, NAME, REVERSENAME, SCRIPTNAME, REVERSESCRIPTNAME, OBJECTTYPEID, BELOWOBJECTTYPEID, ABOVEOBJECTTYPEID, UNIQUEIDENTIFIER FROM ASSOCIATIONTYPE");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentModel.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(1);
                            string reverseName = reader.GetString(2);
                            string scriptName = reader.GetString(3);
                            string reverseScriptName = reader.GetString(4);
                            int sourceID = reader.GetInt32(6);
                            int targetID = reader.GetInt32(7);
                            if (!associationTypesBySourceObjectTypeID.ContainsKey(sourceID))
                            {
                                associationTypesBySourceObjectTypeID[sourceID] = new List<cwLightAssociationType>();
                            }
                            if (!associationTypesBySourceObjectTypeID.ContainsKey(targetID))
                            {
                                associationTypesBySourceObjectTypeID[targetID] = new List<cwLightAssociationType>();
                            }

                            cwLightAssociationType AT = new cwLightAssociationType(currentModel, name, scriptName, sourceID, targetID, false, reader);
                            associationTypesBySourceObjectTypeID[sourceID].Add(AT);
                            cwLightAssociationType ATReversed = new cwLightAssociationType(currentModel, reverseName, reverseScriptName, targetID, sourceID, true, reader);
                            associationTypesBySourceObjectTypeID[targetID].Add(ATReversed);
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
