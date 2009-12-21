using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class CrtRecommendations : IHasID
    {
        [XmlElement("recommendationID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("shipTypeID")]
        public int ShipTypeID;

        [XmlElement("certificateID")]
        public int CertificateID;

        [XmlElement("recommendationLevel")]
        public int Level;

    }
}
