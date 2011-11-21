using System.Xml.Serialization;
using EVEMon.Common.Notifications;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class NotificationSettings
    {
        private readonly SerializableDictionary<NotificationCategory, NotificationCategorySettings> m_categories;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSettings"/> class.
        /// </summary>
        public NotificationSettings()
        {
            m_categories = new SerializableDictionary<NotificationCategory, NotificationCategorySettings>();
            Categories[NotificationCategory.AccountNotInTraining] =
                new NotificationCategorySettings(ToolTipNotificationBehaviour.RepeatUntilClicked);
            EmailPortNumber = 25;
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [XmlElement("categories")]
        public SerializableDictionary<NotificationCategory, NotificationCategorySettings> Categories
        {
            get { return m_categories; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [play sound on skill completion].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [play sound on skill completion]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("playSoundOnSkillCompletion")]
        public bool PlaySoundOnSkillCompletion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send mail alert].
        /// </summary>
        /// <value><c>true</c> if [send mail alert]; otherwise, <c>false</c>.</value>
        [XmlElement("sendMailAlert")]
        public bool SendMailAlert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use email short format].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use email short format]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("useEmailShortFormat")]
        public bool UseEmailShortFormat { get; set; }

        /// <summary>
        /// Gets or sets the email from address.
        /// </summary>
        /// <value>The email from address.</value>
        [XmlElement("emailFromAddress")]
        public string EmailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the email to address.
        /// </summary>
        /// <value>The email to address.</value>
        [XmlElement("emailToAddress")]
        public string EmailToAddress { get; set; }

        /// <summary>
        /// Gets or sets the email SMTP server.
        /// </summary>
        /// <value>The email SMTP server.</value>
        [XmlElement("emailSmtpServer")]
        public string EmailSmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the email port number.
        /// </summary>
        /// <value>The email port number.</value>
        [XmlElement("emailPortNumber")]
        public int EmailPortNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [email authentication required].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [email authentication required]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("emailAuthenticationRequired")]
        public bool EmailAuthenticationRequired { get; set; }

        /// <summary>
        /// Gets or sets the name of the email authentication user.
        /// </summary>
        /// <value>The name of the email authentication user.</value>
        [XmlElement("emailAuthenticationUserName")]
        public string EmailAuthenticationUserName { get; set; }

        /// <summary>
        /// Gets or sets the email authentication password.
        /// </summary>
        /// <value>The email authentication password.</value>
        [XmlElement("emailAuthenticationPassword")]
        public string EmailAuthenticationPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [email server requires SSL].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [email server requires SSL]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("emailServerRequiresSSL")]
        public bool EmailServerRequiresSSL { get; set; }
    }
}