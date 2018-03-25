using System;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents the result of an API operation.
    /// </summary>
    public interface IAPIResult
    {
        /// <summary>
        /// Gets true if there is an error.
        /// </summary>
        /// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
        bool HasError { get; }

        /// <summary>
        /// Gets true if EVE database is out of service.
        /// </summary>
        /// <value><c>true</c> if EVE database is out of service; otherwise, <c>false</c>.</value>
        bool EVEDatabaseError { get; }

        /// <summary>
        /// Gets the type of the error or <see cref="CCPAPIErrors.None"/> when there was no error.
        /// </summary>
        /// <value>The type of the error.</value>
        CCPAPIErrors ErrorType { get; }

        /// <summary>
        /// Gets the error message without bothering about its nature.
        /// </summary>
        /// <value>The error message.</value>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        Exception Exception { get; }

        /// <summary>
        /// Gets the error returned by CCP.
        /// </summary>
        /// <value>The CCP error.</value>
        CCPAPIError CCPError { get; set; }
    }
}
