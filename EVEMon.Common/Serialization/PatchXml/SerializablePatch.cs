using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.PatchXml
{
    [XmlRoot("evemon")]
    public sealed class SerializablePatch
    {
        private readonly Collection<SerializableDatafile> m_datafiles;
        private readonly Collection<SerializableDatafile> m_changedDatafiles;

        public SerializablePatch()
        {
            Release = new SerializableRelease();
            m_datafiles = new Collection<SerializableDatafile>();
            m_changedDatafiles = new Collection<SerializableDatafile>();
        }

        [XmlElement("newest")]
        public SerializableRelease Release { get; set; }

        [XmlArray("datafiles")]
        [XmlArrayItem("datafile")]
        public Collection<SerializableDatafile> Datafiles
        {
            get { return m_datafiles; }
        }

        [XmlIgnore]
        internal Collection<SerializableDatafile> ChangedDatafiles
        {
            get { return m_changedDatafiles; }
        }

        [XmlIgnore]
        internal bool FilesHaveChanged
        {
            get
            {
                m_changedDatafiles.Clear();

                foreach (Datafile datafile in EveMonClient.Datafiles)
                {
                    foreach (SerializableDatafile dfv in Datafiles.Where(dfv => dfv.Name == datafile.Filename))
                    {
                        if (datafile.MD5Sum != dfv.MD5Sum)
                            m_changedDatafiles.Add(dfv);

                        break;
                    }
                }

                return ChangedDatafiles.Count > 0;
            }
        }
    }
}