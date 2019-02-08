using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Casewise.Data.ICM;
using System.Collections;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// 
    /// </summary>
    public class cwLightProperty : IComparable
    {
        private cwLightObjectType parentObjectType = null;
        /// <summary>
        /// 
        /// </summary>
        public string ScriptName = null;
        /// <summary>
        /// 
        /// </summary>
        private int _datatype = -1;

        internal static string[] PROPERTIES_NAMES = new string[] { "OBJECTTYPEID", "SCRIPTNAME", "ID", "PANEID", "UNIQUEIDENTIFIER", "NAME", "HELP", "DATATYPE", "SEQUENCEINPANE" };
        private int _ID = 0;
        private int _panelID = 0;
        private string _UUID = null;
        private string _name = null;
        private string _help = null;
        private int _sequenceInPane = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightProperty"/> class.
        /// </summary>
        /// <param name="cwLightObjectType">Type of the cw light object.</param>
        /// <param name="reader">The reader.</param>
        public cwLightProperty(cwLightObjectType cwLightObjectType, IDataReader reader)
        {
            this.parentObjectType = cwLightObjectType;
            ScriptName = reader.GetString(1);
            _ID = reader.GetInt32(2);
            _panelID = reader.GetInt32(3);
            _UUID = reader.GetString(4);
            _name = reader.GetString(5);
            _help = reader.GetString(6);
            _datatype = reader.GetInt32(7);
            _sequenceInPane = reader.GetInt32(8);
        }


        /// <summary>
        /// Gets the display value from save value.
        /// </summary>
        /// <param name="saveValue">The save value.</param>
        /// <returns></returns>
        public string getDisplayValueFromSaveValue(string saveValue)
        {
            string displayValue = saveValue;
            if (isLookup)
            {
                try
                {
                    if ("".Equals(saveValue))
                    {
                        displayValue = "";
                    }
                    else
                    {
                        int l_id = 0;
                        bool isNumeric = int.TryParse(saveValue, out l_id);
                        if (isNumeric)
                        {
                            displayValue = getLookupValue(l_id);
                        }
                        else
                        {
                            displayValue = getLookupValue(saveValue);
                        }
                    }
                }
                catch (FormatException)
                {
                    displayValue = "";
                }

                if ("NOT_FOUND".Equals(saveValue))
                {
                    displayValue = Properties.Resources.TEXT_CATEGORY_UNDEFINED;
                }
            }
            else if (isBoolean)
            {
                if ("0".Equals(saveValue))
                {
                    displayValue = Properties.Resources.TEXT_FALSE;
                }
                else
                {
                    displayValue = Properties.Resources.TEXT_TRUE;
                }
            }
            return displayValue;
        }

        /// <summary>
        /// Gets the save value from display.
        /// </summary>
        /// <param name="displayValue">The display value.</param>
        /// <returns></returns>
        public string getSaveValueFromDisplay(string displayValue)
        {
            string saveValue = displayValue;
            if (isLookup)
            {
                saveValue = getLookupId(displayValue).ToString();
            }
            else if (isBoolean)
            {
                saveValue = Properties.Resources.TEXT_TRUE.Equals(displayValue) ? "1" : "0";
            }
            return saveValue;
        }


        /// <summary>
        /// Gets a value indicating whether this instance is lookup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is lookup; otherwise, <c>false</c>.
        /// </value>
        public bool isLookup
        {
            get
            {
                return "Lookup".Equals(DataType) || "FixedLookup".Equals(DataType);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is boolean.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is boolean; otherwise, <c>false</c>.
        /// </value>
        public bool isBoolean
        {
            get
            {
                return "Boolean".Equals(DataType);
            }
        }



        /// <summary>
        /// Gets the pane ID.
        /// </summary>
        public int paneID
        {
            get
            {
                return _panelID;
            }
        }

        /// <summary>
        /// Gets the sequence in pane.
        /// </summary>
        public int sequenceInPane
        {
            get
            {
                return _sequenceInPane;
            }
        }
        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public string DataType
        {
            get
            {
                switch (_datatype)
                {
                    case 1:
                        return "Integer";
                    case 2:
                        return "Double";
                    case 4:
                        return "Memo";
                    case 7:
                    // fixed lookup
                    case 9:
                        return "Lookup";
                    case 5:
                        return "Boolean";
                    case 8:
                        return "Date";
                    case 3:
                    default:
                        return "String";

                    //case 1:
                    //    return 
                    //case 2:
                    //    return CwEnumPropertyDataType.Double;
                    //case 4:
                    //    return CwEnumPropertyDataType.Memo;
                    //case 7:
                    //    // fixed lookup
                    //case 9:
                    //    return CwEnumPropertyDataType.Lookup;
                    //case 5:
                    //    return CwEnumPropertyDataType.Boolean;
                    //case 8:
                    //    return CwEnumPropertyDataType.Date;
                    //case 3:
                    //default:
                    //    return CwEnumPropertyDataType.String;
                }

            }
        }

        /// <summary>
        /// Gets the content of the lookup.
        /// </summary>
        /// <value>
        /// The content of the lookup.
        /// </value>
        public List<cwLookup> lookupContent
        {
            get
            {
                return parentObjectType.Model.lookupManager.getLookupsForProperty(this);
            }
        }

        internal string getLookupValue(string Uuid)
        {
            if (Uuid == "")
            {
                return Properties.Resources.TEXT_CATEGORY_UNDEFINED;
            }
            foreach (cwLookup l in lookupContent)
            {
                if (l.Uuid == Uuid)
                {
                    return l.ToString();
                }
            }
            return "NOT_FOUND";
        }

        internal string getLookupValue(int lookupID)
        {
            if (lookupID == 0)
            {
                return Properties.Resources.TEXT_CATEGORY_UNDEFINED;
            }
            foreach (cwLookup l in lookupContent)
            {
                if (l.ID.Equals(lookupID))
                {
                    return l.ToString();
                }
            }
            return "NOT_FOUND";
        }

        /// <summary>
        /// Gets the lookup id.
        /// </summary>
        /// <param name="_value">The _value.</param>
        /// <returns></returns>
        internal string getLookupId(string _value)
        {
            foreach (cwLookup l in lookupContent)
            {
                if (l.ToString().Equals(_value))
                {
                    //return l.ID;
                    return l.Uuid;
                }
            }
            return "";
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _name;
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
