using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

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

    //now with encryption.  Not bulletproof by any means, but better than plaintext
    public class CharLoginInfo : ICharacterSettings
    {
        private string m_username;
        private bool m_ineveSync;

        public string Username
        {
            get { return m_username; }
            set { m_username = value; }
        }

        private const string ENCRYPTED_PREFIX = "ENCRYPTED::";

        private string m_encryptedPassword = String.Empty;
        private string m_password = String.Empty;

        [XmlElement("Password")]
        public string EncryptedPassword
        {
            get
            {
                if (String.IsNullOrEmpty(m_encryptedPassword))
                {
                    m_encryptedPassword = EncryptionHelper.Encrypt(m_username, m_password);
                    if (m_encryptedPassword == m_password)
                    {
                        m_encryptedPassword = String.Empty;
                        return m_password;
                    }
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(ENCRYPTED_PREFIX);
                sb.Append(m_encryptedPassword);
                return sb.ToString();
            }
            set
            {
                if (!value.StartsWith(ENCRYPTED_PREFIX))
                {
                    m_encryptedPassword = String.Empty;
                    m_password = value;
                }
                else
                {
                    m_encryptedPassword = value.Substring(ENCRYPTED_PREFIX.Length);
                    m_password = String.Empty;
                }
            }
        }

        [XmlIgnore]
        public string Password
        {
            get
            {
                if (String.IsNullOrEmpty(m_password) && !String.IsNullOrEmpty(m_encryptedPassword))
                {
                    m_password = EncryptionHelper.Decrypt(m_username, m_encryptedPassword);
                }
                return m_password;
            }
            set
            {
                m_password = value;
                m_encryptedPassword = String.Empty;
            }
        }



        public bool Validate()
        {
            EveSession s = EveSession.GetSession(m_username, m_password);
            return (s.GetCharacterId(m_characterName) > 0);
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
                    SerializableCharacterInfo sci = SerializableCharacterInfo.CreateFromFile(m_filename);
                    m_characterName = sci.Name;
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
