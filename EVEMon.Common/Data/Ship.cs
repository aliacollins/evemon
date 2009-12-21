using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a readonly ship definition
    /// </summary>
    public class Ship : Item
    {
        private readonly StaticRecommendations<StaticCertificate> m_recommendations = new StaticRecommendations<StaticCertificate>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ship"></param>
        internal Ship(MarketGroup group, SerializableItem src)
            : base(group, src)
        {
        }

        /// <summary>
        /// Gets the receommended certificates
        /// </summary>
        public StaticRecommendations<StaticCertificate> Recommendations
        {
            get { return m_recommendations; }
        }
    }
 }
