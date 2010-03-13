using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// Represents the categories a <see cref="Notification"/> can have.
    /// </summary>
    public enum NotificationCategory
    {
        /// <summary>
        /// None of the characters are currently in training.
        /// </summary>
        [Header("Account is not training")]
        AccountNotInTraining = 0,
        /// <summary>
        /// A skill training has been completed.
        /// </summary>
        [Header("Skill completion")]
        SkillCompletion = 1,
        /// <summary>
        /// A character has not enough skill points on his clone.
        /// </summary>
        [Header("Insufficient clone")]
        InsufficientClone = 2,
        /// <summary>
        /// An error occured while the querying of the API.
        /// </summary>
        [Header("API problem")]
        QueryingError = 3,
        /// <summary>
        /// The status of Tranquility changed
        /// </summary>
        [Header("Tranquility status change")]
        ServerStatusChange = 4,
        /// <summary>
        /// The status of Tranquility changed
        /// </summary>
        [Header("IGB service problem")]
        IgbServiceException = 5,
        /// <summary>
        /// Some orders expired since the last time.
        /// </summary>
        [Header("Market orders expired/fulfilled")]
        MarketOrdersEnding = 6,
        /// <summary>
        /// Skill queue has room for more skills.
        /// </summary>
        [Header("Skill queue room availability")]
        SkillQueueRoomAvailable = 7,
        /// <summary>
        /// Testing notification.
        /// </summary>
        [Header("Test Notification")]
        TestNofitication = 8
    }
}
