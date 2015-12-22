using System;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    public sealed class OutputAttributeEnhancer
    {
        [XmlAttribute("attribute")]
        public ImplantSlots Attribute { get; set; }

        [XmlAttribute("bonus")]
        public Int64 Bonus { get; set; }

        [XmlAttribute("name")]
        public String Name { get; set; }
    }
}