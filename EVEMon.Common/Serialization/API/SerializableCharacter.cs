using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableCharacter : SerializableCharacterSheetBase
    {
        public SerializableCharacter()
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
