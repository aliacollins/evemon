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
}
