using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a product of the reprocessing.
    /// </summary>
    public class Material
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public Material(SerializableMaterialQuantity src)
        {
            src.ThrowIfNull(nameof(src));

            Item = StaticItems.GetItemByID(src.ID);
            Quantity = src.Quantity;
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets the reprocessing item.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the reprocessed quantity.
        /// </summary>
        public long Quantity { get; }

        #endregion
    }
}