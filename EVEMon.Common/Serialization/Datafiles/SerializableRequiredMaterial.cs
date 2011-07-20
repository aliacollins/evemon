using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a prerequisite material for a blueprint.
    /// </summary>
    public sealed class SerializableRequiredMaterial
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("quantity")]
        public int Quantity
        {
            get;
            set;
        }

        [XmlAttribute("damagePerJob")]
        public double DamagePerJob
        {
            get;
            set;
        }

        [XmlAttribute("activityId")]
        public int Activity
        {
            get;
            set;
        }

        [XmlAttribute("wasted")]
        public int WasteAffected
        {
            get;
            set;
        }
    }
}
