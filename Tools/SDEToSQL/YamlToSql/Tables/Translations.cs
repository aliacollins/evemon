using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    internal class Translations
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

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <returns></returns>
        internal static DataTable GetTrnTranslationsDataTable()
        {
            DataTable trnTranslationsTable = new DataTable();
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

        /// <summary>
        /// Deletes the translations of the specifies tcID.
        /// </summary>
        /// <param name="tcID">The tc identifier.</param>
        internal static void DeleteTranslations(string tcID)
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters["id"] = tcID;
                    parameters["columnFilter"] = TcIDText;

                    command.CommandText = Database.SqlDeleteCommandText(TrnTranslationsTableName, parameters);
                    command.ExecuteNonQuery();
                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    command.Transaction.Rollback();
                    Util.HandleExceptionForCommand(command, e);
                }
            }
        }

        /// <summary>
        /// Inserts the translations static data.
        /// </summary>
        /// <param name="trnParameters">The translations parameters.</param>
        internal static void InsertTranslationsStaticData(TranslationsParameters trnParameters)
        {
            string tableText = "dbo." + trnParameters.TableName;
            var baseParameters = new Dictionary<string, string>();
            baseParameters[TcGroupIDText] = trnParameters.TcGroupID;
            baseParameters[TcIDText] = trnParameters.TcID;

            var parameters = new Dictionary<string, string>(baseParameters);
            parameters[SourceTableText] = trnParameters.SourceTable.GetTextOrDefaultString();
            parameters[DestinationTableText] = tableText.GetTextOrDefaultString();
            parameters[TranslatedKeyText] = trnParameters.ColumnName.GetTextOrDefaultString();
            parameters["id"] = parameters[SourceTableText];
            parameters["id2"] = parameters[TranslatedKeyText];
            parameters["columnFilter"] = SourceTableText;
            parameters["columnFilter2"] = TranslatedKeyText;

            InsertStaticData(TranslationTableName, parameters);

            parameters = new Dictionary<string, string>(baseParameters);
            parameters[TableNameText] = tableText.GetTextOrDefaultString();
            parameters[ColumnNameText] = trnParameters.ColumnName.GetTextOrDefaultString();
            parameters[MasterIDText] = trnParameters.MasterID.GetTextOrDefaultString();
            parameters["id"] = parameters[TcIDText];
            parameters["columnFilter"] = TcIDText;

            InsertStaticData(TrnTranslationColumnsTableName, parameters);
        }

        /// <summary>
        /// Inserts the translations.
        /// </summary>
        /// <param name="tcID">The tc identifier.</param>
        /// <param name="key">The key.</param>
        /// <param name="nameNodes">The name nodes.</param>
        /// <param name="table">The table.</param>
        internal static void InsertTranslations(string tcID, YamlNode key, YamlMappingNode nameNodes, DataTable table)
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
        /// <param name="data">The data.</param>
        internal static void ImportDataBulk(DataTable data)
        {
            Database.ImportDataBulk(TrnTranslationsTableName, data);
        }

        /// <summary>
        /// Inserts the static data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        private static void InsertStaticData(string tableName, IDictionary<string, string> parameters)
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    command.CommandText = Database.SqlUpdateCommandText(tableName, parameters);
                    if (command.ExecuteNonQuery() == 0)
                    {
                        foreach (KeyValuePair<string, string> parameter in parameters
                            .Where(par => par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                          par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase))
                            .ToList())
                        {
                            parameters.Remove(parameter);
                        }

                        command.CommandText = Database.SqlInsertCommandText(tableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    command.Transaction.Rollback();
                    Util.HandleExceptionForCommand(command, e);
                }
            }
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
