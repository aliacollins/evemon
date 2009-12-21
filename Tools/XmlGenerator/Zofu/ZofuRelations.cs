using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.XmlImporter;

namespace EVEMon.XmlGenerator.Zofu
{
    [XmlRoot("list")]
    public sealed class ZofuRelations<T>
        where T : class, IRelation
    {
        public ZofuRelations()
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

    public interface IRelation
    {
        int Left { get; }
        int Right { get; }
    }
}
