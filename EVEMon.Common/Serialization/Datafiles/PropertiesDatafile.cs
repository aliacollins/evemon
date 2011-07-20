using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    [XmlRoot("propertiesDatafile")]
    public sealed class PropertiesDatafile
    {
        [XmlElement("category")]
        public SerializablePropertyCategory[] Categories
        {
            get;
            set;
        }
    }
}
