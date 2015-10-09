using System;
using System.Data.SQLite;

namespace EVEMon.SDEToSQL.Providers
{
    internal class SqliteConnectionProvider : DbConnectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteConnectionProvider"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        internal SqliteConnectionProvider(String nameOrConnectionString)
        {
            CreateConnection<SQLiteConnection>(nameOrConnectionString);
        }
    }
}
