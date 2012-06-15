using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIMedals
    {
        private readonly Collection<SerializableMedalsListItem> m_corporationMedals;
        private readonly Collection<SerializableMedalsListItem> m_currentCorpMedals;
        private readonly Collection<SerializableMedalsListItem> m_otherCorpsMedals;

        public SerializableAPIMedals()
        {
            m_corporationMedals = new Collection<SerializableMedalsListItem>();
            m_currentCorpMedals = new Collection<SerializableMedalsListItem>();
            m_otherCorpsMedals = new Collection<SerializableMedalsListItem>();
        }

        [XmlArray("medals")]
        [XmlArrayItem("medal")]
        public Collection<SerializableMedalsListItem> CorporationMedals
        {
            get
            {
                foreach (SerializableMedalsListItem medal in m_corporationMedals)
                {
                    medal.Group = MedalGroup.CurrentCorporation;
                }
                return m_corporationMedals;
            }
        }

        [XmlArray("currentCorporation")]
        [XmlArrayItem("medal")]
        public Collection<SerializableMedalsListItem> CurrentCorpMedals
        {
            get
            {
                foreach (SerializableMedalsListItem currentCorpMedal in m_currentCorpMedals)
                {
                    currentCorpMedal.Group = MedalGroup.CurrentCorporation;
                }
                return m_currentCorpMedals;
            }
        }

        [XmlArray("otherCorporations")]
        [XmlArrayItem("medal")]
        public Collection<SerializableMedalsListItem> OtherCorpsMedals
        {
            get
            {
                foreach (SerializableMedalsListItem otherCorpsMedal in m_otherCorpsMedals)
                {
                    otherCorpsMedal.Group = MedalGroup.OtherCorporation;
                }
                return m_otherCorpsMedals;
            }
        }

        [XmlIgnore]
        public IEnumerable<SerializableMedalsListItem> CharacterAllMedals
        {
            get
            {
                List<SerializableMedalsListItem> medals = new List<SerializableMedalsListItem>();
                medals.AddRange(CurrentCorpMedals);
                medals.AddRange(OtherCorpsMedals);
                return medals;
            }
        }
    }
}
