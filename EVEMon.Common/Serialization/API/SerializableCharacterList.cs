using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a characters list. Used for querying CCP.
    /// </summary>
    public sealed class SerializableCharacterList
    {
        /// <summary>
        /// Default constructor for <see cref="XMLSerializer"/>
        /// </summary>
        public SerializableCharacterList()
        {
            Characters = new List<SerializableCharacterListItem>();
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public List<SerializableCharacterListItem> Characters
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a reference to a character in the charactersList API
    /// </summary>
    public sealed class SerializableCharacterListItem : ISerializableCharacterIdentity
    {
        /// <summary>
        /// Default constructor for XML serialization
        /// </summary>
        public SerializableCharacterListItem()
        {
        }

        [XmlAttribute("characterID")]
        public long ID
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

        [XmlAttribute("corporationName")]
        public string CorporationName
        {
            get;
            set;
        }

        [XmlAttribute("corporationID")]
        public long CorporationID
        {
            get;
            set;
        }
    }
}
