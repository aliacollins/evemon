using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class PlanSorting
    {
        public PlanSorting()
        {
            Criteria = PlanEntrySort.None;
            Order = ThreeStateSortOrder.None;
        }

        [XmlAttribute("criteria")]
        public PlanEntrySort Criteria { get; set; }

        [XmlAttribute("order")]
        public ThreeStateSortOrder Order { get; set; }

        [XmlAttribute("groupByPriority")]
        public bool GroupByPriority { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal PlanSorting Clone()
        {
            return (PlanSorting)MemberwiseClone();
        }
    }
}
