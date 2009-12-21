using System;
using System.Xml;
namespace EVEMon.Common.Serialization.API
{
    public interface IAPIResult
    {
        /// <summary>
        /// Gets true if there is an error.
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Gets the type of the error or <see cref="APIErrors.None"/> when there was no error.
        /// </summary>
        APIErrors ErrorType { get; }

        /// <summary>
        /// Gets the error message without bothering about its nature.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets the error returned by CCP.
        /// </summary>
        CCPError CCPError { get; set; }

        /// <summary>
        /// Gets the returned XML document.
        /// </summary>
        XmlDocument XmlDocument { get; set; }
    }
}
