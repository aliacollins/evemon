using System;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization
{
    /// <summary>
    /// Wraps the results returned from a JSON request.
    /// </summary>
    public class JsonResult<T>
    {
        private readonly APIErrorType m_error;
        private readonly Exception m_exception;
        private readonly string m_message;
        private readonly ResponseParams m_response;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonResult(ResponseParams response, T result = default(T))
        {
            m_error = APIErrorType.None;
            m_exception = null;
            m_message = string.Empty;
            m_response = response;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCPAPIResult{T}" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">exception</exception>
        protected JsonResult(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));
            m_exception = exception;
            m_message = exception?.Message ?? string.Empty;
            m_response = new ResponseParams(0);
            Result = default(T);
        }

        /// <summary>
        /// Creates a JSON result wrapped from another JSON result.
        /// </summary>
        /// <param name="wrapped">The JSON result to wrap.</param>
        protected JsonResult(JsonResult<T> wrapped)
        {
            m_exception = wrapped.Exception;
            m_message = wrapped.ErrorMessage;
            m_error = wrapped.ErrorType;
            m_response = wrapped.m_response;
            Result = wrapped.Result;
        }

        /// <summary>
        /// Constructor from an http exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(HttpWebClientServiceException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Http;
        }

        /// <summary>
        /// Constructor from a JSON exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(InvalidDataContractException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Json;
        }
        
        /// <summary>
        /// Constructor from a JSON serialization exception wrapped into an InvalidOperationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(InvalidOperationException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Json;
        }

        /// <summary>
        /// Constructor from a JSON serialization exception wrapped into a SerializationException
        /// </summary>
        /// <param name="exception">The exception.</param>
        public JsonResult(SerializationException exception)
            : this(exception as Exception)
        {
            m_error = APIErrorType.Json;
        }

        /// <summary>
        /// Constructor from a CCP API internal error
        /// </summary>
        /// <param name="response">The response parameters including the error code.</param>
        /// <param name="message">The CCP error message.</param>
        public JsonResult(ResponseParams response, string message)
        {
            m_error = APIErrorType.CCP;
            m_exception = null;
            m_message = message ?? string.Empty;
            m_response = response;
            Result = default(T);
        }

        #endregion


        #region Errors handling

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception => m_exception;

        public bool HasError => Exception != null || m_error != APIErrorType.None;

        /// <summary>
        /// Gets the response code from the server.
        /// </summary>
        public int ResponseCode => m_response.ResponseCode;
        
        #endregion


        #region Properties

        public string ErrorMessage => m_message;

        public APIErrorType ErrorType => m_error;

        public T Result { get; set; }

        public DateTime? CurrentTime => m_response.Time;

        public ResponseParams Response => m_response;

        #endregion
        
    }
}
