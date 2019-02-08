using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Represents a string
    /// </summary>
    public class cwPSFPropertyBoxString : cwPSFPropertyBox
    {
   

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
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxString"/> class.
        /// </summary>
        public cwPSFPropertyBoxString(String _helpName, String _helpDescription, String _keyName)
            : base(_helpName, _helpDescription, _keyName)
        {           
        }


        /// <summary>
        /// Customs the check format.
        /// </summary>
        /// <returns></returns>
        protected override bool customCheckFormat()
        {
            if (Text.Length > 255)
            {
                setErrorInfo(Properties.Resources.PSF_TN_LABEL_SHOULD_HAVE_LESSTHAN_255_CHAR);
                return false;
            }
            return true;
        }

    }
}
