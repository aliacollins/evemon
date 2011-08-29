using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a collection of a character's skills groups
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class SkillGroupCollection : ReadonlyKeyedCollection<string, SkillGroup>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        internal SkillGroupCollection(Character character)
        {
            foreach (SkillGroup group in StaticSkills.AllGroups.Select(srcGroup => new SkillGroup(character, srcGroup)))
            {
                Items[group.Name] = group;
            }
        }

        /// <summary>
        /// Gets the skill group with the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SkillGroup this[string name]
        {
            get { return GetByKey(name); }
        }

    }
}
