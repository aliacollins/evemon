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
        AccountNotInTraining,
        /// <summary>
        /// A skill training has been completed.
        /// </summary>
        [Header("Skill completion")]
        SkillCompletion,
        /// <summary>
        /// A character has not enough balance to fulfill its buy orders.
        /// </summary>
        [Header("Insufficient balance")]
        InsufficientBalance,
        /// <summary>
        /// A character has not enough skill points on his clone.
        /// </summary>
        [Header("Insufficient clone")]
        InsufficientClone,
        /// <summary>
        /// An error occured while the querying of the API.
        /// </summary>
        [Header("API problem")]
        QueryingError,
        /// <summary>
        /// The status of Tranquility changed
        /// </summary>
        [Header("Tranquility status change")]
        ServerStatusChange,
        /// <summary>
        /// The status of Tranquility changed
        /// </summary>
        [Header("IGB service problem")]
        IgbServiceException,
        /// <summary>
        /// Some orders expired since the last time.
        /// </summary>
        [Header("Market orders expired/fulfilled")]
        MarketOrdersEnding,
        /// <summary>
        /// Skill queue has room for more skills.
        /// </summary>
        [Header("Skill queue room availability")]
        SkillQueueRoomAvailable,
        /// <summary>
        /// Testing notification.
        /// </summary>
        [Header("Test Notification")]
        TestNofitication
    }
}
