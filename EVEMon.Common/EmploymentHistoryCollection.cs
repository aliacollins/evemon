using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class EmploymentHistoryCollection : ReadonlyCollection<EmploymentRecord>
    {
        private readonly Character m_character;


        #region Constructor

        public EmploymentHistoryCollection(Character character)
        {
            m_character = character;
        }

        #endregion


        #region Import & Export Methods

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable objects from the API.</param>
        internal void Import(IEnumerable<SerializableEmploymentHistoryListItem> src)
        {
            m_items.Clear();

            // Import the standings from the API
            foreach (SerializableEmploymentHistoryListItem srcEmploymentRecord in src)
            {
                m_items.Add(new EmploymentRecord(m_character, srcEmploymentRecord));
            }
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable standings from the API.</param>
        internal void Import(IEnumerable<SerializableEmploymentHistory> src)
        {
            m_items.Clear();

            foreach (SerializableEmploymentHistory srcEmploymentRecord in src)
            {
                m_items.Add(new EmploymentRecord(m_character, srcEmploymentRecord));
            }
        }

        /// <summary>
        /// Exports the standings to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable research points.</returns>
        internal List<SerializableEmploymentHistory> Export()
        {
            List<SerializableEmploymentHistory> serial = new List<SerializableEmploymentHistory>(m_items.Count);
            serial.AddRange(m_items.Select(employmentRecord => employmentRecord.Export()));
            return serial;
        }

        #endregion
    }
}
