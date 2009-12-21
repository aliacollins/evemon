using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.XmlImporter.Zofu;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class InvGroup : IHasID
    {
        [XmlElement("groupID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("categoryID")]
        public int CategoryID
        {
            get;
            set;
        }

        [XmlElement("groupName")]
        public string Name;
    }
}
