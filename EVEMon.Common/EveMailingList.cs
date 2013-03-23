using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailingList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailingList"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal EveMailingList(SerializableMailingListsListItem src)
        {
            ID = src.ID;
            Name = src.DisplayName;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        internal long ID { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        internal string Name { get; private set; }
    }
}