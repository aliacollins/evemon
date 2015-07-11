using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using EVEMon.Common.Data;
using EVEMon.Common.MarketPricer;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MarketPricerSettings
    {
        private static readonly Dictionary<string, ItemPricer> s_pricer = new Dictionary<string, ItemPricer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketPricerSettings"/> class.
        /// </summary>
        public MarketPricerSettings()
        {
            ProviderName = "EveAddicts";

            foreach (ItemPricer pricer in ItemPricer.Providers)
            {
                s_pricer[pricer.Name] = pricer;
            }
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        [XmlAttribute("provider")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets the pricer.
        /// </summary>
        /// <value>
        /// The pricer.
        /// </value>
        [XmlIgnore]
        public ItemPricer Pricer
        {
            get { return s_pricer[ProviderName]; }
        }
    }
}
