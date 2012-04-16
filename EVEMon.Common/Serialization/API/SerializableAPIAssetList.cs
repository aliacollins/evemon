using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIAssetList
    {
        private readonly Collection<SerializableAssetListItem> m_assets;

        public SerializableAPIAssetList()
        {
            m_assets = new Collection<SerializableAssetListItem>();
        }

        [XmlArray("assets")]
        [XmlArrayItem("asset")]
        public Collection<SerializableAssetListItem> Assets
        {
            get { return m_assets; }
        }
    }
}
