using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPIEveFactionWars : List<EsiEveFactionWarsListItem>
    {
    }
}
