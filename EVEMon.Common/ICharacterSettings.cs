using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace EVEMon.Common
{
    public interface ICharacterSettings
    {
        bool IneveSync
        {
            get;
            set;
        }

        string CharacterName
        {
            get;
            set;
        }

    }

    public class CharLoginInfo : ICharacterSettings
    {
        private bool m_ineveSync;

        // Deprecated - use the details in the AccountDetails class instead
        // keeping for backwards compatability
        private int m_userId = 0;
        public int UserId
        {
            get { return m_userId; }
            set { m_userId = value; }
        }

        // Deprecated - use the details in the AccountDetails class instead
       // keeping for backwards compatability
        private string m_apiKey = string.Empty;
        public string ApiKey
        {
            get { return m_apiKey; }
            set { m_apiKey = value; }
        }

        private AccountDetails m_account;

        [XmlIgnore]
        public AccountDetails Account
        {
            get { return m_account; }
            set { m_account = value; }
        }
	

        #region ICharacterSettings Members

        public bool IneveSync
        {
            get
            {
                return m_ineveSync;
            }
            set
            {
                m_ineveSync = value;
            }
        }
        private string m_characterName;

        public string CharacterName
        {
            get { return m_characterName; }
            set { m_characterName = value; }
        }
        #endregion
    }

    public class CharFileInfo : ICharacterSettings
    {
        private string m_filename;
        private bool m_monitorFile;
        private bool m_ineveSync;
        private string m_characterName;

        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        public bool MonitorFile
        {
            get { return m_monitorFile; }
            set { m_monitorFile = value; }
        }

        #region ICharacterSettings Members

        public bool IneveSync
        {
            get
            {
                return m_ineveSync;
            }
            set
            {
                m_ineveSync = value;
            }
        }

        #endregion

        #region ICharacterSettings Members


        public string CharacterName
        {
            get
            {
                if (m_characterName == null)
                {
                    SerializableCharacterSheet sci = SerializableCharacterSheet.CreateFromFile(m_filename);
                    m_characterName = sci.CharacterSheet.Name;
                }
                return m_characterName;
            }
            set
            {
                m_characterName = value;
            }
        }

        #endregion
    }
}
