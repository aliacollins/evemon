using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Notifications;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class NotificationSettings
    {
        public NotificationSettings()
        {
            Categories = new SerializableDictionary<NotificationCategory, NotificationCategorySettings>();
            Categories[NotificationCategory.AccountNotInTraining] = 
                new NotificationCategorySettings(ToolTipNotificationBehaviour.RepeatUntiClicked);
            EmailPortNumber = 25;
        }

        [XmlElement("categories")]
        public SerializableDictionary<NotificationCategory, NotificationCategorySettings> Categories
        {
            get;
            set;
        }

        [XmlElement("playSoundOnSkillCompletion")]
        public bool PlaySoundOnSkillCompletion
        {
            get;
            set;
        }

        [XmlElement("sendMailAlert")]
        public bool SendMailAlert
        {
            get;
            set;
        }

        [XmlElement("useEmailShortFormat")]
        public bool UseEmailShortFormat
        {
            get;
            set;
        }

        [XmlElement("emailFromAddress")]
        public string EmailFromAddress
        {
            get;
            set;
        }

        [XmlElement("emailToAddress")]
        public string EmailToAddress
        {
            get;
            set;
        }

        [XmlElement("emailSmtpServer")]
        public string EmailSmtpServer
        {
            get;
            set;
        }

        [XmlElement("emailPortNumber")]
        public int EmailPortNumber
        {
            get;
            set;
        }

        [XmlElement("emailAuthenticationRequired")]
        public bool EmailAuthenticationRequired
        {
            get;
            set;
        }

        [XmlElement("emailAuthenticationUserName")]
        public string EmailAuthenticationUserName
        {
            get;
            set;
        }

        [XmlElement("emailAuthenticationPassword")]
        public string EmailAuthenticationPassword
        {
            get;
            set;
        }

        [XmlElement("emailServerRequiresSSL")]
        public bool EmailServerRequiresSSL
        {
            get;
            set;
        }

        internal NotificationSettings Clone()
        {
            var clone = (NotificationSettings)MemberwiseClone();

            // Add copy of behaviours
            clone.Categories = new SerializableDictionary<NotificationCategory, NotificationCategorySettings>();
            foreach (var pair in this.Categories) 
            {
                clone.Categories[pair.Key] = pair.Value.Clone();
            }
            return clone;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class NotificationCategorySettings
    {
        public NotificationCategorySettings()
        {
            ToolTipBehaviour = ToolTipNotificationBehaviour.Once;
            ShowOnMainWindow = true;
        }

        public NotificationCategorySettings(ToolTipNotificationBehaviour toolTipBehaviour)
        {
            ToolTipBehaviour = toolTipBehaviour;
            ShowOnMainWindow = true;
        }

        [XmlAttribute("toolTipBehaviour")]
        public ToolTipNotificationBehaviour ToolTipBehaviour
        {
            get;
            set;
        }

        [XmlAttribute("showOnMainWindow")]
        public bool ShowOnMainWindow
        {
            get;
            set;
        }

        public NotificationCategorySettings Clone()
        {
            return (NotificationCategorySettings)MemberwiseClone();
        }
    }

    /// <summary>
    /// Represents the behaviour of the tooltip notifications (alerts for skills completion, etc)
    /// </summary>
    public enum ToolTipNotificationBehaviour
    {
        /// <summary>
        /// Never notify
        /// </summary>
        Never = 0,
        /// <summary>
        /// Notify once only 
        /// </summary>
        Once = 1,
        /// <summary>
        /// Every minutes, the warning is repeated until the user clicks the tooltip
        /// </summary>
        RepeatUntiClicked = 2
    }


}
