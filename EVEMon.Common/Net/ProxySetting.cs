using System;
using System.Xml.Serialization;
using EVEMon.Common.Net;

namespace EVEMon.Common.Net
{
    [XmlRoot("proxySetting")]
    public class ProxySetting : ICloneable
    {
        private string _host = String.Empty;
        private int _port;
        private ProxyAuthType _authType = ProxyAuthType.None;
        private string _username = String.Empty;
        private string _password = String.Empty;

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public ProxyAuthType AuthType
        {
            get { return _authType; }
            set { _authType = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }


        #region ICloneable Members

        public ProxySetting Clone()
        {
            return (ProxySetting)((ICloneable)this).Clone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}