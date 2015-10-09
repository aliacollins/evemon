using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.Importers.YamlToSQL
{
    internal static class Translations
    {
        private const string TranslationTableName = "translationTables";
        private const string TrnTranslationColumnsTableName = "trnTranslationColumns";
        private const string TrnTranslationsTableName= "trnTranslations";

        // translationTables and trnTranslationColumns and trnTranslations
        private const string TcIDText = "tcID";

        // translationTables and trnTranslationColumns
        private const string TcGroupIDText = "tcGroupID";

        // translationTables
        private const string SourceTableText = "sourceTable";
        private const string DestinationTableText = "destinationTable";
        private const string TranslatedKeyText = "translatedKey";


        // trnTranslationColumns
        private const string TableNameText = "tableName";
        private const string ColumnNameText = "columnName";
        private const string MasterIDText = "masterID";


        // trnTranslations
        private const string KeyIDText = "keyID";
        private const string LanguageIDText = "languageID";
        private const string TextText = "text";

        internal const string EnglishLanguageIDText = "en";

        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static Translations()
        {
            Util.Closing += Util_Closing;
        }

        /// <summary>
        /// Handles the Closing event of the Program control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Util_Closing(object sender, EventArgs e)
        {
            s_isClosing = true;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <returns></returns>
        internal static DataTable GetTrnTranslationsDataTable()
        {
            using (DataTable trnTranslationsTable = new DataTable())
            {
                trnTranslationsTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(TcIDText, typeof(SqlInt16)),
                    new DataColumn(KeyIDText, typeof(SqlInt32)),
                    new DataColumn(LanguageIDText, typeof(SqlString)),
                    new DataColumn(TextText, typeof(SqlString)),
                });

                return trnTranslationsTable;
            }
        }

        /// <summary>
        /// Deletes the translations of the specifies tcID.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="tcID">The tc identifier.</param>
        internal static void DeleteTranslations(DbConnectionProvider provider, string tcID)
        {
            if (s_isClosing)
                return;

            SqlConnectionProvider sqlConnectionProvider  = provider as SqlConnectionProvider;

            if (sqlConnectionProvider == null)
                return;

            SqlConnection sqlConnection = sqlConnectionProvider.Connection as SqlConnection;

            if (sqlConnection == null)
                return;

            using (DbCommand command = new SqlCommand(
                String.Empty,
                sqlConnection,
                sqlConnection.BeginTransaction()))
            {
                try
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters["columnFilter1"] = TcIDText;
                    parameters["id1"] = tcID;

                    command.CommandText = SqlConnectionProvider.SqlDeleteCommandText(TrnTranslationsTableName, parameters);
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
        /// Inserts the translations static data.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="trnParameters">The translations parameters.</param>
        internal static void InsertTranslationsStaticData(DbConnectionProvider provider,  TranslationsParameters trnParameters)
        {
            string tableText = "dbo." + trnParameters.TableName;
            var baseParameters = new Dictionary<string, string>();
            baseParameters[TcGroupIDText] = trnParameters.TcGroupID;
            baseParameters[TcIDText] = trnParameters.TcID;

            var parameters = new Dictionary<string, string>(baseParameters);
            parameters[SourceTableText] = trnParameters.SourceTable.GetTextOrDefaultString();
            parameters[DestinationTableText] = tableText.GetTextOrDefaultString();
            parameters[TranslatedKeyText] = trnParameters.ColumnName.GetTextOrDefaultString();
            parameters["columnFilter1"] = SourceTableText;
            parameters["id1"] = parameters[SourceTableText];
            parameters["columnFilter2"] = TranslatedKeyText;
            parameters["id2"] = parameters[TranslatedKeyText];

            InsertStaticData(provider, TranslationTableName, parameters);

            parameters = new Dictionary<string, string>(baseParameters);
            parameters[TableNameText] = tableText.GetTextOrDefaultString();
            parameters[ColumnNameText] = trnParameters.ColumnName.GetTextOrDefaultString();
            parameters[MasterIDText] = trnParameters.MasterID.GetTextOrDefaultString();
            parameters["columnFilter1"] = TcIDText;
            parameters["id1"] = parameters[TcIDText];

            InsertStaticData(provider, TrnTranslationColumnsTableName, parameters);
        }

        /// <summary>
        /// Inserts the static data.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        private static void InsertStaticData(DbConnectionProvider provider, string tableName, IDictionary<string, string> parameters)
        {
            if (s_isClosing)
                return;

            SqlConnectionProvider sqlConnectionProvider = provider as SqlConnectionProvider;

            if (sqlConnectionProvider == null)
                return;

            SqlConnection sqlConnection = sqlConnectionProvider.Connection as SqlConnection;

            if (sqlConnection == null)
                return;
            
            using (DbCommand command = new SqlCommand(
                String.Empty,
                sqlConnection,
                sqlConnection.BeginTransaction()))
            {
                try
                {
                    command.CommandText = SqlConnectionProvider.SqlUpdateCommandText(tableName, parameters);
                    if (command.ExecuteNonQuery() == 0)
                    {
                        foreach (KeyValuePair<string, string> parameter in parameters
                            .Where(par => par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                          par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase))
                            .ToList())
                        {
                            parameters.Remove(parameter);
                        }

                        command.CommandText = SqlConnectionProvider.SqlInsertCommandText(tableName, parameters);
                        command.ExecuteNonQuery();
                    }

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
        /// Adds the translations parameters.
        /// </summary>
        /// <param name="tcID">The tc identifier.</param>
        /// <param name="key">The key.</param>
        /// <param name="nameNodes">The name nodes.</param>
        /// <param name="table">The table.</param>
        internal static void AddTranslationsParameters(string tcID, YamlNode key, YamlMappingNode nameNodes, DataTable table)
        {
            foreach (KeyValuePair<YamlNode, YamlNode> pair in nameNodes)
            {
                DataRow row = table.NewRow();
                row[TcIDText] = SqlInt16.Parse(tcID);
                row[KeyIDText] = SqlInt32.Parse(key.ToString());
                row[LanguageIDText] = GetProperLanguageID(pair.Key);
                row[TextText] = !nameNodes.Children.ContainsKey(new YamlScalarNode(pair.Key.ToString())) ||
                                nameNodes.Children[new YamlScalarNode(pair.Key.ToString())] == null
                    ? Convert.DBNull.ToString()
                    : nameNodes.Children[new YamlScalarNode(pair.Key.ToString())].ToString();

                table.Rows.Add(row);
            }
        }

        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="data">The data.</param>
        internal static void ImportDataBulk(DbConnectionProvider provider, DataTable data)
        {
            SqlConnectionProvider sqlConnectionProvider = provider as SqlConnectionProvider;

            if (sqlConnectionProvider == null)
                return;

            sqlConnectionProvider.ImportDataBulk(TrnTranslationsTableName, data);
        }

        /// <summary>
        /// Gets the proper language identifier.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string GetProperLanguageID(YamlNode key)
        {
            string languageIDText = key.ToString();
            if (String.Equals(languageIDText, EnglishLanguageIDText, StringComparison.OrdinalIgnoreCase))
                languageIDText = languageIDText + "-US";

            return languageIDText.ToUpperInvariant();
        }
    }
}
