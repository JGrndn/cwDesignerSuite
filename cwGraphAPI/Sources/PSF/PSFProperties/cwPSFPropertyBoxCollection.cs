using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Act as a container or list
    /// </summary>
    public class cwPSFPropertyBoxCollection : cwPSFPropertyBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxCollection"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        public cwPSFPropertyBoxCollection(String _helpName, String _helpDescription, String _keyName)
            : base(_helpName, _helpDescription, _keyName)
        {
        }


        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            throw new NotImplementedException("getControl : " + GetType().Name);
        }
        /// <summary>
        /// Searches the name of the in collection by script.
        /// </summary>
        /// <param name="_ScriptName">Name of the _ script.</param>
        /// <returns></returns>
        internal virtual object searchInCollectionByScriptName(string _ScriptName)
        {
            throw new NotImplementedException("searchInCollectionByScriptName : " + GetType().Name);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToStringCollection();
        }

        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public virtual String ToStringCollection()
        {
            throw new NotImplementedException("ToStringCollection : " + GetType().Name);
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="_xmlWriter">The _XML writer.</param>
        internal override void saveToXML(System.Xml.XmlWriter _xmlWriter)
        {
            _xmlWriter.WriteAttributeString(this.keyName, ToStringCollection());
        }

        /// <summary>
        /// Loads the nodes.
        /// </summary>
        public virtual void loadNodes(object _sourcecwLightObjectType)
        {
            throw new NotImplementedException("loadNodes : " + GetType().Name);
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public virtual void clearItems()
        {
            throw new NotImplementedException("clearItems : " + GetType().Name);
        }

        /// <summary>
        /// Gets the content of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> getCollectionContent<T>() where T : class
        {
            throw new NotImplementedException();
        }


    }
}
