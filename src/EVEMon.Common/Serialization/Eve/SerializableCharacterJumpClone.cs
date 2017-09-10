using System;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableCharacterJumpClone
    {
        [XmlAttribute("jumpCloneID")]
        public long JumpCloneID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("locationID")]
        public long LocationID { get; set; }

        [XmlAttribute("cloneName")]
        public string CloneNameXml
        {
            get { return CloneName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    CloneName = value;

                var station = LocationID != 0 && LocationID <= Int32.MaxValue
                    ? Station.GetByID(Convert.ToInt32(LocationID))
                    : null;

                CloneName = String.Format("Clone in {0}", station != null
                    ? station.Name
                    : String.Format("Clone Vat Bay ({0})", LocationID));
            }
        }

        [XmlIgnore]
        public string CloneName { get; set; }

    }
}