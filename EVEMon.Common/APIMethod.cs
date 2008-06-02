using System.Xml.Serialization;

namespace EVEMon.Common
{
    /// <summary>
    /// Serializable class for an API method and its path. Each APIConfiguration maintains a list of APIMethods.
    /// </summary>
    public class APIMethod
    {
        private APIMethods _method;
        private string _path;

        public APIMethod()
        {}

        public APIMethod(APIMethods method, string path)
        {
            _method = method;
            _path = path;
        }

        /// <summary>
        /// Returns the APIMethods enumeration member for this APIMethod.
        /// </summary>
        [XmlAttribute]
        public APIMethods Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /// <summary>
        /// Returns the defined URL path for this APIMethod.
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
