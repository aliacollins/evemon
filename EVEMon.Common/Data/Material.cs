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
        /// <param name="src">The SRC.</param>
        public Material(SerializableMaterialQuantity src)
        {
            Product = StaticItems.GetItemByID(src.ID);
            Quantity = src.Quantity;
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets the reprocessing product.
        /// </summary>
        public Item Product { get; private set; }

        /// <summary>
        /// Gets the produced quantity.
        /// </summary>
        public int Quantity { get; private set; }

        #endregion
    }
}
