using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using EVEMon.SDEToSQL.Utils;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.Providers
{
    internal class SqlConnectionProvider : DbConnectionProvider
    {
        private readonly SqlConnection m_sqlConnection;
        private bool m_isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionProvider"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        internal SqlConnectionProvider(String nameOrConnectionString)
        {
            CreateConnection<SqlConnection>(nameOrConnectionString);
            m_sqlConnection = (SqlConnection)Connection;

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
        /// SQL insert command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlInsertCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Key);
                if (!parameter.Equals(parameters.Last()))
                    sb.Append(", ");
            }
            sb.Append(") VALUES (");

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Value);
                sb.Append(!parameter.Equals(parameters.Last()) ? ", " : ")");
            }

            return String.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} {1}", tableName, sb);
        }

        /// <summary>
        /// SQL update command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlUpdateCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> pars = parameters
                .Where(par =>
                    !par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase) &&
                    !par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var parameter in pars)
            {
                sb.AppendFormat("{0} = {1}", parameter.Key, parameter.Value);
                if (!parameter.Equals(pars.Last()))
                    sb.Append(", ");
            }

            if (!String.IsNullOrWhiteSpace(parameters["columnFilter1"]) && !String.IsNullOrWhiteSpace(parameters["id1"]))
            {
                for (int i = 1; i < 5; i++)
                {
                    string filterName = "columnFilter" + i;
                    string idName = "id" + i;

                    if (parameters.ContainsKey(filterName) &&
                        parameters.ContainsKey(idName) &&
                        !String.IsNullOrWhiteSpace(parameters[filterName]) &&
                        !String.IsNullOrWhiteSpace(parameters[idName]))
                    {
                        sb.AppendFormat("{0}{1} = {2}", i == 1 ? " WHERE " : " AND ", parameters[filterName], parameters[idName]);
                    }
                }
            }

            return String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET {1}", tableName, sb);
        }

        /// <summary>
        /// SQL delete command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlDeleteCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(parameters["columnFilter1"]) && !String.IsNullOrWhiteSpace(parameters["id1"]))
            {
                for (int i = 1; i < 5; i++)
                {
                    string filterName = "columnFilter" + i;
                    string idName = "id" + i;

                    if (parameters.ContainsKey(filterName) &&
                        parameters.ContainsKey(idName) &&
                        !String.IsNullOrWhiteSpace(parameters[filterName]) &&
                        !String.IsNullOrWhiteSpace(parameters[idName]))
                    {
                        sb.AppendFormat("{0}{1} = {2}", i == 1 ? " WHERE " : " AND ", parameters[filterName], parameters[idName]);
                    }
                }
            }

            return String.Format(CultureInfo.InvariantCulture, "DELETE {0}{1}", tableName, sb);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        /// <param name="searchKey">The search key.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        /// <returns>
        ///   <c>true</c> if the table is empty; otherwise, <c>false</c>.
        /// </returns>
        internal bool CreateTableOrColumns(YamlMappingNode rNode, string searchKey, string tableName,
            IDictionary<string, string> columns)
        {
            if (DropAndCreateTable(rNode, searchKey, tableName))
                return true;

            CreateColumns(tableName, columns);
            return false;
        }

        /// <summary>
        /// Drops and Creates the specified table.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        /// <param name="searchKey">The search key.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        internal bool DropAndCreateTable(YamlMappingNode rNode, string searchKey, string tableName)
        {
            if (!rNode.Children.Select(pair => pair.Value)
                .OfType<YamlMappingNode>()
                .Select(cNode => cNode.Any(x => x.Key.ToString() == searchKey))
                .Any(createTable => createTable))
            {
                return false;
            }

            DropAndCreateTable(tableName);
            return true;
        }

        /// <summary>
        /// Drops and Creates the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        internal void DropAndCreateTable(String tableName)
        {
            if (m_isClosing)
                return;

            if (Connection == null)
                return;

            using (DbCommand command = new SqlCommand(
                Util.GetScriptFor(tableName),
                m_sqlConnection,
                m_sqlConnection.BeginTransaction()))
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    Util.HandleExceptionForCommand(command, e);

                    if (command.Transaction != null)
                        command.Transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        private void CreateColumns(String tableName, IDictionary<string, string> columns)
        {
            if (m_isClosing)
                return;

            if (Connection == null)
                return;

            if (Connection.GetSchema("columns").Select(String.Format(CultureInfo.InvariantCulture, "TABLE_NAME = '{0}'", tableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", tableName);
                Util.PressAnyKey(-1);
            }

            foreach (KeyValuePair<string, string> column in columns)
            {
                CreateColumn(tableName, column.Key, column.Value);
            }
        }

        /// <summary>
        /// Creates the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        private void CreateColumn(String tableName, String columnName, String columnType, string defaultValue = "Null")
        {
            if (m_isClosing)
                return;

            if (Connection == null)
                return;

            if (Connection.GetSchema("columns")
                .Select(String.Format(CultureInfo.InvariantCulture, "TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", tableName, columnName))
                .Length != 0)
            {
                return;
            }

            double number;
            string commandText = String.Format(CultureInfo.InvariantCulture, "ALTER TABLE {0} ADD {1} {2} {3} NULL {4}", tableName, columnName, columnType,
                defaultValue != SqlString.Null.ToString() ? "NOT" : String.Empty,
                defaultValue != SqlString.Null.ToString()
                    ? String.Format(CultureInfo.InvariantCulture, "DEFAULT ({0})",
                        Double.TryParse(defaultValue, out number)
                            ? String.Format(CultureInfo.InvariantCulture, "({0})", defaultValue)
                            : String.Format(CultureInfo.InvariantCulture, "'{0}'", defaultValue)
                        )
                    : String.Empty);

            using (DbCommand command = new SqlCommand(
                commandText,
                m_sqlConnection,
                m_sqlConnection.BeginTransaction()))
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    Util.HandleExceptionForCommand(command, e);

                    if (command.Transaction != null)
                        command.Transaction.Rollback();
                }
            }
        }
        
        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="data">The data.</param>
        internal void ImportDataBulk(string tableName, DataTable data)
        {
            if (m_isClosing)
                return;

            if (Connection == null)
                return;

            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(
                Connection.ConnectionString,
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sqlBulkCopy.DestinationTableName = tableName;

                try
                {
                    sqlBulkCopy.WriteToServer(data);
                }
                catch (Exception e)
                {
                    string text = String.Format(CultureInfo.InvariantCulture, "Unable to import {0}", tableName);
                    Util.HandleExceptionWithReason(Text, text, e.Message);
                }
            }
        }

        /// <summary>
        /// Imports the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        internal void Import(IQueryable<object> data)
        {
            if (m_isClosing)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            string tableName = data.GetType().GetGenericArguments().First().Name;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            Console.Write(@"Importing {0}... ", tableName);

            DropAndCreateTable(tableName);

            DataTable table = data.ToDataTable();

            ImportDataBulk(tableName, table);

            Util.UpdatePercentDone(table.Rows.Count);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }
    }
}
