using System;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents an attribute for a character scratchpad
    /// </summary>
    public sealed class CharacterAttribute : ICharacterAttribute
    {
        private readonly EveAttribute m_attrib;
        private readonly Character m_character;

        /// <summary>
        /// Constructor from a character attribute.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="attrib"></param>
        internal CharacterAttribute(Character character, EveAttribute attrib)
        {
            Base = EveConstants.CharacterBaseAttributePoints;
            m_attrib = attrib;
            m_character = character;
        }

        /// <summary>
        /// Gets the base attribute.
        /// </summary>
        public Int64 Base { get; internal set; }

        /// <summary>
        /// Gets the bonus granted by the implant.
        /// </summary>
        public Int64 ImplantBonus
        {
            get { return m_character.CurrentImplants[m_attrib].Bonus; }
        }

        /// <summary>
        /// Gets the effective attribute value.
        /// </summary>
        public Int64 EffectiveValue
        {
            get { return Base + ImplantBonus; }
        }

        /// <summary>
        /// Gets a string representation with the provided format. The following parameters are accepted :
        /// <list type="bullet">
        /// <item>%n for name (lower case)</item>
        /// <item>%N for name (CamelCase)</item>
        /// <item>%B for attribute base value</item>
        /// <item>%b for base bonus</item>
        /// <item>%i for implant bonus</item>
        /// <item>%r for remapping points</item>
        /// <item>%e for effective value</item>
        /// </list>
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToString(string format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            format = format.Replace("%n", m_attrib.ToString().ToLower(CultureConstants.DefaultCulture));
            format = format.Replace("%N", m_attrib.ToString());
            format = format.Replace("%B", EveConstants.CharacterBaseAttributePoints.ToString(CultureConstants.DefaultCulture));
            format = format.Replace("%b", Base.ToString(CultureConstants.DefaultCulture));
            format = format.Replace("%i", ImplantBonus.ToString(CultureConstants.DefaultCulture));
            format = format.Replace("%r", (Base - EveConstants.CharacterBaseAttributePoints).ToString(CultureConstants.DefaultCulture));
            format = format.Replace("%e", EffectiveValue.ToString("0", CultureConstants.DefaultCulture));
            return format;
        }

        /// <summary>
        /// Gets a string representation with the following format : "<c>Intelligence : 15</c>"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureConstants.DefaultCulture, "{0} : {1}", m_attrib, EffectiveValue);
        }
    }
}