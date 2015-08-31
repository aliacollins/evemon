using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite.EF6;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql
{
    internal class SqliteDbConfiguration : DbConfiguration
    {
        public SqliteDbConfiguration()
        {
            string assemblyName = typeof(SQLiteProviderFactory).Assembly.GetName().Name;

            RegisterDbProviderFactories(assemblyName);
            SetProviderFactory(assemblyName, SQLiteProviderFactory.Instance);
            SetProviderServices(assemblyName,
                (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }

        /// <summary>
        /// Registers the database provider factories.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        static void RegisterDbProviderFactories(String assemblyName)
        {
            var dataSet = ConfigurationManager.GetSection("system.data") as DataSet;
            if (dataSet == null)
                return;

            var dbProviderFactoriesDataTable = dataSet.Tables.OfType<DataTable>()
                .First(x => x.TableName == typeof(DbProviderFactories).Name);

            var dataRow = dbProviderFactoriesDataTable.Rows.OfType<DataRow>()
                .FirstOrDefault(x => x.ItemArray[2].ToString() == assemblyName);

            if (dataRow != null)
                dbProviderFactoriesDataTable.Rows.Remove(dataRow);

            dbProviderFactoriesDataTable.Rows.Add(
                "SQLite Data Provider (Entity Framework 6)",
                ".NET Framework Data Provider for SQLite (Entity Framework 6)",
                assemblyName,
                typeof(SQLiteProviderFactory).AssemblyQualifiedName
                );
        }
    }
}
