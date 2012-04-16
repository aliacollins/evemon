using System.Linq;
using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    public sealed class AssetCollection : ReadonlyCollection<Asset>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
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
            // Importation can take a serious amount of time depending on the amount of assets,
            // therefore we invoke it on another thread
            Dispatcher.BackgroundInvoke(() =>
                                            {
                                                Items.Clear();

                                                // Import the assets from the API
                                                foreach (SerializableAssetListItem srcAsset in src)
                                                {
                                                    Asset asset = new Asset(m_character, srcAsset);
                                                    Items.Add(asset);

                                                    Items.AddRange(srcAsset.Contents.Select(
                                                        content =>
                                                        new Asset(m_character, content)
                                                            { LocationID = asset.LocationID, Container = asset.Item.Name }));
                                                }

                                                // Invoke back to the UI thread
                                                // Fires the event regarding assets update
                                                Dispatcher.Invoke(() => EveMonClient.OnCharacterAssetsUpdated(m_character));
                                            });
        }
    }
}