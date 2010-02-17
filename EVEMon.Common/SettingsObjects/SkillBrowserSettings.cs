using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class SkillBrowserSettings
    {
        [XmlElement("filter")]
        public SkillFilter Filter
        {
            get;
            set;
        }

        [XmlElement("sort")]
        public SkillSort Sort
        {
            get;
            set;
        }

        [XmlElement("textSearch")]
        public string TextSearch
        {
            get;
            set;
        }

        [XmlElement("iconsGroupIndex")]
        public int IconsGroupIndex
        {
            get;
            set;
        }

        [XmlElement("showNonPublicSkills")]
        public bool ShowNonPublicSkills
        {
            get;
            set;
        }

        internal SkillBrowserSettings Clone()
        {
            return (SkillBrowserSettings)MemberwiseClone();
        }
    }

    public enum SkillFilter
    {
        All = 0,
        NoLv5 = 1,
        Known = 2,
        Lv1Ready = 3,
        Unknown = 4,
        UnknownButOwned = 5,
        UnknownButTrainable = 6,
        UnknownAndNotOwned = 7,
        UnknownAndNotTrainable = 8,
        NotPlanned = 9,
        NotPlannedButTrainable = 10,
        PartiallyTrained = 11,
        Planned = 12,
        Trainable = 13,
        TrailAccountFriendly = 14
    }

    public enum SkillSort
    {
        None = 0,
        TimeToNextLevel = 1,
        TimeToLevel5 = 2,
        Rank = 3,
        SPPerHour = 4
    }
}
