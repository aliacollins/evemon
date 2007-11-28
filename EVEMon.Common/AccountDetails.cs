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
            set { m_storedCharacterList = value; }
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
