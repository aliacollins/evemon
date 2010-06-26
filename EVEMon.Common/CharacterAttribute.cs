using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents an attribute for a character scratchpad
    /// </summary>
    public sealed class CharacterAttribute : ICharacterAttribute
    {
        private readonly EveAttribute m_attrib;
        private readonly Character m_character;
        private readonly Skill m_lowerSkill;
        private readonly Skill m_upperSkill;
        private int m_base;

        /// <summary>
        /// Constructor from a character attribute.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attrib"></param>
        internal CharacterAttribute(Character character, EveAttribute attrib)
        {
            m_base = 5;
            m_attrib = attrib;
            m_character = character;

            m_lowerSkill = character.Skills[StaticSkills.GetLowerAttributeLearningSkill(attrib)];
            m_upperSkill = character.Skills[StaticSkills.GetUpperAttributeLearningSkill(attrib)];
        }

        /// <summary>
        /// Gets the base attribute.
        /// </summary>
        public int Base
        {
            get { return m_base; }
            internal set { m_base = value; }
        }

        /// <summary>
        /// Gets the bonus granted by the implant
        /// </summary>
        public int ImplantBonus
        {
            get { return m_character.CurrentImplants[m_attrib].Bonus; }
        }

        /// <summary>
        /// Gets or sets the bonus granted by the lower-tier learning skill for this attribute.
        /// </summary>
        public int LowerSkillBonus
        {
            get { return m_lowerSkill.Level; }
        }

        /// <summary>
        /// Gets or sets the bonus granted by the upper-tier learning skill for this attribute.
        /// </summary>
        public int UpperSkillBonus
        {
            get { return m_upperSkill.Level; }
        }

        /// <summary>
        /// Gets the bonus granted by the lower-tier and upper-tier learning skill for this attribute.
        /// </summary>
        public int SkillsBonus
        {
            get { return m_lowerSkill.Level + m_upperSkill.Level; }
        }

        /// <summary>
        /// Gets the effective attribute before the "learning" skill factor is applied.
        /// </summary>
        public int PreLearningEffectiveAttribute
        {
            get { return m_base + ImplantBonus + m_lowerSkill.Level + m_upperSkill.Level; }
        }

        /// <summary>
        /// Gets the effective attribute value.
        /// </summary>
        public float EffectiveValue
        {
            get { return PreLearningEffectiveAttribute * m_character.LearningFactor; }
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
            format = format.Replace("%n",   m_attrib.ToString().ToLower(CultureConstants.DefaultCulture));
            format = format.Replace("%N",   m_attrib.ToString());
            format = format.Replace("%b",   m_base.ToString());
            format = format.Replace("%i",   ImplantBonus.ToString());
            format = format.Replace("%s",   SkillsBonus.ToString());
            format = format.Replace("%s1",  LowerSkillBonus.ToString());
            format = format.Replace("%s2",  UpperSkillBonus.ToString());
            format = format.Replace("%f",   m_character.LearningFactor.ToString("0.00"));
            format = format.Replace("%e",   EffectiveValue.ToString("0.00"));
            return format;
        }

        /// <summary>
        /// Gets a string representation with the following format : "<c>Intelligence : 15</c>"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
 	         return m_attrib.ToString() + " : " + EffectiveValue.ToString();
        }
    }
}
