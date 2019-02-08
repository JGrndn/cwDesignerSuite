using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// The parent of all properties boxes
    /// </summary>
    public class cwPSFPropertyBox : TextBox
    {
        /// <summary>
        /// allow to create an error rule for the property box
        /// </summary>
        protected cwPSFToolTip errorToolTip = null;
        /// <summary>
        /// The display name of the property
        /// </summary>
        public String helpName = "";
        /// <summary>
        /// helpDescription used for the popup
        /// </summary>
        public String helpDescription = "";
        internal String keyName = "";
        private bool valueCanBeEmpty = true;

        /// <summary>
        /// isPSFHidden
        /// </summary>
        public bool isPSFHidden = false;

        /// <summary>
        /// Used to resize the tree view
        /// </summary>
        public const int TREEVIEW_ITEM_SIZE = 18;

        private cwPSFTreeNode parentPSFTreeNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBox"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        public cwPSFPropertyBox(String _helpName, String _helpDescription, String _keyName)
        {
            this.helpName = _helpName;
            this.helpDescription = _helpDescription;
            this.keyName = _keyName;
            errorToolTip = new cwPSFToolTip(this.Font);
            errorToolTip.ToolTipTitle = Properties.Resources.PSF_PROPERTY_FORMAT_ERROR;
            this.TextChanged += new EventHandler(cwPSFProperyBox_Changed);
            this.Resize += new EventHandler(cwPSFPropertyBox_Resize);
            BackColor.Equals(Color.White);
            updateControlCheckNodeEvents();
        }

        /// <summary>
        /// Updates the control check node events.
        /// </summary>
        protected virtual void updateControlCheckNodeEvents()
        {
            this.LostFocus += new EventHandler(cwPSFPropertyBox_CheckNode);
            this.TextChanged += new EventHandler(cwPSFPropertyBox_CheckNode);
        }

        /// <summary>
        /// Gets the tooltip text.
        /// </summary>
        /// <returns></returns>
        public string getTooltipText()
        {
            return errorToolTip.GetToolTip(getControl());
        }

        /// <summary>
        /// Gets the tooltip title.
        /// </summary>
        /// <returns></returns>
        public string getTooltipTitle()
        {
            return errorToolTip.ToolTipTitle;
        }

      
        /// <summary>
        /// Sets to error mode.
        /// </summary>
        protected virtual void setToErrorMode()
        {
            getControl().BackColor = cwTools.errorColor;
            BackColor = cwTools.errorColor;
        }

        /// <summary>
        /// Restores to correct mode.
        /// </summary>
        protected virtual void restoreToCorrectMode()
        {
            getControl().BackColor = Color.White;
            BackColor = Color.White;
        }


        /// <summary>
        /// Handles the LostFocus event of the cwPSFPropertyBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void cwPSFPropertyBox_CheckNode(object sender, EventArgs e)
        {
            checkFormat();
            if (ParentTreeNode != null)
            {
                ParentTreeNode.checkNodeStructureRec();
            }
        }

        private void cwPSFPropertyBox_Resize(object sender, EventArgs e)
        {
            removeErrorToolTip();
            checkFormat();
        }

        /// <summary>
        /// Sets the PSF parent tree node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void setPSFParentTreeNode(cwPSFTreeNode node)
        {
            parentPSFTreeNode = node;
        }

        /// <summary>
        /// Gets the parent tree node.
        /// </summary>
        public cwPSFTreeNode ParentTreeNode
        {
            get
            {
                return parentPSFTreeNode;
            }
        }


        /// <summary>
        /// Sets the value empty.
        /// </summary>
        public void setValueEmpty()
        {
            valueCanBeEmpty = true;
            checkFormat();
        }

        /// <summary>
        /// Sets the value not empty.
        /// </summary>
        public void setValueNotEmpty()
        {
            valueCanBeEmpty = false;
            checkFormat();
        }


        /// <summary>
        /// Removes the error tool tip.
        /// </summary>
        public void removeErrorToolTip()
        {
            restoreToCorrectMode();
            errorToolTip.hideTooltip(this);
        }
        /// <summary>
        /// Unsets the error text.
        /// </summary>
        public void unsetErrorInfo()
        {
            if (!BackColor.Equals(Color.White))
            {
                removeErrorToolTip();
                if (parentPSFTreeNode != null)
                {
                    parentPSFTreeNode.checkNodeStructureRec();
                }
            }
        }

        /// <summary>
        /// Sets the error info.
        /// </summary>
        /// <param name="text">The text.</param>
        protected void setErrorInfo(string text)
        {
            string errorTooltip = errorToolTip.GetToolTip(getControl()).ToString();
            if (Tag != null && Tag.Equals(errorTooltip))
            {
                return;
            }
            if (parentPSFTreeNode != null)
            {
                parentPSFTreeNode.setTooltipError(text);
            }
            setToErrorMode();
            errorToolTip.SetToolTip(getControl(), text);
        }

        /// <summary>
        /// Customs the check format.
        /// </summary>
        /// <returns></returns>
        protected virtual bool customCheckFormat()
        {
            return true;
        }

        /// <summary>
        /// Checks the format.
        /// </summary>
        /// <returns></returns>
        public virtual bool checkFormat()
        {                         
            if (!customCheckFormat())
            {
                return false;
            }

            if (!valueCanBeEmpty)
            {
                string _value = ToString();
                if (0.Equals(_value.Length))
                {
                    setErrorInfo(helpDescription + "\n" + Properties.Resources.PSF_PROPERTY_VALUE_CANT_BE_EMPTY);
                    return false;
                }
                unsetErrorInfo();
            }
            else
            {
                unsetErrorInfo();
            }
            return true;
        }

        /// <summary>
        /// Handles the Changed event of the cwPSFPropertyBoxFloat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void cwPSFProperyBox_Changed(object sender, EventArgs e)
        {
            checkFormat();
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public virtual void setValue(object _item)
        {
            Text = _item.ToString();
        }
        /// <summary>
        /// Gets the control.
        /// </summary>
        public virtual Control getControl()
        {
            return this;
        }

        /// <summary>
        /// Cleans the and lock.
        /// </summary>
        public virtual void cleanAndDisable()
        {
            Text = "";
            removeErrorToolTip();
            Enabled = false;
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        public virtual void enable()
        {
            Enabled = true;
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public virtual void disable()
        {
            Enabled = false;
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="_xmlWriter">The _XML writer.</param>
        internal virtual void saveToXML(System.Xml.XmlWriter _xmlWriter)
        {
            _xmlWriter.WriteAttributeString(keyName, ToString());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
