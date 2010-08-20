using System;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents the category of error which can occur with the API.
    /// </summary>
    public enum APIErrors
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
}