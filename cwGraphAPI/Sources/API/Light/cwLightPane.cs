using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Pane
    /// </summary>
    public class cwLightPane
    {
        private int _id = 0;
        private string _name = null;
        private cwLightObjectType _objectType = null;
        private int _sequence = 0;

        private SortedDictionary<int, cwLightProperty> _sortedProperties = new SortedDictionary<int, cwLightProperty>();

        private SortedDictionary<int, List<cwLightProperty>> _sortedPropertiesBySequence = new SortedDictionary<int, List<cwLightProperty>>();

        private string _scriptName = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightPane"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="objectType">Type of the object.</param>
        public cwLightPane(int id, string name, int sequence, string scriptName, cwLightObjectType objectType)
        {
            _id = id;
            _name = name;
            _sequence = sequence;
            _objectType = objectType;
            _scriptName = scriptName;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return _objectType.Model;
            }
        }

        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        /// <value>
        /// The name of the script.
        /// </value>
        public string ScriptName
        {
            get
            {
                return _scriptName;
            }
        }

        /// <summary>
        /// Gets the sorted properties.
        /// </summary>
        public List<cwLightProperty> sortedProperties
        {
            get
            {
                if (0.Equals(_sortedProperties.Count))
                {
                    var order = 0;
                    foreach (var pVar in _sortedPropertiesBySequence)
                    {
                        pVar.Value.ForEach(p =>
                        {
                            _sortedProperties[order] = p;
                            order += 1;
                        });

                    }
                }
                return _sortedProperties.Values.ToList<cwLightProperty>();
            }
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="sequenceInPane">The sequence in pane.</param>
        /// <param name="property">The property.</param>
        public void addProperty(int sequenceInPane, cwLightProperty property)
        {
            if (!_sortedPropertiesBySequence.ContainsKey(sequenceInPane))
            {
                _sortedPropertiesBySequence[sequenceInPane] = new List<cwLightProperty>();
            }
            _sortedPropertiesBySequence[sequenceInPane].Add(property);
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
    }
}
