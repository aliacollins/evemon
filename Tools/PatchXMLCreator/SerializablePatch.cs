using System.Xml.Serialization;
using System.Collections.Generic;

namespace PatchXmlCreator
{
    [XmlRoot("evemon")]
    public sealed partial class SerializablePatch
    {
        internal SerializablePatch()
        {
            Release = new SerializableRelease();
            Datafiles = new List<SerializableDatafile>();
        }

        [XmlElement("newest")]
        public SerializableRelease Release
        {
            get;
            set;
        }

        [XmlArray("datafiles")]
        [XmlArrayItem("datafile")]
        public List<SerializableDatafile> Datafiles
        {
            get;
            set;
        }
    }
}
