using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a certificate
    /// </summary>
    public sealed class SerializableCharacterCertificate
    {
        [XmlAttribute("certificateID")]
        public int CertificateID
        {
            get;
            set;
        }
    }
}
