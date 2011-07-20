using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a connection between two jump gates.
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
