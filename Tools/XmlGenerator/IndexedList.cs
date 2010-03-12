using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    [XmlRoot("list")]
    public sealed class IndexedList<T>
        where T:IHasID
    {
        public IndexedList()
        {
            Items = new List<T>();
        }

        [XmlElement("item")]
        public List<T> Items
        {
            get;
            set;
        }

        public Bag<T> ToBag()
        {
            return new Bag<T>(this);
        }
    }
}
