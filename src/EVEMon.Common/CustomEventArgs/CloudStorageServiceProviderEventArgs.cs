using System;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class CloudStorageServiceProviderEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public CloudStorageServiceProviderEventArgs(string errorMessage)
        {
            if (errorMessage != null && String.IsNullOrWhiteSpace(errorMessage))
                errorMessage = @"An error occured.";

            HasError = !String.IsNullOrWhiteSpace(errorMessage);
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this event has error.
        /// </summary>
        /// <value><c>true</c> if this event has error; otherwise, <c>false</c>.</value>
        public bool HasError { get; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; }
    }
}