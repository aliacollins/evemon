using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a readonly ship definition.
    /// </summary>
    public class Ship : Item
    {
        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ship"></param>
        internal Ship(MarketGroup group, SerializableItem src)
            : base(group, src)
        {
            Recommendations = new StaticRecommendations<StaticCertificate>();
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets the recommended certificates.
        /// </summary>
        public StaticRecommendations<StaticCertificate> Recommendations { get; private set; }

        #endregion
    }
 }
