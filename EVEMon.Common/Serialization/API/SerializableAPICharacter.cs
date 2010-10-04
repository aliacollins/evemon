using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICharacter : SerializableCharacterBase
    {
        public SerializableAPICharacter()
        {
            Implants = new SerializableAPIImplantSet();
        }

        [XmlElement("attributeEnhancers")]
        public SerializableAPIImplantSet Implants
        {
            get;
            set;
        }
    }
}
