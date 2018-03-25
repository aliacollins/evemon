using System;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;
using System.Net;

namespace EVEMon.Common.Serialization.Esi
{
    public sealed class EsiResult<T> : IAPIResult
    {
        private readonly CCPAPIErrors m_error;
        private readonly Exception m_exception;
        private readonly string m_message;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EsiResult()
        {
            m_error = CCPAPIErrors.None;
            CCPError = null;
            m_exception = null;
            m_message = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCPAPIResult{T}" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        private EsiResult(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            CCPError = null;
            m_exception = exception;
            m_message = exception?.Message ?? string.Empty;
        }

        /// <summary>
        /// Constructor from an http exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public EsiResult(HttpWebClientServiceException exception)
            : this(exception as Exception)
        {
            m_error = CCPAPIErrors.Http;
        }

        /// <summary>
        /// Constructor from a JSON exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public EsiResult(InvalidDataContractException exception)
            : this(exception as Exception)
        {
            m_error = CCPAPIErrors.Json;
        }
        
        /// <summary>
        /// Constructor from an XML serialization exception wrapped into an InvalidOperationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public EsiResult(InvalidOperationException exception)
            : this(exception as Exception)
        {
            m_error = CCPAPIErrors.Json;
        }

        /// <summary>
        /// Constructor from a custom exception.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="message">The message.</param>
        public EsiResult(CCPAPIErrors error, string message)
        {
            m_error = error;
            m_exception = null;
            m_message = message ?? string.Empty;
        }

        #endregion


        #region Errors handling

        /// <summary>
        /// Gets true if the information is outdated.
        /// </summary>
        public bool IsOutdated => DateTime.UtcNow > CachedUntil;

        /// <summary>
        /// Gets true if there is an error.
        /// </summary>
        public bool HasError => CCPError != null || m_error != CCPAPIErrors.None || Result == null;

        /// <summary>
        /// Gets true if EVE database is out of service.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if EVE database is out of service; otherwise, <c>false</c>.
        /// </value>
        public bool EVEDatabaseError => CCPError?.IsWebSiteDatabaseDisabled ?? false;

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception => m_exception;
        
        #endregion


        #region Properties
        
        public DateTime CurrentTime { get; set; }

        public DateTime CachedUntil { get; set; }

        public string ErrorMessage => m_message;

        public CCPAPIErrors ErrorType => m_error;

        public CCPAPIError CCPError { get; set; }

        public T Result { get; set; }

        #endregion


        #region Time fixing

        /// <summary>
        /// Fixup the result time to match the user's clock.
        /// This should ONLY be called when the ESI result is first received from CCP.
        /// </summary>
        /// <param name="millisecondsDrift"></param>
        public void SynchronizeWithLocalClock(TimeSpan drift)
        {
            // Fix the start/end times for the results implementing synchronization
            ISynchronizableWithLocalClock synchronizable = Result as ISynchronizableWithLocalClock;

            synchronizable?.SynchronizeWithLocalClock(drift);
        }

        #endregion
    }
}
