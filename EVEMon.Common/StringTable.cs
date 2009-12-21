using System;
using System.Collections.Generic;

namespace EVEMon.Common
{
    /// <summary>
    /// Maintains a pseudo-static dictionary mapping strings to a single instance of
    /// the string. This saves memory in places like the item database, which contains
    /// thousands of idential strings that otherwise would be stored multiple times in
    /// memory. By using a string table, all copies of identical strings can point to the
    /// same singular copy in memory.
    /// </summary>
    public class StringTable
    {
        public static StringTableContext GetInstanceScope()
        {
            return new StringTableContext();
        }

        private static StringTable sm_scopedInstance = null;

        public class StringTableContext : IDisposable
        {
            private bool m_disposes;

            public StringTableContext()
            {
                if (sm_scopedInstance == null)
                {
                    m_disposes = true;
                    sm_scopedInstance = GetInstance();
                }
                else
                {
                    m_disposes = false;
                }
            }

            #region IDisposable Members
            public void Dispose()
            {
                if (m_disposes)
                {
                    sm_scopedInstance = null;
                }
            }
            #endregion
        }

        private static WeakReference<StringTable> sm_tableRef;

        public static StringTable GetInstance()
        {
            if (sm_scopedInstance != null)
            {
                return sm_scopedInstance;
            }

            StringTable st = null;
            if (sm_tableRef != null)
            {
                st = sm_tableRef.Target;
            }
            if (st == null)
            {
                st = new StringTable();
                sm_tableRef = new WeakReference<StringTable>(st);
            }
            return st;
        }

        private StringTable()
        {
        }

        private Dictionary<string, string> m_table = new Dictionary<string, string>();

        public Dictionary<string, string> Table
        {
            get { return m_table; }
        }

        public static string GetSharedString(string value)
        {
            StringTable st = GetInstance();
            if (st.Table.ContainsKey(value))
            {
                return st.Table[value];
            }
            st.Table[value] = value;
            return value;
        }
    }
}