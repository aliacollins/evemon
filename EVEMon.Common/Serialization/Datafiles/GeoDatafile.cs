using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents out certificates datafile
    /// </summary>
    [XmlRoot("geographyDatafile")]
    public sealed class GeoDatafile
    {
        [XmlArray("regions")]
        [XmlArrayItem("region")]
        public SerializableRegion[] Regions
        {
            get;
            set;
        }

        [XmlArray("jumps")]
        [XmlArrayItem("jump")]
        public SerializableJump[] Jumps
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a region of the universe.
    /// </summary>
    public sealed class SerializableRegion
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("constellations")]
        public SerializableConstellation[] Constellations
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a constellation.
    /// </summary>
    public sealed class SerializableConstellation
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("systems")]
        public SerializableSolarSystem[] Systems
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Represents a solar system.
    /// </summary>
    public sealed class SerializableSolarSystem
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("x")]
        public int X
        {
            get;
            set;
        }
        [XmlAttribute("y")]
        public int Y
        {
            get;
            set;
        }
        [XmlAttribute("z")]
        public int Z
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("securityLevel")]
        public float SecurityLevel
        {
            get;
            set;
        }

        [XmlElement("stations")]
        public SerializableStation[] Stations
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a solar system.
    /// </summary>
    public sealed class SerializableStation
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("corporationID")]
        public int CorporationID
        {
            get;
            set;
        }

        [XmlElement("reprocessingEfficiency")]
        public float ReprocessingEfficiency
        {
            get;
            set;
        }

        [XmlElement("reprocessingStationsTake")]
        public float ReprocessingStationsTake
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a connection between two gates.
    /// </summary>
    public sealed class SerializableJump
    {
        [XmlAttribute("id1")]
        public int FirstSystemID
        {
            get;
            set;
        }

        [XmlAttribute("id2")]
        public int SecondSystemID
        {
            get;
            set;
        }
    }

}
