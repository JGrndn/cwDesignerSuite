using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Check box property
    /// </summary>
    public class cwPSFPropertyBoxCheckBox : cwPSFPropertyBox
    {

        private CheckBox checkBox = new CheckBox();
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxCheckBox"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_defaultValue">if set to <c>true</c> [_default value].</param>
        public cwPSFPropertyBoxCheckBox(String _helpName, String _helpDescription, String _keyName, bool _defaultValue)
            : base (_helpName, _helpDescription, _keyName)
        {
            if (_defaultValue == true)
            {
                checkBox.Checked = true;
            }
            else
            {
                checkBox.Checked = false;
            }

            checkBoxChanged(new EventHandler(cwPSFCheckBox_CheckedChanged));
            

        }

        /// <summary>
        /// Updates the control check node events.
        /// </summary>
        protected override void updateControlCheckNodeEvents()
        {
            checkBox.LostFocus += new EventHandler(cwPSFPropertyBox_CheckNode);
            checkBox.CheckedChanged += new EventHandler(cwPSFPropertyBox_CheckNode);
        }


        /// <summary>
        /// Selecteds the index changed.
        /// </summary>
        /// <param name="e">The e.</param>
        public void checkBoxChanged(EventHandler e)
        {
            this.checkBox.CheckedChanged += e;
        }


        /// <summary>
        /// Gets a value indicating whether this <see cref="cwPSFPropertyBoxCheckBox"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        public bool Checked
        {
            get 
            {
                return checkBox.Checked;
            }
            set {
                checkBox.Checked = value;
            }
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public override void setValue(object _item)
        {
            bool boolItem = false;
            if ("".Equals(_item.ToString()))
            {
                _item = false;
            }
            if (false.Equals(Boolean.TryParse(_item.ToString(), out boolItem)))
            {
                throw new cwExceptionWarning("Provided value [" + _item.ToString() + "] should be a boolean, for [" + helpName + "]");
            }
            checkBox.Checked = boolItem;
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            return checkBox;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cwPSFCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void cwPSFCheckBox_CheckedChanged(object sender, EventArgs e)
        { 
            Text = ToString();
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        public override void disable()
        {
            checkBox.Enabled = false;
        }

        /// <summary>
        /// Cleans the and lock.
        /// </summary>
        public override void cleanAndDisable()
        {
            checkBox.Checked = false;
            disable();
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        public override void enable()
        {
            checkBox.Enabled = true;
        }
        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return checkBox.Checked.ToString().ToLower();
        }

        ///// <summary>
        ///// Loads from configuration file.
        ///// </summary>
        ///// <param name="optionName">Name of the option.</param>
        ///// <param name="config">The config.</param>
        ///// <param name="guiHelper">The GUI helper.</param>
        //public void loadFromConfigurationFile(string optionName, cwConfiguration config, cwGUIHelper guiHelper)
        //{
        //    if (config == null) throw new cwExceptionFatal("Configuration is required while loading a property");
        //    string _value = config.getOptionValue(optionName);
        //    if (_value == null) throw new cwExceptionFatal("The following property should be set [" + optionName + "] in .exe.config file");
            
        //    bool loadedCheckedValue = false;
        //    if (!bool.TryParse(_value, out loadedCheckedValue))
        //    {
        //        throw new cwExceptionFatal("The following property [" + optionName + "] should be a boolean (true/false) in .exe.config file");
        //    }

        //    checkBox.Checked = loadedCheckedValue;
        //    this.checkBox.Text = _value;
        //}
    
    }
}
