using System;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a product of the reprocessing.
    /// </summary>
    public sealed class Material
    {
        # region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="src">The source.</param>
        public Material(SerializableMaterialQuantity src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            Item = StaticItems.GetItemByID(src.ID);
            Quantity = src.Quantity;
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets the reprocessing item.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// Gets the reprocessed quantity.
        /// </summary>
        public long Quantity { get; private set; }

        #endregion
    }
}