using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICharacterMedals
    {
        private readonly Collection<SerializableMedalsListItem> m_currentCorpMedals;
        private readonly Collection<SerializableMedalsListItem> m_otherCorpsMedals;

        public SerializableAPICharacterMedals()
        {
            m_currentCorpMedals = new Collection<SerializableMedalsListItem>();
            m_otherCorpsMedals = new Collection<SerializableMedalsListItem>();
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
        public IEnumerable<SerializableMedalsListItem> AllMedals
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
