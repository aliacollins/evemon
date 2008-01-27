using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    public class AccountDetails
    {
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

        private DateTime m_cachedUntil = DateTime.MinValue;
        public DateTime CachedUntil
        {
            get { return m_cachedUntil; }
            set { m_cachedUntil = value; }
        }

        private List<Pair<string, int>> m_storedCharacterList = null;
        public List<Pair<string, int>> StoredCharacterList
        {
            get { return m_storedCharacterList; }
            set 
            { 
                m_storedCharacterList = value; 
            }
        }


        public void CheckForTransfer()
        {
            // check to see if any characters on this account have been transferred
            // from another account
            foreach (AccountDetails acd in Settings.GetInstance().Accounts)
            {
                if (acd == this) continue;
                foreach (Pair<string, int> p in m_storedCharacterList)
                {
                    acd.RemoveCharacter(p.B);
                }
            }

        }
        /// <summary>
        /// Remove a specific character id from this account if it exists.
        /// This is done by copying existing list to a new list, but skipping
        /// any character id that matches the parameter.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="charid"></param>
        public void RemoveCharacter(int charid)
        {
            List<Pair<string, int>> newList = new List<Pair<string, int>>();
            foreach (Pair<string, int> character in m_storedCharacterList)
            {
                if (character.B != charid)
                {
                    newList.Add(character);
                }
            }
            m_storedCharacterList = newList;
        }

        public bool HasCharacter(string charName)
        {
            if (m_storedCharacterList == null) return false;

            foreach (Pair<string, int> p in m_storedCharacterList)
            {
                if (p.A == charName) return true;
            }
            return false;
        }

        public List<string> GetCharacterNames()
        {
            List<string> charList = new List<string>();
            if (m_storedCharacterList == null) return charList;

            foreach (Pair<string, int> p in m_storedCharacterList)
            {
                charList.Add(p.A);
            }
            return charList;
        }
    }
}
