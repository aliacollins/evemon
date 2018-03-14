using System.Runtime.Serialization;
using System.Collections.Generic;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiJumpCloneListItem : EsiLocationBase
    {
        [DataMember(Name = "jump_clone_id")]
        public int JumpCloneID { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }
        
        [DataMember(Name = "implants")]
        public List<int> Implants { get; set; }

        public SerializableCharacterJumpClone ToXMLItem()
        {
            return new SerializableCharacterJumpClone()
            {
                LocationID = LocationID,
                // TypeID is not used in EVEMon
                CloneName = Name,
                JumpCloneID = JumpCloneID
            };
        }
    }
}
