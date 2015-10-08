using System;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace EVEMon.SDEToSQL.Importers.DataDumpToSQL
{
    internal class DataDumpImporter : IImporter
    {
        private readonly DbConnection m_sqlConnection;
        private readonly Restore m_restore;

        private bool m_isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDumpImporter" /> class.
        /// </summary>
        /// <param name="sqlConnectionProvider">The SQL provider.</param>
        /// <param name="restore">The restore.</param>
        /// <exception cref="System.ArgumentNullException">sqlConnection</exception>
        internal DataDumpImporter(DbConnectionProvider sqlConnectionProvider, Restore restore)
        {
            if (sqlConnectionProvider == null)
                throw new ArgumentNullException("sqlConnectionProvider");
            if (restore == null)
                throw new ArgumentNullException("restore");

            m_sqlConnection = sqlConnectionProvider.Connection;
            m_restore = restore;

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

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string filePath = Util.CheckDataDumpExists().Single();

            string text = String.Format(CultureInfo.InvariantCulture, "Restoring data dump to '{0}' database... ", m_sqlConnection.Database);
            Console.Write(text);

            try
            {
                ServerConnection serverConnection = new ServerConnection(m_sqlConnection.DataSource);
                Server server = new Server(serverConnection);

                string defaultDataPath = String.IsNullOrEmpty(server.Settings.DefaultFile)
                    ? server.MasterDBPath
                    : server.Settings.DefaultFile;
                string defaultLogPath = String.IsNullOrEmpty(server.Settings.DefaultLog)
                    ? server.MasterDBLogPath
                    : server.Settings.DefaultLog;

                m_restore.Database = m_sqlConnection.Database;
                m_restore.ReplaceDatabase = true;
                m_restore.PercentCompleteNotification = 1;
                m_restore.PercentComplete += Restore_PercentComplete;
                m_restore.Devices.AddDevice(filePath, DeviceType.File);
                m_restore.RelocateFiles.AddRange(
                    new[]
                    {
                        new RelocateFile("ebs_DATADUMP",
                            String.Format(CultureInfo.InvariantCulture, "{0}{1}.mdf", defaultDataPath, m_restore.Database)),
                        new RelocateFile("ebs_DATADUMP_log",
                            String.Format(CultureInfo.InvariantCulture, "{0}{1}_log.ldf", defaultLogPath, m_restore.Database))
                    });

                if (!m_isClosing)
                    m_restore.SqlRestore(server);

                Util.DisplayEndTime(stopwatch);
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                text = String.Format(CultureInfo.InvariantCulture, "Restoring data dump to '{0}' Database: Failed\n{1}",
                    m_sqlConnection.Database,
                    ex.Message);
                Util.HandleExceptionWithReason(text, text, ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Handles the PercentComplete event of the Restore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PercentCompleteEventArgs"/> instance containing the event data.</param>
        private static void Restore_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            Util.UpdatePercentDone(100);
        }
    }
}
