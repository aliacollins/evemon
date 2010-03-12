using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator
{
    [XmlRoot("list")]
    public sealed class Relations<T>
        where T : class, IRelation
    {
        public Relations()
        {
            Items = new List<T>();
        }

        [XmlElement("item")]
        public List<T> Items
        {
            get;
            set;
        }

        public RelationSet<T> ToSet()
        {
            return new RelationSet<T>(Items);
        }
    }
}
