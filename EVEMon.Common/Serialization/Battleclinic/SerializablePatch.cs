using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    [XmlRoot("evemon")]
    public sealed class SerializablePatch
    {
        public SerializablePatch()
        {
            Release = new SerializableRelease();
            Datafiles = new List<SerializableDatafile>();
            ChangedDataFiles = new List<SerializableDatafile>();
        }

        [XmlElement("newest")]
        public SerializableRelease Release { get; set; }

        [XmlArray("datafiles")]
        [XmlArrayItem("datafile")]
        public List<SerializableDatafile> Datafiles { get; set; }

        [XmlIgnore]
        internal List<SerializableDatafile> ChangedDataFiles { get; private set; }

        [XmlIgnore]
        internal bool FilesHaveChanged
        {
            get
            {
                ChangedDataFiles.Clear();

                foreach (SerializableDatafile dfv in Datafiles)
                {
                    foreach (Datafile datafile in EveMonClient.Datafiles.Where(datafile => datafile.Filename == dfv.Name))
                    {
                        if (datafile.MD5Sum != dfv.MD5Sum)
                            ChangedDataFiles.Add(dfv);

                        break;
                    }
                }

                return ChangedDataFiles.Count > 0;
            }
        }
    }
}