using System;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    class CCPAPIMapping
    {
        [XmlAttribute("version")]
        public int APIVersion
        {
            get;
            set;
        }

        [XmlElement("currentTime")]
        public string CurrentTimeXml
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
        public string CachedUntilXml
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
        public CCPAPIError CCPError
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
    }
}
