using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    [XmlRoot("list")]
    public sealed class ZofuList<T>
    {
        public ZofuList()
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
