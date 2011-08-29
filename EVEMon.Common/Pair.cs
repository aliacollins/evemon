using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("pair")]
    public class Pair<TypeA, TypeB>
    {
        public Pair()
        {
        }

        public Pair(TypeA a, TypeB b)
        {
            A = a;
            B = b;
        }

        public TypeA A { get; set; }

        public TypeB B { get; set; }

    }
}