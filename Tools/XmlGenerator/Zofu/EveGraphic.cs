using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class EveGraphic : IHasID
    {
        [XmlElement("graphicID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("icon")]
        public string Icon;
    }
}
