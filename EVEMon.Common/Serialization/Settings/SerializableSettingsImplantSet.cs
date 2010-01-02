using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a collection of implants sets
    /// </summary>
    public sealed class SerializableImplantSetCollection
    {
        public SerializableImplantSetCollection()
        {
            API = new SerializableSettingsImplantSet();
            OldAPI = new SerializableSettingsImplantSet();
            CustomSets = new List<SerializableSettingsImplantSet>();
        }

        [XmlElement("api")]
        public SerializableSettingsImplantSet API
        {
            get;
            set;
        }

        [XmlElement("oldApi")]
        public SerializableSettingsImplantSet OldAPI
        {
            get;
            set;
        }

        [XmlElement("customSet")]
        public List<SerializableSettingsImplantSet> CustomSets
        {
            get;
            set;
        }

        [XmlElement("selectedIndex")]
        public int SelectedIndex
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the set of attributes enhancers
    /// </summary>
    public sealed class SerializableSettingsImplantSet
    {
        public SerializableSettingsImplantSet()
        {
            this.Name = "Custom";
            this.Intelligence = Implant.None.Name;
            this.Perception = Implant.None.Name;
            this.Willpower = Implant.None.Name;
            this.Charisma = Implant.None.Name;
            this.Memory = Implant.None.Name;
            this.Slot6 = Implant.None.Name;
            this.Slot7 = Implant.None.Name;
            this.Slot8 = Implant.None.Name;
            this.Slot9 = Implant.None.Name;
            this.Slot10 = Implant.None.Name;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("intelligence")]
        public string Intelligence
        {
            get;
            set;
        }

        [XmlElement("memory")]
        public string Memory
        {
            get;
            set;
        }

        [XmlElement("willpower")]
        public string Willpower
        {
            get;
            set;
        }

        [XmlElement("perception")]
        public string Perception
        {
            get;
            set;
        }

        [XmlElement("charisma")]
        public string Charisma
        {
            get;
            set;
        }

        [XmlElement("slot6")]
        public string Slot6
        {
            get;
            set;
        }

        [XmlElement("slot7")]
        public string Slot7
        {
            get;
            set;
        }

        [XmlElement("slot8")]
        public string Slot8
        {
            get;
            set;
        }

        [XmlElement("slot9")]
        public string Slot9
        {
            get;
            set;
        }

        [XmlElement("slot10")]
        public string Slot10
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
