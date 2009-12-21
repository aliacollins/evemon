using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class EveUnit : IHasID
    {
        [XmlElement("unitID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("unitName")]
        public string Name;

        [XmlElement("displayName")]
        public string DisplayName;

        [XmlElement("description")]
        public string Description;

    }
}
