namespace EVEMon.Common.Enumerations.API
{
    /// <summary>
    /// Enumeration of API key types.
    /// </summary>
    public enum APIKeyType
    {
        /// <summary>
        /// The API key type wouldn't be checked because of an error.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// This is an account wide API key.
        /// </summary>
        Account = 1,

        /// <summary>
        /// This is a character wide API key.
        /// </summary>
        Character = 2,

        /// <summary>
        /// This is a corporation wide API key.
        /// </summary>
        Corporation = 3

    }
}