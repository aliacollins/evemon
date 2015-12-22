namespace EVEMon.Common.Enumerations
{
    /// <summary>
    /// Enumeration of skill browser filter.
    /// </summary>
    public enum SkillFilter
    {
        None = -1,
        All = 0,
        ByAttributes = 1,
        NoLv5 = 2,
        Known = 3,
        Lv1Ready = 4,
        Unknown = 5,
        UnknownButOwned = 6,
        UnknownButTrainable = 7,
        UnknownAndNotOwned = 8,
        UnknownAndNotTrainable = 9,
        NotPlanned = 10,
        NotPlannedButTrainable = 11,
        PartiallyTrained = 12,
        Planned = 13,
        Trainable = 14,
        TrailAccountFriendly = 15
    }
}