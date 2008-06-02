using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    /// <summary>
    /// Serializable class providing connection details for an API server and its methods.
    /// </summary>
    public class APIConfiguration
    {
        private string _name = string.Empty;
        private string _server = string.Empty;
        private List<APIMethod> _methods = new List<APIMethod>();

        /// <summary>
        /// Returns the name of this APIConfiguration.
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Returns the server host for this APIConfiguration.
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        /// <summary>
        /// Returns a list of APIMethods supported by this APIConfiguration.
        /// </summary>
        public List<APIMethod> Methods
        {
            get { return _methods; }
            set { _methods = value; }
        }

        /// <summary>
        /// Returns true if this APIConfiguration represents the default API service.
        /// </summary>
        public bool IsDefault
        {
            get { return _name == APIConstants.DefaultConfigurationName; }
        }

        /// <summary>
        /// Returns the full canonical URL for the specified APIMethod as constructed from the Server and APIMethod properties.
        /// </summary>
        /// <param name="requestMethod">An APIMethods enumeration member specfying the method for which the URL is required.</param>
        /// <returns>A String representing the full URL path of the specified method.</returns>
        public string MethodUrl(APIMethods requestMethod)
        {
            Uri methodUri = new Uri(_server);
            foreach (APIMethod method in _methods)
            {
                if (method.Method == requestMethod)
                {
                    methodUri = new Uri(methodUri, method.Path);
                }
            }
            return methodUri.AbsoluteUri;
        }

        /// <summary>
        /// Returns the configuration name as a String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Static utility property to return an APIConfiguration configured using the default API server and methods.
        /// </summary>
        public static APIConfiguration DefaultConfiguration
        {
            get
            {
                APIConfiguration configuration = new APIConfiguration();
                configuration.Name = APIConstants.DefaultConfigurationName;
                configuration.Server = APIConstants.Server;
                configuration.Methods = DefaultMethods;
                return configuration;
            }
        }

        /// <summary>
        /// Static utility property to return a List of the default API methods.
        /// </summary>
        public static List<APIMethod> DefaultMethods
        {
            get
            {
                List<APIMethod> methods = new List<APIMethod>();
                foreach (string methodName in Enum.GetNames(typeof(APIMethods)))
                {
                    APIMethods method = (APIMethods)Enum.Parse(typeof (APIMethods), methodName);
                    string methodPath = APIConstants.ResourceManager.GetString(methodName);
                    methods.Add(new APIMethod(method, methodPath));
                }
                return methods;
            }
        }
    }
}
