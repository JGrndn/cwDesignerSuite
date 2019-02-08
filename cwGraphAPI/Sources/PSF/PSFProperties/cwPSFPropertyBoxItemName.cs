using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Represents a string
    /// </summary>
    public class cwPSFPropertyBoxItemName : cwPSFPropertyBoxString
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

        private cwLightObjectType mainObjectType = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxString"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        /// <param name="mainOT">The main OT.</param>
        public cwPSFPropertyBoxItemName(String _helpName, String _helpDescription, String _keyName, cwLightObjectType mainOT)
            : base(_helpName, _helpDescription, _keyName)
        {
            mainObjectType = mainOT;
        }


        /// <summary>
        /// Customs the check format.
        /// </summary>
        /// <returns></returns>
        protected override bool customCheckFormat()
        {
            if (mainObjectType.objectExistsByName(Text))
            {
                setErrorInfo(Properties.Resources.FORM_ADDITEM_ITEM_ALEADY_EXISTS);
                return false;
            }
            return base.customCheckFormat();
        }

    }
}
