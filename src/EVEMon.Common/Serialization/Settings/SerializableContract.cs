using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableContract
    {
        [XmlAttribute("contractID")]
        public long ContractID { get; set; }

        [XmlAttribute("contractState")]
        public ContractState ContractState { get; set; }

        [XmlAttribute("issuedFor")]
        public IssuedFor IssuedFor { get; set; }
    }
}
