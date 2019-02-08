using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// acwPSFPropertyDataGridViewComboBoxCell
    /// </summary>
    public class cwPSFPropertyDataGridViewComboBoxCell : DataGridViewComboBoxCell
	{
        /// <summary>
        /// property
        /// </summary>
        public cwLightProperty property = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyDataGridViewComboBoxCell"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public cwPSFPropertyDataGridViewComboBoxCell(cwLightProperty property)
        {
            this.property = property;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return property.ToString();
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
                return property.ScriptName;
            }
        }
	}
}
