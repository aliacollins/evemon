using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class ContractItem
    {
        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        internal ContractItem(SerializableContractItemsListItem src)
        {
            RecordID = src.RecordID;
            Item = StaticItems.GetItemByID(src.TypeID);
            Quantity = src.Quantity;
            RawQuantity = src.RawQuantity;
            Singleton = src.Singleton;
            Included = src.Included;
        }


        #region Properties

        /// <summary>
        /// Gets or sets the record ID.
        /// </summary>
        /// <value>The record ID.</value>
        public long RecordID { get; private set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public Item Item { get; private set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public long Quantity { get; private set; }

        /// <summary>
        /// Gets or sets the raw quantity.
        /// </summary>
        /// <value>The raw quantity.</value>
        public short RawQuantity { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContractItem"/> is singleton.
        /// </summary>
        /// <value><c>true</c> if singleton; otherwise, <c>false</c>.</value>
        public bool Singleton { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContractItem"/> is included.
        /// </summary>
        /// <value><c>true</c> if included; otherwise, <c>false</c>.</value>
        public bool Included { get; private set; }


        #endregion
    }
}