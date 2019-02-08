using System;
using System.Collections.Generic;
using System.Text;
using CasewiseDataTier2004;

namespace Casewise.GraphAPI.Logon
{
    internal class cwLogonPolicy : IcwLogonPolicy
    {
        ICM_LOGON_AUTHENTICATION_TYPE _type;
        string _user;
        string _password;
        string _domain;

        public cwLogonPolicy(ICM_LOGON_AUTHENTICATION_TYPE eType, string sUserName, string sPassword, string sDomainName)
        {
            _type = eType;
            _user = sUserName;
            _password = sPassword;
            _domain = sDomainName;
        }

        #region IcwLogonPolicy Members

        public ICM_LOGON_AUTHENTICATION_TYPE AuthenticationType
        {
            get { return _type; }
        }

        public string DomainName
        {
            get { return _domain; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string UserName
        {
            get { return _user; }
        }

        #endregion
    }
}