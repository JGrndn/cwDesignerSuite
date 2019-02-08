using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Configuration;
using Casewise.GraphAPI.ProgramManager;
using Microsoft.Win32;

namespace Casewise.GraphAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationCultureLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCultureLoader"/> class.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        public ApplicationCultureLoader(string assemblyLocation)
        {
            string casewiseLocalLanguange = (string)Registry.GetValue(cwProgramManager.CM_REGISTERY_KEY + "UserDetails", "Locale", @"EN");
            Dictionary<string,string> activeLanguages = new  Dictionary<string,string>();
            activeLanguages["EN"] = "en-GB";
            activeLanguages["FR"] = "fr-FR";
            if (activeLanguages.ContainsKey(casewiseLocalLanguange))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(activeLanguages[casewiseLocalLanguange]);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(activeLanguages[casewiseLocalLanguange]);
            }
            loadCultureFromAppConfig(assemblyLocation);
            
        }

        /// <summary>
        /// Loads the culture from app config.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        public void loadCultureFromAppConfig(string assemblyLocation)
        {
            KeyValueConfigurationCollection appSettingSection = ApplicationConfigurationLoader.getAppSettingConfiguration(assemblyLocation);
            if (appSettingSection["culture"] != null && appSettingSection["culture"].Value != null)
            {
                string cultureName = appSettingSection["culture"].Value.ToString();
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
            }

        }
    }
}
