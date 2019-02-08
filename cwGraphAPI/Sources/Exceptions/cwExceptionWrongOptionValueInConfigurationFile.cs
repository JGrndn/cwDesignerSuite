using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// An option is wrong in the configuration file
    /// </summary>
    public class cwExceptionWrongOptionValueInConfigurationFile : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionWrongOptionValueInConfigurationFile"/> class.
        /// </summary>
        /// <param name="optionMissing">The option missing.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <param name="configurationFile">The configuration file.</param>
        public cwExceptionWrongOptionValueInConfigurationFile(string optionMissing, string providedValue, string configurationFile)
            : base(@"<option:" + optionMissing + @"> has a wrong value for the choosed model. Loaded value is <" + providedValue + @">
In File : " + configurationFile)
        {
        }
    }
}
