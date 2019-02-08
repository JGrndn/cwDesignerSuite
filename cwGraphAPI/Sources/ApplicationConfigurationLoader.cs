using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using log4net;
using System.IO;
using System.Reflection;

namespace Casewise.GraphAPI
{
    /// <summary>
    /// Load the App.Config
    /// </summary>
    public class ApplicationConfigurationLoader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationConfigurationLoader));
        /// <summary>
        /// Container for the app.config options
        /// </summary>
        public KeyValueConfigurationCollection appSettingSection = null;

        /// <summary>
        /// Store the login informations
        /// </summary>
        public struct cwPSFLoginInfo
        {
            /// <summary>
            /// the name of the user
            /// </summary>
            public string name;
            /// <summary>
            /// the password of the user
            /// </summary>
            public string password;
            /// <summary>
            /// the domain if required
            /// </summary>
            public string domaine;
            /// <summary>
            /// the database connection name
            /// </summary>
            public string database;
            /// <summary>
            /// the default model filename
            /// </summary>
            public string modelFileName;
        }


        /// <summary>
        /// The defaut item ID to load
        /// </summary>
        public int itemID = 0;

        /// <summary>
        /// Stores the login informations
        /// </summary>
        public cwPSFLoginInfo loginInfo = new cwPSFLoginInfo();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigurationLoader"/> class.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        public ApplicationConfigurationLoader(string assemblyLocation)
        {
            try
            {
                appSettingSection = getAppSettingConfiguration(assemblyLocation);
                loadAppConfigInformation();

            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

        }

        /// <summary>
        /// Gets the app setting configuration.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <returns></returns>
        public static KeyValueConfigurationCollection getAppSettingConfiguration(string assemblyLocation)
        {
            string exeFileName = assemblyLocation + ".config";
            if (!File.Exists(exeFileName))
            {
                throw new ApplicationException(String.Format(Properties.Resources.ERROR_CONFIGURATION_FILE_NOT_FOUND, exeFileName));
            }
            Configuration currentApplicationConfiguration = ConfigurationManager.OpenExeConfiguration(assemblyLocation);
            KeyValueConfigurationCollection appSettingSection = currentApplicationConfiguration.AppSettings.Settings;
            return appSettingSection;
        }



        /// <summary>
        /// Determines whether [has default credentials].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has default credentials]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasDefaultCredentials()
        {
            return loginInfo.name != null && loginInfo.password != null;
        }

        /// <summary>
        /// Determines whether [has default model].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has default model]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasDefaultModel()
        {
            return loginInfo.modelFileName != null;
        }

        /// <summary>
        /// Determines whether [has default domain].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has default domain]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasDefaultDomain()
        {
            return loginInfo.domaine != null;
        }

        /// <summary>
        /// Determines whether [has default database].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has default database]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasDefaultDatabase()
        {
            return loginInfo.database != null;
        }

        /// <summary>
        /// Loads the app config information.
        /// </summary>
        private void loadAppConfigInformation()
        {
            if (appSettingSection["Database"] != null)
            {
                loginInfo.database = appSettingSection["Database"].Value.ToString();
            }
            if (appSettingSection["Credentials"] != null)
            {
                string connection_credentials = appSettingSection["Credentials"].Value.ToString();
                if (connection_credentials != null)
                {
                    string user = connection_credentials.Split(':')[0];
                    string password = connection_credentials.Split(':')[1];
                    loginInfo.name = user;
                    loginInfo.password = password;
                }
            }
            if (appSettingSection["Model_Filename"] != null)
            {
                loginInfo.modelFileName = appSettingSection["Model_Filename"].Value.ToString();
            }
            if (appSettingSection["Domain_Name"] != null)
            {
                loginInfo.domaine = appSettingSection["Domain_Name"].Value.ToString();
            }
            if (appSettingSection["Item_ID"] != null)
            {
                itemID = Convert.ToInt32(appSettingSection["Item_ID"].Value.ToString());
            }

            log.Debug("LOADING-PSF : " + "Configuration file found & loaded");
        }

    }
}
