using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to manage an Integer
    /// </summary>
    public class cwPSFPropertyBoxInt : cwPSFPropertyBox
    {
        /// <summary>
        /// The Integer value
        /// </summary>
        public int update_value = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxInt"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="_defaultValue">The _default value.</param>
        public cwPSFPropertyBoxInt(String _helpName, String _helpDescription, String _keyName, int _defaultValue)
            : base(_helpName, _helpDescription, _keyName)
        {
            update_value = _defaultValue;
            Text = update_value.ToString();
            this.TextChanged += new EventHandler(cwPSFProperyBoxInt_Changed);
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Text.ToString();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public int getValue()
        {
            return update_value;
        }

        /// <summary>
        /// Handles the Changed event of the cwPSFPropertyBoxFloat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void cwPSFProperyBoxInt_Changed(object sender, EventArgs e)
        {
            int new_value = 0;
            if (!int.TryParse(Text, out new_value))
            {
                this.BackColor = Color.OrangeRed;
                errorToolTip.SetToolTip(this, "Please provide an int value");                
            }
            else
            {
                this.BackColor = Color.White;
                errorToolTip.RemoveAll();                
                update_value = new_value;
            }
        }
    }
}
