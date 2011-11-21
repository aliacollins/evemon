using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        Collection<StaticSkillLevel> Prerequisites { get; }

        EveAttribute PrimaryAttribute { get; }
        EveAttribute SecondaryAttribute { get; }

        Skill ToCharacter(Character character);
    }
}