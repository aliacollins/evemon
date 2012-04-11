using System;
using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class AssetCollection : ReadonlyCollection<Asset>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal AssetCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable assets from the API.</param>
        internal void Import(IEnumerable<SerializableAssetListItem> src)
        {
            Items.Clear();

            // Import the research points from the API
            foreach (SerializableAssetListItem srcAsset in src)
            {
                Asset asset = new Asset(m_character, srcAsset) { Container = String.Empty };
                Items.Add(asset);

                foreach (SerializableAssetListItem content in srcAsset.Contents)
                {
                    Items.Add(new Asset(m_character, content) { LocationID = asset.LocationID, Container = asset.Item.Name });
                }
            }
        }
    }
}