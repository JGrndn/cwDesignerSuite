using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;
using System.ComponentModel;
using Casewise.GraphAPI.API;
using System.Drawing;


namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// act as a combox box
    /// </summary>
    public class cwPSFPropertyBoxComboBox : cwPSFPropertyBoxCollection
    {

        internal ComboBox boxComboBox = new ComboBox();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxComboBox"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="defaultItems">The default items.</param>
        public cwPSFPropertyBoxComboBox(String _helpName, String _helpDescription, String _keyName, object[] defaultItems)
            : base(_helpName, _helpDescription, _keyName)
        {
            boxComboBox.Items.AddRange(defaultItems);

            boxComboBox.Sorted = true;
            boxComboBox.FlatStyle = FlatStyle.Standard;
            boxComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SelectedIndexChanged(new EventHandler(CB_SelectedValueChanged));
        }


        /// <summary>
        /// Updates the control check node events.
        /// </summary>
        protected override void updateControlCheckNodeEvents()
        {
            boxComboBox.LostFocus += new EventHandler(cwPSFPropertyBox_CheckNode);
            boxComboBox.SelectedIndexChanged += new EventHandler(cwPSFPropertyBox_CheckNode);
        }


        /// <summary>
        /// Handles the SelectedValueChanged event of the CB control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void CB_SelectedValueChanged(object sender, EventArgs e)
        {
            checkFormat();
        }

        private object searchInItemsByProperty<T>(string cwLightAssociationTypeScriptName)
        {
            foreach (object node in boxComboBox.Items)
            {
                cwLightAssociationType AT_Node = node as cwLightAssociationType;
                if (AT_Node == null)
                {
                    if (node.Equals(cwLightAssociationTypeScriptName))
                    {
                        return node.ToString();
                    }
                }
                else
                {
                    if (AT_Node.ScriptName.Equals(cwLightAssociationTypeScriptName))
                    {
                        return AT_Node;
                    }
                }
            }
            return null;
        }



        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public override void setValue(object _item)
        {
            boxComboBox.SelectedItem = _item;
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void clearItems()
        {
            boxComboBox.Items.Clear();
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        public override void enable()
        {
            boxComboBox.Enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public override void disable()
        {
            boxComboBox.Enabled = false;
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            return boxComboBox;
        }



        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public virtual void setItem(object _item)
        {
            boxComboBox.SelectedItem = _item;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void addRange(object[] items)
        {
            boxComboBox.Items.AddRange(items);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void add(object item)
        {
            boxComboBox.Items.Add(item);
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return getStringValue();
        }


        /// <summary>
        /// Selecteds the index changed.
        /// </summary>
        /// <param name="e">The e.</param>
        public void SelectedIndexChanged(EventHandler e)
        {
            boxComboBox.SelectedIndexChanged += e;
        }

        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public override String ToStringCollection()
        {
            return ToString();
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <returns></returns>
        private string getStringValue()
        {
            if (boxComboBox.SelectedItem != null)
                return boxComboBox.SelectedItem.ToString();
            else 
                return string.Empty;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public object getValue()
        {
            return boxComboBox.SelectedItem;
        }


        /// <summary>
        /// Gets the content of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> getCollectionContent<T>() 
        {
            List<T> items = new List<T>();
            foreach (object o in boxComboBox.Items)
            {
                if ((o as T) != null)
                {
                    items.Add(o as T);
                }
            }
            return items;
        }
    }
}
