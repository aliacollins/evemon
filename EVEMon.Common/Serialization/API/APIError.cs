using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// CCP Api error message handling - for application error messages within the xml.
    /// </summary>
    [XmlRoot("error")]
    public sealed class CCPError
    {
        [XmlAttribute("code")]
        /// <summary>
        /// If this is non 0 then an error has occurred
        /// </summary>
        /// <value>105</value> Invaid character id
        /// <value>201</value> Character does not belong to account
        /// <value>202</value> Invalid API Key
        /// <value>""</value> or <value>null</value> when the operation was successful (will be one of 5 values - "characterID", "TryAgainIn", "currentTimeTQOffset" and "currentTime"
        /// This is the variable that needs checking to see if the thing has worked.
        public int ErrorCode
        {
            get;
            set;
        }

        [XmlText]
        /// <summary>
        /// If this is non 0 length or not null then an error has occurred and will this will be the only value in the entire class other than "current time
        /// </summary>
        /// <value>"Invalid characterID."</value>
        /// <value>"Authentication Failure."</value>
        /// <value>"Cached API key authentication failure"</value>
        /// <value>"Character does not belong to account"</value>
        /// <value>""</value> or <value>null</value> when the operation was successful (will be one of 5 values - "characterID", "TryAgainIn", "currentTimeTQOffset" and "currentTime"
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets true whether a full key was required.
        /// </summary>
        [XmlIgnore]
        public bool IsLimitedKeyError
        {
            get { return (ErrorCode == 200); }
        }
    }
}
