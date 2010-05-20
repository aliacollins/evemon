using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization;

namespace EVEMon.Common.Serialization.API
{
    [XmlRoot("eveapi")]
    public sealed class APIResult<T> : IAPIResult
    {
        private APIErrors m_error = APIErrors.None;
        private readonly string m_errorMessage = String.Empty;

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public APIResult()
        {
            m_error = APIErrors.None;
            m_errorMessage = String.Empty;
        }

        /// <summary>
        /// Constructor from an http exception
        /// </summary>
        public APIResult(HttpWebServiceException exc)
        {
            m_error = APIErrors.Http;
            m_errorMessage = exc.Message;
        }

        /// <summary>
        /// Constructor from an XML exception
        /// </summary>
        public APIResult(XmlException exc)
        {
            m_error = APIErrors.Xml;
            m_errorMessage = exc.Message;
        }

        /// <summary>
        /// Constructor from an XSLT exception
        /// </summary>
        public APIResult(System.Xml.Xsl.XsltException exc)
        {
            m_error = APIErrors.Xslt;
            m_errorMessage = exc.Message;
        }

        /// <summary>
        /// Constructor from an XML serialization exception wrapped into an InvalidOperationException
        /// </summary>
        public APIResult(InvalidOperationException exc)
        {
            m_error = APIErrors.Xml;
            m_errorMessage = (exc.InnerException == null ? exc.Message : exc.InnerException.Message);
        }

        /// <summary>
        /// Constructor from a custom exception.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        public APIResult(APIErrors error, string message)
        {
            m_error = error;
            m_errorMessage = message;
        }
        #endregion


        #region Errors handling
        /// <summary>
        /// Gets true if the information is outdated
        /// </summary>
        public bool IsOutdated
        {
            get
            {
                return DateTime.UtcNow > CachedUntil;
            }
        }

        /// <summary>
        /// Gets true if there is an error
        /// </summary>
        public bool HasError
        {
            get 
            {
                if (CCPError != null) return true;
                return m_error != APIErrors.None; 
            }
        }

        /// <summary>
        /// Gets the type of the error or <see cref="APIErrors.None"/> when there was no error.
        /// </summary>
        public APIErrors ErrorType
        {
            get 
            {
                if (CCPError != null) return APIErrors.CCP;
                return m_error; 
            }
        }

        /// <summary>
        /// Gets the error message without bothering about its nature
        /// </summary>
        public string ErrorMessage
        {
            get 
            {
                if (CCPError != null) return CCPError.ErrorMessage;
                return m_errorMessage; 
            }
        }

        /// <summary>
        /// Gets / sets the XML document when there's no HTTP error
        /// </summary>
        [XmlIgnore]
        public XmlDocument XmlDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time left before a new version is available.
        /// </summary>
        public TimeSpan RemainingTime
        {
            get { return CachedUntil - DateTime.UtcNow; }
        }
        #endregion


        #region CCP Mapping
        [XmlAttribute("version")]
        public int APIVersion
        {
            get;
            set;
        }

        [XmlElement("currentTime")]
        public string _CurrentTime
        {
            get { return CurrentTime.ToCCPTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CurrentTime = value.CCPTimeStringToDateTime(); 
            }
        }

        [XmlElement("cachedUntil")]
        public string _CachedUntil
        {
            get { return CachedUntil.ToCCPTimeString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                CachedUntil = value.CCPTimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime CurrentTime
        {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime CachedUntil
        {
            get;
            set;
        }

        [XmlElement("error")]
        public CCPError CCPError
        {
            get;
            set;
        }

        [XmlElement("result")]
        public T Result
        {
            get;
            set;
        }
        #endregion


        #region Times fixing
        /// <summary>
        /// Fixup the currentTime and cachedUntil time to match the user's clock.
        /// This should ONLY be called when the xml is first recieved from CCP
        /// </summary>
        /// <param name="millisecondsDrift"></param>
        public void SynchronizeWithLocalClock(double millisecondsDrift)
        {
            // convert the drift between webserver time and local time
            // to a timespan. It is possible for millisecondsDrift to
            // be erroniously outside of the range of an int thus we 
            // need to catch an overflow exception and reset to 0.
            TimeSpan drift;
            try
            {
                drift = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(millisecondsDrift));
            }
            catch (OverflowException)
            {
                drift = new TimeSpan(0, 0, 0, 0);
            }

            // Now fix the server time to align with local time
            this.CurrentTime -= drift;
            this.CachedUntil -= drift;

            // Fix the TQ start/end times first
            ISynchronizableWithLocalClock synchronizable = ((Object)Result) as ISynchronizableWithLocalClock;
            if (synchronizable != null) synchronizable.SynchronizeWithLocalClock(drift);

        }
        #endregion
    }



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
        /// The error occured during the XSL transformation.
        /// </summary>
        Xslt,
        /// <summary>
        /// The error occured during the XML deserialization.
        /// </summary>
        Xml,
        /// <summary>
        /// It was a managed CCP error.
        /// </summary>
        CCP
    }

}
