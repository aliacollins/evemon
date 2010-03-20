using System.Xml.Serialization;
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;
using System;
using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;
using System.ComponentModel;

namespace EVEMon.Common
{
    /// <summary>
    /// Enumeration of the supported API methods. Each method should have an entry in APIMethods and
    /// an equivalent string entry in APIConstants indicating the default path of the method.
    /// </summary>
    public enum APIMethods
    {
        None = 0,

        /// <summary>
        /// The Tranquility server status
        /// </summary>
        [Header("Tranquility Status")]
        [Description("The status of the Tranquility server.")]
        [Update(UpdatePeriod.Minutes5, UpdatePeriod.Never, UpdatePeriod.Hours1)]
        ServerStatus = 1,

        /// <summary>
        /// The characters available on an account.
        /// </summary>
        [Header("Characters on Account")]
        [Description("The retrieval of the characters list available on every account.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1)]
        [ForcedOnStartup]
        CharacterList = 2,

        /// <summary>
        /// A character sheet (bio, skills, implants, etc).
        /// </summary>
        [Header("Character Sheet")]
        [Description("A character sheet lists biography, skills, attributes and implants informations about a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1)]
        CharacterSheet = 3,

        /// <summary>
        /// A character's skill queue.
        /// </summary>
        [Header("Skill Queue")]
        [Description("A character skill queue.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1)]
        SkillQueue = 4,

        /// <summary>
        /// The market orders for a character.
        /// </summary>
        [FullKey]
        [Header("Market Orders")]
        [Description("The market orders for a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1)]
        MarketOrders = 5,

        /// <summary>
        /// A frequently updated wallet balance. Only used for testing whether the API key is full or limited.
        /// </summary>
        [FullKey]
        CharacterAccountBalance = 6,

        /// <summary>
        /// The conquerable station list. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        ConquerableStationList = 7,

        /// <summary>
        /// Mail messages for a character. Only downloaded when a full API key is provided.
        /// </summary>
        [Header("Mail Messages")]
        [Description("Mail messages for a charater.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30)]
        [FullKey]
        MailMessages = 8,

        /// <summary>
        /// Notifications for a character. Only downloaded when a full API key is provided.
        /// </summary>
        [Header("Notifications")]
        [Description("Notifications messages for a charater.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30)]
        [FullKey]
        Notifications = 9
    }

    /// <summary>
    /// Serializable class for an API method and its path. Each APIConfiguration maintains a list of APIMethods.
    /// </summary>
    public class APIMethod
    {
        private APIMethods m_method;
        private string m_path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        public APIMethod(APIMethods method, string path)
        {
            m_method = method;
            m_path = path;
        }

        /// <summary>
        /// Returns the APIMethods enumeration member for this APIMethod.
        /// </summary>
        public APIMethods Method
        {
            get { return m_method; }
        }

        /// <summary>
        /// Returns the defined URL suffix path for this APIMethod.
        /// </summary>
        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        /// <summary>
        /// Creates a set of API methods with their default urls.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<APIMethod> CreateDefaultSet()
        {
            foreach (string methodName in Enum.GetNames(typeof(APIMethods)))
            {
                APIMethods methodEnum = (APIMethods)Enum.Parse(typeof(APIMethods), methodName);
                string methodURL = NetworkConstants.ResourceManager.GetString("API" + methodName);
                yield return new APIMethod(methodEnum, methodURL);
            }
        }

    }
}
