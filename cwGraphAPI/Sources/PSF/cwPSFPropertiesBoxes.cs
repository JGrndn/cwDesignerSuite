using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// The container for the properties box
    /// </summary>
    public class cwPSFPropertiesBoxes
    {
        internal Dictionary<string, cwPSFPropertyBox> propertiesBox = new Dictionary<string, cwPSFPropertyBox>();
        private cwPSFTreeNode parentNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertiesBoxes"/> class.
        /// </summary>
        /// <param name="_parentNode">The _parent node.</param>
        public cwPSFPropertiesBoxes(cwPSFTreeNode _parentNode)
        {
            parentNode = _parentNode;
        }
        /// <summary>
        /// Adds the property box.
        /// </summary>
        public void addPropertyBox(cwPSFPropertyBox _propertyBox)
        {
            if (!propertiesBox.ContainsKey(_propertyBox.keyName))
            {
                _propertyBox.setPSFParentTreeNode(parentNode);
                propertiesBox.Add(_propertyBox.keyName, _propertyBox);
            }
        }

        /// <summary>
        /// Gets the property box boolean.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public cwPSFPropertyBoxCheckBox getPropertyBoxBoolean(String keyName)
        {
            cwPSFPropertyBox box = getPropertyBox(keyName);
            cwPSFPropertyBoxCheckBox cb_box = box as cwPSFPropertyBoxCheckBox;
            if (cb_box == null)
            {
                throw new cwExceptionWarning("Requested keyName [" + keyName + "] is not available as a Boolean Property");
            }
            return cb_box;
        }

        /// <summary>
        /// Gets the boxes.
        /// </summary>
        public List<cwPSFPropertyBox> Boxes
        {
            get
            {
                return propertiesBox.Values.ToList<cwPSFPropertyBox>();
            }
        }

        /// <summary>
        /// Gets the property box combo box.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public cwPSFPropertyBoxComboBox getPropertyBoxComboBox(String keyName)
        {
            cwPSFPropertyBox box = getPropertyBox(keyName);
            cwPSFPropertyBoxComboBox cb_box = box as cwPSFPropertyBoxComboBox;
            if (cb_box == null)
            {
                throw new cwExceptionWarning("Requested keyName [" + keyName + "] is not available as a Combo Box Property");
            }
            return cb_box;
        }


        /// <summary>
        /// Gets the property box int.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public cwPSFPropertyBoxInt getPropertyBoxInt(String keyName)
        {
            if (propertiesBox.ContainsKey(keyName))
            {
                return propertiesBox[keyName] as cwPSFPropertyBoxInt;
            }
            else
            {
                throw new cwExceptionSimpleNotice("Key name [" + keyName + "] is missing in " + this.GetType().Name.ToString());
            }
        }

        /// <summary>
        /// Gets the property box float.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public cwPSFPropertyBoxFloat getPropertyBoxFloat(String keyName)
        {
            if (propertiesBox.ContainsKey(keyName))
            {
                return propertiesBox[keyName] as cwPSFPropertyBoxFloat;
            }
            else
            {
                throw new cwExceptionSimpleNotice("Key name [" + keyName + "] is missing in " + this.GetType().Name.ToString());
            }
        }



        /// <summary>
        /// Gets the property box.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public T getPropertyBox<T>(String keyName) where T : cwPSFPropertyBox
        {
            if (propertiesBox.ContainsKey(keyName))
            {
                return propertiesBox[keyName] as T;
            }
            else
            {
                throw new cwExceptionSimpleNotice("Key name [" + keyName + "] is missing in " + this.GetType().Name.ToString());
            }
        }

        /// <summary>
        /// Gets the property box.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public cwPSFPropertyBox getPropertyBox(String keyName)
        {
            if (propertiesBox.ContainsKey(keyName))
            {
                return propertiesBox[keyName];
            }
            else
            {
                throw new cwExceptionSimpleNotice("Key name [" + keyName + "] is missing in " + this.GetType().Name.ToString());
            }
        }

        /// <summary>
        /// Draws the properties.
        /// </summary>
        public void addPropertiesToTreeNodeDrawTableBox(cwPSFTableLayoutPropertiesBoxes _tableLayoutPanelDetails)
        {
            _tableLayoutPanelDetails.Reset();
            foreach (var _propertyBoxVar in propertiesBox)
            {
                cwPSFPropertyBox _propertyBox = _propertyBoxVar.Value;
                if (!_propertyBox.isPSFHidden)
                {
                    _tableLayoutPanelDetails.addPropertyBox(_propertyBox);
                }
            }
        }

        /// <summary>
        /// Loads the properties.
        /// </summary>
        /// <param name="_node">The _node.</param>
        public void loadProperties(XmlNode _node)
        {

            foreach (var _propertyBoxVar in propertiesBox)
            {
                try
                {
                    cwPSFPropertyBox _propertyBox = _propertyBoxVar.Value;
                    XmlNode propertyAttributeNode = _node.SelectSingleNode("@" + _propertyBox.keyName);
                    propertiesBox[_propertyBoxVar.Key].setValue((propertyAttributeNode == null) ? "" : propertyAttributeNode.Value.ToString());
                }
                catch (cwExceptionSimpleNotice e)
                {
                    cwPSFTreeNode.log.Info(e.Message.ToString());
                }
            }
            try
            {
                //parentNode.updateNodeName();
            }
            catch (cwExceptionSimpleNotice e)
            {
                cwPSFTreeNode.log.Info(e.Message.ToString());
            }



        }

        /// <summary>
        /// Saves the properties box.
        /// </summary>
        /// <param name="_xmlWriter">The _XML writer.</param>
        internal void savePropertiesBox(XmlWriter _xmlWriter)
        {
            foreach (var _propertyBoxVar in propertiesBox)
            {
                cwPSFPropertyBox _propertyBox = _propertyBoxVar.Value;
                _propertyBox.saveToXML(_xmlWriter);
            }
        }


        /// <summary>
        /// Gets the property box collection.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        internal cwPSFPropertyBoxCollection getPropertyBoxCollection(string keyName)
        {
            cwPSFPropertyBox box = getPropertyBox(keyName);
            cwPSFPropertyBoxCollection collection_box = box as cwPSFPropertyBoxCollection;
            if (collection_box == null)
            {
                throw new cwExceptionWarning("Requested keyName [" + keyName + "] is not available as a Collection Property");
            }
            return collection_box;
        }

    }
}
