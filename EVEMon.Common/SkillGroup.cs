using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a skills group
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class SkillGroup : ReadonlyKeyedCollection<string, Skill>
    {
        /// <summary>
        /// Constructor, only used by SkillCollection
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal SkillGroup(Character character, StaticSkillGroup src)
        {
            StaticData = src;

            foreach (StaticSkill srcSkill in src)
            {
                Items[srcSkill.Name] = new Skill(character, this, srcSkill);
            }
        }

        /// <summary>
        /// Gets the static data associated with this group
        /// </summary>
        public StaticSkillGroup StaticData { get; private set; }

        /// <summary>
        /// Gets the group's ID
        /// </summary>
        public long ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets the group's name
        /// </summary>
        public string Name
        {
            get { return StaticData.Name; }
        }

        /// <summary>
        /// Gets the skill with the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Skill this[string name]
        {
            get { return GetByKey(name); }
        }

        /// <summary>
        /// Gets a skill by its name
        /// </summary>
        /// <param name="skillName"></param>
        /// <returns></returns>
        public bool Contains(string skillName)
        {
            return Items.ContainsKey(skillName);
        }

        /// <summary>
        /// Gets the total number of SP in this group
        /// </summary>
        public int TotalSP
        {
            get { return Items.Values.Sum(gs => gs.SkillPoints); }
        }
    }
}
