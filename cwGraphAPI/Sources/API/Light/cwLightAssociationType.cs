using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Casewise.GraphAPI.API
{


    /// <summary>
    /// 
    /// </summary>
    public class cwLightAssociationType : IComparable
    {
        private bool _isReversed = false;
        private string _name = null;
        private string _scriptName = null;
        private cwLightObjectType _source = null;
        private cwLightObjectType _target = null;
        private cwLightObjectType _intersection = null;
        private int _ID = 0;
        private bool _hasIntersection = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightAssociationType"/> class.
        /// </summary>
        /// <param name="currentModel">The current model.</param>
        /// <param name="name">The name.</param>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="sourceID">The source ID.</param>
        /// <param name="targetID">The target ID.</param>
        /// <param name="isReversed">if set to <c>true</c> [is reversed].</param>
        /// <param name="reader">The reader.</param>
        public cwLightAssociationType(cwLightModel currentModel, string name, string scriptName, int sourceID, int targetID, bool isReversed, IDataReader reader)
        {
            this._name = name;
            this._scriptName = scriptName;
            _ID = reader.GetInt32(0);
            int intersectionID = reader.GetInt32(5);
            _source = currentModel.getObjectTypeByID(sourceID);
            _target = currentModel.getObjectTypeByID(targetID);
            _intersection = currentModel.getObjectTypeByID(intersectionID);
            if (_intersection != null)
            {
                _hasIntersection = true;
            }
            _isReversed = isReversed;
            //ID, NAME, REVERSENAME, SCRIPTNAME, REVERSESCRIPTNAME, OBJECTTYPEID, BELOWOBJECTTYPEID, ABOVEOBJECTTYPEID, UNIQUEIDENTIFIER
        }
        /// <summary>
        /// 
        /// </summary>
        public string ScriptName
        {
            get
            {
                return _scriptName;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance has intersection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has intersection; otherwise, <c>false</c>.
        /// </value>
        public bool hasIntersection
        {
            get
            {
                return _hasIntersection;
            }
        }
        /// <summary>
        /// Gets the target.
        /// </summary>
        public cwLightObjectType Target
        {
            get
            {
                return _target;
            }
        }
        /// <summary>
        /// Gets the source.
        /// </summary>
        public cwLightObjectType Source
        {
            get
            {
                return _source;
            }
        }
        /// <summary>
        /// Gets the intersection.
        /// </summary>
        public cwLightObjectType Intersection
        {
            get
            {
                return _intersection;
            }
        }

        /// <summary>
        /// Determines whether this instance is reversed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is reversed; otherwise, <c>false</c>.
        /// </returns>
        public bool isReversed
        {
            //ICM : Informations.TargetOptional
            get { return _isReversed; }
        }

        /// <summary>
        /// Gets the name of the verbe.
        /// </summary>
        /// <returns></returns>
        public string getVerbeName()
        {
            return _name.ToString();
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
            cwLightAssociationType AT = obj as cwLightAssociationType;
            return ToString().CompareTo(AT.ToString());
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _source.ToString() + " - " + _target.ToString() + " (" + _name.ToString() + ")";
        }
    }
}
