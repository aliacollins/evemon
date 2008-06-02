namespace EVEMon.Common
{
    /// <summary>
    /// Enumeration of the supported API methods. Each method should have an entry in APIMethods and
    /// an equivalent string entry in APIConstants indicating the default path of the method.
    /// </summary>
    public enum APIMethods
    {
        CharacterList,
        CharacterSheet,
        SkillInTraining
    }
}
