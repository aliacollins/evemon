using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of asset list. Used for querying CCP.
    /// </summary>
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
