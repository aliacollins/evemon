using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    public sealed class SerializableBCAPIFiles
    {
        private readonly Collection<SerializableFilesListItem> m_files;

        public SerializableBCAPIFiles()
        {
            m_files = new Collection<SerializableFilesListItem>();
        }

        [XmlArray("files")]
        [XmlArrayItem("file")]
        public Collection<SerializableFilesListItem> Files
        {
            get { return m_files; }
        }
    }
}