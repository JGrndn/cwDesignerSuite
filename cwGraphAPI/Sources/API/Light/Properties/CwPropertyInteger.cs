using System;
using Casewise.Data.ICM;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// CwPropertyDistribution
    /// </summary>
    public class CwPropertyInteger : CwProperty
    {
        /// <summary>
        /// The _distribution
        /// </summary>
        protected Int32 _integer = 0;
        /// <summary>
        /// The display name
        /// </summary>
        public const String DisplayName = "Decimal Number";

        /// <summary>
        /// Initializes a new instance of the <see cref="CwPropertyInteger"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="ordinalIndex">Index of the ordinal.</param>
        public CwPropertyInteger(ICMDataReader reader, int ordinalIndex)
            : base(CwEnumPropertyDataType.Integer)
        {
            this._integer = reader.GetInt32(ordinalIndex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CwPropertyInteger" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CwPropertyInteger(Int32 value)
            : base(CwEnumPropertyDataType.Integer)
        {
            this._integer = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CwPropertyInteger"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CwPropertyInteger(object value)
            : base(CwEnumPropertyDataType.Integer)
        {
            this._integer = Convert.ToInt32(value);
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="CwPropertyInteger" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CwPropertyInteger(string value)
            : base(CwEnumPropertyDataType.Integer)
        {
            this._integer = Convert.ToInt32(value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override object Value
        {
            get
            {
                return this._integer;
            }
        }

        /// <summary>
        /// Gets the distribution.
        /// </summary>
        /// <returns></returns>
        public Int32 getInteger()
        {
            return this._integer;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this._integer.ToString();
        }


        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        /// <value>
        /// The display value.
        /// </value>
        public override string DisplayValue
        {
            get
            {
                return this.Value.ToString();
            }
            set
            {
                this._integer = Convert.ToInt32(value);
            }
        }
    }
}
