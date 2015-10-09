using System;
using System.Diagnostics;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;

namespace EVEMon.SDEToSQL.Importers.YamlToSQL
{
    internal class YamlImporter : IImporter
    {
        private readonly DbConnectionProvider m_sqlConnectionProvider;

        private bool m_isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlImporter"/> class.
        /// </summary>
        internal YamlImporter(DbConnectionProvider sqlConnectionProvider)
        {
            if (sqlConnectionProvider == null)
                throw new ArgumentNullException("sqlConnectionProvider");

            m_sqlConnectionProvider = sqlConnectionProvider;

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

            if (Debugger.IsAttached)
            {
                Types.Import(m_sqlConnectionProvider);
                return;
            }

            Categories.Import(m_sqlConnectionProvider);
            Groups.Import(m_sqlConnectionProvider);
            Graphics.Import(m_sqlConnectionProvider);
            Icons.Import(m_sqlConnectionProvider);
            Skins.Import(m_sqlConnectionProvider);
            SkinMaterials.Import(m_sqlConnectionProvider);
            SkinLicenses.Import(m_sqlConnectionProvider);
            Types.Import(m_sqlConnectionProvider);
            Certificates.Import(m_sqlConnectionProvider);
            Blueprints.Import(m_sqlConnectionProvider);
        }
    }
}
