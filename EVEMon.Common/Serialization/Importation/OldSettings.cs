using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Serialization.Importation
{
    [XmlRoot("OldSettings")]
    public sealed class OldSettings
    {
        public OldSettings()
        {
            Accounts = new List<SerializableAccount>();
            Characters = new List<OldCharacter>();
            Plans = new List<OldPlan>();
        }

        [XmlArray("accounts")]
        [XmlArrayItem("account")]
        public List<SerializableAccount> Accounts
        {
            get;
            set;
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public List<OldCharacter> Characters
        {
            get;
            set;
        }

        [XmlArray("plans")]
        [XmlArrayItem("plan")]
        public List<OldPlan> Plans
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class OldCharacter
    {
        [XmlAttribute("id")]
        public long ID
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
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class OldPlan
    {
        public OldPlan()
        {
            Entries = new List<SerializablePlanEntry>();
        }

        [XmlAttribute("character")]
        public string Owner
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

        [XmlElement("entry")]
        public List<SerializablePlanEntry> Entries
        {
            get;
            set;
        }
    }
}
