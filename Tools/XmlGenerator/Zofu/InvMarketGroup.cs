using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class InvMarketGroup : IHasID
    {
        [XmlElement("marketGroupID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("marketGroupName")]
        public string Name;

        [XmlElement("parentGroupID")]
        public Nullable<int> ParentID;

        [XmlElement("graphicID")]
        public Nullable<int> GraphicID;

        [XmlElement("description")]
        public string Description;

    }
}
