using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.PSF;
using log4net;


namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// 
    /// </summary>
    public class cwLightObject : IComparable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwLightObject));
        /// <summary>
        /// properties
        /// </summary>
        public Dictionary<string, string> properties = new Dictionary<string, string>();
        /// <summary>
        /// ID
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// OTID
        /// </summary>
        public int OTID = 0;
        /// <summary>
        /// UNIQUEIDENTIFIER
        /// </summary>
        public const string UNIQUEIDENTIFIER = "UNIQUEIDENTIFIER";
        /// <summary>
        /// WHENUPDATED
        /// </summary>
        public const string WHEN_UPDATED = "WHENUPDATED";
        /// <summary>
        /// WHENCREATED
        /// </summary>
        public const string WHEN_CREATED = "WHENCREATED";


        private cwLightModel currentLightModel = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObject"/> class.
        /// </summary>
        /// <param name="currentLightModel">The current light model.</param>
        /// <param name="_ID">The _ ID.</param>
        /// <param name="_OTID">The _ OTID.</param>
        public cwLightObject(cwLightModel currentLightModel, int _ID, int _OTID)
        {
            this.ID = _ID;
            this.OTID = _OTID;
            this.currentLightModel = currentLightModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightObject"/> class.
        /// </summary>
        /// <param name="currentLightModel">The current light model.</param>
        /// <param name="_ID">The _ ID.</param>
        /// <param name="_OTID">The _ OTID.</param>
        /// <param name="name">The name.</param>
        public cwLightObject(cwLightModel currentLightModel, int _ID, int _OTID, string name)
        {
            this.ID = _ID;
            this.OTID = _OTID;
            this.properties["NAME"] = name;
            this.currentLightModel = currentLightModel;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Freezes the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void freeze(int level)
        {
            this.properties["EXPORTFLAG"] = level.ToString();
            this.updatePropertiesInModel();
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return currentLightModel;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                 string key = getObjectType().getNamePropertyScriptName();
                 try
                 {
                     //switch (OTID)
                     //{
                     //    case 13211:
                     //        key = "TEXT";
                     //        break;
                     //    case 9225:
                     //        key = "CONDITION";
                     //        break;
                     //}
                     return properties[key];
                 }
                catch (KeyNotFoundException)
                {
                    throw new cwExceptionFatal(String.Format("The requried key [{0}] has not been loaded for {1} with id {2}", key, getObjectType().ToString(), ID.ToString()));
                }
            }
        }


        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
        public bool delete()
        {
            try
            {
                getObjectType().deleteALLObjectsUsingWhereClause(" ID = " + ID.ToString() + ")");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <returns></returns>
        public cwLightObjectType getObjectType()
        {
            if (!currentLightModel.hasObjectTypeByID(OTID))
            {
                throw new ApplicationException(String.Format(Properties.Resources.ERROR_WARN_MISSING_cwLightObjectType, OTID));
            }
            return currentLightModel.getObjectTypeByID(OTID);
        }

        /// <summary>
        /// Updates the property value.
        /// </summary>
        /// <returns></returns>
        public bool updatePropertiesInModel()
        {
            cwLightModel _lightModel = getObjectType().Model;
            Dictionary<string, string> _usedProperties = new Dictionary<string, string>();
            foreach (var propertyWithValue in properties)
            {
                if (propertyWithValue.Key.EndsWith(cwLookupManager.LOOKUPID_KEY) ||
                    propertyWithValue.Key.EndsWith(cwLookupManager.LOOKUPABBR_KEY) ||
                    propertyWithValue.Key.Equals(UNIQUEIDENTIFIER))
                {
                    continue;
                }
                _usedProperties[propertyWithValue.Key] = propertyWithValue.Value;
            }


            StringBuilder query = new StringBuilder();
            query.Append("UPDATE ");
            cwLightObjectType OT = getObjectType();
            query.Append(OT.ScriptName);
            int i = 0;
            query.Append(" SET ");
            foreach (var propertyWithValue in _usedProperties)
            {
                query.Append("  " + propertyWithValue.Key + " = @" + propertyWithValue.Key);
                if (i < _usedProperties.Count - 1)
                {
                    query.Append(", ");
                }
                i++;
            }
            query.Append(" WHERE ID = @OBJECTID");

            try
            {
                ICMConnection connection = _lightModel.getConnection();
                using (ICMCommand command = new ICMCommand(query.ToString(), connection))
                {
                    command.Parameters.Add("@OBJECTID", this.ID);
                    foreach (var propertyWithValue in _usedProperties)
                    {
                        cwLightProperty p = getObjectType().getProperty(propertyWithValue.Key);
                        if (p.isLookup)
                        {
                            command.Parameters.Add("@" + propertyWithValue.Key, properties[propertyWithValue.Key + cwLookupManager.LOOKUPID_KEY]);
                        }
                        else
                        {
                            command.Parameters.Add("@" + propertyWithValue.Key, propertyWithValue.Value);
                        }
                    }

                    using (IDbTransaction trans = connection.BeginTransaction())
                    {
                        command.ExecuteReader();
                        trans.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                throw e;
            }

            return true;
        }


        /// <summary>
        /// Compares the properties.
        /// </summary>
        /// <param name="_sourceProperties">The _source properties.</param>
        /// <param name="_targetProperties">The _target properties.</param>
        /// <returns></returns>
        public static bool compareProperties(Dictionary<string, string> _sourceProperties, Dictionary<string, string> _targetProperties)
        {
            foreach (var propertyVar in _sourceProperties)
            {
                if (propertyVar.Key.EndsWith(cwLookupManager.LOOKUPID_KEY)) continue;
                if (!_targetProperties[propertyVar.Key].Equals(propertyVar.Value)) return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the properties.
        /// </summary>
        /// <param name="_targetProperties">The _target properties.</param>
        /// <returns></returns>
        public bool compareProperties(Dictionary<string, string> _targetProperties)
        {
            foreach (var propertyVar in properties)
            {
                if (propertyVar.Key.EndsWith(cwLookupManager.LOOKUPID_KEY)) continue;
                if (propertyVar.Key.EndsWith(cwLookupManager.LOOKUPABBR_KEY)) continue;
                if (!_targetProperties[propertyVar.Key].Equals(propertyVar.Value)) return false;
            }
            return true;
        }


        /// <summary>
        /// Associates to with transaction.
        /// </summary>
        /// <param name="AT_TO_TARGET">The A t_ T o_ TARGET.</param>
        /// <param name="targetObject">The target object.</param>
        public void associateToWithTransaction(cwLightAssociationType AT_TO_TARGET, cwLightObject targetObject)
        {
            associateToWithTransaction(AT_TO_TARGET, targetObject, new Dictionary<string, string>());
        }

        /// <summary>
        /// Associates to with transaction.
        /// </summary>
        /// <param name="AT_TO_TARGET">The A t_ T o_ TARGET.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="_extraProperties">The _extra properties.</param>
        public void associateToWithTransaction(cwLightAssociationType AT_TO_TARGET, cwLightObject targetObject, Dictionary<string, string> _extraProperties)
        {
            using (IDbTransaction trans = Model.getConnection().BeginTransaction())
            {
                associateToWithoutTransaction(AT_TO_TARGET, targetObject, _extraProperties);
                trans.Commit();
            }
        }


        /// <summary>
        /// Associates to.
        /// </summary>
        /// <param name="AT_TO_TARGET">The A t_ T o_ TARGET.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="_extraProperties">The _extra properties.</param>
        internal void associateToWithoutTransaction(cwLightAssociationType AT_TO_TARGET, cwLightObject targetObject, Dictionary<string, string> _extraProperties)
        {
            try
            {
                string OT_Intersection_ScriptName = AT_TO_TARGET.Intersection.ScriptName;

                if (!_extraProperties.ContainsKey("NAME"))
                {
                    _extraProperties["NAME"] = "";
                }

                string extraPropertiesString = "";
                int extraPropertiesCount = _extraProperties.Count;
                if (extraPropertiesCount > 0)
                {
                    string extraPropertiesStringKey = "(";
                    string extraPropertiesStringValues = "(";
                    int i = 0;
                    foreach (var property in _extraProperties)
                    {
                        extraPropertiesStringKey += property.Key;
                        extraPropertiesStringValues += "@" + property.Key.Replace('É', 'E');
                        if (i < extraPropertiesCount - 1)
                        {
                            extraPropertiesStringKey += ",";
                            extraPropertiesStringValues += ",";
                        }
                        i++;
                    }
                    extraPropertiesStringKey += ")";
                    extraPropertiesStringValues += ")";
                    extraPropertiesString = " " + extraPropertiesStringKey + " VALUES " + extraPropertiesStringValues + " ";
                }

                StringBuilder query = new StringBuilder();

                query.Append("INSERT INTO ");
                query.Append(AT_TO_TARGET.Intersection.ScriptName);
                query.Append("(NAME " + cwLightModel.queryOrderForIntersectionOTWithTableNumber(AT_TO_TARGET) + ") VALUES ('@NAME', @SOURCEID, @TARGETID, @SOURCETNID, @TARGETTNID)");
                ICMConnection connection = Model.getConnection();
                using (ICMCommand command = new ICMCommand(query.ToString(), connection))
                {
                    command.Parameters.Add("@SOURCEID", this.ID);
                    command.Parameters.Add("@TARGETID", targetObject.ID);
                    command.Parameters.Add("@SOURCETNID", this.OTID);
                    command.Parameters.Add("@TARGETTNID", targetObject.OTID);
                    foreach (var property in _extraProperties)
                    {
                        command.Parameters.Add("@" + property.Key.Replace('É', 'E'), property.Value);
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        ///   </exception>
        public override bool Equals(object obj)
        {
            cwLightObject o = obj as cwLightObject;
            return OTID.Equals(o.OTID) && ID.Equals(o.ID);
        }

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
            cwLightObject o = obj as cwLightObject;
            if (0.Equals(OTID.CompareTo(o.OTID)))
            {
                return ID.CompareTo(o.ID);
            }
            return OTID.CompareTo(o.OTID);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Model.FileName + OTID.ToString() + ID.ToString()).GetHashCode();
        }

    }
}
