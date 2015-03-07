using System.ComponentModel;

namespace EVEMon.Common.Serialization.API
{


    #region API Errors

    /// <summary>
    /// Represents the category of error which can occur with the API.
    /// </summary>
    public enum APIError
    {
        /// <summary>
        /// There was no error.
        /// </summary>
        None,

        /// <summary>
        /// The error was caused by the network.
        /// </summary>
        Http,

        /// <summary>
        /// The error occurred during the XSL transformation.
        /// </summary>
        Xslt,

        /// <summary>
        /// The error occurred during the XML deserialization.
        /// </summary>
        Xml,

        /// <summary>
        /// It was a managed CCP error.
        /// </summary>
        CCP
    }

    #endregion


    #region API Order State

    /// <summary>
    /// The status of a market order.
    /// </summary>
    public enum CCPOrderState
    {
        Opened = 0,
        Closed = 1,
        ExpiredOrFulfilled = 2,
        Canceled = 3,
        Pending = 4,
        CharacterDeleted = 5
    }

    #endregion


    #region API Job Completed Status

    public enum CCPJobCompletedStatus
    {
        Installed = 1,
        Paused = 2,
        Ready = 3,

        Delivered = 101,
        Canceled = 102,
        Reverted = 103,
    }

    #endregion


    #region API Contracts

    public enum CCPContractStatus
    {
        [Description("None")]
        None,

        [Description("Outstanding")]
        Outstanding,

        [Description("In Progress")]
        InProgress,

        [Description("Deleted")]
        Deleted,

        [Description("Finished")]
        Completed,

        [Description("Failed")]
        Failed,

        [Description("Completed By Issuer")]
        CompletedByIssuer,

        [Description("Completed By Contractor")]
        CompletedByContractor,

        [Description("Canceled")]
        Canceled,

        [Description("Rejected")]
        Rejected,

        [Description("Overdue")]
        Overdue,

        [Description("Reversed")]
        Reversed
    }

    #endregion
}
