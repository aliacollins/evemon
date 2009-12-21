using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class CrtClasses : IHasID
    {
        [XmlElement("classID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description;

        [XmlElement("className")]
        public string ClassName;

    }
}
