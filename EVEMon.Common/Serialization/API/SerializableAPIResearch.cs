using System.Xml.Serialization;
using System.Collections.Generic;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIResearchList
    {
        public SerializableAPIResearchList()
        {
            this.ResearchPoints = new List<SerializableResearchListItem>();
        }

        [XmlArray("research")]
        [XmlArrayItem("researc")]
        public List<SerializableResearchListItem> ResearchPoints
        {
            get;
            set;
        }
    }
}
