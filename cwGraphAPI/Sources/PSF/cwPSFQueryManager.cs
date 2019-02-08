using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using Casewise.Data.ICM;
using System.Data;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Statif methods to help the creation of query
    /// </summary>
    public class cwPSFQueryManager
    {



        //internal static string sourceIDForIntersectionOT(cwLightAssociationType AT)
        //{
        //    if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName))
        //    {
        //        return AT.Source.ScriptName + "ID";
        //    }
        //    else
        //    {
        //        if (!AT.isReversed)
        //        {
        //            return "ABOVEOBJECTID";
        //        }
        //        else
        //        {
        //            return "BELOWOBJECTID";
        //        }
        //    }
        //}

        /// <summary>
        /// Sources the ID for intersection OT.
        /// </summary>
        /// <param name="AT">The AT.</param>
        /// <returns></returns>
        public static string sourceIDForIntersectionOT(cwLightAssociationType AT)
        {
            if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName) || "ENTITYSYNONYM".Equals(AT.Intersection.ScriptName) || "DATAMODELUSAGE".Equals(AT.Intersection.ScriptName))
            {
                return AT.Source.ScriptName + "ID";
            }
            else if ("ISSUELINK".Equals(AT.Intersection.ScriptName))
            {
                if ("ISSUE".Equals(AT.Target.ScriptName))
                {
                    return "OBJECTID";
                }
                else
                {
                    return "ISSUEID";
                }
            }
            else
            {
                if (!AT.isReversed)
                {
                    return "ABOVEOBJECTID";
                }
                else
                {
                    return "BELOWOBJECTID";
                }
            }
        }

        /// <summary>
        /// Targets the ID for intersection OT.
        /// </summary>
        /// <param name="AT">The AT.</param>
        /// <returns></returns>
        public static string targetIDForIntersectionOT(cwLightAssociationType AT)
        {
            if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName) || "ENTITYSYNONYM".Equals(AT.Intersection.ScriptName) || "DATAMODELUSAGE".Equals(AT.Intersection.ScriptName))
            {
                return AT.Target.ScriptName + "ID";
            }
            else if ("ISSUELINK".Equals(AT.Intersection.ScriptName))
            {
                if ("ISSUE".Equals(AT.Target.ScriptName))
                {
                    return "ISSUEID";
                }
                else
                {
                    return "OBJECTID";
                }
            }
            else
            {
                if (AT.isReversed)
                {
                    return "ABOVEOBJECTID";
                }
                else
                {
                    return "BELOWOBJECTID";
                }
            }
        }


        /// <summary>
        /// Queries the order for intersection OT.
        /// </summary>
        /// <param name="AT">The AT.</param>
        /// <returns></returns>
        //internal static string queryOrderForIntersectionOT(cwLightAssociationType AT)
        //{
        //    if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName))
        //    {
        //        return ", " + AT.Source.ScriptName + "ID, " + AT.Target.ScriptName + "ID";
        //    }
        //    else
        //    {
        //        if (!AT.isReversed)
        //        {
        //            return ", ABOVEOBJECTID, BELOWOBJECTID ";
        //        }
        //        else
        //        {
        //            return ", BELOWOBJECTID, ABOVEOBJECTID ";
        //        }
        //    }
        //}



        internal static string queryOrderForIntersectionOT(cwLightAssociationType AT)
        {
            if ("REASONFORINVOLVEMENT".Equals(AT.Intersection.ScriptName) || "ENTITYSYNONYM".Equals(AT.Intersection.ScriptName) || "DATAMODELUSAGE".Equals(AT.Intersection.ScriptName))
            {
                return ", " + AT.Source.ScriptName + "ID, " + AT.Target.ScriptName + "ID";
            }
            else if ("ISSUELINK".Equals(AT.Intersection.ScriptName))
            {
                return ", ISSUEID, OBJECTID";
            }
            else
            {
                if (!AT.isReversed)
                {
                    return ", ABOVEOBJECTID, BELOWOBJECTID ";
                }
                else
                {
                    return ", BELOWOBJECTID, ABOVEOBJECTID ";
                }
            }
        }



        /// <summary>
        /// Creates the properties to select query string.
        /// </summary>
        /// <param name="_propertiesToLoad">The _properties to load.</param>
        /// <param name="_propertiesTypeByIterationNumber">The _properties type by iteration number.</param>
        /// <param name="_propertiesTypeByScriptName">Name of the _properties type by script.</param>
        /// <param name="OT">The OT.</param>
        /// <returns></returns>
        public static string createPropertiesToSelectQueryString(List<string> _propertiesToLoad, Dictionary<int, string> _propertiesTypeByIterationNumber, Dictionary<string, string> _propertiesTypeByScriptName, cwLightObjectType OT)
        {
            StringBuilder sb = new StringBuilder();

            int _propertiesToLoadCount = _propertiesToLoad.Count;
            for (int i = 0; i < _propertiesToLoadCount; ++i)
            {
                string propertyScriptname = _propertiesToLoad[i];
                cwLightProperty _type = OT.getProperty(propertyScriptname);
                _propertiesTypeByIterationNumber[i] = _type.DataType;
                _propertiesTypeByScriptName[propertyScriptname] = _type.DataType;
                sb.Append(propertyScriptname + ", ");
            }
            sb.Append(OT.getIDPropertyScriptName());
            if (OT.hasUniqueIdentifier())
            {
                sb.Append(", " + cwLightObject.UNIQUEIDENTIFIER + " ");
            }
            if (!OT.isIntersectionObjectType)
            {
                sb.Append(", " + OT.getNamePropertyScriptName());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Updates the command using query for filter properties.
        /// </summary>
        /// <param name="_propertiesFiltersKeep">The _properties filters keep.</param>
        /// <param name="command">The command.</param>
        /// <param name="OT">The OT.</param>
        public static void updateCommandUsingQueryForFilterProperties(Dictionary<string, List<KeyValuePair<string, string>>> _propertiesFiltersKeep, ICMCommand command, cwLightObjectType OT)
        {
            foreach (var propertyFilter in _propertiesFiltersKeep)
            {
                string scriptname = propertyFilter.Key;
                if (!OT.hasProperty(scriptname)) continue;
                cwLightProperty _type = OT.getProperty(scriptname);
                List<KeyValuePair<string, string>> values = propertyFilter.Value;
                for (int i = 0; i < values.Count; ++i)
                {
                    string a = char.ConvertFromUtf32(i + 65);
                    if (_type.DataType.Equals("Boolean"))
                    {
                        command.Parameters.Add("@" + scriptname.ToUpper() + a, "0");
                    }
                    else if (_type.DataType.Equals("Lookup"))
                    {
                        int lookupId = 0;
                        cwLookup lu = _type.lookupContent.Find(l => l.Uuid.Equals(values[i].Value));
                        if (lu != null)
                        {
                            lookupId = lu.ID;
                        }
                        command.Parameters.Add("@" + scriptname.ToUpper() + a, lookupId);
                    }
                    else
                    {
                        string _value = values[i].Value;

                        if ("Memo".Equals(_type.DataType)) continue;
                        command.Parameters.Add("@" + scriptname.ToUpper() + a, _value);
                    }
                }
            }
        }


        /// <summary>
        /// Updates the properties from query response.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="_properties">The _properties.</param>
        /// <param name="_propertiesToLoad">The _properties to load.</param>
        /// <param name="_propertiesTypeByIterationNumber">The _properties type by iteration number.</param>
        /// <param name="_lightModel">The _light model.</param>
        public static void updatePropertiesFromQueryResponse(IDataReader reader, Dictionary<string, string> _properties, List<string> _propertiesToLoad, Dictionary<int, string> _propertiesTypeByIterationNumber, cwLightModel _lightModel)
        {
            int attrToLoadCount = _propertiesToLoad.Count;
            for (var i = 0; i < attrToLoadCount; ++i)
            {
                if ("Lookup".Equals(_propertiesTypeByIterationNumber[i]) || "FixedLookup".Equals(_propertiesTypeByIterationNumber[i]))
                {
                    int lookupID = reader.GetInt32(i);
                    if (lookupID == 0)
                    {
                        _properties[_propertiesToLoad[i]] = "";
                        _properties[_propertiesToLoad[i] + cwLookupManager.LOOKUPID_KEY] = "0";
                        _properties[_propertiesToLoad[i] + cwLookupManager.LOOKUPABBR_KEY] = "";
                    }
                    else
                    {
                        _properties[_propertiesToLoad[i]] = _lightModel.lookupManager.getLookupNameByID(lookupID);
                        _properties[_propertiesToLoad[i] + cwLookupManager.LOOKUPID_KEY] = lookupID.ToString();
                        _properties[_propertiesToLoad[i] + cwLookupManager.LOOKUPABBR_KEY] = _lightModel.lookupManager.getLookupAbbreviationByID(lookupID);
                    }

                }
                else
                {
                    object value = reader.GetValue(i);
                    if (value != null)
                    {
                        _properties[_propertiesToLoad[i]] = value.ToString();
                    }

                }
            }
        }

        /// <summary>
        /// Creates the properties where query string.
        /// </summary>
        /// <param name="_propertiesFiltersKeep">The _properties filters keep.</param>
        /// <param name="OT">The ot.</param>
        /// <returns></returns>
        public static string createPropertiesWhereQueryString(Dictionary<string, List<KeyValuePair<string, string>>> _propertiesFiltersKeep, cwLightObjectType OT)
        {
            string _queryClause = "";
            if (_propertiesFiltersKeep != null && _propertiesFiltersKeep.Count > 0)
            {
                foreach (string scriptname in _propertiesFiltersKeep.Keys)
                {
                    if (!OT.hasProperty(scriptname)) continue;
                    cwLightProperty _type = OT.getProperty(scriptname);
                    List<KeyValuePair<string, string>> values = _propertiesFiltersKeep[scriptname];
                    for (int i = 0; i < values.Count; ++i)
                    {
                        string _operationFilter = "=";
                        string a = char.ConvertFromUtf32(i + 65);

                        if (_type.DataType.Equals("Boolean"))
                        {
                            if (values[i].Value.Equals("0"))
                            {
                                if (!values[i].Key.Equals("="))
                                {
                                    _operationFilter = " <> ";
                                }
                            }
                            else
                            {
                                if (values[i].Key.Equals("="))
                                {
                                    _operationFilter = " <> ";
                                }
                            }
                            _queryClause += " AND " + scriptname + _operationFilter + "@" + scriptname.ToUpper() + "" + a.ToString();
                        }
                        else
                        {
                            // check if the current object respect at leat one rule
                            switch (values[i].Key)
                            {
                                case "≠":
                                    _operationFilter = " <> ";
                                    _queryClause += " AND " + scriptname + _operationFilter + "@" + scriptname.ToUpper() + "" + a.ToString();
                                    break;
                                default:
                                    _operationFilter = " " + values[i].Key + " ";
                                    _queryClause += " AND " + scriptname + _operationFilter + "@" + scriptname.ToUpper() + "" + a.ToString();
                                    break;
                            }

                        }
                        
                    }

                }
                _queryClause = _queryClause.Substring(" AND ".Length);
                _queryClause = " WHERE " + _queryClause;
            }
            return _queryClause;
        }
    }
}
