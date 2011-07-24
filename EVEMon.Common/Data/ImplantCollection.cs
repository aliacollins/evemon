using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    #region ImplantSlot
    /// <summary>
    /// Represents a collection of all the implants bound to a given group.
    /// </summary>
    public sealed class ImplantCollection : ReadonlyCollection<Implant>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal ImplantCollection(ImplantSlots slot)
            : base()
        {
            Slot = slot;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the slot represented by this group.
        /// </summary>
        public ImplantSlots Slot { get; private set; }

        #endregion


        #region Indexers

        /// <summary>
        /// Gets an implant by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Implant this[string name]
        {
            get
            {
                foreach (var implant in m_items)
                {
                    if (implant.Name == name)
                        return implant;
                }
                return null;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Add an implant to this slot.
        /// </summary>
        /// <param name="implant"></param>
        internal void Add(Implant implant)
        {
            m_items.Add(implant);
        }

        #endregion

    }
    #endregion
}
