using System;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a static skill and level tuple
    /// </summary>
    public interface ISkillLevel
    {
        Int64 Level { get; }
        StaticSkill Skill { get; }
    }
}