using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// a Lookup
    /// </summary>
    public class cwLookup : IComparable
    {
        private int _id;
        private string _abbreviation;
        private string _name;
        private string _uuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLookup" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="abbreviation">The abbreviation.</param>
        /// <param name="name">The name.</param>
        /// <param name="uuid">The UUID.</param>
        public cwLookup(int id, string abbreviation, string name, string uuid)
        {
            this._id = id;
            this._abbreviation = abbreviation;
            this._name = name;
            this._uuid = uuid;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public int ID
        {
            get
            {
                return _id;
            }
        }
        /// <summary>
        /// Gets the Abbreviation.
        /// </summary>
        public string Abbreviation
        {
            get
            {
                return _abbreviation;
            }
        }
        /// <summary>
        /// Gets the Name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the UUID.
        /// </summary>
        /// <value>
        /// The UUID.
        /// </value>
        public string Uuid
        {
            get
            {
                return _uuid;
            }
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
            return this.ToString().CompareTo(obj.ToString());
        }

        #endregion
    }
}
