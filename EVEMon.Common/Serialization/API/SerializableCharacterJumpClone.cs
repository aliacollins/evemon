using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableCharacterJumpClone
    {
        [XmlAttribute("jumpCloneID")]
        public long JumpCloneID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [XmlAttribute("cloneName")]
        public string CloneName { get; set; }
    }
}