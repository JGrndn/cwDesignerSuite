using System;
using Casewise.Data.ICM;
using CasewiseDataTier2004;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// 
    /// </summary>
    public enum CwEnumPropertyDataType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The integer
        /// </summary>
        Integer,
        /// <summary>
        /// The double
        /// </summary>
        Double,
        /// <summary>
        /// The memo
        /// </summary>
        Memo,
        /// <summary>
        /// The lookup
        /// </summary>
        Lookup,
        /// <summary>
        /// The boolean
        /// </summary>
        Boolean,
        /// <summary>
        /// The date
        /// </summary>
        Date,
        /// <summary>
        /// The string
        /// </summary>
        String
    }

    /// <summary>
    /// CwProperty
    /// </summary>
    public class CwProperty
    {
        /// <summary>
        /// The _datatype
        /// </summary>
        protected CwEnumPropertyDataType _datatype = CwEnumPropertyDataType.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="CwProperty" /> class.
        /// </summary>
        /// <param name="datatype">The datatype.</param>
        public CwProperty(CwEnumPropertyDataType datatype)
        {
            this._datatype = datatype;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="System.NotImplementedException">getValue</exception>
        public virtual object Value
        {
            get
            {
                throw new NotImplementedException("getValue");
            }
            set
            {
                //this.updateValue(value);
                throw new NotImplementedException("setValue");
            }
        }


        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        /// <value>
        /// The display value.
        /// </value>
        public virtual String DisplayValue
        {
            get
            {
                return this.ToString();
            }
            set
            {
                this.updateFromDisplayValue(value);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException">ToString</exception>
        public override string ToString()
        {
            throw new NotImplementedException("ToString");
        }

        /// <summary>
        /// Gets the serializable value.
        /// </summary>
        /// <value>
        /// The serializable value.
        /// </value>
        public virtual object SerializableValue
        {
            get
            {
                return this.Value;
            }
        }


        /// <summary>
        /// Gets the save value from display.
        /// </summary>
        /// <param name="displayValue">The display value.</param>
        protected virtual void updateFromDisplayValue(string displayValue)
        {
            this.Value = displayValue;
        }

        /// <summary>
        /// Updates the command for update.
        /// </summary>
        /// <param name="keyParameter">The key parameter.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="System.NotImplementedException">updateCommandForUpdate</exception>
        public virtual void updateCommandForUpdate(string keyParameter, ICMCommand command)
        {
            if (this.Value is ICMDistribution)
            {
                ICMDistribution distributionObject = this.Value as ICMDistribution;
                IcwDistribution oDistribution = (IcwDistribution)distributionObject.GetRawDistribution();
                command.Parameters.Add(keyParameter, oDistribution);
            }
            else
            {
                command.Parameters.Add(keyParameter, this.Value);
            }

        }



        ///// <summary>
        ///// Creates the property.
        ///// </summary>
        ///// <param name="propertyType">Type of the property.</param>
        ///// <param name="serializedValue">The serialized value.</param>
        ///// <returns></returns>
        //public static CwProperty CreateProperty(cwLightProperty propertyType, object serializedValue)
        //{
        //    switch (propertyType.DataType)
        //    {
        //        case CwEnumPropertyDataType.Integer:
        //            return new CwPropertyInteger(serializedValue);
        //        case CwEnumPropertyDataType.Memo:
        //            return new CwPropertyMemo(serializedValue);
        //        case CwEnumPropertyDataType.String:
        //            return new CwPropertyString(serializedValue);
        //        case CwEnumPropertyDataType.Double:
        //            return new CwPropertyDouble(serializedValue);
        //        case CwEnumPropertyDataType.Boolean:
        //            return new CwPropertyBoolean(serializedValue);
        //        case CwEnumPropertyDataType.Date:
        //            return new CwPropertyDateTime(serializedValue);
        //        case CwEnumPropertyDataType.Lookup:
        //            return new CwPropertyLookup(serializedValue, propertyType);
        //        default:
        //            return null;
        //    }
        //}

        ///// <summary>
        ///// Creates the property.
        ///// </summary>
        ///// <param name="datatype">The datatype.</param>
        ///// <param name="reader">The reader.</param>
        ///// <param name="ordinalIndex">Index of the ordinal.</param>
        ///// <param name="propertyType">Type of the property.</param>
        ///// <returns></returns>
        //public static CwProperty CreateProperty(CwEnumPropertyDataType datatype, ICMDataReader reader, int ordinalIndex, cwLightProperty propertyType)
        //{
        //    switch (datatype)
        //    {
        //        case CwEnumPropertyDataType.Integer:
        //            return new CwPropertyInteger(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.Memo:
        //            return new CwPropertyMemo(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.String:
        //            return new CwPropertyString(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.Double:
        //            return new CwPropertyDouble(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.Boolean:
        //            return new CwPropertyBoolean(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.Date:
        //            return new CwPropertyDateTime(reader, ordinalIndex);
        //        case CwEnumPropertyDataType.Lookup:
        //            return new CwPropertyLookup(reader, ordinalIndex, propertyType);
        //        default:
        //            return null;
        //    }
        //}
    }
}
