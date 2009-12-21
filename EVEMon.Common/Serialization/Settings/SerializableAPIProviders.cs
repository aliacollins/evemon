using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a serializable version of the set of API providers. Used for settings persistence.
    /// </summary>
    public sealed class SerializableAPIProviders
    {
        public SerializableAPIProviders()
        {
            CustomProviders = new List<SerializableAPIProvider>();
        }

        [XmlElement("currentProvider")]
        public string CurrentProviderName
        {
            get;
            set;
        }


        [XmlArray("customProviders")]
        [XmlArrayItem("provider")]
        public List<SerializableAPIProvider> CustomProviders
        {
            get;
            set;
        }

        public override string ToString()
        {
            return CurrentProviderName;
        }
    }


    /// <summary>
    /// Represents a serializable version of an API provider. Used for settings persistence.
    /// </summary>
    public sealed class SerializableAPIProvider
    {
        public SerializableAPIProvider()
        {
            Name = "New provider";
            Url = NetworkConstants.APIBase;
            Methods = new List<SerializableAPIMethod>();
            foreach (string methodName in Enum.GetNames(typeof(APIMethods)))
            {
                APIMethods methodEnum = (APIMethods)Enum.Parse(typeof(APIMethods), methodName);
                string methodURL = NetworkConstants.ResourceManager.GetString(methodName);
                Methods.Add(new SerializableAPIMethod { Method = methodEnum, Path = methodURL });
            }
        }

        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("url")]
        public string Url
        {
            get;
            set;
        }

        [XmlArray("methods")]
        [XmlArrayItem("method")]
        public List<SerializableAPIMethod> Methods
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Represents a serializable version of an API method. Used for settings persistence.
    /// </summary>
    public sealed class SerializableAPIMethod
    {
        [XmlAttribute("code")]
        public APIMethods Method
        {
            get;
            set;
        }

        [XmlAttribute("path")]
        public string Path
        {
            get;
            set;
        }
    }

}
