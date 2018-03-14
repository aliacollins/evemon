using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIAllianceHistory : List<EsiAllianceHistoryListItem>
    {
        // There is no similar XML version
    }
}
