using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableMedalsListItem
    {
        [XmlAttribute("medalID")]
        public long MedalID { get; set; }

        [XmlAttribute("reason")]
        public string Reason { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        // Xml attribute used in char medals call
        [XmlAttribute("issuerID")]
        public long IssuerIDXml
        {
            get { return IssuerID; }
            set
            {
                if (IssuerID == 0)
                    IssuerID = value;
            }
        }

        // Xml attribute used in corp medals call
        [XmlAttribute("creatorID")]
        public long CreatorIDXml
        {
            get { return IssuerID; }
            set
            {
                if (IssuerID == 0)
                    IssuerID = value;
            }
        }

        // Xml attribute used in char medals call
        [XmlAttribute("issued")]
        public string IssuedXml
        {
            get { return Issued.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value) && Issued == DateTime.MinValue)
                    Issued = value.TimeStringToDateTime();
            }
        }

        // Xml attribute used in corp medals call
        [XmlAttribute("created")]
        public string CreatedXml
        {
            get { return Issued.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value) && Issued == DateTime.MinValue)
                    Issued = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("corporationID")]
        public long CorporationID { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlIgnore]
        public long IssuerID { get; set; }

        [XmlIgnore]
        public DateTime Issued { get; set; }

        [XmlIgnore]
        public MedalGroup Group { get; set; }
    }
}