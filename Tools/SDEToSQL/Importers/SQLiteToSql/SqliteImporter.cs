using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EVEMon.SDEToSQL.Importers.SQLiteToSQL.Models;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;

namespace EVEMon.SDEToSQL.Importers.SQLiteToSQL
{
    internal class SqliteImporter : IImporter
    {
        private readonly DbConnectionProvider m_sqliteConnectionProvider;
        private readonly SqlConnectionProvider m_sqlConnectionProvider;

        private bool m_isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteImporter" /> class.
        /// </summary>
        /// <param name="sqliteConnectionProvider">The sqlite provider.</param>
        /// <param name="sqlConnectionProvider">The SQL provider.</param>
        /// <exception cref="System.ArgumentNullException">sqliteConnectionProvider
        /// or
        /// sqlConnectionProvider</exception>
        internal SqliteImporter(DbConnectionProvider sqliteConnectionProvider, DbConnectionProvider sqlConnectionProvider)
        {
            if (sqliteConnectionProvider == null)
                throw new ArgumentNullException("sqliteConnectionProvider");

            if (sqlConnectionProvider == null)
                throw new ArgumentNullException("sqlConnectionProvider");

            m_sqliteConnectionProvider = sqliteConnectionProvider;
            m_sqlConnectionProvider = (SqlConnectionProvider)sqlConnectionProvider;

            Util.Closing += Util_Closing;
        }

        /// <summary>
        /// Handles the Closing event of the Program control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Util_Closing(object sender, EventArgs e)
        {
            m_isClosing = true;
        }

        /// <summary>
        /// Imports the files.
        /// </summary>
        public void ImportFiles()
        {
            if (m_isClosing)
                return;

            if (m_sqlConnectionProvider.Connection == null || m_sqliteConnectionProvider.Connection == null)
                return;

            using (UniverseData universeDataContext = new UniverseData(m_sqliteConnectionProvider.Connection))
            {
                if (Debugger.IsAttached)
                    m_sqlConnectionProvider.Import(universeDataContext.mapRegions);
                else
                {
                    typeof(UniverseData).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .ToList()
                        .ForEach(property =>
                        {
                            if (!m_isClosing)
                                m_sqlConnectionProvider.Import(property.GetValue(universeDataContext, null) as IQueryable<object>);
                        });
                }

                m_sqliteConnectionProvider.CloseConnection();
            }
        }
    }
}
