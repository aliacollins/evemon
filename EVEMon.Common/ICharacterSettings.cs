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

    //now with encryption.  Not bulletproof by any means, but better than plaintext
    public class CharLoginInfo : ICharacterSettings
    {
        private bool m_ineveSync;

        private int m_userId = 0;

        public int UserId
        {
            get { return m_userId; }
            set { m_userId = value; }
        }

        private string m_apiKey = string.Empty;
        public string ApiKey
        {
            get { return m_apiKey; }
            set { m_apiKey = value; }
        }


        /// <summary>
        /// Ensures that the api details are accurate and that the character belongs to this
        /// api key.
        /// </summary>
        /// <returns></returns>
        /// 
        /*
        public bool Validate()
        {
            // check that we have an user id, character id and api key
            bool invalid = (m_userId == 0 || m_apiKey == string.Empty);
            if (!invalid)
            {
                // yes, now see if the are valid
                string errMessage;
                List<Pair<string,int>> m_characterList = EveSession.GetCharacterList(Convert.ToString(m_userId),m_apiKey, out errMessage);
                invalid = (m_characterList.Count == 0);
                if (!invalid)
                {
                    // we have at least one character - see any of them belong to this api Key
                    invalid = true;
                    foreach (Pair<string,int> pair in m_characterList)
                    {
                        if (pair.A == m_characterName)
                        {
                            // found the guy!
                            invalid = false;
                            break;
                        }
                    }
                }
                /*
                if (invalid)
                {
                    // userid/apikey/char id is blank or char name cannot be found on this account
                    // pop up the change logon box
                    using (ChangeLoginWindow f = new ChangeLoginWindow())
                    {
                        f.ShowInvalidKey = true;
                        f.CharacterName = m_characterName;
                        DialogResult dr = f.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            m_userId = f.UserId;
                            m_apiKey = f.ApiKey;
                        }
                    }
                    return Validate();
                }
                 
             }
             return !invalid;
        }
*/
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
