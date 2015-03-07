using System;
using System.Collections.ObjectModel;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    public interface IStaticSkill
    {
        int ID { get; }
        int ArrayIndex { get; }
        string Name { get; }

        Int64 Rank { get; }
        long Cost { get; }
        StaticSkillGroup Group { get; }

        Collection<StaticSkillLevel> Prerequisites { get; }

        EveAttribute PrimaryAttribute { get; }
        EveAttribute SecondaryAttribute { get; }

        Skill ToCharacter(Character character);
    }
}