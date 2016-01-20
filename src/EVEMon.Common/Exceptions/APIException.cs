using System;
using EVEMon.Common.Serialization;

namespace EVEMon.Common.Exceptions
{
    internal class APIException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="APIException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public APIException(SerializableAPIError error)
            : base(error.ErrorMessage)
        {
            ErrorCode = error.ErrorCode;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }
    }
}