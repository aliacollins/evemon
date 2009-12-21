using System;
using System.Collections.Generic;
namespace EVEMon.Common
{
    public interface ICharacterAttribute
    {
        int Base { get; }
        int ImplantBonus { get; }
        int LowerSkillBonus { get; }
        int UpperSkillBonus { get; }
        int PreLearningEffectiveAttribute { get; }
        float EffectiveValue { get; }


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
        string ToString(string format);
    }
}
