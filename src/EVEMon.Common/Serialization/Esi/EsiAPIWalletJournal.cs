using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIWalletJournal : List<EsiWalletJournalListItem>
    {
        public SerializableAPIWalletJournal ToXMLItem()
        {
            var ret = new SerializableAPIWalletJournal();
            foreach (var entry in this)
                ret.WalletJournal.Add(entry.ToXMLItem());
            return ret;
        }
    }
}
