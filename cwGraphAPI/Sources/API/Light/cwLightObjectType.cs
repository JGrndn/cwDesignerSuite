using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// 
    /// </summary>
    public class cwLightObjectType : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// 
        /// </summary>
        public string ScriptName = null;
        private string Name = null;
        private cwLightModel _model = null;
        internal Dictionary<int, cwLightObject> objects = new Dictionary<int, cwLightObject>();
        internal Dictionary<string, cwLightProperty> propertiesByScriptName = new Dictionary<string, cwLightProperty>();
        internal SortedDictionary<int, cwLightPane> panesByID = new SortedDictionary<int, cwLightPane>();
        internal SortedDictionary<int, cwLightPane> panesBySequence = new SortedDictionary<int, cwLightPane>();
        private int lastID = 0;

        private bool _isIntersection = false;
        private bool _isPSFEnabled = false;
        private bool _isPOLDAT = false;
        private bool _isDiagramDependant = false;
        private bool _isAdmin = false;


        /// <summary>
        /// EVENTRESULT_SCRIPTNAME
        /// </summary>
        public const string EVENTRESULT_SCRIPTNAME = "EVENTRESULT";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObjectType"/> class.
        /// </summary>
        /// <param name="_model">The _model.</param>
        /// <param name="OTID">The OTID.</param>
        /// <param name="OTName">Name of the OT.</param>
        /// <param name="OTscriptName">Name of the O tscript.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="lastID">The last ID.</param>
        public cwLightObjectType(cwLightModel _model, int OTID, string OTName, string OTscriptName, int flags, int lastID)
        {
            this.ID = OTID;
            this.Name = OTName;
            this.ScriptName = OTscriptName;
            this._model = _model;
            setUpTypeFromFlags(flags);
            this.lastID = lastID;
        }


        /// <summary>
        /// Creates the using name and get.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public cwLightObject createUsingNameAndGet(string name)
        {
            createObjectWithName(name);
            return getObjectByName(name);
        }

        /// <summary>
        /// Updates the last ID.
        /// </summary>
        private void updateLastID()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT LASTID FROM OBJECTTYPE WHERE ID = @OTID");
                using (ICMCommand command = new ICMCommand(query.ToString(), Model.getConnection()))
                {
                    command.Parameters.Add("@OTID", ID);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.lastID = reader.GetInt32(0);
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
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return _model;
            }
        }


        /// <summary>
        /// Gets the sorted panes.
        /// </summary>
        /// <returns></returns>
        public List<cwLightPane> getSortedPanes()
        {
            return panesBySequence.Values.ToList<cwLightPane>();
        }

        /// <summary>
        /// Gets the pane by ID.
        /// </summary>
        /// <param name="paneID">The pane ID.</param>
        /// <returns></returns>
        public cwLightPane getPaneByID(int paneID)
        {
            return panesByID[paneID];
        }


        /// <summary>
        /// Objects the name of the exists by.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        public bool objectExistsByName(string objectName)
        {
            List<cwLightObject> objects = getObjectsByFilter(new string[] { "ID" }.ToList<string>(), getNamePropertyScriptName(), objectName, "=");
            if (1.Equals(objects.Count()))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the name of the object by.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        public cwLightObject getObjectByName(string objectName)
        {
            List<string> propertiesToSelect = new List<string>();
            propertiesToSelect.Add(getNamePropertyScriptName());
            if (hasProperty("DESCRIPTION"))
            {
                propertiesToSelect.Add("DESCRIPTION");
            }
            List<cwLightObject> objects = getObjectsByFilter(propertiesToSelect, getNamePropertyScriptName(), objectName, "=");
            if (1.Equals(objects.Count()))
            {
                return objects.First();
            }
            if (objects.Count > 1)
            {
                throw new cwExceptionWarning(String.Format("Requested object ({0}) exists too many time ({1}) for object type [{2}]", objectName, objects.Count, ToString()));
            }
            throw new cwExceptionWarning(String.Format("Requested object ({0}) do not exists in the object type [{1}]", objectName, ToString()));
        }

        /// <summary>
        /// Gets the object by ID.
        /// </summary>
        /// <param name="objectID">The object ID.</param>
        /// <returns></returns>
        public cwLightObject getObjectByID(int objectID)
        {
            List<cwLightObject> objects = getObjectsByFilter(new string[] { getNamePropertyScriptName(), "DESCRIPTION" }.ToList<string>(), getIDPropertyScriptName(), objectID.ToString(), "=");
            if (1.Equals(objects.Count()))
            {
                return objects.First();
            }
            throw new cwExceptionWarning(String.Format("Requested object ({0}) do not exists in the object type [{1}]", objectID.ToString(), ToString()));
        }

        /// <summary>
        /// Gets the objects by filter.
        /// </summary>
        /// <param name="propertiesScriptNameToSelect">The properties script name to select.</param>
        /// <param name="filterPropertyScriptName">Name of the filter property script.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterOperation">The filter operation.</param>
        /// <returns></returns>
        public List<cwLightObject> getObjectsByFilter(List<string> propertiesScriptNameToSelect, string filterPropertyScriptName, string filterValue, string filterOperation)
        {
            cwLightNodeObjectType OG_WD = new cwLightNodeObjectType(this);
            foreach (string propertyScriptName in propertiesScriptNameToSelect)
            {
                if (hasProperty(propertyScriptName))
                {
                    OG_WD.addPropertyToSelect(propertyScriptName);
                }
            }
            OG_WD.addAttributeForFilterAND(filterPropertyScriptName, filterValue, filterOperation);
            OG_WD.preloadLightObjects();
            return OG_WD.usedOTLightObjects;
        }


        private Dictionary<string, string> getPropertiesForCreation(Dictionary<string, string> sourceProperties)
        {
            Dictionary<string, string> _usedProperties = new Dictionary<string, string>();
            foreach (var pVar in sourceProperties)
            {
                if (pVar.Key.Equals("NAME"))
                {
                    _usedProperties[getNamePropertyScriptName()] = pVar.Value;
                    continue;
                }
                if (pVar.Key.EndsWith(cwLookupManager.LOOKUPID_KEY) || pVar.Key.EndsWith(cwLookupManager.LOOKUPABBR_KEY))
                {
                    continue;
                }
                _usedProperties[pVar.Key] = pVar.Value;
            }
            return _usedProperties;
        }


        /// <summary>
        /// Deletes the ALL objects.
        /// </summary>
        public void deleteALLObjects()
        {
            ICMCommand command = new ICMCommand("DELETE FROM " + ScriptName, Model.getConnection());
            try
            {
                using (IDbTransaction trans = Model.getConnection().BeginTransaction())
                {
                    command.ExecuteReader();
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Deletes the ALL objects.
        /// </summary>
        public void deleteALLObjectsUsingWhereClause(string whereClause)
        {
            ICMCommand command = new ICMCommand("DELETE FROM " + ScriptName + " WHERE " + whereClause, Model.getConnection());
            try
            {
                using (IDbTransaction trans = Model.getConnection().BeginTransaction())
                {
                    command.ExecuteReader();
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates the name of the object with.
        /// </summary>
        /// <param name="name">The name.</param>
        public int createObjectWithName(string name)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties[getNamePropertyScriptName()] = name;
            return createObject(properties);
        }

        /// <summary>
        /// Gets the last created object.
        /// </summary>
        /// <returns></returns>
        public cwLightObject getLastCreatedObject()
        {
            return getObjectByID(lastID);
        }


        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="sourceProperties">The source properties.</param>
        /// <returns></returns>
        public int createObject(Dictionary<string, string> sourceProperties)
        {
            return createObject(sourceProperties, true);
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="sourceProperties">The source properties.</param>
        /// <param name="doTransaction">if set to <c>true</c> [do transaction].</param>
        /// <returns></returns>
        public int createObject(Dictionary<string, string> sourceProperties, bool doTransaction)
        {
            StringBuilder query = new StringBuilder();
            Dictionary<string, string> _usedProperties = getPropertiesForCreation(sourceProperties);
            string propertiesNameToInsert = cwTools.stringToStringSeparatedby(",", _usedProperties.Keys.ToList<string>(), false);

            query.Append("INSERT INTO ");
            query.Append(ScriptName);
            query.Append(" (");
            if (!"SHAPE".Equals(ScriptName) && !"JOINER".Equals(ScriptName))
            {
                query.Append("ID, ");
            }
            query.Append(propertiesNameToInsert);
            query.Append(" ) VALUES (");
            if (!"SHAPE".Equals(ScriptName) && !"JOINER".Equals(ScriptName))
            {
                query.Append(" @@IDENTITY,  ");
            }
            query.Append("@" + cwTools.stringToStringSeparatedby(",@", _usedProperties.Keys.ToList<string>(), false));
            query.Append(")");

            ICMCommand command = new ICMCommand(query.ToString(), _model.getConnection());
            try
            {
                foreach (var propertyVar in _usedProperties)
                {
                    cwLightProperty p = getProperty(propertyVar.Key);
                    if (p.isLookup)
                    {
                        string lookupID = p.getLookupId(propertyVar.Value);
                        if ("".Equals(lookupID))
                        {
                            Model.lookupManager.createLookupValue(p, propertyVar.Value);
                            lookupID = p.getLookupId(propertyVar.Value);
                        }
                        command.Parameters.Add("@" + propertyVar.Key, lookupID);
                    }
                    else
                    {
                        command.Parameters.Add("@" + propertyVar.Key, propertyVar.Value);
                    }
                }

                if (doTransaction)
                {
                    using (IDbTransaction trans = Model.getConnection().BeginTransaction())
                    {
                        command.ExecuteReader();
                        trans.Commit();
                    }
                }
                else
                {
                    command.ExecuteReader();
                }

                updateLastID();
                return lastID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void setUpTypeFromFlags(int flags)
        {
            switch (flags)
            {
                case 0:
                case 2:
                    _isAdmin = true;
                    break;
                case 16: // poldat OT
                    _isPSFEnabled = true;
                    _isPOLDAT = true;
                    break;
                case 24: // user defined OT
                    _isPSFEnabled = true;
                    break;
                case 84: // poldat intersections
                    _isPOLDAT = true;
                    _isIntersection = true;
                    _isPSFEnabled = true;
                    break;
                case 92: // user defined intersection
                    _isIntersection = true;
                    _isPSFEnabled = true;
                    break;
                case 4188: // tertiaire intersection ?
                    _isIntersection = true;
                    _isPSFEnabled = true;
                    break;


                default:
                    break;
            }
            if ("DIAGRAM".Equals(ScriptName))
            {
                _isPSFEnabled = true;
            }
            if ("PUBLICATIONSET".Equals(ScriptName))
            {
                _isPSFEnabled = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is admin.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is admin; otherwise, <c>false</c>.
        /// </value>
        public bool isAdmin
        {
            get
            {
                return _isAdmin;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is PSF enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is PSF enabled; otherwise, <c>false</c>.
        /// </value>
        public bool isPSFEnabled
        {
            get
            {
                return _isPSFEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is diagram dependant.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is diagram dependant; otherwise, <c>false</c>.
        /// </value>
        public bool isDiagramDependant
        {
            get
            {
                return _isDiagramDependant;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is POLDAT.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is POLDAT; otherwise, <c>false</c>.
        /// </value>
        public bool isPOLDAT
        {
            get
            {
                return _isPOLDAT;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is intersection object type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is intersection object type; otherwise, <c>false</c>.
        /// </value>
        public bool isIntersectionObjectType
        {
            get { return _isIntersection; }
        }



        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
        public List<cwLightProperty> getProperties()
        {
            return propertiesByScriptName.Values.ToList<cwLightProperty>();
        }

        /// <summary>
        /// Gets the sorted properties.
        /// </summary>
        /// <returns></returns>
        public List<cwLightProperty> getSortedProperties()
        {
            List<cwLightProperty> properties = getProperties();
            properties.Sort();
            return properties;
        }



        /// <summary>
        /// Gets the type of the associations.
        /// </summary>
        /// <value>
        /// The type of the associations.
        /// </value>
        public List<cwLightAssociationType> AssociationsType
        {
            get
            {
                List<cwLightAssociationType> ATs = new List<cwLightAssociationType>();
                foreach (cwLightAssociationType AT in _model.associationTypesManager.getAssociationTypesForSourceObjectType(this.ID))
                {
                    if (AT.Target.isPSFEnabled)
                    {
                        ATs.Add(AT);
                    }
                }
                return ATs;
                //return model.associationTypesManager.getAssociationTypesForSourceObjectType(this.ID);
            }
        }

        /// <summary>
        /// Gets the name of the association type by script.
        /// </summary>
        /// <param name="AssociationTypeScriptName">Name of the association type script.</param>
        /// <returns></returns>
        public cwLightAssociationType getAssociationTypeByScriptName(string AssociationTypeScriptName)
        {
            foreach (cwLightAssociationType AT in AssociationsType)
            {
                if (AT.ScriptName.Equals(AssociationTypeScriptName))
                {
                    return AT;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the type of the association types by target object.
        /// </summary>
        /// <returns></returns>
        public SortedDictionary<cwLightObjectType, List<cwLightAssociationType>> getAssociationTypesByTargetObjectType()
        {
            SortedDictionary<cwLightObjectType, List<cwLightAssociationType>> ATByTargetOT = new SortedDictionary<cwLightObjectType, List<cwLightAssociationType>>();
            foreach (cwLightAssociationType AT in AssociationsType)
            {
                if (!ATByTargetOT.ContainsKey(AT.Target))
                {
                    ATByTargetOT[AT.Target] = new List<cwLightAssociationType>();
                }
                ATByTargetOT[AT.Target].Add(AT);
            }
            foreach (var ATVar in ATByTargetOT)
            {
                ATByTargetOT[ATVar.Key].Sort();
            }
            return ATByTargetOT;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determines whether the specified property script name has property.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <returns>
        ///   <c>true</c> if the specified property script name has property; otherwise, <c>false</c>.
        /// </returns>
        public bool hasProperty(string propertyScriptName)
        {
            if (propertyScriptName == null) return false;
            if ("NAME".Equals(propertyScriptName))
            {
                propertyScriptName = getNamePropertyScriptName();
            }
            return _model.propertyManager.getPropertiesByScriptName(this.ID).ContainsKey(propertyScriptName);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyScriptName">Name of the property script.</param>
        /// <returns></returns>
        public cwLightProperty getProperty(string propertyScriptName)
        {
            if ("NAME".Equals(propertyScriptName))
            {
                propertyScriptName = getNamePropertyScriptName();
            }
            return _model.propertyManager.getPropertiesByScriptName(this.ID)[propertyScriptName];
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
            cwLightObjectType OT = obj as cwLightObjectType;
            return ToString().CompareTo(OT.ToString());
        }

        #endregion


        /// <summary>
        /// Gets the name of the type property script.
        /// </summary>
        /// <returns></returns>
        public string getTypePropertyScriptName()
        {
            switch (ScriptName)
            {
                case "CONNECTOR":
                    return "TYPE";
                case "EVENTRESULT":
                    return "INTERNALEXTERNAL";
                default:
                    return "TYPE";
            }
        }


        /// <summary>
        /// Determines whether [has unique identifier].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has unique identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasUniqueIdentifier()
        {
            switch (ScriptName)
            {
                case "FONT":
                    return false;
                default:
                    return true;
            }
        }


        /// <summary>
        /// Gets the name of the ID property script.
        /// </summary>
        /// <returns></returns>
        public string getIDPropertyScriptName()
        {
            switch (ScriptName)
            {
                case "FONT":
                    return "FONTNR";
                default:
                    return "ID";
            }
        }

        /// <summary>
        /// Gets the name of the name property script.
        /// </summary>
        /// <returns></returns>
        public string getNamePropertyScriptName()
        {
            switch (ScriptName)
            {
                case "CONNECTOR":
                    return "CONDITION";
                case "FREETEXTOBJECT":
                    return "TEXT";
                case "ITERATIONGROUP":
                    return "ITERATIONSTATEMENT";
                case "RELATIONSHIP":
                    return "DOWNNAME";
                case "DATAMODELUSAGE":
                case "ACCESSRIGHTS":
                case "BREAKPOINT":
                case "ISSUELINK":
                case "KEYMEMBER":
                case "REASONFORINVOLVEMENT":
                case "RELATIONSHIPSETMEMBER":
                    return "ID";
                case "SUBJECTAREAMEMBER":
                    return "OBJECTID";
                case "FONT":
                    return "FONTFACE";
                default:
                    return "NAME";

            }
        }
    }
}
