using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.API;
using System.Globalization;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// allow to manage a float
    /// </summary>
    public class cwPSFPropertyBoxFloat : cwPSFPropertyBox
    {
        /// <summary>
        /// the float value
        /// </summary>
        public float update_value = 0;


        /// <summary>
        /// Gets the diagram box value.
        /// </summary>
        /// <returns></returns>
        public int getDiagramBoxValue()
        {
            return Convert.ToInt32(update_value * cwLightDiagram.one_pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxFloat"/> class.
        /// </summary>
        public cwPSFPropertyBoxFloat(String _helpName, String _helpDescription, String _keyName)
            : base(_helpName, _helpDescription, _keyName)
        {
            initcwPSFProperyBoxFloat();
        }

        /// <summary>
        /// Initcws the designer text box float.
        /// </summary>
        private void initcwPSFProperyBoxFloat()
        {
            if ("".Equals(Text))
            {
                Text = "0";
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
            return Text;
        }

        /// <summary>
        /// Checks the format.
        /// </summary>
        public override bool checkFormat()
        {
            float new_value = 0;
            if (!float.TryParse(Text, NumberStyles.Float, CultureInfo.InvariantCulture, out new_value))
            {
                setErrorInfo(Properties.Resources.PSF_VN_FLOAT_VALUE);
                return false;
            }
            else
            {
                unsetErrorInfo();
                update_value = new_value;
            }
            return true;
        }
    }
}
