using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICharacter : SerializableCharacterBase
    {
        public SerializableAPICharacter()
        {
            this.Implants = new SerializableAPIImplantSet();
        }

        [XmlElement("attributeEnhancers")]
        public SerializableAPIImplantSet Implants
        {
            get;
            set;
        }

    }
}
