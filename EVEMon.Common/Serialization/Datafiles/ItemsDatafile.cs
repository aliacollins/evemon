using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our items datafile.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("itemsDatafile")]
    public sealed class ItemsDatafile
    {
        private readonly Collection<SerializableMarketGroup> m_marketGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsDatafile"/> class.
        /// </summary>
        public ItemsDatafile()
        {
            m_marketGroups = new Collection<SerializableMarketGroup>();
        }

        /// <summary>
        /// Gets the market groups.
        /// </summary>
        /// <value>The market groups.</value>
        [XmlElement("marketGroup")]
        public Collection<SerializableMarketGroup> MarketGroups
        {
            get { return m_marketGroups; }
        }

        /// <summary>
        /// Adds the specified market groups.
        /// </summary>
        /// <param name="marketGroups">The market groups.</param>
        public void AddRange(IEnumerable<SerializableMarketGroup> marketGroups)
        {
            m_marketGroups.Clear();
            marketGroups.ToList().ForEach(marketGroup => m_marketGroups.Add(marketGroup));
        }
    }
}