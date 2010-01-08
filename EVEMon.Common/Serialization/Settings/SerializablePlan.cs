using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a plan
    /// </summary>
    public class SerializablePlan
    {
        public SerializablePlan()
        {
            Entries = new List<SerializablePlanEntry>();
            InvalidEntries = new List<SerializableInvalidPlanEntry>();
            SortingPreferences = new PlanSorting();
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("owner")]
        public Guid Owner
        {
            get;
            set;
        }

        [XmlElement("sorting")]
        public PlanSorting SortingPreferences
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

        [XmlElement("invalidEntry")]
        public List<SerializableInvalidPlanEntry> InvalidEntries
        {
            get;
            set;
        }
  
        internal SerializablePlan Clone()
        {
            var clone = new SerializablePlan();
            clone.Name = this.Name;
            clone.Owner = this.Owner;
            clone.SortingPreferences = this.SortingPreferences.Clone();
            clone.Entries.AddRange(this.Entries.Select(x => x.Clone()));
            clone.InvalidEntries.AddRange(this.InvalidEntries.Select(x => x.Clone()));
            return clone;
        }
    }

    /// <summary>
    /// Represents a plan entry
    /// </summary>
    public sealed class SerializableInvalidPlanEntry
    {
        [XmlAttribute("skill")]
        public string SkillName
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public int PlannedLevel
        {
            get;
            set;
        }

        [XmlAttribute("acknowledged")]
        public bool Acknowledged
        {
            get;
            set;
        }

        internal SerializableInvalidPlanEntry Clone()
        {
            // We need a skill for the plan's character
            SerializableInvalidPlanEntry clone = new SerializableInvalidPlanEntry()
            {
                SkillName = SkillName,
                PlannedLevel = PlannedLevel,
                Acknowledged = Acknowledged
            };

            return clone;
        }
    }

    /// <summary>
    /// Represents a plan entry
    /// </summary>
    public sealed class SerializablePlanEntry
    {
        public SerializablePlanEntry()
        {
            this.PlanGroups = new List<string>();
            Priority = 3;
        }

        [XmlAttribute("skillID")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("skill")]
        public string SkillName
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public int Level
        {
            get;
            set;
        }

        [XmlAttribute("priority")]
        public int Priority
        {
            get;
            set;
        }

        [XmlAttribute("type")]
        public PlanEntryType Type
        {
            get;
            set;
        }

        [XmlElement("notes")]
        public string Notes
        {
            get;
            set;
        }

        [XmlElement("group")]
        public List<string> PlanGroups
        {
            get;
            set;
        }

        [XmlElement("remapping")]
        public SerializableRemappingPoint Remapping
        {
            get;
            set;
        }

        internal SerializablePlanEntry Clone()
        {
            var clone = (SerializablePlanEntry)this.MemberwiseClone();
            clone.PlanGroups = new List<string>(this.PlanGroups);
            clone.Remapping = this.Remapping.Clone();
            return clone;
        }
    }

    /// <summary>
    /// Represents a remapping point
    /// </summary>
    public sealed class SerializableRemappingPoint
    {
        [XmlAttribute("status")]
        public RemappingPoint.PointStatus Status
        {
            get;
            set;
        }

        [XmlAttribute("per")]
        public int Perception
        {
            get;
            set;
        }

        [XmlAttribute("int")]
        public int Intelligence
        {
            get;
            set;
        }

        [XmlAttribute("mem")]
        public int Memory
        {
            get;
            set;
        }

        [XmlAttribute("wil")]
        public int Willpower
        {
            get;
            set;
        }

        [XmlAttribute("cha")]
        public int Charisma
        {
            get;
            set;
        }

        public SerializableRemappingPoint Clone()
        {
            return (SerializableRemappingPoint)this.MemberwiseClone();
        }
    }
}
