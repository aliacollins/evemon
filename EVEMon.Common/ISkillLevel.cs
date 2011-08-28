using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a static skill and level tuple
    /// </summary>
    public interface ISkillLevel
    {
        int Level { get; }
        StaticSkill Skill { get; }
    }
}