using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    /// <summary>
    /// Performs a comparison between two <see cref="EveMailMessage"/> types.
    /// </summary>
    public sealed class EveMailMessageComparer : Comparer<EveMailMessage>
    {
        private readonly EveMailMessagesColumn m_column;
        private readonly bool m_isAscending;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailMessageComparer"/> class.
        /// </summary>
        /// <param name="column">The industry job column.</param>
        /// <param name="isAscending">Is ascending flag.</param>
        public EveMailMessageComparer(EveMailMessagesColumn column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="EveMailMessage" /> type and returns a value
        /// indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public override int Compare(EveMailMessage x, EveMailMessage y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        /// <summary>
        /// Performs a comparison of two objects of the <see cref="EveMailMessage" /> type and returns a value
        /// indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        private int CompareCore(EveMailMessage x, EveMailMessage y)
        {
            switch (m_column)
            {
                case EveMailMessagesColumn.SenderName:
                    return String.Compare(x.Sender, y.Sender, StringComparison.CurrentCulture);

                case EveMailMessagesColumn.Title:
                    return String.Compare(x.Title, y.Title, StringComparison.CurrentCulture);

                case EveMailMessagesColumn.SentDate:
                    return x.SentDate.CompareTo(y.SentDate);

                case EveMailMessagesColumn.ToCharacters:
                    return String.Compare(x.ToCharacters.First(), y.ToCharacters.First(), StringComparison.CurrentCulture);

                case EveMailMessagesColumn.ToCorpOrAlliance:
                    return String.Compare(x.ToCorpOrAlliance, y.ToCorpOrAlliance, StringComparison.CurrentCulture);

                case EveMailMessagesColumn.ToMailingList:
                    return String.Compare(x.ToMailingLists.First(), y.ToMailingLists.First(), StringComparison.CurrentCulture);

                default:
                    return 0;
            }
        }
    }
}