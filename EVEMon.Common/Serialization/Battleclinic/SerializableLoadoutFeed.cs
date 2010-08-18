using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Web;

namespace EVEMon.Common.Serialization.BattleClinic
{
    [XmlRoot("loadouts")]
    public class SerializableLoadoutFeed
    {
        [XmlElement("race")]
        public SerializableLoadoutRace Race
        {
            get;
            set;
        }
    }

    public class SerializableLoadoutRace
    {
        [XmlArray("ship")]
        [XmlArrayItem("loadout")]
        public SerializableLoadout[] Loadouts
        {
            get;
            set;
        }
    }

    public class SerializableLoadout
    {
        public SerializableLoadout()
        {
            Slots = new List<SerializableLoadoutSlot>();
        }

        private string m_loadoutName;
        [XmlAttribute("name")]
        public string LoadoutName
        {
            get 
            { 
                return HttpUtility.HtmlDecode(m_loadoutName); 
            }
            set { m_loadoutName = value; }
        }

        [XmlAttribute("Author")]
        public string  Author
        {
            get;
            set;
        }

        [XmlAttribute("rating")]
        public double rating
        {
            get;
            set;
        }

        [XmlAttribute("loadoutID")]
        public string LoadoutId
        {
            get;
            set;
        }

        [XmlAttribute("date")]
        public string SubmissionDateString
        {
            get;
            set;
        }

        [XmlIgnore()]
        public DateTime SubmissionDate
        {
            get
            {
                DateTime parsedDate;
                if (DateTime.TryParse(SubmissionDateString, out parsedDate))
                {
                    return parsedDate;
                }
                return DateTime.MinValue;
            }
        }

        [XmlAttribute("topic")]
        public int Topic
        {
            get;
            set;
        }

        [XmlElement("slot")]
        public List<SerializableLoadoutSlot> Slots
        {
            get;
            set;
        }
    }

    public class SerializableLoadoutSlot
    {
        [XmlAttribute("type")]
        public string SlotType
        {
            get;
            set;
        }

        [XmlAttribute("position")]
        public string SlotPosition
        {
            get;
            set;
        }

        [XmlText]
        public int ItemID
        {
            get;
            set;
        }
    }
}
