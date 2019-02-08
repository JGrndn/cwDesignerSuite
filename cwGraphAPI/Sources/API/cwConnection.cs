using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using CasewiseDataTier2004;
using Casewise.GraphAPI.Exceptions;
using log4net;
using Casewise.GraphAPI.Logon;
using System.Windows.Forms;

namespace Casewise.GraphAPI.API
{

    ///// <summary>
    ///// 
    ///// </summary>
    //public sealed class LicensingPolicy
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="LicensingPolicy"/> class.
    //    /// </summary>
    //    /// <param name="productFeature">The product feature.</param>
    //    public LicensingPolicy(int productFeature)
    //    { 

    //    }
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="LicensingPolicy"/> class.
    //    /// </summary>
    //    /// <param name="productFeature">The product feature.</param>
    //    /// <param name="networkFeature">The network feature.</param>
    //    public LicensingPolicy(int productFeature, int networkFeature)
    //    { 
    //    }

    //    /// <summary>
    //    /// Adds the specified product feature.
    //    /// </summary>
    //    /// <param name="productFeature">The product feature.</param>
    //    public void Add(int productFeature)
    //    { }
    //    /// <summary>
    //    /// Adds the specified product feature.
    //    /// </summary>
    //    /// <param name="productFeature">The product feature.</param>
    //    /// <param name="networkFeature">The network feature.</param>
    //    public void Add(int productFeature, int networkFeature)
    //    { }
    //    /// <summary>
    //    /// Adds the specified product feature.
    //    /// </summary>
    //    /// <param name="productFeature">The product feature.</param>
    //    /// <param name="networkFeature">The network feature.</param>
    //    /// <param name="logonFeature">The logon feature.</param>
    //    public void Add(int productFeature, int networkFeature, int logonFeature)
    //    { }
    //}



    /// <summary>
    /// This class represents a connection to the Corporate Exchange referential. Once a connection is initiated, access to model and other objects is available.
    /// </summary>
    public class cwConnection
    {
        /// <summary>
        /// diagramDesignerLicenceID
        /// </summary>
        public static int diagramDesignerLicenceID = 300;

        /// <summary>
        /// L_CM
        /// </summary>
        public static int L_CM = 4;
        /// <summary>
        /// L_MDB
        /// </summary>
        public static int L_MDB = 5;
        private static readonly ILog log = LogManager.GetLogger(typeof(cwConnection));
        private ICMConnectionList ICMConnectionList = null;

        /// <summary>
        /// Indicates the state of the <see cref="cwConnection"/>.
        /// </summary>
        public enum cwConnectionStatus
        {
            /// <summary>
            /// Not yet loged
            /// </summary>
            LOGON_NOTDONE,
            /// <summary>
            /// Login success
            /// </summary>
            LOGON_SUCCESS,
            /// <summary>
            /// Login failed
            /// </summary>
            LOGON_FAILED
        };

        private cwConnectionStatus ConnectionStatus = cwConnectionStatus.LOGON_NOTDONE;
        private Dictionary<string, cwLightModel> Models = new Dictionary<string, cwLightModel>();

        #region Login
        /// <summary>
        /// Gets the logon status.
        /// </summary>
        public cwConnectionStatus getLogonStatus()
        {
            return ConnectionStatus;
        }

