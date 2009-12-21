using EVEMon.Common.Net;

namespace EVEMon.Common
{
    /// <summary>
    /// Container class to provide access to commmon services instead of using singletons
    /// The use of Settings.GetInstance will eventually be replaced :)
    /// </summary>
    public class CommonContext
    {
        private static readonly object _syncLock = new object();
        private static HttpWebService _httpWebServiceInstance = null;

        public static Settings Settings
        {
            get { return Common.Settings.GetInstance(); }
        }

        public static HttpWebService HttpWebService
        {
            get
            {
                lock (_syncLock)
                {
                    if (_httpWebServiceInstance == null)
                    {
                        _httpWebServiceInstance = new HttpWebService();
                    }
                }
                return _httpWebServiceInstance;
            }
        }
    }
}
