using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;

namespace EVEMon.Common
{
    public class APIConfiguration
    {
        private string _name = string.Empty;
        private string _server = string.Empty;
        private List<APIMethod> _methods = new List<APIMethod>();

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public List<APIMethod> Methods
        {
            get { return _methods; }
            set { _methods = value; }
        }

        public bool IsDefault
        {
            get { return _name == APIConstants.DefaultConfigurationName; }
        }

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

        public override string ToString()
        {
            return _name;
        }
    }
}