        /// <summary>
        /// Determines whether this instance is connected.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool isConnected()
        {
            if (ICMConnectionList == null) return false;
            return ConnectionStatus == cwConnectionStatus.LOGON_SUCCESS;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwConnection"/> class.
        /// </summary>
        /// <param name="_user">The user name.</param>
        /// <param name="_password">The password.</param>
        public cwConnection(string _user, string _password)
        {
            createConnection(cwConnectionType.credentials, _user, _password, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwConnection"/> class.
        /// </summary>
        public cwConnection()
        {
            createConnection(cwConnectionType.popup, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwConnection"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public cwConnection(cwLightModel model)
        {
            createConnection(cwConnectionType.from_model, null, null, model);
        }

        private void createConnectionWithCredentials(string _user, string _password)
        {
            //cwLogonPolicy logonPolicy = new cwLogonPolicy(ICM_LOGON_AUTHENTICATION_TYPE.lat_CorporateModelerUser, _user, _password, null);
            //LicensingPolicy lp = new LicensingPolicy(diagramDesignerLicenceID);
            //lp.Add(L_CM);
            //lp.Add(L_MDB);
            //ICMConnectionList = ICMConnectionList.CreateNewConnectionListFromPolicyObjects(cwTools.casewiseDefaultConnection, logonPolicy, lp);
            ICMConnectionList = ICMConnectionList.CreateNewConnectionListUsingCMAuthentication(_user, _password);
        }

        private void createConnectionFromModel(cwLightModel model)
        {
            if (model == null)
            {
                throw new ICMLogonCancelledException("Connection from a model needs a valid model");
            }
            ICMConnectionList = ICMConnectionList.CreateNewConnectionListFromICMLogon(model.getConnection().ConnectionList.LogonContext);
        }

        private void createConnectionUsingPopup()
        {
            ICMConnectionList = ICMConnectionList.CreateNewConnectionList();
            //if (ICMConnectionList.RequestLicense(diagramDesignerLicenceID))
            //{
            //}
            //else
            //{
            //    cwProductRegistration registerRequired = new cwProductRegistration();
            //    //registerRequired.NoLicenseForFeatureDialog(ICMConnectionList.LogonContext, 0, diagramDesignerLicenceID);
            //    MessageBox.Show(Properties.Resources.TEXT_NO_LICENCE_AVAILABLE, Properties.Resources.TEXT_WARNING);
            //    IntPtr eventHandle = Casewise.Services.Utils.GetHandleToCorporateModelerEvent();
            //    //  Signal the event is case other processes waiting
            //    Casewise.Win32.Functions.SetEvent(eventHandle);

            //    throw new ICMLogonCancelledException(Properties.Resources.TEXT_NO_LICENCE_AVAILABLE);
            //}

        }

        private enum cwConnectionType
        {
            popup,
            credentials,
            from_model
        }


        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="_user">The _user.</param>
        /// <param name="_password">The _password.</param>
        /// <param name="model">The model.</param>
        private void createConnection(cwConnectionType connectionType, string _user, string _password, cwLightModel model)
        {
            try
            {
                switch (connectionType)
                {
                    case cwConnectionType.popup:
                        createConnectionUsingPopup();
                        break;
                    case cwConnectionType.credentials:
                        createConnectionWithCredentials(_user, _password);
                        break;
                    case cwConnectionType.from_model:
                        createConnectionFromModel(model);
                        break;
                }

                ConnectionStatus = cwConnectionStatus.LOGON_SUCCESS;
            }
            catch (ICMLogonCancelledException)
            {
                ConnectionStatus = cwConnectionStatus.LOGON_FAILED;
                log.Info("Connection has been canceled from popup");
            }
            catch (ICMException ex)
            {
                if (ex.Code == ICMException.Reason.LOGON_FAILED)
                {
                    ConnectionStatus = cwConnectionStatus.LOGON_FAILED;
                }
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (ICMConnectionList == null)
            {
                throw new cwExceptionFatal("Connection should be setup before closing it");
            }
            foreach (ICMConnection _connection in ICMConnectionList.Connections)
            {
                _connection.Close();
                _connection.Dispose();
            }
            //ICMConnectionList.ReleaseLicense(diagramDesignerLicenceID);
            ICMConnectionList.Dispose();
            ICMConnectionList = null;
        }
        #endregion

        #region Models


        /// <summary>
        /// Loads the models.
        /// </summary>
        public void loadModels()
        {
            if (ICMConnectionList != null)
            {
                foreach (ICMConnection _connection in ICMConnectionList.Connections)
                {
                    Models[_connection.FileName] = new cwLightModel(_connection);
                }
            }
        }


        /// <summary>
        /// Loads the and get model.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public cwLightModel loadAndGetModel(string fileName)
        {
            if (ICMConnectionList != null)
            {
                foreach (ICMConnection _connection in ICMConnectionList.Connections)
                {
                    if (fileName.Equals(_connection.FileName))
                    {
                        Models[_connection.FileName] = new cwLightModel(_connection);
                        return Models[_connection.FileName];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public cwLightModel getModel(string fileName)
        {
            if (Models.ContainsKey(fileName))
            {
                return Models[fileName];
            }
            loadModels();
            throw new cwExceptionFatal(String.Format("Model filename is not valid [{0}].", fileName));
        }

        /// <summary>
        /// Gets the models.
        /// </summary>
        /// <returns></returns>
        public List<cwLightModel> getModels()
        {
            return Models.Values.ToList<cwLightModel>();
        }

        #endregion

    }
}
