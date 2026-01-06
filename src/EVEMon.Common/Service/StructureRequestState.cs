namespace EVEMon.Common.Service
{
    /// <summary>
    /// Represents the state of a structure lookup request.
    /// </summary>
    internal enum StructureRequestState
    {
        /// <summary>No request has been attempted.</summary>
        Pending,

        /// <summary>Request is currently in flight.</summary>
        InProgress,

        /// <summary>Request completed successfully.</summary>
        Completed,

        /// <summary>All available characters tried, none had access.</summary>
        Inaccessible,

        /// <summary>Structure confirmed destroyed (404 from ESI).</summary>
        Destroyed
    }
}
