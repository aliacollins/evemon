using System;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableMedalsListItem
    {
        [XmlAttribute("medalID")]
        public long MedalID { get; set; }

        [XmlAttribute("reason")]
        public string ReasonXml
        {
            get { return Reason; }
            set { Reason = value == null ? String.Empty : value.HtmlDecode(); }
        }

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
        public string TitleXml
        {
            get { return Title; }
            set { Title = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("description")]
        public string DescriptionXml
        {
            get { return Description; }
            set { Description = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlIgnore]
        public string Reason { get; set; }

        [XmlIgnore]
        public long IssuerID { get; set; }

        [XmlIgnore]
        public DateTime Issued { get; set; }

        [XmlIgnore]
        public MedalGroup Group { get; set; }

        [XmlIgnore]
        public string Title { get; set; }

        [XmlIgnore]
        public string Description { get; set; }
    }
}