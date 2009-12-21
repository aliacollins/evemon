using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.XmlImporter.Zofu;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class CrtCertificates : IHasID
    {
        [XmlElement("certificateID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("categoryID")]
        public int CategoryID;

        [XmlElement("classID")]
        public int ClassID;

        [XmlElement("grade")]
        public int Grade;

        [XmlElement("description")]
        public string Description;

    }
}
