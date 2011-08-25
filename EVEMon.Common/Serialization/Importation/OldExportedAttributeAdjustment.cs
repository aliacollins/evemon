using System;

namespace EVEMon.Common.Serialization.Importation
{
    [Flags]
    public enum OldExportedAttributeAdjustment
    {
        Base = 1,
        Skills = 2,
        Implants = 4,
        AllWithoutLearning = 7,
        Learning = 8,
        AllWithLearning = 15
    }
}
