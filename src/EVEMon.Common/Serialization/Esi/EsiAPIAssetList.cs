using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [CollectionDataContract]
    public sealed class EsiAPIAssetList : List<EsiAssetListItem>
    {
        public ICollection<SerializableAssetListItem> CreateAssetList()
        {
            var assets = new List<SerializableAssetListItem>(Count >> 1 + 1);
            var lookup = new Dictionary<long, SerializableAssetListItem>(Count);
            var remaining = new LinkedList<SerializableAssetListItem>();
            SerializableAssetListItem item;
            // NOTE: ESI uses flat assets, but XML had container assets etc. as nested
            foreach (var asset in this)
            {
                long id = asset.ItemID;
                if (lookup.ContainsKey(id) || asset.LocationType == CCPAPILocationType.Other)
                    // This item is in a container
                    remaining.AddLast(asset.ToXMLItem());
                else
                    lookup.Add(id, asset.ToXMLItem());
            }
            // "other" items belong in containers?
            foreach (var leftoverAsset in remaining)
            {
                if (lookup.TryGetValue(leftoverAsset.LocationID, out item))
                    item.Contents.Add(leftoverAsset);
                else
                    lookup.Add(leftoverAsset.ItemID, leftoverAsset);
            }
            foreach (var asset in lookup.Values)
                assets.Add(asset);
            assets.TrimExcess();
            return assets;
        }
    }
}
