using System;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableCharacterJumpClone
    {
        [XmlAttribute("jumpCloneID")]
        public long JumpCloneID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("locationID")]
        public int LocationID { get; set; }

        [XmlAttribute("cloneName")]
        public string CloneNameXml
        {
            get { return CloneName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    CloneName = value;

                CloneName = String.Format("Clone in {0}", Station.GetByID(LocationID).Name);
            }
        }

        [XmlIgnore]
        public string CloneName { get; set; }

    }
}