using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializablePlanetaryColony
    {
        [XmlAttribute("planetID")]
        public long PlanetID { get; set; }

        [XmlAttribute("planetName")]
        public string PlanetName { get; set; }

        [XmlAttribute("planetTypeID")]
        public int PlanetTypeID { get; set; }

        [XmlAttribute("planetTypeName")]
        public string PlanetTypeName { get; set; }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlAttribute("solarSystemName")]
        public string SolarSystemName { get; set; }

        [XmlAttribute("ownerID")]
        public long OwnerID { get; set; }

        [XmlAttribute("ownerName")]
        public string OwnerName { get; set; }

        [XmlAttribute("lastUpdate")]
        public string LastUpdateXml
        {
            get { return LastUpdate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    LastUpdate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("upgradeLevel")]
        public int UpgradeLevel { get; set; }

        [XmlAttribute("numberOfPins")]
        public int NumberOfPins { get; set; }

        [XmlIgnore]
        public DateTime LastUpdate { get; set; }
    }
}