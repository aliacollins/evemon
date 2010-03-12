using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    [XmlRoot("list")]
    public sealed class SimpleList<T>
    {
        public SimpleList()
        {
            Items = new List<T>();
        }

        [XmlElement("item")]
        public List<T> Items
        {
            get;
            set;
        }
    }
}
