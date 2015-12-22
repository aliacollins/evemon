using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializablePlanetaryPin
    {
        [XmlAttribute("pinID")]
        public long PinID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }
        
        [XmlAttribute("typeName")]
        public string TypeName { get; set; }

        [XmlAttribute("schematicID")]
        public int SchematicID { get; set; }

        [XmlAttribute("cycleTime")]
        public short CycleTime { get; set; }

        [XmlAttribute("quantityPerCycle")]
        public int QuantityPerCycle { get; set; }

        [XmlAttribute("contentTypeID")]
        public int ContentTypeID { get; set; }

        [XmlAttribute("contentTypeName")]
        public string ContentTypeName { get; set; }

        [XmlAttribute("contentQuantity")]
        public int ContentQuantity { get; set; }

        [XmlAttribute("longitude")]
        public double Longitude { get; set; }

        [XmlAttribute("latitude")]
        public double Latitude { get; set; }

        [XmlAttribute("lastLaunchTime")]
        public string LastLaunchTimeXml
        {
            get { return LastLaunchTime.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    LastLaunchTime = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("installTime")]
        public string InstallTimeXml
        {
            get { return InstallTime.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    InstallTime = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("expiryTime")]
        public string ExpiryTimeXml
        {
            get { return ExpiryTime.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    ExpiryTime = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime LastLaunchTime { get; set; }

        [XmlIgnore]
        public DateTime InstallTime { get; set; }

        [XmlIgnore]
        public DateTime ExpiryTime { get; set; }
    }
}