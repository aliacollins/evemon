using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.PatchXml
{
    [XmlRoot("evemon")]
    public sealed class SerializablePatch
    {
        private readonly Collection<SerializableDatafile> m_datafiles;
        private readonly Collection<SerializableDatafile> m_changedDatafiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializablePatch"/> class.
        /// </summary>
        public SerializablePatch()
        {
            Release = new SerializableRelease();
            m_datafiles = new Collection<SerializableDatafile>();
            m_changedDatafiles = new Collection<SerializableDatafile>();
        }

        /// <summary>
        /// Gets or sets the release.
        /// </summary>
        /// <value>
        /// The release.
        /// </value>
        [XmlElement("newest")]
        public SerializableRelease Release { get; set; }

        /// <summary>
        /// Gets the datafiles.
        /// </summary>
        /// <value>
        /// The datafiles.
        /// </value>
        [XmlArray("datafiles")]
        [XmlArrayItem("datafile")]
        public Collection<SerializableDatafile> Datafiles => m_datafiles;

        /// <summary>
        /// Gets the changed datafiles.
        /// </summary>
        /// <value>
        /// The changed datafiles.
        /// </value>
        [XmlIgnore]
        internal Collection<SerializableDatafile> ChangedDatafiles => m_changedDatafiles;

        /// <summary>
        /// Gets a value indicating whether files have changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if files have changed; otherwise, <c>false</c>.
        /// </value>
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