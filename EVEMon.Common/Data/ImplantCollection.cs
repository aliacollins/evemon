using System.Linq;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a collection of all the implants bound to a given group.
    /// </summary>
    public sealed class ImplantCollection : ReadonlyCollection<Implant>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="slot"></param>
        internal ImplantCollection(ImplantSlots slot)
        {
            Slot = slot;
        }

        #endregion


        #region Properties

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
            get { return Items.FirstOrDefault(implant => implant.Name == name); }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Add an implant to this slot.
        /// </summary>
        /// <param name="implant"></param>
        internal void Add(Implant implant)
        {
            Items.Add(implant);
        }

        #endregion
    }
}