using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIJobList
    {
        public SerializableAPIJobList()
        {
            this.Jobs = new List<SerializableAPIJob>();
        }

        [XmlArray("jobs")]
        [XmlArrayItem("job")]
        public List<SerializableAPIJob> Jobs
        {
            get;
            set;
        }
    }
}
