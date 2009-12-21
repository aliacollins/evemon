using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents an attribute for a character scratchpad
    /// </summary>
    public sealed class CharacterAttributeScratchpad : ICharacterAttribute
    {
        private readonly EveAttribute m_attrib;

        private int m_base;
        private int m_implantBonus;
        private int m_lowerSkillBonus;
        private int m_upperSkillBonus;
        private float m_effectiveAttribute;
        private float m_learningFactor = 1.0f;

        /// <summary>
        /// Constructor from a character attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <param name="src"></param>
        internal CharacterAttributeScratchpad(EveAttribute attrib)
        {
            m_attrib = attrib;
        }

        /// <summary>
        /// Resets the attribute with the given source
        /// </summary>
        /// <param name="baseAttribute"></param>
        internal void Reset(int baseAttribute, int implantBonus)
        {
            m_base = baseAttribute;
            m_implantBonus = implantBonus;
            m_lowerSkillBonus = 0;
            m_upperSkillBonus = 0;
            Update(1.0f);
        }

        /// <summary>
        /// Resets the attribute with the given source
        /// </summary>
        /// <param name="src"></param>
        internal void Reset(ICharacterAttribute src, float learningFactor)
        {
            m_base = src.Base;
            m_implantBonus = src.ImplantBonus;
            m_lowerSkillBonus = src.LowerSkillBonus;
            m_upperSkillBonus = src.UpperSkillBonus;
            Update(learningFactor);
        }

        /// <summary>
        /// Updates the effective attribute with the given learning factor
        /// </summary>
        /// <param name="learningFactor"></param>
        internal void Update(float learningFactor)
        {
            m_learningFactor = learningFactor;
            m_effectiveAttribute = (m_base + m_implantBonus + m_lowerSkillBonus + m_upperSkillBonus) * learningFactor;
        }

        /// <summary>
        /// Gets or sets the base attribute.
        /// </summary>
        public int Base
        {
            get { return m_base; }
            set
            {
                m_base = value;
                Update(m_learningFactor);
            }
        }

        /// <summary>
        /// Gets or sets the bonus granted by the implant
        /// </summary>
        public int ImplantBonus
        {
            get { return m_implantBonus; }
            set
            {
                m_implantBonus = value;
                Update(m_learningFactor);
            }
        }

        /// <summary>
        /// Gets or sets the bonus granted by the lower-tier learning skill for this attribute.
        /// </summary>
        public int LowerSkillBonus
        {
            get { return m_lowerSkillBonus; }
            set
            {
                m_lowerSkillBonus = value;
                Update(m_learningFactor);
            }
        }

        /// <summary>
        /// Gets or sets the bonus granted by the upper-tier learning skill for this attribute.
        /// </summary>
        public int UpperSkillBonus
        {
            get { return m_upperSkillBonus; }
            set
            {
                m_upperSkillBonus = value;
                Update(m_learningFactor);
            }
        }

        /// <summary>
        /// Gets the bonus granted by the lower-tier and upper-tier learning skill for this attribute.
        /// </summary>
        public int SkillsBonus
        {
            get { return m_lowerSkillBonus + m_upperSkillBonus; }
        }

        /// <summary>
        /// Gets the effective attribute before the "learning" skill factor is applied.
        /// </summary>
        public int PreLearningEffectiveAttribute
        {
            get { return m_base + m_implantBonus + m_lowerSkillBonus + m_upperSkillBonus; }
        }

        /// <summary>
        /// Gets the effective attribute value.
        /// </summary>
        public float EffectiveValue
        {
            get { return m_effectiveAttribute; }
        }

        /// <summary>
        /// Gets a string representation with the provided format. The following parameters are accepted :
        /// <list type="bullet">
        /// <item>%n for name (lower case)</item>
        /// <item>%N for name (CamelCase)</item>
        /// <item>%b for base value</item>
        /// <item>%i for implant bonus</item>
        /// <item>%s for skills bonus</item>
        /// <item>%s1 for lower skill bonus</item>
        /// <item>%s2 for upper skill bonus</item>
        /// <item>%f for learning factor</item>
        /// <item>%e for effective value</item>
        /// </list>
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToString(string format)
        {
            format = format.Replace("%n",   m_attrib.ToString().ToLower());
            format = format.Replace("%N",   m_attrib.ToString());
            format = format.Replace("%b",   m_base.ToString());
            format = format.Replace("%i",   m_implantBonus.ToString());
            format = format.Replace("%s",   SkillsBonus.ToString());
            format = format.Replace("%s1",  m_lowerSkillBonus.ToString());
            format = format.Replace("%s2",  m_upperSkillBonus.ToString());
            format = format.Replace("%f",   m_learningFactor.ToString("0.00"));
            format = format.Replace("%e",   m_effectiveAttribute.ToString("0.00"));
            return format;
        }

        /// <summary>
        /// Gets a string representation with the following format : "<c>Intelligence : 15</c>"
        /// </summary>
        /// <returns></returns>
        public override string  ToString()
        {
 	         return m_attrib.ToString() + " : " + m_effectiveAttribute.ToString();
        }
    }
}
