using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using Casewise.Services.ICM;
using Casewise.GraphAPI.Exceptions;
using System.Windows.Forms;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Indicates the Database management system
    /// </summary>
    public enum DbManagementSystem
    {
        /// <summary>
        /// SQL Server
        /// </summary>
        SQL_Server,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle
    }

    /// <summary>
    /// cwLightModel
    /// </summary>
    public class cwLightModel : IComparable
    {
        private static string[] blackListedcwLightObjectTypes = { "FONT", "HIERARCHYLINK", "BINARYDATA", "POINT", "PRINTFRAME", "PROPERTYTYPE", "TITLE", "VERSIONCONTROL", "PICTURE" };

        /// <summary>lookupManager
        /// 
        /// </summary>
        public cwLookupManager lookupManager = null;

        internal ICMConnection currentConnection = null;
        private Dictionary<int, cwLightObjectType> lightObjectTypesByID = new Dictionary<int, cwLightObjectType>();
        private Dictionary<string, cwLightObjectType> lightObjectTypesByScriptName = new Dictionary<string, cwLightObjectType>();
        private Dictionary<int, ICMDiagramLock> diagramsLock = new Dictionary<int, ICMDiagramLock>();

        /// <summary>
        /// 
        /// </summary>
        //public ModelLoader modelLoader = null;
        internal Dictionary<int, Dictionary<int, List<cwLightDiagram>>> diagramsExploded = new Dictionary<int, Dictionary<int, List<cwLightDiagram>>>();
        internal Dictionary<int, Dictionary<int, List<int>>> objectsOnDiagramsByObject = new Dictionary<int, Dictionary<int, List<int>>>();
        /// <summary>
        /// Liste of objects classed by object type and diagram
        /// dico[diagramID][OT_ID] = List(Objects ID) which are On the diagram
        /// </summary>
        public Dictionary<int, Dictionary<int, List<int>>> objectsOnDiagramsByDiagram = new Dictionary<int, Dictionary<int, List<int>>>();

        internal cwPropertyManager propertyManager = null;
        internal cwAssociationTypeManager associationTypesManager = null;
        internal cwPaneManager paneManager = null;
        internal DbManagementSystem DataBaseType;

        

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightModel"/> class.
        /// </summary>
        /// <param name="_currentConnection">The _current connection.</param>
        public cwLightModel(ICMConnection _currentConnection)
        {
            currentConnection = _currentConnection;
            currentConnection.Open();
            CasewiseDatabaseSelection2005.IcwDBMSInformation dbInfo = currentConnection.ConnectionList.DBMS as CasewiseDatabaseSelection2005.IcwDBMSInformation;
            if (dbInfo != null)
            {
                if (dbInfo.DBMSType.Equals(CasewiseDatabaseSelection2005.DBMS_TYPE.dbms_SQLServer))
                {
                    this.DataBaseType = DbManagementSystem.SQL_Server;
                }
                else if (dbInfo.DBMSType.Equals(CasewiseDatabaseSelection2005.DBMS_TYPE.dbms_Oracle))
                {
                    this.DataBaseType = DbManagementSystem.Oracle;
                }
            }
        }


        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void closeConnection()
        {
            getConnection().Close();
            getConnection().Dispose();
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get
            {
                return currentConnection.FileName;
            }
        }


        /// <summary>
        /// Loads the diagrams on.
        /// </summary>
        public void loadDiagramsOn()
        {
            try
            {
                objectsOnDiagramsByDiagram = new Dictionary<int, Dictionary<int, List<int>>>();
                objectsOnDiagramsByObject = new Dictionary<int, Dictionary<int, List<int>>>();
                StringBuilder query = new StringBuilder();
                query.Append("SELECT TABLENUMBER, OBJECTID, DIAGRAMID FROM SHAPE WHERE OBJECTID <> 0");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentConnection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int OT_ID = reader.GetInt32(0);
                            int O_ID = reader.GetInt32(1);
                            int D_ID = reader.GetInt32(2);
                            // OT ID
                            if (!objectsOnDiagramsByObject.ContainsKey(OT_ID)) objectsOnDiagramsByObject[OT_ID] = new Dictionary<int, List<int>>();
                            // Object ID 
                            if (!objectsOnDiagramsByObject[OT_ID].ContainsKey(O_ID)) objectsOnDiagramsByObject[OT_ID][O_ID] = new List<int>();
                            // ADD Diagram ID
                            if (!objectsOnDiagramsByObject[OT_ID][O_ID].Contains(D_ID)) objectsOnDiagramsByObject[OT_ID][O_ID].Add(D_ID);
                            // Diagram ID
                            if (!objectsOnDiagramsByDiagram.ContainsKey(D_ID)) objectsOnDiagramsByDiagram[D_ID] = new Dictionary<int, List<int>>();
                            // OT ID
                            if (!objectsOnDiagramsByDiagram[D_ID].ContainsKey(OT_ID)) objectsOnDiagramsByDiagram[D_ID][OT_ID] = new List<int>();
                            // ADD Object ID
                            if (!objectsOnDiagramsByDiagram[D_ID][OT_ID].Contains(O_ID)) objectsOnDiagramsByDiagram[D_ID][OT_ID].Add(O_ID);
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
        /// Loads the diagrams exploded.
        /// </summary>
        public void loadDiagramsExploded()
        {
            try
            {
                diagramsExploded = new Dictionary<int, Dictionary<int, List<cwLightDiagram>>>();
                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + cwLightDiagram.diagramSelectionProperties + " FROM DIAGRAM WHERE OBJECTID <> 0");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentConnection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int OT_ID = reader.GetInt32(5);
                            int O_ID = reader.GetInt32(6);
                            cwLightDiagram diagram = cwLightDiagram.createDiagramFromReader(reader, this);

                            if (!diagramsExploded.ContainsKey(OT_ID))
                            {
                                diagramsExploded[OT_ID] = new Dictionary<int, List<cwLightDiagram>>();
                            }
                            if (!diagramsExploded[OT_ID].ContainsKey(O_ID))
                            {
                                diagramsExploded[OT_ID][O_ID] = new List<cwLightDiagram>();
                            }
                            if (!diagramsExploded[OT_ID][O_ID].Contains(diagram))
                            {
                                diagramsExploded[OT_ID][O_ID].Add(diagram);
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
        /// Determines whether [has object type by ID] [the specified _ O t_ ID].
        /// </summary>
        /// <param name="_OT_ID">The _ O t_ ID.</param>
        /// <returns>
        ///   <c>true</c> if [has object type by ID] [the specified _ O t_ ID]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasObjectTypeByID(int _OT_ID)
        {
            return lightObjectTypesByID.ContainsKey(_OT_ID);
        }

        /// <summary>
        /// Determines whether [has object type by script name] [the specified object type script name].
        /// </summary>
        /// <param name="objectTypeScriptName">Name of the object type script.</param>
        /// <returns>
        ///   <c>true</c> if [has object type by script name] [the specified object type script name]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasObjectTypeByScriptName(string objectTypeScriptName)
        {
            return lightObjectTypesByScriptName.ContainsKey(objectTypeScriptName);
        }

        /// <summary>
        /// Gets the name of the object type by script.
        /// </summary>
        /// <param name="ScriptName">Name of the script.</param>
        /// <returns></returns>
        public cwLightObjectType getObjectTypeByScriptName(string ScriptName)
        {
            if (!lightObjectTypesByScriptName.ContainsKey(ScriptName))
            {
                throw new cwExceptionFatal("Required OT ScriptName [" + ScriptName + "] do not exists in the model");
            }
            return lightObjectTypesByScriptName[ScriptName];
        }

        /// <summary>
        /// Gets the object type by ID.
        /// </summary>
        /// <param name="_OT_ID">The _ O t_ ID.</param>
        /// <returns></returns>
        public cwLightObjectType getObjectTypeByID(int _OT_ID)
        {
            if (0.Equals(_OT_ID)) return null;
            if (!lightObjectTypesByID.ContainsKey(_OT_ID))
            {
                throw new cwExceptionFatal("Required OT ID [" + _OT_ID + "] do not exists in the model");
            }
            return lightObjectTypesByID[_OT_ID];
        }

        /// <summary>
        /// Gets the name of the object type ID by script.
        /// </summary>
        /// <param name="OTScriptName">Name of the OT script.</param>
        /// <returns></returns>
        public int getObjectTypeIDByScriptName(string OTScriptName)
        {
            if (lightObjectTypesByScriptName.ContainsKey(OTScriptName))
            {
                return lightObjectTypesByScriptName[OTScriptName].ID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Loads the content of the light model.
        /// </summary>
        public void loadLightModelContent()
        {
            lookupManager = new cwLookupManager(this);
            //loadcwLightAssociationTypes();
            loadLightObjectTypes();
            paneManager = new cwPaneManager(this);
            propertyManager = new cwPropertyManager(this);

            // set properties
            foreach (cwLightObjectType lightOT in getObjectTypes())
            {
                lightOT.propertiesByScriptName = this.propertyManager.getPropertiesByScriptName(lightOT.ID);
            }
            this.associationTypesManager = new cwAssociationTypeManager(this);
        }



        /// <summary>
        /// Gets the <see cref="Casewise.GraphAPI.API.cwLightObjectType"/> with the specified script name.
        /// </summary>
        public cwLightObjectType this[string ScriptName]
        {
            get
            {
                if (0.Equals(lightObjectTypesByScriptName.Count))
                {
                    loadLightModelContent();
                }
                if (lightObjectTypesByScriptName.ContainsKey(ScriptName))
                {
                    return lightObjectTypesByScriptName[ScriptName];
                }
                else
                {
                    throw new cwExceptionWarning("The requested Object Type [" + ScriptName + "] do not exists in the model [" + this.currentConnection.FileName + "]");
                }
            }
        }

        /// <summary>
        /// Gets the lightcw light object types.
        /// </summary>
        /// <returns></returns>
        public List<cwLightObjectType> getObjectTypes()
        {
            List<cwLightObjectType> OTs = lightObjectTypesByID.Values.ToList();
            OTs.Sort();
            return OTs;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return currentConnection.Text;
        }

        /// <summary>
        /// Gets the light usablecw light object types.
        /// </summary>
        /// <returns></returns>
        public List<cwLightObjectType> getPSFEnabledObjectTypes(bool includeIntersectionObjectTypes)
        {
            List<cwLightObjectType> OTs = new List<cwLightObjectType>();
            foreach (KeyValuePair<string, cwLightObjectType> OTVar in lightObjectTypesByScriptName)
            {
                if (!OTVar.Value.isPSFEnabled)
                {
                    continue;
                }
                if (!includeIntersectionObjectTypes && OTVar.Value.isIntersectionObjectType)
                {
                    continue;
                }
                OTs.Add(OTVar.Value);
            }
            OTs.Sort();
            return OTs;
        }



        /// <summary>
        /// Loads the light object types.
        /// </summary>
        public void loadLightObjectTypes()
        {
            try
            {
                lightObjectTypesByID.Clear();
                lightObjectTypesByScriptName.Clear();
                StringBuilder query = new StringBuilder();
                query.Append("SELECT SCRIPTNAME, ID, NAME, FLAGS, LASTID FROM OBJECTTYPE");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentConnection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ScriptName = reader.GetString(0);
                            int ID = reader.GetInt32(1);
                            string Name = reader.GetString(2);
                            int flags = reader.GetInt32(3);
                            int lastID = reader.GetInt32(4);
                            cwLightObjectType lightcwLightObjectType = new cwLightObjectType(this, ID, Name, ScriptName, flags, lastID);

                            lightObjectTypesByID.Add(ID, lightcwLightObjectType);
                            lightObjectTypesByScriptName.Add(ScriptName, lightcwLightObjectType);
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
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public ICMConnection getConnection()
        {
            return currentConnection;
        }



        /// <summary>
        /// Determines whether [is diagram locked] [the specified diagram ID].
        /// </summary>
        /// <param name="diagramID">The diagram ID.</param>
        /// <returns>
        ///   <c>true</c> if [is diagram locked] [the specified diagram ID]; otherwise, <c>false</c>.
        /// </returns>
        public bool isDiagramLocked(int diagramID)
        {
            return currentConnection.IsDiagramLocked(diagramID);
        }

        /// <summary>
        /// Locks the diagram.
        /// </summary>
        /// <param name="diagramID">The diagram ID.</param>
        public void lockDiagram(int diagramID)
        {
            try
            {
                if (!currentConnection.IsDiagramLocked(diagramID))
                {
                    diagramsLock[diagramID] = currentConnection.LockDiagram(diagramID);
                }
            }
            catch (ICMException e)
            {
                if (ICMException.Reason.COULD_NOT_LOCK_DIAGRAM.Equals(e.Code))
                {
                    throw new cwExceptionWarning("Impossible to lock the diagram, please check the diagram [ID:" + diagramID + "] is not open");
                }
                throw e;
            }

        }
        /// <summary>
        /// Unlocks the diagram.
        /// </summary>
        /// <param name="diagramID">The diagram ID.</param>
        public void unlockDiagram(int diagramID)
        {
            if (diagramsLock.ContainsKey(diagramID))
            {
                diagramsLock[diagramID].Dispose();
                diagramsLock.Remove(diagramID);
            }
        }

        internal static string queryOrderForIntersectionOTWithTableNumber(cwLightAssociationType AT)
        {
            if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName))
            {
                return ", " + AT.Source.ScriptName + "ID, " + AT.Target.ScriptName + "ID";
            }
            else
            {
                if (!AT.isReversed)
                {
                    return ", ABOVEOBJECTID, BELOWOBJECTID, ABOVETABLENUMBER, BELOWTABLENUMBER ";
                }
                else
                {
                    return ", BELOWOBJECTID, ABOVEOBJECTID, BELOWTABLENUMBER, ABOVETABLENUMBER ";
                }
            }
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance is less than <paramref name="obj"/>.
        /// Zero
        /// This instance is equal to <paramref name="obj"/>.
        /// Greater than zero
        /// This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj"/> is not the same type as this instance.
        ///   </exception>
        public int CompareTo(object obj)
        {
            return ToString().CompareTo(obj.ToString());
        }

        #endregion
    }
}
