using System.Collections.Generic;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    public interface IStaticSkill
    {
        long ID { get; }
        int ArrayIndex { get; }
        string Name { get; }

        int Rank { get; }
        long Cost { get; }
        StaticSkillGroup Group { get; }

        List<StaticSkillLevel> Prerequisites { get; }

        EveAttribute PrimaryAttribute { get; }
        EveAttribute SecondaryAttribute { get; }

        Skill ToCharacter(Character character);
    }
}
