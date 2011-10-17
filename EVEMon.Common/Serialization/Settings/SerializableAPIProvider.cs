using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
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
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlArray("methods")]
        [XmlArrayItem("method")]
        public List<SerializableAPIMethod> Methods { get; set; }
    }
}