using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiCharacterNameListItem
    {
        [DataMember(Name = "character_id")]
        public long ID { get; set; }

        [DataMember(Name = "character_name")]
        public string Name { get; set; }
    }
}
