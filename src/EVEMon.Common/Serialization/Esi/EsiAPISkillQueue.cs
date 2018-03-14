using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPISkillQueue : List<EsiSkillQueueListItem>
    {
        public SerializableAPISkillQueue ToXMLItem()
        {
            var ret = new SerializableAPISkillQueue();
            foreach (var queueItem in this)
                ret.Queue.Add(queueItem.ToXMLItem());
            return ret;
        }
    }
}
