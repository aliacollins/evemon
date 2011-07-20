using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Base serializable class for items, ships and implants.
    /// </summary>
    public sealed class SerializableItem
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlAttribute("icon")]
        public string Icon
        {
            get;
            set;
        }

        [XmlAttribute("race")]
        public Race Race
        {
            get;
            set;
        }

        [XmlAttribute("metaLevel")]
        public int MetaLevel
        {
            get;
            set;
        }

        [XmlAttribute("metaGroup")]
        public ItemMetaGroup MetaGroup
        {
            get;
            set;
        }

        [XmlAttribute("slot")]
        public ItemSlot Slot
        {
            get;
            set;
        }

        [XmlAttribute("family")]
        public ItemFamily Family
        {
            get;
            set;
        }

        [XmlElement("s")]
        public SerializablePrerequisiteSkill[] Prereqs
        {
            get;
            set;
        }

        [XmlElement("p")]
        public SerializablePropertyValue[] Properties
        {
            get;
            set;
        }

        [XmlElement("r")]
        public SerializableReprocessingMaterial[] Reprocessing
        {
            get;
            set;
        }
    }
}
