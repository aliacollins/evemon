using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("attributes")]
    public class EveAttributes
    {

        // m_owner is used for the adjusted Attribute method, which in turn is only
        // ever used for the "save as... text" method
        private CharacterSheetResult m_owner;

        internal void SetOwner(CharacterSheetResult cs)
        {
            m_owner = cs;
        }

        private int[] m_values = new int[5] { 0, 0, 0, 0, 0 };

        [XmlElement("intelligence")]
        public int BaseIntelligence
        {
            get { return m_values[(int)EveAttribute.Intelligence]; }
            set { m_values[(int)EveAttribute.Intelligence] = value; }
        }

        [XmlElement("charisma")]
        public int BaseCharisma
        {
            get { return m_values[(int)EveAttribute.Charisma]; }
            set { m_values[(int)EveAttribute.Charisma] = value; }
        }

        [XmlElement("perception")]
        public int BasePerception
        {
            get { return m_values[(int)EveAttribute.Perception]; }
            set { m_values[(int)EveAttribute.Perception] = value; }
        }

        [XmlElement("memory")]
        public int BaseMemory
        {
            get { return m_values[(int)EveAttribute.Memory]; }
            set { m_values[(int)EveAttribute.Memory] = value; }
        }

        [XmlElement("willpower")]
        public int BaseWillpower
        {
            get { return m_values[(int)EveAttribute.Willpower]; }
            set { m_values[(int)EveAttribute.Willpower] = value; }
        }

        // AdjustedXXX are only used by the "save as text" method.
        [XmlIgnore]
        public double AdjustedIntelligence
        {
            get { return GetAdjustedAttribute(EveAttribute.Intelligence); }
        }

        [XmlIgnore]
        public double AdjustedCharisma
        {
            get { return GetAdjustedAttribute(EveAttribute.Charisma); }
        }

        [XmlIgnore]
        public double AdjustedPerception
        {
            get { return GetAdjustedAttribute(EveAttribute.Perception); }
        }

        [XmlIgnore]
        public double AdjustedMemory
        {
            get { return GetAdjustedAttribute(EveAttribute.Memory); }
        }

        [XmlIgnore]
        public double AdjustedWillpower
        {
            get { return GetAdjustedAttribute(EveAttribute.Willpower); }
        }

        // only used by the "save as... text" method
        private double GetAttributeAdjustment(EveAttribute eveAttribute, SerializableEveAttributeAdjustment adjustment)
        {
            double result = 0.0;
            double learningBonus = 1.0;
            if ((adjustment & SerializableEveAttributeAdjustment.Base) != 0)
            {
                result += m_values[(int)eveAttribute];
            }
            if ((adjustment & SerializableEveAttributeAdjustment.Implants) != 0)
            {
                foreach (SerializableEveAttributeBonus eab in m_owner.AttributeBonuses.Bonuses)
                {
                    if (eab.EveAttribute == eveAttribute)
                    {
                        result += eab.Amount;
                    }
                }
            }
            if (((adjustment & SerializableEveAttributeAdjustment.Skills) != 0) ||
                ((adjustment & SerializableEveAttributeAdjustment.Learning) != 0))
            {
                foreach (SerializableSkillGroup sg in m_owner.SkillGroups)
                {
                    if (sg.Name == "Learning")
                    {
                        foreach (SerializableSkill s in sg.Skills)
                        {
                            if ((adjustment & SerializableEveAttributeAdjustment.Skills) != 0)
                            {
                                switch (eveAttribute)
                                {
                                    case EveAttribute.Intelligence:
                                        if (s.Name == "Analytical Mind" || s.Name == "Logic")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Charisma:
                                        if (s.Name == "Empathy" || s.Name == "Presence")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Memory:
                                        if (s.Name == "Instant Recall" || s.Name == "Eidetic Memory")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Willpower:
                                        if (s.Name == "Iron Will" || s.Name == "Focus")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                    case EveAttribute.Perception:
                                        if (s.Name == "Spatial Awareness" || s.Name == "Clarity")
                                        {
                                            result += s.Level;
                                        }
                                        break;
                                }
                            }
                            if (s.Name == "Learning")
                            {
                                learningBonus = 1.0 + (0.02 * s.Level);
                            }
                        }
                    }
                }
            }
            if ((adjustment & SerializableEveAttributeAdjustment.Learning) != 0)
            {
                result = result * learningBonus;
            }
            return result;
        }

        // only used by "save as... text file"
        private double GetAdjustedAttribute(EveAttribute eveAttribute)
        {
            return GetAttributeAdjustment(eveAttribute, SerializableEveAttributeAdjustment.AllWithLearning);
        }
    }
}