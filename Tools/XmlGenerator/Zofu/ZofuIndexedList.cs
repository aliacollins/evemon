using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.XmlGenerator;

namespace EVEMon.XmlImporter.Zofu
{
    [XmlRoot("list")]
    public sealed class ZofuIndexedList<T>
        where T:IHasID
    {
        public ZofuIndexedList()
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

    public interface IHasID
    {
        int ID { get; }
    }
}
