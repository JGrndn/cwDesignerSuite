using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.Exceptions
{
    /// <summary>
    /// An option is missing in the configuration file
    /// </summary>
    internal class cwExceptionMissingOptionInConfigurationFile: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwExceptionMissingOptionInConfigurationFile"/> class.
        /// </summary>
        /// <param name="optionMissing">The option missing.</param>
        /// <param name="configurationFile">The configuration file.</param>
        public cwExceptionMissingOptionInConfigurationFile(string optionMissing, string configurationFile)
            : base("<option:" + optionMissing + @"> is required in File : " + configurationFile)
        {
        }
    }
}
