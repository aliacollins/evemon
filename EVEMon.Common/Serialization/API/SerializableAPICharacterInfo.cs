using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICharacterInfo
    {
        [XmlElement("shipName")]
        public string ShipName
        {
            get;
            set;
        }

        [XmlElement("shipTypeName")]
        public string ShipTypeName
        {
            get;
            set;
        }

        [XmlElement("lastKnownLocation")]
        public string LastKnownLocation
        {
            get;
            set;
        }

        [XmlElement("securityStatus")]
        public double SecurityStatus
        {
            get;
            set;
        }
    }
}
