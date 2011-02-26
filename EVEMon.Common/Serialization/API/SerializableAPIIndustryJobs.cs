using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIIndustryJobs
    {
        public SerializableAPIIndustryJobs()
        {
            this.Jobs = new List<SerializableJobListItem>();
        }

        [XmlArray("jobs")]
        [XmlArrayItem("job")]
        public List<SerializableJobListItem> Jobs
        {
            get;
            set;
        }
    }
}
