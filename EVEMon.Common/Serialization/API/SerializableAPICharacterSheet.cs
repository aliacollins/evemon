using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICharacterSheet : SerializableCharacterSheetBase
    {
        public SerializableAPICharacterSheet()
        {
            Implants = new SerializableImplantSet();
        }

        [XmlElement("attributeEnhancers")]
        public SerializableImplantSet Implants
        {
            get;
            set;
        }
    }
}
