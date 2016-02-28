using System;
using System.ComponentModel;

namespace EVEMon.Common.Enumerations
{
    public enum CertificateSort
    {
        [Description("No Sorting")]
        None = 0,

        [Description("Time to Next Level")]
        TimeToNextLevel = 1,

        [Description("Time to Max Level")]
        TimeToMaxLevel = 2,

        // Obsolete member, here only for backwards compatibility
        // Transistion is being done in Settins.OnImportCompleted method
        [Description("")]
        Name = 99
    }
}
