using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class Contact
    {
        private readonly long m_contactID;

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Contact(SerializableContactListItem src)
        {
            m_contactID = src.ContactID;
            ContactName = src.ContactName;
            IsInWatchlist = src.InWatchlist;
            Standing = src.Standing;
            Group = src.Group;
        }


        #region Public Properties

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <value>
        /// The name of the contact.
        /// </value>
        public string ContactName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the contact is in the watchlist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contact is in the watchlist; otherwise, <c>false</c>.
        /// </value>
        public bool IsInWatchlist { get; private set; }

        /// <summary>
        /// Gets the standing.
        /// </summary>
        public double Standing { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public ContactGroup Group { get; private set; }

        #endregion
    }
}