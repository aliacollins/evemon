using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.CloudStorage
{
    public sealed class SerializableBCAPICredentials
    {
        [XmlElement("userID")]
        public uint UserID { get; set; }
    }
}